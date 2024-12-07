using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using Age.Core;
using Age.Core.Extensions;
using Age.Rendering.Interfaces;
using Age.Rendering.Vulkan;
using ThirdParty.Shaderc;
using ThirdParty.Shaderc.Enums;
using ThirdParty.Slang;
using ThirdParty.SpirvCross;
using ThirdParty.SpirvCross.Enums;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace Age.Rendering.Resources;

public class ShaderCompilationException(string message) : Exception(message);

public enum StencilKind
{
    None,
    Mask,
    Content,
}

public struct ShaderOptions
{
    #region 4-bytes
    public required VkSampleCountFlags RasterizationSamples;

    public VkFrontFace FrontFace;
    public StencilKind Stencil;
    public uint        Subpass;
    #endregion

    #region 2-bytes
    public bool Watch;

    #endregion
}

public abstract class Shader(RenderPass renderPass) : Resource
{
    public event Action? Changed;

    public RenderPass RenderPass { get; } = renderPass;

    public abstract VkPipelineBindPoint   BindPoint           { get; }
    public abstract VkDescriptorSetLayout DescriptorSetLayout { get; }
    public abstract VkPipeline            Pipeline            { get; }
    public abstract VkPipelineLayout      PipelineLayout      { get; }
    public abstract VkDescriptorType[]    UniformBindings     { get; }

    protected void InvokeChanged() =>
        this.Changed?.Invoke();
}

public abstract unsafe class Shader<TVertexInput, TPushConstant> : Shader
where TVertexInput  : IVertexInput
where TPushConstant : IPushConstant
{
    private const int DEBOUNCE_DELAY = 50;

    private static readonly string            shadersPath        = Path.GetFullPath(Debugger.IsAttached ? Path.Join(Directory.GetCurrentDirectory(), "source/Age.Rendering/Shaders") : Path.Join(AppContext.BaseDirectory, $"Shaders"));
    private static readonly SlangSession      globalSlangSession = new();
    private static readonly FileSystemWatcher watcher            = new(shadersPath) { EnableRaisingEvents = true, IncludeSubdirectories = true };

    private readonly Dictionary<string, CancellationTokenSource>    cancellationTokenSources = [];
    private readonly HashSet<string>                                files                    = [];
    private readonly Dictionary<string, byte[]>                     hashes                   = [];
    private readonly Dictionary<string, List<string>>               includeShaders           = [];
    private readonly Dictionary<string, Lock>                       locks                    = [];
    private readonly Dictionary<string, Dictionary<string, byte[]>> shaderIncludes           = [];
    private readonly Lock                                           shaderLock               = new();

    private VkDescriptorSetLayout descriptorSetLayout;
    private VkPipeline            pipeline;
    private VkPipelineLayout      pipelineLayout;
    private VkDescriptorType[]    uniformBindings;

    private ShaderOptions options;

    public abstract string              Name              { get; }
    public abstract VkPrimitiveTopology PrimitiveTopology { get; }

    public override VkPipeline            Pipeline            => this.pipeline;
    public override VkDescriptorSetLayout DescriptorSetLayout => this.descriptorSetLayout;
    public override VkPipelineLayout      PipelineLayout      => this.pipelineLayout;
    public override VkDescriptorType[]    UniformBindings     => this.uniformBindings;

    public Dictionary<VkShaderStageFlags, byte[]> Stages { get; } = [];

    public Shader(RenderPass renderPass, Span<string> files, in ShaderOptions options) : base(renderPass)
    {
        this.options = options;

        foreach (var file in files)
        {
            var filepath = string.Intern(Path.IsPathRooted(file) ? file : Path.GetFullPath(Path.Join(shadersPath, file)));
            var filename = string.Intern(Path.GetFileName(filepath));

            if (!watcher.Filters.Contains(filename))
            {
                watcher.Filters.Add(filename);
            }

            this.cancellationTokenSources[filepath] = new();
            this.shaderIncludes[filepath] = [];
            this.locks[filepath] = new();

            this.CompileShader(filepath);
            this.files.Add(filepath);
        }

        if (options.Watch)
        {
            watcher.Changed += this.OnFileChanged;
        }

        this.UpdatePipeline();
    }

    private VkShaderModule CreateShaderModule(byte[] buffer)
    {
        fixed (byte* pCode = buffer)
        {
            var createInfo = new VkShaderModuleCreateInfo
            {
                CodeSize = (uint)buffer.Length,
                PCode    = (uint*)pCode,
            };

            return VulkanRenderer.Singleton.Context.Device.CreateShaderModule(createInfo);
        }
    }

    private void CompileShaderWithDebounce(string filepath, string? trigger)
    {
        var cancellationTokenSource = this.cancellationTokenSources[filepath];

        cancellationTokenSource.Cancel();
        cancellationTokenSource = new();

        void action(Task task)
        {
            lock (this.locks[filepath])
            {
                if (task.IsCompletedSuccessfully && this.CompileShader(filepath, true, trigger))
                {
                    this.ShaderChanged();
                }
            }
        }

        Task.Delay(DEBOUNCE_DELAY, cancellationTokenSource.Token)
            .ContinueWith(action, TaskScheduler.Default);
    }

    private bool CompileShader(string filepath, bool allowFailure = false, string? trigger = null) =>
        filepath.EndsWith(".slang")
            ? this.CompileSlangShader(filepath, allowFailure)
            : this.CompileGLSLShader(filepath, allowFailure, trigger);

    private bool CompileGLSLShader(string filepath, bool allowFailure = false, string? trigger = null)
    {
        var (shaderStage, shaderKind) = Path.GetExtension(filepath) switch
        {
            ".frag"  => (VkShaderStageFlags.Fragment, ShaderKind.FragmentShader),
            ".vert"  => (VkShaderStageFlags.Vertex,   ShaderKind.VertexShader),
            _ => throw new InvalidOperationException()
        };

        try
        {
            var source   = File.ReadAllBytes(filepath);
            var checksum = MD5.HashData(source);

            var canCompile = trigger != null
                ? !this.shaderIncludes[filepath].TryGetValue(trigger, out var includeHash) || !includeHash.AsSpan().SequenceEqual(MD5.HashData(File.ReadAllBytes(trigger)))
                : !this.hashes.TryGetValue(filepath, out var shaderHash) || !shaderHash.AsSpan().SequenceEqual(checksum);

            if (canCompile)
            {
                this.hashes[filepath] = checksum;

                Logger.Trace($"Compiling Shader {this.Name}.{shaderStage} [{filepath}]");

                foreach (var include in this.shaderIncludes[filepath])
                {
                    watcher.Filters.Remove(Path.GetRelativePath(shadersPath, include.Key));
                }

                foreach (var includeShader in this.includeShaders.Values)
                {
                    includeShader.Remove(filepath);
                }

                this.shaderIncludes[filepath].Clear();

                using var compiler = new ThirdParty.Shaderc.Compiler();
                using var options  = new CompilerOptions { IncludeResolver = this.GetIncludeResolver(filepath) };

                var result = compiler.CompileIntoSpv(source, shaderKind, filepath, "main", options);

                foreach (var include in this.shaderIncludes[filepath])
                {
                    var filename = string.Intern(Path.GetFileName(include.Key));

                    if (!watcher.Filters.Contains(filename))
                    {
                        watcher.Filters.Add(filename);
                    }
                }

                if (result.CompilationStatus == CompilationStatus.Success)
                {
                    this.Stages[shaderStage] = result.Bytes;

                    Logger.Info($"Shader {this.Name}.{shaderStage} [{filepath}] compiled with {(result.Warnings > 0 ? $"{result.Warnings} warnings" : "success")}.");

                    return true;
                }
                else
                {
                    if (!allowFailure)
                    {
                        throw new ShaderCompilationException(result.ErrorMessage);
                    }

                    Logger.Error(result.ErrorMessage);
                }
            }
        }
        catch (Exception exception)
        {
            if (!allowFailure)
            {
                throw;
            }

            Logger.Error(exception.Message);
        }

        return false;
    }

    private bool CompileSlangShader(string filepath, bool allowFailure = false, string? trigger = null)
    {
        try
        {
            var source   = File.ReadAllBytes(filepath);
            var checksum = MD5.HashData(source);

            var canCompile = trigger != null
                ? !this.shaderIncludes[filepath].TryGetValue(trigger, out var includeHash) || !includeHash.AsSpan().SequenceEqual(MD5.HashData(File.ReadAllBytes(trigger)))
                : !this.hashes.TryGetValue(filepath, out var shaderHash) || !shaderHash.AsSpan().SequenceEqual(checksum);

            if (canCompile)
            {
                this.hashes[filepath] = checksum;

                Logger.Trace($"Compiling Shader [{filepath}]");

                foreach (var include in this.shaderIncludes[filepath])
                {
                    watcher.Filters.Remove(Path.GetRelativePath(shadersPath, include.Key));
                }

                foreach (var includeShader in this.includeShaders.Values)
                {
                    includeShader.Remove(filepath);
                }

                this.shaderIncludes[filepath].Clear();

                using var request = new SlangCompileRequest(globalSlangSession);

                var translationUnitIndex = request.AddTranslationUnit(SlangSourceLanguage.Slang, Path.GetFileName(filepath.AsSpan()));

                request.AddTranslationUnitSourceString(translationUnitIndex, filepath, source);
                request.SetCodeGenTarget(SlangCompileTarget.Spirv);
                request.SetTargetProfile(0, globalSlangSession.FindProfile("spirv_1_0"));

                if (request.Compile())
                {
                    var dependencies = request.GetDependencyFiles();

                    foreach (var dependecy in dependencies)
                    {
                        if (dependecy != filepath)
                        {
                            if (!this.includeShaders.TryGetValue(dependecy, out var shaders))
                            {
                                this.includeShaders[filepath] = shaders = [];
                            }

                            shaders.Add(filepath);

                            var content = File.ReadAllBytes(dependecy);

                            this.shaderIncludes[filepath][dependecy] = MD5.HashData(content);

                            var filename = string.Intern(Path.GetFileName(dependecy));

                            if (!watcher.Filters.Contains(filename))
                            {
                                watcher.Filters.Add(filename);
                            }
                        }
                    }

                    var reflection  = request.GetReflection();
                    var entryPoints = reflection.GetEntryPoints();

                    for (var i = 0; i < entryPoints.Length; i++)
                    {
                        var entryPoint = entryPoints[i];
                        var shaderStage = entryPoint.GetStage() switch
                        {
                            SlangStage.Vertex   => VkShaderStageFlags.Vertex,
                            SlangStage.Fragment => VkShaderStageFlags.Fragment,
                            _ => throw new InvalidOperationException(),
                        };

                        this.Stages[shaderStage] = request.GetEntryPointCode(i);

                        Logger.Info($"Shader {this.Name}.{shaderStage} [{filepath}] compiled with success.");
                    }

                    return true;
                }
                else
                {
                    var diagnostic = request.GetDiagnosticOutput()!;

                    if (!allowFailure)
                    {
                        throw new ShaderCompilationException(diagnostic);
                    }

                    Logger.Error(diagnostic);
                }
            }
        }
        catch (Exception)
        {

            throw;
        }

        return false;
    }

    private IncludeResolve GetIncludeResolver(string shaderfile) =>
        (requestedSource, type, requestingSource, includeDepth) =>
        {
            Logger.Trace($"Resolving include \"{requestedSource}\" requested by \"{requestingSource}\"");

            var filepath = type == IncludeType.Relative
                ? Path.GetFullPath(Path.Join(Path.GetDirectoryName(requestingSource), requestedSource))
                : requestedSource;

            Logger.Trace($"Include resolved to \"{filepath}\"");

            if (!this.includeShaders.TryGetValue(filepath, out var shaders))
            {
                this.includeShaders[filepath] = shaders = [];
            }

            shaders.Add(shaderfile);

            var content = File.ReadAllBytes(filepath);

            this.shaderIncludes[shaderfile][filepath] = MD5.HashData(content);

            return new()
            {
                SourceName = filepath,
                Content    = content,
            };
        };

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        var filepath = Path.GetFullPath(e.FullPath);

        if (this.files.Contains(filepath))
        {
            this.CompileShaderWithDebounce(filepath, null);
        }
        else if (this.includeShaders.TryGetValue(filepath, out var files))
        {
            foreach (var file in files)
            {
                this.CompileShaderWithDebounce(file, filepath);
            }
        }
    }

    private void ShaderChanged()
    {
        lock (this.shaderLock)
        {
            Span<IDisposable> disposables = [this.pipeline, this.pipelineLayout, this.descriptorSetLayout];

            this.UpdatePipeline();

            VulkanRenderer.Singleton.DeferredDispose(disposables);

            this.InvokeChanged();
        }
    }

    [MemberNotNull(nameof(pipeline), nameof(pipelineLayout), nameof(descriptorSetLayout), nameof(uniformBindings))]
    private void UpdatePipeline()
    {
        fixed (byte* pName = "main"u8)
        {
            var bindings                       = new List<VkDescriptorSetLayoutBinding>();
            var pipelineShaderStageCreateInfos = new List<VkPipelineShaderStageCreateInfo>();
            var pushConstantRanges             = new List<VkPushConstantRange>();

            if (TPushConstant.Size > 0)
            {
                var pushConstantRange = new VkPushConstantRange
                {
                    Size       = TPushConstant.Size,
                    Offset     = TPushConstant.Offset,
                    StageFlags = TPushConstant.Stages,
                };

                pushConstantRanges.Add(pushConstantRange);
            }

            using var disposables = new Disposables();
            using var context     = new Context();

            foreach (var stage in this.Stages)
            {
                var spirv     = context.ParseSpirv(stage.Value);
                var compiler  = context.CreateCompiler(Backend.Glsl, spirv, CaptureMode.TakeOwnership);
                var resources = compiler.CreateShaderResources();

                foreach (var resource in resources.GetResourceListForType(ResorceType.SampledImage))
                {
                    var binding = compiler.GetDecoration(resource.Id, Decoration.Binding);

                    var layout = new VkDescriptorSetLayoutBinding()
                    {
                        Binding         = binding,
                        DescriptorCount = 1,
                        DescriptorType  = VkDescriptorType.CombinedImageSampler,
                        StageFlags      = stage.Key,
                    };

                    bindings.Add(layout);
                }

                foreach (var resource in resources.GetResourceListForType(ResorceType.UniformBuffer))
                {
                    var binding = compiler.GetDecoration(resource.Id, Decoration.Binding);

                    var layout = new VkDescriptorSetLayoutBinding()
                    {
                        Binding         = binding,
                        DescriptorCount = 1,
                        DescriptorType  = VkDescriptorType.UniformBuffer,
                        StageFlags      = stage.Key,
                    };

                    bindings.Add(layout);
                }

                var shaderModule = this.CreateShaderModule(stage.Value);

                disposables.Add(shaderModule);

                var createInfo = new VkPipelineShaderStageCreateInfo()
                {
                    Module = shaderModule.Handle,
                    PName  = pName,
                    Stage  = stage.Key,
                };

                pipelineShaderStageCreateInfos.Add(createInfo);
            }

            this.uniformBindings = bindings.OrderBy(x => x.Binding).Select(x => x.DescriptorType).ToArray();

            var vertexInputAttributeDescription = TVertexInput.GetAttributes();
            var vertexInputBindingDescription   = TVertexInput.GetBindings();

            var dynamicStates = new VkDynamicState[]
            {
                VkDynamicState.Viewport,
                VkDynamicState.Scissor,
            };

            fixed (VkDescriptorSetLayoutBinding*      pBindings                       = bindings.AsSpan())
            fixed (VkVertexInputAttributeDescription* pVertexAttributeDescriptions    = vertexInputAttributeDescription)
            fixed (VkDynamicState*                    pDynamicStates                  = dynamicStates)
            fixed (VkPipelineShaderStageCreateInfo*   pPipelineShaderStageCreateInfos = pipelineShaderStageCreateInfos.AsSpan())
            fixed (VkPushConstantRange*               pPushConstantRanges             = pushConstantRanges.AsSpan())
            {
                var descriptorSetLayoutCreateInfo = new VkDescriptorSetLayoutCreateInfo
                {
                    PBindings    = pBindings,
                    BindingCount = (uint)bindings.Count,
                };

                this.descriptorSetLayout = VulkanRenderer.Singleton.Context.Device.CreateDescriptorSetLayout(descriptorSetLayoutCreateInfo);

                var descriptorSetLayoutHandle = this.descriptorSetLayout.Handle;

                var pipelineLayoutCreateInfo = new VkPipelineLayoutCreateInfo
                {
                    PSetLayouts            = &descriptorSetLayoutHandle,
                    SetLayoutCount         = 1,
                    PPushConstantRanges    = pPushConstantRanges,
                    PushConstantRangeCount = (uint)pushConstantRanges.Count,
                };

                this.pipelineLayout = VulkanRenderer.Singleton.Context.Device.CreatePipelineLayout(pipelineLayoutCreateInfo);

                var inputAssembly = new VkPipelineInputAssemblyStateCreateInfo
                {
                    Topology = this.PrimitiveTopology,
                };

                var pipelineVertexInputStateCreateInfo = new VkPipelineVertexInputStateCreateInfo
                {
                    PVertexAttributeDescriptions    = pVertexAttributeDescriptions,
                    PVertexBindingDescriptions      = &vertexInputBindingDescription,
                    VertexAttributeDescriptionCount = (uint)vertexInputAttributeDescription.Length,
                    VertexBindingDescriptionCount   = 1,
                };

                var pipelineDynamicStateCreateInfo = new VkPipelineDynamicStateCreateInfo
                {
                    DynamicStateCount = (uint)dynamicStates.Length,
                    PDynamicStates    = pDynamicStates,
                };

                var pipelineColorBlendAttachmentState = new VkPipelineColorBlendAttachmentState
                {
                    AlphaBlendOp   = VkBlendOp.Add,
                    BlendEnable    = true,
                    ColorWriteMask = VkColorComponentFlags.R
                        | VkColorComponentFlags.G
                        | VkColorComponentFlags.B
                        | VkColorComponentFlags.A,
                    DstColorBlendFactor = VkBlendFactor.OneMinusSrcAlpha,
                    SrcAlphaBlendFactor = VkBlendFactor.One,
                    SrcColorBlendFactor = VkBlendFactor.SrcAlpha,
                };

                var pipelineColorBlendStateCreateInfo = new VkPipelineColorBlendStateCreateInfo
                {
                    AttachmentCount = 1,
                    LogicOp         = VkLogicOp.Copy,
                    PAttachments    = &pipelineColorBlendAttachmentState,
                };

                var pipelineMultisampleStateCreateInfo = new VkPipelineMultisampleStateCreateInfo
                {
                    SampleShadingEnable  = true,
                    RasterizationSamples = this.options.RasterizationSamples,
                    MinSampleShading     = 1,
                };

                var pipelineRasterizationStateCreateInfo = new VkPipelineRasterizationStateCreateInfo
                {
                    CullMode    = VkCullModeFlags.Back,
                    FrontFace   = this.options.FrontFace,
                    LineWidth   = 1,
                    PolygonMode = VkPolygonMode.Fill,
                };

                var pipelineViewportStateCreateInfo = new VkPipelineViewportStateCreateInfo
                {
                    ViewportCount = 1,
                    ScissorCount  = 1,
                };

                var stencilOp = this.options.Stencil switch
                {
                    StencilKind.Mask => new VkStencilOpState
                    {
                        CompareMask = 0xFF,
                        CompareOp   = VkCompareOp.Always,
                        DepthFailOp = VkStencilOp.Replace,
                        FailOp      = VkStencilOp.Replace,
                        PassOp      = VkStencilOp.Replace,
                        Reference   = 1,
                        WriteMask   = 0xFF,
                    },
                    StencilKind.Content => new VkStencilOpState
                    {
                        CompareMask = 0xFF,
                        CompareOp   = VkCompareOp.Equal,
                        DepthFailOp = VkStencilOp.Keep,
                        FailOp      = VkStencilOp.Keep,
                        PassOp      = VkStencilOp.Replace,
                        Reference   = 1,
                        WriteMask   = 0xFF,
                    },
                    _ => default
                };

                var depthStencilState = new VkPipelineDepthStencilStateCreateInfo
                {
                    DepthTestEnable       = this.options.Stencil == StencilKind.None,
                    DepthWriteEnable      = true,
                    DepthCompareOp        = VkCompareOp.Less,
                    DepthBoundsTestEnable = false,
                    StencilTestEnable     = this.options.Stencil != StencilKind.None,
                    Front                 = stencilOp,
                    Back                  = stencilOp,
                };

                var graphicsPipelineCreateInfo = new VkGraphicsPipelineCreateInfo
                {
                    Layout              = this.PipelineLayout.Handle,
                    PColorBlendState    = &pipelineColorBlendStateCreateInfo,
                    PDynamicState       = &pipelineDynamicStateCreateInfo,
                    PInputAssemblyState = &inputAssembly,
                    PMultisampleState   = &pipelineMultisampleStateCreateInfo,
                    PRasterizationState = &pipelineRasterizationStateCreateInfo,
                    PStages             = pPipelineShaderStageCreateInfos,
                    PVertexInputState   = &pipelineVertexInputStateCreateInfo,
                    PViewportState      = &pipelineViewportStateCreateInfo,
                    StageCount          = (uint)pipelineShaderStageCreateInfos.Count,
                    RenderPass          = this.RenderPass.Instance.Handle,
                    PDepthStencilState  = &depthStencilState,
                    Subpass             = this.options.Subpass,
                };

                this.pipeline = VulkanRenderer.Singleton.Context.Device.CreateGraphicsPipelines(graphicsPipelineCreateInfo);
            }
        }
    }

    protected override void Disposed()
    {
        foreach (var file in this.files)
        {
            watcher.Filters.Remove(file);
        }

        watcher.Changed -= this.OnFileChanged;

        VulkanRenderer.Singleton.DeferredDispose(this.Pipeline);
        VulkanRenderer.Singleton.DeferredDispose(this.PipelineLayout);
        VulkanRenderer.Singleton.DeferredDispose(this.DescriptorSetLayout);
    }
}

