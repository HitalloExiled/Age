using Age.Core.Collections;
using Age.Core.Extensions;
using Age.Core;
using Age.Rendering.Resources;
using Age.Rendering.Vulkan;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ThirdParty.Slang;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan;

namespace Age.Rendering;

public partial class ShaderCompiler : Disposable
{
    private const string GLSL           = "glsl";
    private const int    DEBOUNCE_DELAY = 50;

    private readonly Watcher? watcher;

    [MemberNotNullWhen(true, nameof(watcher))]
    private bool Watching { get; }

    public ShaderCompiler(bool watch)
    {
        if (this.Watching = watch)
        {
            this.watcher = new();

            this.watcher.Changed += this.OnFileChanged;
        }
    }

    private void OnFileChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
    {
        var filepath = Path.GetFullPath(fileSystemEventArgs.FullPath);

        ref var fileEntry = ref this.watcher!.Files.GetValueRefOrNullRef(filepath);

        if (Unsafe.IsNullRef(ref fileEntry))
        {
            return;
        }

        var token = fileEntry.RefreshToken();

        void action(Task task)
        {
            if (task.IsCompletedSuccessfully && !token.IsCancellationRequested)
            {
                Logger.Info($"FileChanged: {filepath}");

                this.OnFileChanged(filepath);
            }
        }

        Task.Delay(DEBOUNCE_DELAY, token)
            .ContinueWith(action, TaskScheduler.Default);
    }

    private void OnFileChanged(string filepath)
    {
        using var bytes = FileReader.ReadAllBytesAsRef(filepath);

        var dependencyHasChanged = !this.watcher!.Files.TryGetValue(filepath, out var fileEntry) || fileEntry.Hash != MD5Hash.Create(bytes);

        if (dependencyHasChanged && this.watcher!.Dependencies.TryGetValue(filepath, out var dependents))
        {
            foreach (var dependent in dependents)
            {
                this.Recompile(dependent, true);
            }
        }
        else
        {
            this.Recompile(filepath, false);
        }
    }

    private static unsafe VkShaderModule CreateShaderModule(ReadOnlySpan<byte> buffer)
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static VkShaderStageFlags SlangStageToVkShaderStageFlags(SlangStage stage) =>
        stage switch
        {
            SlangStage.Vertex   => VkShaderStageFlags.Vertex,
            SlangStage.Fragment => VkShaderStageFlags.Fragment,
            _ => throw new InvalidOperationException("Unsuported shader stage"),
        };

    private unsafe void CreateResources(SlangCompileRequest compileRequest, Shader shader, in ShaderOptions shaderOptions)
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

            var shaderModule = CreateShaderModule(compileRequest.GetEntryPointCode(i));

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
                            StageFlags = stages,
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

        using var vertexInputAttributeDescription = shader.GetAttributes();
        var vertexInputBindingDescription         = shader.GetBindings();

        Span<VkDynamicState> dynamicStates =
        [
            VkDynamicState.Scissor,
            VkDynamicState.StencilReference,
            VkDynamicState.Viewport,
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

            var descriptorSetLayout = VulkanRenderer.Singleton.Context.Device.CreateDescriptorSetLayout(descriptorSetLayoutCreateInfo);

            var descriptorSetLayoutHandle = descriptorSetLayout.Handle;

            var pipelineLayoutCreateInfo = new VkPipelineLayoutCreateInfo
            {
                PSetLayouts            = &descriptorSetLayoutHandle,
                SetLayoutCount         = 1,
                PPushConstantRanges    = pPushConstantRanges,
                PushConstantRangeCount = (uint)pushConstantRanges.Count,
            };

            var pipelineLayout = VulkanRenderer.Singleton.Context.Device.CreatePipelineLayout(pipelineLayoutCreateInfo);

            var inputAssembly = new VkPipelineInputAssemblyStateCreateInfo
            {
                Topology = shader.PrimitiveTopology,
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

            var isStencilMask = shaderOptions.StencilOp != StencilOp.None;

            var pipelineColorBlendAttachmentState = new VkPipelineColorBlendAttachmentState
            {
                AlphaBlendOp        = VkBlendOp.Add,
                BlendEnable         = !isStencilMask,
                ColorWriteMask      = !isStencilMask ? VkColorComponentFlags.All : default,
                DstColorBlendFactor = VkBlendFactor.OneMinusSrcAlpha,
                SrcAlphaBlendFactor = VkBlendFactor.One,
                SrcColorBlendFactor = VkBlendFactor.SrcAlpha,
            };

            var pipelineColorBlendStateCreateInfo = new VkPipelineColorBlendStateCreateInfo
            {
                LogicOpEnable   = false,
                AttachmentCount = 1,
                LogicOp         = VkLogicOp.Copy,
                PAttachments    = &pipelineColorBlendAttachmentState,
            };

            var pipelineMultisampleStateCreateInfo = new VkPipelineMultisampleStateCreateInfo
            {
                SampleShadingEnable  = !isStencilMask,
                RasterizationSamples = (VkSampleCountFlags)shaderOptions.RasterizationSamples,
                MinSampleShading     = 1,
            };

            var pipelineRasterizationStateCreateInfo = new VkPipelineRasterizationStateCreateInfo
            {
                CullMode    = !isStencilMask ? VkCullModeFlags.Back : default,
                FrontFace   = shader.FrontFace,
                LineWidth   = 1,
                PolygonMode = VkPolygonMode.Fill,
            };

            var pipelineViewportStateCreateInfo = new VkPipelineViewportStateCreateInfo
            {
                ViewportCount = 1,
                ScissorCount  = 1,
            };

            var stencilOp = shaderOptions.StencilOp switch
            {
                StencilOp.Write => new VkStencilOpState
                {
                    CompareMask = 0xFF,
                    CompareOp   = VkCompareOp.Always,
                    DepthFailOp = VkStencilOp.Replace,
                    FailOp      = VkStencilOp.Keep,
                    PassOp      = VkStencilOp.IncrementAndClamp,
                    WriteMask   = 0xFF,
                },
                StencilOp.Erase => new VkStencilOpState
                {
                    CompareMask = 0xFF,
                    CompareOp   = VkCompareOp.Always,
                    DepthFailOp = VkStencilOp.Replace,
                    FailOp      = VkStencilOp.Keep,
                    PassOp      = VkStencilOp.DecrementAndClamp,
                    WriteMask   = 0xFF,
                },
                _ => new VkStencilOpState
                {
                    CompareMask = 0xFF,
                    CompareOp   = VkCompareOp.Equal,
                    DepthFailOp = VkStencilOp.Keep,
                    FailOp      = VkStencilOp.Keep,
                    PassOp      = VkStencilOp.Replace,
                    WriteMask   = 0x00,
                },
            };

            var depthStencilState = new VkPipelineDepthStencilStateCreateInfo
            {
                DepthTestEnable   = !isStencilMask,
                DepthWriteEnable  = !isStencilMask,
                DepthCompareOp    = isStencilMask ? VkCompareOp.Always : VkCompareOp.LessOrEqual,
                StencilTestEnable = true,
                Front             = stencilOp,
                Back              = stencilOp,
            };

            var graphicsPipelineCreateInfo = new VkGraphicsPipelineCreateInfo
            {
                Layout              = pipelineLayout.Handle,
                PColorBlendState    = &pipelineColorBlendStateCreateInfo,
                PDynamicState       = &pipelineDynamicStateCreateInfo,
                PInputAssemblyState = &inputAssembly,
                PMultisampleState   = &pipelineMultisampleStateCreateInfo,
                PRasterizationState = &pipelineRasterizationStateCreateInfo,
                PStages             = pPipelineShaderStageCreateInfos,
                PVertexInputState   = &pipelineVertexInputStateCreateInfo,
                PViewportState      = &pipelineViewportStateCreateInfo,
                StageCount          = (uint)pipelineShaderStageCreateInfos.Count,
                RenderPass          = shader.RenderPass.Handle,
                PDepthStencilState  = &depthStencilState,
                Subpass             = shaderOptions.Subpass,
            };

            shader.Pipeline            = VulkanRenderer.Singleton.Context.Device.CreateGraphicsPipelines(graphicsPipelineCreateInfo);
            shader.PipelineLayout      = pipelineLayout;
            shader.DescriptorSetLayout = descriptorSetLayout;
            shader.PushConstantStages  = stages;
            shader.UniformBindings     = [..uniformBindings];

            shader.IsCompiled = true;
        }
    }

    private void Recompile(string filepath, bool dependencyHasChanged)
    {
        if (this.watcher!.Shaders.TryGetValue(filepath, out var entries))
        {
            foreach (var entry in entries.Values)
            {
                var shader        = entry.Shader;
                var shaderOptions = entry.ShaderOptions;

                lock (entry.Lock)
                {
                    using var source = FileReader.ReadAllBytesAsRef(shader.Filepath);

                    var hasChanged = dependencyHasChanged || !this.watcher!.Files.TryGetValue(shader.Filepath, out var fileEntry) || fileEntry.Hash != MD5Hash.Create(source);

                    if (hasChanged && shader.IsCompiled)
                    {
                        try
                        {
                            Span<IDisposable> disposables = [shader.Pipeline, shader.PipelineLayout, shader.DescriptorSetLayout];

                            this.CompileShader(shader, source, shaderOptions);

                            VulkanRenderer.Singleton.DeferredDispose(disposables);

                            shader.InvokeChanged();
                        }
                        catch (Exception e)
                        {
                            Logger.Error(e.Message);
                        }
                    }
                }
            }
        }
    }

    protected override void OnDisposed(bool disposing) => throw new NotImplementedException();

    private void CompileShader(Shader shader, ReadOnlySpan<byte> source, in ShaderOptions shaderOptions)
    {
        Logger.Trace($"Compiling Shader [{shader.Filepath}]");

        var start = Stopwatch.GetTimestamp();

        using var session = new SlangSession();
        using var request = new SlangCompileRequest(session);

        var translationUnitIndex = request.AddTranslationUnit(SlangSourceLanguage.Slang, Path.GetFileName(shader.Filepath.AsSpan()));

        request.AddTranslationUnitSourceString(translationUnitIndex, shader.Filepath, source);
        request.SetCodeGenTarget(SlangCompileTarget.Spirv);
        request.SetTargetProfile(0, session.FindProfile("spirv_1_0"));

        if (request.Compile())
        {
            this.CreateResources(request, shader, shaderOptions);

            Logger.Info($"Shader {shader.Filepath} compiled in {Stopwatch.GetElapsedTime(start).Milliseconds}ms.");

            if (this.Watching)
            {
                var dependencies = request.GetDependencyFiles().AsSpan(1);

                if (dependencies.Length > 0 && dependencies[^1] == GLSL)
                {
                    dependencies = dependencies[..^1];
                }

                this.watcher.Update(shader, dependencies);

                this.watcher.Files[shader.Filepath] = new FileEntry(MD5Hash.Create(source));
            }
        }
        else
        {
            var diagnostic = request.GetDiagnosticOutput()!;

            Logger.Error(diagnostic);

            throw new ShaderCompilationException(diagnostic);
        }
    }

    public void CompileShader(Shader shader, in ShaderOptions shaderOptions)
    {
        if (this.Watching)
        {
            this.watcher.Watch(shader, shaderOptions);
        }

        using var source = FileReader.ReadAllBytesAsRef(shader.Filepath);

        this.CompileShader(shader, source, shaderOptions);
    }
}
