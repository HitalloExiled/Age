using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Age.Core;
using Age.Core.Extensions;
using Age.Rendering.Interfaces;
using Age.Rendering.Vulkan;
using ThirdParty.Slang;
using ThirdParty.SpirvCross;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace Age.Rendering.Resources;

public abstract class ShaderV2(RenderPass renderPass) : Resource
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

public abstract unsafe class ShaderV2<TVertexInput, TPushConstant> : ShaderV2
where TVertexInput  : IVertexInput
where TPushConstant : IPushConstant
{
    private const int DEBOUNCE_DELAY = 50;

    private static readonly string                  shadersPath       = Path.GetFullPath(Debugger.IsAttached ? Path.Join(Directory.GetCurrentDirectory(), "source/Age/Shaders") : Path.Join(AppContext.BaseDirectory, $"Shaders"));
    private static readonly Dictionary<string, int> dependenciesUsers = [];
    private static readonly FileSystemWatcher       watcher           = new(shadersPath) { EnableRaisingEvents = true, IncludeSubdirectories = true };

    private readonly string                           filepath;
    private readonly Dictionary<string, byte[]>       dependenciesHash = [];
    private readonly Lock                             @lock            = new();
    private readonly Lock                             shaderLock       = new();
    private readonly SlangSession                     slangSession     = new();

    private byte[]                  hash                    = [];
    private CancellationTokenSource cancellationTokenSource = new();
    private VkDescriptorSetLayout   descriptorSetLayout;
    private VkPipeline              pipeline;
    private VkPipelineLayout        pipelineLayout;
    private VkDescriptorType[]      uniformBindings;

    private ShaderOptions options;

    public abstract string              Name              { get; }
    public abstract VkPrimitiveTopology PrimitiveTopology { get; }

    public override VkPipeline            Pipeline            => this.pipeline;
    public override VkDescriptorSetLayout DescriptorSetLayout => this.descriptorSetLayout;
    public override VkPipelineLayout      PipelineLayout      => this.pipelineLayout;
    public override VkDescriptorType[]    UniformBindings     => this.uniformBindings;

    public ShaderV2(RenderPass renderPass, string file, in ShaderOptions options) : base(renderPass)
    {
        this.filepath = string.Intern(Path.IsPathRooted(file) ? file : Path.GetFullPath(Path.Join(shadersPath, file)));
        this.options  = options;

        var filename = string.Intern(Path.GetFileName(this.filepath));

        if (!watcher.Filters.Contains(filename))
        {
            watcher.Filters.Add(filename);
        }

        this.CompileShader(false, null, out var reflection);
        this.UpdatePipeline(reflection!);

        if (options.Watch)
        {
            watcher.Changed += this.OnFileChanged;
        }
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
        this.cancellationTokenSource.Cancel();
        this.cancellationTokenSource = new();

        void action(Task task)
        {
            lock (this.@lock)
            {
                if (task.IsCompletedSuccessfully && this.CompileShader(true, trigger, out var reflection))
                {
                    this.ShaderChanged(reflection);
                }
            }
        }

        Task.Delay(DEBOUNCE_DELAY, this.cancellationTokenSource.Token)
            .ContinueWith(action, TaskScheduler.Default);
    }

    private bool CompileShader(bool allowFailure, string? dependency, [NotNullWhen(true)] out SlangReflection? reflection)
    {
        try
        {
            var source = File.ReadAllBytes(this.filepath);
            var hash  = MD5.HashData(source);

            var canCompile = dependency != null
                ? !this.dependenciesHash.TryGetValue(dependency, out var dependencyHash) || !dependencyHash.AsSpan().SequenceEqual(MD5.HashData(File.ReadAllBytes(dependency)))
                : !this.hash.AsSpan().SequenceEqual(this.hash);

            if (canCompile)
            {
                this.hash = hash;

                Logger.Trace($"Compiling Shader [{this.filepath}]");

                foreach (var entry in this.dependenciesHash)
                {
                    ref var users = ref dependenciesUsers.GetValueRefOrNullRef(entry.Key);

                    if (!Unsafe.IsNullRef(ref users))
                    {
                        users--;

                        if (users == 0)
                        {
                            watcher.Filters.Remove(Path.GetRelativePath(shadersPath, entry.Key));
                            dependenciesUsers.Remove(entry.Key);
                        }
                    }
                }

                this.dependenciesHash.Clear();

                using var request = new SlangCompileRequest(this.slangSession);

                var translationUnitIndex = request.AddTranslationUnit(SlangSourceLanguage.Slang, Path.GetFileName(this.filepath.AsSpan()));

                request.AddSearchPath(Path.Join(shadersPath.AsSpan(), "Modules"));
                request.AddTranslationUnitSourceString(translationUnitIndex, this.filepath, source);
                request.SetCodeGenTarget(SlangCompileTarget.Spirv);
                request.SetTargetProfile(0, this.slangSession.FindProfile("spirv_1_0"));

                if (request.Compile())
                {
                    var dependencies = request.GetDependencyFiles();

                    foreach (var dependecy in dependencies)
                    {
                        if (dependecy == "glsl")
                        {
                            continue;
                        }

                        if (dependecy != this.filepath)
                        {
                            var content = File.ReadAllBytes(dependecy);

                            this.dependenciesHash[dependecy] = MD5.HashData(content);

                            ref var users = ref dependenciesUsers.GetValueRefOrAddDefault(dependecy, out var hasUsers);

                            if (!hasUsers)
                            {
                                var filename = string.Intern(Path.GetFileName(dependecy));

                                watcher.Filters.Add(filename);
                            }

                            users++;
                        }
                    }

                    reflection = request.GetReflection();

                    Logger.Info($"Shader {this.filepath} compiled with success.");

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

        reflection = null;

        return false;
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        var filepath = Path.GetFullPath(e.FullPath);

        this.CompileShaderWithDebounce(filepath, filepath != this.filepath ? filepath : null);
    }

    private void ShaderChanged(SlangReflection reflection)
    {
        lock (this.shaderLock)
        {
            Span<IDisposable> disposables = [this.pipeline, this.pipelineLayout, this.descriptorSetLayout];

            this.UpdatePipeline(reflection);

            VulkanRenderer.Singleton.DeferredDispose(disposables);

            this.InvokeChanged();
        }
    }

    [MemberNotNull(nameof(pipeline), nameof(pipelineLayout), nameof(descriptorSetLayout), nameof(uniformBindings))]
    private void UpdatePipeline(SlangReflection reflection)
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

        var entryPoints = reflection.GetEntryPoints();

        Span<nint> entryPointNames = stackalloc nint[entryPoints.Length];

        for (var i = 0; i < entryPoints.Length; i++)
        {
            var entryPoint = entryPoints[i];
            var stage      = entryPoint.GetStage() switch
            {
                SlangStage.Vertex => VkShaderStageFlags.Vertex,
                SlangStage.Fragment => VkShaderStageFlags.Fragment,
                _ => throw new InvalidOperationException("Unsuported shader stage"),
            };

            var parameters = reflection.GetParameters();

            foreach (var parameter in parameters)
            {
                // var x reflection.GetTypeParameters();
                // reflection.GetRe
                // foreach (var resource in resources.GetResourceListForType(ResorceType.SampledImage))
                // {
                //     var binding = compiler.GetDecoration(resource.Id, Decoration.Binding);

                //     var layout = new VkDescriptorSetLayoutBinding()
                //     {
                //         Binding         = binding,
                //         DescriptorCount = 1,
                //         DescriptorType  = VkDescriptorType.CombinedImageSampler,
                //         StageFlags      = stage.Key,
                //     };

                //     bindings.Add(layout);
                // }

                // foreach (var resource in resources.GetResourceListForType(ResorceType.UniformBuffer))
                // {
                //     var binding = compiler.GetDecoration(resource.Id, Decoration.Binding);

                //     var layout = new VkDescriptorSetLayoutBinding()
                //     {
                //         Binding         = binding,
                //         DescriptorCount = 1,
                //         DescriptorType  = VkDescriptorType.UniformBuffer,
                //         StageFlags      = stage.Key,
                //     };

                //     bindings.Add(layout);
                // }
            }

            var shaderModule = this.CreateShaderModule(reflection.Request.GetEntryPointCode(i));

            disposables.Add(shaderModule);

            var createInfo = new VkPipelineShaderStageCreateInfo()
            {
                Module = shaderModule.Handle,
                PName  = (byte*)entryPointNames[i],
                Stage  = stage,
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

    protected override void Disposed()
    {
        watcher.Filters.Remove(this.filepath);

        watcher.Changed -= this.OnFileChanged;

        VulkanRenderer.Singleton.DeferredDispose(this.Pipeline);
        VulkanRenderer.Singleton.DeferredDispose(this.PipelineLayout);
        VulkanRenderer.Singleton.DeferredDispose(this.DescriptorSetLayout);
    }
}

