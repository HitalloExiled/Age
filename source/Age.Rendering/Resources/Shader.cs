using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Age.Core;
using Age.Core.Collections;
using Age.Core.Extensions;
using Age.Rendering.Interfaces;
using Age.Rendering.Vulkan;
using ThirdParty.Slang;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace Age.Rendering.Resources;

public abstract class Shader(RenderPass renderPass) : Resource
{
    public event Action? Changed;

    public RenderPass RenderPass { get; } = renderPass;

    public abstract VkShaderStageFlags PushConstantStages { get; }

    public abstract VkPipelineBindPoint           BindPoint           { get; }
    public abstract VkDescriptorSetLayout         DescriptorSetLayout { get; }
    public abstract VkPipeline                    Pipeline            { get; }
    public abstract VkPipelineLayout              PipelineLayout      { get; }
    public abstract NativeArray<VkDescriptorType> UniformBindings     { get; }

    protected void InvokeChanged() =>
        this.Changed?.Invoke();
}

public abstract unsafe class Shader<TVertexInput> : Shader
where TVertexInput  : IVertexInput
{
    private const string GLSL           = "glsl";
    private const int    DEBOUNCE_DELAY = 50;

    private static readonly Dictionary<string, int> dependenciesUsers = [];
    private static readonly string                  shadersPath       = Path.GetFullPath(Debugger.IsAttached ? Path.Join(Directory.GetCurrentDirectory(), "source/Age/Shaders") : Path.Join(AppContext.BaseDirectory, $"Shaders"));
    private static readonly FileSystemWatcher       watcher           = new(shadersPath) { EnableRaisingEvents = true, IncludeSubdirectories = true };

    private static SpinLock spinLock;

    private readonly Lock                          @lock            = new();
    private readonly Dictionary<string, MD5Hash>   dependenciesHash = [];
    private readonly string                        filepath;
    private readonly NativeArray<VkDescriptorType> uniformBindings  = new();

    private CancellationTokenSource cancellationTokenSource = new();
    private VkDescriptorSetLayout   descriptorSetLayout;
    private MD5Hash                 hash;
    private VkPipeline              pipeline;
    private VkPipelineLayout        pipelineLayout;

    private ShaderOptions      options;
    private VkShaderStageFlags pushConstantStages;

    public abstract string              Name              { get; }
    public abstract VkPrimitiveTopology PrimitiveTopology { get; }

    public sealed override VkDescriptorSetLayout         DescriptorSetLayout => this.descriptorSetLayout;
    public sealed override VkPipeline                    Pipeline            => this.pipeline;
    public sealed override VkPipelineLayout              PipelineLayout      => this.pipelineLayout;
    public sealed override VkShaderStageFlags            PushConstantStages  => this.pushConstantStages;
    public sealed override NativeArray<VkDescriptorType> UniformBindings     => this.uniformBindings;

    public Shader(string file, RenderPass renderPass, in ShaderOptions options) : base(renderPass)
    {
        this.filepath = string.Intern(Path.IsPathRooted(file) ? file : Path.GetFullPath(Path.Join(shadersPath, file)));
        this.options  = options;

        using var source = FileReader.ReadAllBytesAsRef(this.filepath);

        this.CompileShader(source);

        this.hash = MD5Hash.Create(source);

        var filename = string.Intern(Path.GetFileName(this.filepath));

        if (!watcher.Filters.Contains(filename))
        {
            watcher.Filters.Add(filename);
        }

        if (options.Watch)
        {
            watcher.Changed += this.OnFileChanged;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static VkShaderStageFlags SlangStageToVkShaderStageFlags(SlangStage stage) =>
        stage switch
        {
            SlangStage.Vertex   => VkShaderStageFlags.Vertex,
            SlangStage.Fragment => VkShaderStageFlags.Fragment,
            _ => throw new InvalidOperationException("Unsuported shader stage"),
        };

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

    [MemberNotNull(nameof(pipeline), nameof(pipelineLayout), nameof(descriptorSetLayout))]
    private void CompileShader(scoped ReadOnlySpan<byte> source)
    {
        Logger.Trace($"Compiling Shader [{this.filepath}]");
        var start = Stopwatch.GetTimestamp();

        using var session = new SlangSession();
        using var request = new SlangCompileRequest(session);

        var translationUnitIndex = request.AddTranslationUnit(SlangSourceLanguage.Slang, Path.GetFileName(this.filepath.AsSpan()));

        request.AddTranslationUnitSourceString(translationUnitIndex, this.filepath, source);
        request.SetCodeGenTarget(SlangCompileTarget.Spirv);
        request.SetTargetProfile(0, session.FindProfile("spirv_1_0"));

        if (request.Compile())
        {
            var dependencies = request.GetDependencyFiles().AsSpan(1);

            var dependenciesToRemove = new List<string>(this.dependenciesHash.Count);

            foreach (var key in this.dependenciesHash.Keys)
            {
                if (!dependencies.Contains(key))
                {
                    dependenciesToRemove.Add(key);
                }
            }

            if (dependencies.Length > 0)
            {
                if (dependencies[^1] == GLSL)
                {
                    dependencies = dependencies[..^1];
                }

                foreach (var dependecy in dependencies)
                {
                    ref var dependencieHash = ref this.dependenciesHash.GetValueRefOrAddDefault(dependecy, out var _);

                    using var bytes = FileReader.ReadAllBytesAsRef(dependecy);

                    MD5Hash.Update(bytes, ref dependencieHash);
                }

                var lockTaken = false;
                spinLock.Enter(ref lockTaken);

                foreach (var dependecy in dependencies)
                {
                    ref var users = ref dependenciesUsers.GetValueRefOrAddDefault(dependecy, out var hasUsers);

                    if (!hasUsers)
                    {
                        var filename = string.Intern(Path.GetFileName(dependecy));

                        watcher.Filters.Add(filename);
                    }

                    users++;
                }

                if (lockTaken)
                {
                    spinLock.Exit(false);
                }
            }

            if (dependenciesToRemove.Count > 0)
            {
                var lockTaken = false;
                spinLock.Enter(ref lockTaken);

                foreach (var dependecy in dependenciesToRemove)
                {
                    this.dependenciesHash.Remove(dependecy);

                    ref var users = ref dependenciesUsers.GetValueRefOrNullRef(dependecy);

                    if (!Unsafe.IsNullRef(ref users))
                    {
                        users--;

                        if (users == 0)
                        {
                            watcher.Filters.Remove(Path.GetRelativePath(shadersPath, dependecy));
                            dependenciesUsers.Remove(dependecy);
                        }
                    }
                }

                if (lockTaken)
                {
                    spinLock.Exit(false);
                }
            }

            this.UpdatePipeline(request);

            Logger.Info($"Shader {this.filepath} compiled in {Stopwatch.GetElapsedTime(start).Milliseconds}ms.");
        }
        else
        {
            var diagnostic = request.GetDiagnosticOutput()!;

            Logger.Error(diagnostic);

            throw new ShaderCompilationException(diagnostic);
        }
    }

    private void RecompileShader(string? dependency)
    {
        bool hasChanged;

        using var source = FileReader.ReadAllBytesAsRef(this.filepath);

        if (dependency != null)
        {
            using var bytes = FileReader.ReadAllBytesAsRef(dependency);

            hasChanged = !this.dependenciesHash.TryGetValue(dependency, out var dependencyHash) || dependencyHash != MD5Hash.Create(bytes);
        }
        else
        {
            var hash = MD5Hash.Create(source);

            if (hasChanged = this.hash != hash)
            {
                this.hash = hash;
            }
        }

        if (hasChanged)
        {
            try
            {
                Span<IDisposable> disposables = [this.pipeline, this.pipelineLayout, this.descriptorSetLayout];

                this.CompileShader(source);

                VulkanRenderer.Singleton.DeferredDispose(disposables);

                this.InvokeChanged();
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
            }
        }
    }

    private void RecompileShaderWithDebounce(string? trigger = null)
    {
        this.cancellationTokenSource.Cancel();
        this.cancellationTokenSource.Dispose();
        this.cancellationTokenSource = new();

        void action(Task task)
        {
            if (task.IsCompletedSuccessfully)
            {
                lock (this.@lock)
                {
                    this.RecompileShader(trigger);
                }
            }
        }

        Task.Delay(DEBOUNCE_DELAY, this.cancellationTokenSource.Token)
            .ContinueWith(action, TaskScheduler.Default);
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        var filepath = Path.GetFullPath(e.FullPath);

        if (filepath == this.filepath)
        {
            this.RecompileShaderWithDebounce();
        }
        else if (this.dependenciesHash.ContainsKey(filepath))
        {
            this.RecompileShaderWithDebounce(filepath);
        }
    }

    [MemberNotNull(nameof(pipeline), nameof(pipelineLayout), nameof(descriptorSetLayout))]
    private void UpdatePipeline(SlangCompileRequest compileRequest)
    {
        var reflection = compileRequest.GetReflection();

        using var bindings                       = new RefList<VkDescriptorSetLayoutBinding>((int)reflection.ParameterCount);
        using var pipelineShaderStageCreateInfos = new RefList<VkPipelineShaderStageCreateInfo>((int)reflection.EntryPointCount);
        using var pushConstantRanges             = new RefList<VkPushConstantRange>();

        var entryPoints = reflection.EntryPoints;
        var parameters  = reflection.Parameters;
        var fields      = reflection.GlobalParamsTypeLayout.Fields;

        using var disposables     = new Disposables();
        using var entryPointNames = new NativeStringList(entryPoints.Length);

        VkShaderStageFlags stages = default;

        for (var i = 0; i < entryPoints.Length; i++)
        {
            var entryPoint = entryPoints[i];

            entryPointNames.Add(entryPoint.Name);

            var shaderModule = this.CreateShaderModule(compileRequest.GetEntryPointCode(i));

            disposables.Add(shaderModule);

            var stage = SlangStageToVkShaderStageFlags(entryPoint.Stage);

            stages |= stage;

            var createInfo = new VkPipelineShaderStageCreateInfo()
            {
                Module = shaderModule.Handle,
                PName  = (byte*)entryPointNames.GetHandle(i),
                Stage  = stage,
            };

            pipelineShaderStageCreateInfos.Add(createInfo);
        }

        using var uniformBindings = new RefList<VkDescriptorType>(parameters.Length);

        for (var i = 0; i < parameters.Length; i++)
        {
            var field      = fields[i];
            var typeLayout = field.TypeLayout;
            var binding    = parameters[i].BindingIndex;

            switch (typeLayout)
            {
                case { Kind: SlangTypeKind.ConstantBuffer, ParameterCategory: SlangParameterCategory.PushConstantBuffer }:
                    {
                        var pushConstantRange = new VkPushConstantRange
                        {
                            Size       = (uint)typeLayout.ElementTypeLayout!.ParameterSize,
                            Offset     = (uint)typeLayout.ElementVarLayout!.ParameterOffset,
                            StageFlags = this.pushConstantStages = stages,
                        };

                        pushConstantRanges.Add(pushConstantRange);
                    }

                    break;
                case { Kind: SlangTypeKind.ConstantBuffer, ParameterCategory: SlangParameterCategory.DescriptorTableSlot }:
                    {
                        uniformBindings.Add(VkDescriptorType.UniformBuffer);

                        var layout = new VkDescriptorSetLayoutBinding()
                        {
                            Binding         = binding,
                            DescriptorCount = 1,
                            DescriptorType  = VkDescriptorType.UniformBuffer,
                            StageFlags      = stages,
                        };

                        bindings.Add(layout);
                    }

                    break;
                case { Kind: SlangTypeKind.Resource, Type.ResourceShape: SlangResourceShape.Texture2D }:
                    {
                        uniformBindings.Add(VkDescriptorType.CombinedImageSampler);

                        var layout = new VkDescriptorSetLayoutBinding()
                        {
                            Binding         = binding,
                            DescriptorCount = 1,
                            DescriptorType  = VkDescriptorType.CombinedImageSampler,
                            StageFlags      = stages,
                        };

                        bindings.Add(layout);
                    }
                    break;
            }
        }

        this.uniformBindings.ResizeCopy(uniformBindings);

        using var vertexInputAttributeDescription = TVertexInput.GetAttributes();
        var vertexInputBindingDescription = TVertexInput.GetBindings();

        Span<VkDynamicState> dynamicStates =
        [
            VkDynamicState.Viewport,
            VkDynamicState.Scissor,
        ];

        fixed (VkDescriptorSetLayoutBinding*      pBindings                       = bindings.AsSpan())
        fixed (VkVertexInputAttributeDescription* pVertexAttributeDescriptions    = vertexInputAttributeDescription.AsSpan())
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

    protected override void OnDisposed()
    {
        watcher.Filters.Remove(this.filepath);

        watcher.Changed -= this.OnFileChanged;

        VulkanRenderer.Singleton.DeferredDispose(this.Pipeline);
        VulkanRenderer.Singleton.DeferredDispose(this.PipelineLayout);
        VulkanRenderer.Singleton.DeferredDispose(this.DescriptorSetLayout);
    }
}
