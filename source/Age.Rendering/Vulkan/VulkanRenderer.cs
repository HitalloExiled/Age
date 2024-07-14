using System.Numerics;
using System.Runtime.InteropServices;
using Age.Core;
using Age.Core.Extensions;
using Age.Core.Interop;
using Age.Numerics;
using Age.Rendering.Interfaces;
using Age.Rendering.Resources;
using Age.Rendering.Shaders;
using ThirdParty.SpirvCross;
using ThirdParty.SpirvCross.Enums;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

using Buffer = Age.Rendering.Resources.Buffer;

namespace Age.Rendering.Vulkan;

#pragma warning disable CA1822 // TODO Remove;

public unsafe partial class VulkanRenderer : IDisposable
{
    public event Action? SwapchainRecreated;
    private const ushort MAX_DESCRIPTORS_PER_POOL        = 64;
    private const ushort FRAMES_BETWEEN_PENDING_DISPOSES = 2;

    private static VulkanRenderer? singleton;

    public static VulkanRenderer Singleton => singleton ?? throw new NullReferenceException();

    private readonly object padlock = new();

    private readonly Dictionary<VkCommandBuffer, CommandBuffer> commandBuffers = [];
    private readonly VulkanContext                              context = new();
    private readonly Queue<IDisposable>                         pendingDisposes = new();

    private ushort framesUntilPendingDispose;
    private bool   disposed;

    public uint CurrentFrame => this.context.Frame.Index;

    public CommandBuffer CurrentCommandBuffer
    {
        get
        {
            var vkCommandBuffer = this.context.Frame.CommandBuffer;

            if (!this.commandBuffers.TryGetValue(vkCommandBuffer, out var commandBuffer))
            {
                this.commandBuffers[vkCommandBuffer] = commandBuffer = new(vkCommandBuffer, false);
            }

            return commandBuffer;
        }
    }

    public VkQueue GraphicsQueue => this.context.GraphicsQueue;

    public VkFormat           DepthBufferFormat   { get; private set; }
    public VkSampleCountFlags MaxUsableSampleCount { get; private set; }

    public VulkanRenderer()
    {
        singleton = this;

        this.context.SwapchainRecreated += () => this.SwapchainRecreated?.Invoke();
        this.context.DeviceInitialized  += this.OnContextDeviceInitialized;
    }

    private unsafe static VkBool32 DebugCallback(VkDebugUtilsMessageSeverityFlagsEXT messageSeverity, VkDebugUtilsMessageTypeFlagsEXT messageTypes, VkDebugUtilsMessengerCallbackDataEXT* pCallbackData, void* pUserData)
    {
        var loglevel = messageSeverity switch
        {
            VkDebugUtilsMessageSeverityFlagsEXT.Error   => LogLevel.Error,
            VkDebugUtilsMessageSeverityFlagsEXT.Warning => LogLevel.Warning,
            VkDebugUtilsMessageSeverityFlagsEXT.Info    => LogLevel.Info,
            _ => LogLevel.None
        };

        Logger.Log(Marshal.PtrToStringAnsi((nint)pCallbackData->PMessage)!, loglevel);

        return true;
    }

    private VkDescriptorSet[] CreateDescriptorSets(VkDescriptorPool descriptorPool, VkDescriptorSetLayout descriptorSetLayout)
    {
        var descriptorSetLayouts = new VkHandle<VkDescriptorSetLayout>[VulkanContext.MAX_FRAMES_IN_FLIGHT]
        {
            descriptorSetLayout.Handle,
            descriptorSetLayout.Handle,
        };

        fixed (VkHandle<VkDescriptorSetLayout>* pSetLayouts = descriptorSetLayouts)
        {
            var descriptorSetAllocateInfo = new VkDescriptorSetAllocateInfo
            {
                DescriptorSetCount = VulkanContext.MAX_FRAMES_IN_FLIGHT,
                PSetLayouts        = pSetLayouts,
            };

            return descriptorPool.AllocateDescriptorSets(descriptorSetAllocateInfo);
        }
    }

    private VkImageView CreateImageView(Image image, VkImageAspectFlags aspect)
    {
        var imageViewCreateInfo = new VkImageViewCreateInfo
        {
            Format           = image.Format,
            Image            = image.Value.Handle,
            SubresourceRange = new()
            {
                AspectMask = aspect,
                LayerCount = 1,
                LevelCount = 1,
            },
            ViewType = VkImageViewType.N2D,
        };

        var imageView = this.context.Device.CreateImageView(imageViewCreateInfo);

        return imageView;
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

            return this.context.Device.CreateShaderModule(createInfo);
        }
    }

    private void CreatePipeline<TShader, TVertexInput, TPushConstant>(
        TShader          shaderResources,
        RenderPass                renderPass,
        out VkPipeline            pipeline,
        out VkPipelineLayout      pipelineLayout,
        out VkDescriptorSetLayout descriptorSetLayout,
        out VkDescriptorType[]    uniformBindings
    )
    where TShader : Shader<TVertexInput, TPushConstant>
    where TVertexInput     : IVertexInput
    where TPushConstant    : IPushConstant
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

            foreach (var stage in shaderResources.Stages)
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

            uniformBindings = bindings.OrderBy(x => x.Binding).Select(x => x.DescriptorType).ToArray();

            var vertexInputAttributeDescription = TVertexInput.GetAttributes();
            var vertexInputBindingDescription   = TVertexInput.GetBindings();

            var dynamicStates = new VkDynamicState[]
            {
                VkDynamicState.Viewport,
                VkDynamicState.Scissor,
            };

            fixed (VkDescriptorSetLayoutBinding*      pBindings                       = CollectionsMarshal.AsSpan(bindings))
            fixed (VkVertexInputAttributeDescription* pVertexAttributeDescriptions    = vertexInputAttributeDescription)
            fixed (VkDynamicState*                    pDynamicStates                  = dynamicStates)
            fixed (VkPipelineShaderStageCreateInfo*   pPipelineShaderStageCreateInfos = CollectionsMarshal.AsSpan(pipelineShaderStageCreateInfos))
            fixed (VkPushConstantRange*               pPushConstantRanges             = CollectionsMarshal.AsSpan(pushConstantRanges))
            {
                var descriptorSetLayoutCreateInfo = new VkDescriptorSetLayoutCreateInfo
                {
                    PBindings    = pBindings,
                    BindingCount = (uint)bindings.Count,
                };

                descriptorSetLayout = this.context.Device.CreateDescriptorSetLayout(descriptorSetLayoutCreateInfo);

                var descriptorSetLayoutHandle = descriptorSetLayout.Handle;

                var pipelineLayoutCreateInfo = new VkPipelineLayoutCreateInfo
                {
                    PSetLayouts            = &descriptorSetLayoutHandle,
                    SetLayoutCount         = 1,
                    PPushConstantRanges    = pPushConstantRanges,
                    PushConstantRangeCount = (uint)pushConstantRanges.Count,
                };

                pipelineLayout = this.context.Device.CreatePipelineLayout(pipelineLayoutCreateInfo);

                var inputAssembly = new VkPipelineInputAssemblyStateCreateInfo
                {
                    Topology = shaderResources.PrimitiveTopology,
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
                    RasterizationSamples = shaderResources.RasterizationSamples,
                    MinSampleShading     = 1,
                };

                var pipelineRasterizationStateCreateInfo = new VkPipelineRasterizationStateCreateInfo
                {
                    CullMode    = VkCullModeFlags.Back,
                    FrontFace   = shaderResources.FrontFace,
                    LineWidth   = 1,
                    PolygonMode = VkPolygonMode.Fill,
                };

                var pipelineViewportStateCreateInfo = new VkPipelineViewportStateCreateInfo
                {
                    ViewportCount = 1,
                    ScissorCount  = 1,
                };

                var depthStencilState = new VkPipelineDepthStencilStateCreateInfo
                {
                    DepthTestEnable       = true,
                    DepthWriteEnable      = true,
                    DepthCompareOp        = VkCompareOp.Less,
                    DepthBoundsTestEnable = false,
                    StencilTestEnable     = false
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
                    RenderPass          = renderPass.Value.Handle,
                    PDepthStencilState  = &depthStencilState,
                };

                pipeline = this.context.Device.CreateGraphicsPipelines(graphicsPipelineCreateInfo);
            }
        }
    }

    private void DisposePendingResources(bool immediate = false)
    {
        if (this.pendingDisposes.Count > 0)
        {
            if (immediate || this.framesUntilPendingDispose == 0)
            {
                while (this.pendingDisposes.Count > 0)
                {
                    this.pendingDisposes.Dequeue().Dispose();
                }
            }
            else
            {
                this.framesUntilPendingDispose--;
            }
        }
    }

    private void OnContextDeviceInitialized()
    {
        this.DepthBufferFormat = this.FindSupportedFormat([VkFormat.D32Sfloat, VkFormat.D32SfloatS8Uint, VkFormat.D24UnormS8Uint], VkImageTiling.Optimal, VkFormatFeatureFlags.DepthStencilAttachment);

        this.context.Device.PhysicalDevice.GetProperties(out var physicalDeviceProperties);

        var counts = physicalDeviceProperties.Limits.FramebufferColorSampleCounts & physicalDeviceProperties.Limits.FramebufferDepthSampleCounts;

        this.MaxUsableSampleCount = counts.HasFlag(VkSampleCountFlags.N64)
            ? VkSampleCountFlags.N64
            : counts.HasFlag(VkSampleCountFlags.N32)
                ? VkSampleCountFlags.N32
                : counts.HasFlag(VkSampleCountFlags.N16)
                    ? VkSampleCountFlags.N16
                    : counts.HasFlag(VkSampleCountFlags.N8)
                        ? VkSampleCountFlags.N8
                        : counts.HasFlag(VkSampleCountFlags.N4)
                            ? VkSampleCountFlags.N4
                            : counts.HasFlag(VkSampleCountFlags.N2)
                                ? VkSampleCountFlags.N2
                                : VkSampleCountFlags.N1;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            this.context.Device.WaitIdle();

            this.DisposePendingResources(true);

            DescriptorPool.Clear();

            this.context.Dispose();

            this.disposed = true;
        }
    }

    public CommandBuffer AllocateCommand(VkCommandBufferLevel commandBufferLevel) =>
        new(this.context.AllocateCommand(commandBufferLevel), true);

    public void BeginFrame()
    {
        this.DisposePendingResources();

        this.context.PrepareBuffers();

        if (this.context.Frame.BufferPrepared)
        {
            this.context.Frame.CommandBuffer.Begin();
        }
    }

    public CommandBuffer BeginSingleTimeCommands()
    {
        var commandBuffer = this.context.Frame.CommandPool.AllocateCommand(VkCommandBufferLevel.Primary);

        commandBuffer.Begin(VkCommandBufferUsageFlags.OneTimeSubmit);

        return new(commandBuffer, true);
    }

    public Buffer CreateBuffer(ulong size, VkBufferUsageFlags usage, VkMemoryPropertyFlags properties)
    {
        var bufferCreateInfo = new VkBufferCreateInfo
        {
            Size  = size,
            Usage = usage,
        };

        var device = this.context.Device;

        var buffer = device.CreateBuffer(bufferCreateInfo);

        buffer.GetMemoryRequirements(out var memRequirements);

        var memoryType = this.context.FindMemoryType(memRequirements.MemoryTypeBits, properties);

        var memoryAllocateInfo = new VkMemoryAllocateInfo
        {
            AllocationSize  = memRequirements.Size,
            MemoryTypeIndex = memoryType
        };

        var memory = device.AllocateMemory(memoryAllocateInfo);

        buffer.BindMemory(memory, 0);

        return new(buffer)
        {
            Allocation = new()
            {
                Alignment  = memRequirements.Alignment,
                Memory     = memory,
                Memorytype = memoryType,
                Offset     = 0,
                Size       = size,
            },
            Usage = usage,
        };
    }

    public DescriptorPool CreateDescriptorPool(VkDescriptorType descriptorType) =>
        DescriptorPool.CreateDescriptorPool(this.context.Device, descriptorType);

    public Image CreateImage(in VkImageCreateInfo createInfo)
    {
        var image = this.context.Device.CreateImage(createInfo);

        image.GetMemoryRequirements(out var memRequirements);

        var memoryType = this.context.FindMemoryType(memRequirements.MemoryTypeBits, VkMemoryPropertyFlags.DeviceLocal);

        var memoryAllocateInfo = new VkMemoryAllocateInfo
        {
            AllocationSize  = memRequirements.Size,
            MemoryTypeIndex = memoryType,
        };

        var deviceMemory = this.context.Device.AllocateMemory(memoryAllocateInfo);

        image.BindMemory(deviceMemory, 0);

        return new(image)
        {
            Allocation = new()
            {
                Alignment  = memRequirements.Alignment,
                Memory     = deviceMemory,
                Memorytype = memoryType,
                Offset     = 0,
                Size       = memRequirements.Size,
            },
            Extent = createInfo.Extent,
            Format = createInfo.Format,
            Type   = createInfo.ImageType,
            Usage  = createInfo.Usage,
        };
    }

    public Image[] CreateImage(in VkImageCreateInfo createInfo, int count)
    {
        var images = new Image[count];

        for (var i = 0; i < count; i++)
        {
            images[i] = this.CreateImage(createInfo);
        }

        return images;
    }

    public IndexBuffer CreateIndexBuffer(Span<ushort> indices) =>
        this.CreateIndexBuffer(indices, VkIndexType.Uint16);

    public IndexBuffer CreateIndexBuffer(Span<uint> indices) =>
        this.CreateIndexBuffer(indices, VkIndexType.Uint32);

    public IndexBuffer CreateIndexBuffer<T>(Span<T> indices, VkIndexType indexType) where T : unmanaged, INumber<T>
    {
        var bufferSize = (ulong)(sizeof(T) * indices.Length);

        var buffer = this.CreateBuffer(
            bufferSize,
            VkBufferUsageFlags.TransferDst | VkBufferUsageFlags.IndexBuffer,
            VkMemoryPropertyFlags.DeviceLocal
        );

        buffer.Update([..indices]);

        return new()
        {
            Buffer = buffer,
            Type   = indexType,
            Size   = (uint)indices.Length,
        };
    }

    public Framebuffer CreateFramebuffer(in FramebufferCreateInfo createInfo)
    {
        var imageViews = new VkImageView[createInfo.Attachments.Length];

        for (var i = 0; i < createInfo.Attachments.Length; i++)
        {
            imageViews[i] = this.CreateImageView(createInfo.Attachments[i].Image, createInfo.Attachments[i].ImageAspect);
        }

        var extent = new VkExtent2D
        {
            Width  = createInfo.Attachments[0].Image.Extent.Width,
            Height = createInfo.Attachments[0].Image.Extent.Height,
        };

        var framebuffer = this.context.CreateFrameBuffer(createInfo.RenderPass.Value, imageViews.AsSpan(), extent);

        return new(framebuffer)
        {
            ImageViews = imageViews,
            Extent     = extent,
        };
    }

    public RenderPass CreateRenderPass(in RenderPassCreateInfo createInfo)
    {
        using var disposables = new Disposables();

        using var subpassDescriptions     = new NativeList<VkSubpassDescription>();
        using var attachmentDescriptions  = new NativeList<VkAttachmentDescription>();
        using var depthStencilAttachments = new NativeList<VkAttachmentDescription>();

        var renderPassSubPasses = new List<RenderPass.SubPass>(createInfo.SubPasses.Length);

        foreach (var subpass in createInfo.SubPasses)
        {
            var colorAttachmentReferences        = new NativeList<VkAttachmentReference>(subpass.ColorAttachments.Length);
            var resolveAttachmentReferences      = new NativeList<VkAttachmentReference>();
            var depthStencilAttachmentReferences = new NativeList<VkAttachmentReference>();

            var subPassColorAttachments = new NativeList<RenderPass.SubPass.ColorAttachment>(subpass.ColorAttachments.Length);

            disposables.Add(colorAttachmentReferences);
            disposables.Add(resolveAttachmentReferences);
            disposables.Add(depthStencilAttachmentReferences);

            foreach (var attachment in subpass.ColorAttachments)
            {
                colorAttachmentReferences.Add(new() { Attachment = (uint)attachmentDescriptions.Count, Layout = VkImageLayout.ColorAttachmentOptimal });
                attachmentDescriptions.Add(attachment.Color);

                if (attachment.Resolve.HasValue)
                {
                    resolveAttachmentReferences.Add(new() { Attachment = (uint)attachmentDescriptions.Count, Layout = VkImageLayout.ColorAttachmentOptimal });
                    attachmentDescriptions.Add(attachment.Resolve.Value);
                }

                var subPassColorAttachment = new RenderPass.SubPass.ColorAttachment
                {
                    Color = new()
                    {
                        Format  = attachment.Color.Format,
                        Samples = attachment.Color.Samples,
                    },
                    Resolve = attachment.Resolve.HasValue
                        ? new()
                        {
                            Format  = attachment.Resolve.Value.Format,
                            Samples = attachment.Resolve.Value.Samples,
                        } : default,
                };

                subPassColorAttachments.Add(subPassColorAttachment);
            }

            if (subpass.DepthStencilAttachment.HasValue)
            {
                depthStencilAttachmentReferences.Add(new() { Attachment = (uint)attachmentDescriptions.Count, Layout = VkImageLayout.DepthStencilAttachmentOptimal });
                attachmentDescriptions.Add(subpass.DepthStencilAttachment.Value);
            }

            var subpassDescription = new VkSubpassDescription
            {
                PipelineBindPoint       = subpass.PipelineBindPoint,
                PColorAttachments       = colorAttachmentReferences.AsPointer(),
                PResolveAttachments     = resolveAttachmentReferences.AsPointer(),
                ColorAttachmentCount    = (uint)colorAttachmentReferences.Count,
                PDepthStencilAttachment = depthStencilAttachmentReferences.AsPointer(),
            };

            subpassDescriptions.Add(subpassDescription);

            var renderPassSubPass = new RenderPass.SubPass
            {
                PipelineBindPoint = subpass.PipelineBindPoint,
                ColorAttachments  = [..subPassColorAttachments],
                DepthStencilAttachment = subpass.DepthStencilAttachment.HasValue
                    ? new()
                    {
                        Format  = subpass.DepthStencilAttachment.Value.Format,
                        Samples = subpass.DepthStencilAttachment.Value.Samples,
                    } : default,
            };

            renderPassSubPasses.Add(renderPassSubPass);
        }

        fixed (VkSubpassDependency* pDependencies = createInfo.SubpassDependencies)
        {
            var renderPassCreateInfo = new VkRenderPassCreateInfo
            {
                AttachmentCount = (uint)attachmentDescriptions.Count,
                PAttachments    = attachmentDescriptions.AsPointer(),
                DependencyCount = (uint)createInfo.SubpassDependencies.Length,
                PDependencies   = pDependencies,
                SubpassCount    = (uint)subpassDescriptions.Count,
                PSubpasses      = subpassDescriptions.AsPointer(),
            };

            var renderPass = this.context.Device.CreateRenderPass(renderPassCreateInfo);

            return new()
            {
                Value     = renderPass,
                SubPasses = [..renderPassSubPasses],
            };
        }
    }

    public RenderPipeline CreateRenderPipeline(in RenderPipelineCreateInfo createInfo)
    {
        var subPasses   = new RenderPassCreateInfo.SubPass[createInfo.SubPasses.Length];
        var attachments = new List<FramebufferCreateInfo.Attachment>();

        for (var subPassIndex = 0; subPassIndex < createInfo.SubPasses.Length; subPassIndex++)
        {
            ref var subPass      = ref createInfo.SubPasses[subPassIndex];
            var colorAttachments = new RenderPassCreateInfo.ColorAttachment[subPass.ColorAttachments.Length];

            ref var renderPassSubPass = ref subPasses[subPassIndex];

            for (var i = 0; i < subPass.ColorAttachments.Length; i++)
            {
                ref var colorAttachment = ref subPass.ColorAttachments[i];

                attachments.Add(new(this.CreateImage(colorAttachment.Color.Image), VkImageAspectFlags.Color));

                if (colorAttachment.Resolve.HasValue)
                {
                    attachments.Add(new(this.CreateImage(colorAttachment.Resolve.Value.Image), VkImageAspectFlags.Color));
                }

                colorAttachments[i] = new()
                {
                    Color   = colorAttachment.Color.Description,
                    Resolve = colorAttachment.Resolve?.Description,
                };
            }

            if (subPass.DepthStencilAttachment.HasValue)
            {
                attachments.Add(new(this.CreateImage(subPass.DepthStencilAttachment.Value.Image), VkImageAspectFlags.Depth));
            }

            subPasses[subPassIndex] = new()
            {
                PipelineBindPoint      = subPass.PipelineBindPoint,
                ColorAttachments       = colorAttachments,
                DepthStencilAttachment = subPass.DepthStencilAttachment?.Description,
            };
        }

        var renderPassCreateInfo = new RenderPassCreateInfo
        {
            SubpassDependencies = createInfo.SubpassDependencies,
            SubPasses           = subPasses,
        };

        var renderPass = this.CreateRenderPass(renderPassCreateInfo);

        var framebufferCreateInfo = new FramebufferCreateInfo
        {
            RenderPass  = renderPass,
            Attachments = [..attachments],
        };

        var framebuffer = this.CreateFramebuffer(framebufferCreateInfo);

        return new()
        {
            RenderPass  = renderPass,
            Framebuffer = framebuffer,
        };
    }

    public RenderTargetOld CreateRenderTarget(in RenderTargetCreateInfo createInfo)
    {
        var attachments = new List<FramebufferCreateInfo.Attachment>();

        var imageCreateInfo = new VkImageCreateInfo
        {
            ArrayLayers = 1,
            Extent      = new()
            {
                Width  = createInfo.Extent.Width,
                Height = createInfo.Extent.Height,
                Depth  = 1,
            },
            Format        = default,
            ImageType     = VkImageType.N2D,
            InitialLayout = VkImageLayout.Undefined,
            MipLevels     = 1,
            Samples       = default,
            Tiling        = VkImageTiling.Optimal,
        };

        var images = new List<Image>();

        Image? output = null;

        foreach (var subPass in createInfo.RenderPass.SubPasses)
        {
            foreach (var colorAttachments in subPass.ColorAttachments)
            {
                var colorImageCreateInfo = imageCreateInfo;

                colorImageCreateInfo.Format  = colorAttachments.Color.Format;
                colorImageCreateInfo.Samples = colorAttachments.Color.Samples;
                colorImageCreateInfo.Usage   = VkImageUsageFlags.TransientAttachment | VkImageUsageFlags.ColorAttachment;

                var colorImage = output = this.CreateImage(colorImageCreateInfo);

                images.Add(colorImage);

                attachments.Add(new FramebufferCreateInfo.Attachment(colorImage, VkImageAspectFlags.Color));

                if (colorAttachments.Resolve.HasValue)
                {
                    var resolveImageCreateInfo = colorImageCreateInfo;

                    resolveImageCreateInfo.Format  = colorAttachments.Resolve.Value.Format;
                    resolveImageCreateInfo.Samples = colorAttachments.Resolve.Value.Samples;
                    resolveImageCreateInfo.Usage   = VkImageUsageFlags.ColorAttachment | VkImageUsageFlags.TransferDst | VkImageUsageFlags.Sampled;

                    var resolveImage = output = this.CreateImage(resolveImageCreateInfo);

                    images.Add(resolveImage);

                    attachments.Add(new FramebufferCreateInfo.Attachment(resolveImage, VkImageAspectFlags.Color));
                }
            }

            if (subPass.DepthStencilAttachment.HasValue)
            {
                var depthImageCreateInfo = imageCreateInfo;

                depthImageCreateInfo.Format  = subPass.DepthStencilAttachment.Value.Format;
                depthImageCreateInfo.Samples = subPass.DepthStencilAttachment.Value.Samples;
                depthImageCreateInfo.Usage   = VkImageUsageFlags.DepthStencilAttachment;

                var depthImage = this.CreateImage(depthImageCreateInfo);

                images.Add(depthImage);

                attachments.Add(new FramebufferCreateInfo.Attachment(depthImage, VkImageAspectFlags.Depth));
            }
        }

        if (output == null)
        {
            foreach (var image in images.AsSpan())
            {
                image.Dispose();
            }

            throw new InvalidOperationException($"No valid color attachment founded on provided render pass");
        }

        var framebufferCreateInfo = new FramebufferCreateInfo
        {
            RenderPass  = createInfo.RenderPass,
            Attachments = attachments.AsSpan(),
        };

        var framebuffer = this.CreateFramebuffer(framebufferCreateInfo);

        return new(this.CreateTexture(output, true), framebuffer)
        {
            Dependencies = [..images]
        };
    }

    public Pipeline CreatePipeline<TShader, TVertexInput, TPushConstant>(TShader shader, RenderPass renderPass)
    where TShader       : Shader<TVertexInput, TPushConstant>
    where TVertexInput  : IVertexInput
    where TPushConstant : IPushConstant
    {
        this.CreatePipeline<TShader, TVertexInput, TPushConstant>(
            shader,
            renderPass,
            out var pipeline,
            out var pipelineLayout,
            out var descriptorSetLayout,
            out var uniformBindings
        );

        return new(pipeline, VkPipelineBindPoint.Graphics, shader, renderPass)
        {
            DescriptorSetLayout = descriptorSetLayout,
            Layout              = pipelineLayout,
            UniformBindings     = uniformBindings,
        };
    }

    public Pipeline CreatePipelineAndWatch<TShader, TVertexInput, TPushConstant>(TShader shader, RenderPass renderPass)
    where TShader       : Shader<TVertexInput, TPushConstant>
    where TVertexInput  : IVertexInput
    where TPushConstant : IPushConstant
    {
        var pipeline = this.CreatePipeline<TShader, TVertexInput, TPushConstant>(shader, renderPass);

        void action()
        {
            this.CreatePipeline<TShader, TVertexInput, TPushConstant>(
                shader,
                renderPass,
                out var vkPipeline,
                out var vkPipelineLayout,
                out var vkDescriptorSetLayout,
                out var uniformBindings
            );

            lock (this.padlock)
            {
                IDisposable disposables = new Disposables(pipeline.Value, pipeline.Layout, pipeline.DescriptorSetLayout);

                pipeline.Value               = vkPipeline;
                pipeline.Layout              = vkPipelineLayout;
                pipeline.DescriptorSetLayout = vkDescriptorSetLayout;
                pipeline.UniformBindings     = uniformBindings;

                this.DeferredDispose(disposables);
            }
        }

        pipeline.Changed += action;

        return pipeline;
    }

    public Sampler CreateSampler()
    {
        this.context.Device.PhysicalDevice.GetProperties(out var properties);

        var createInfo = new VkSamplerCreateInfo
        {
            AddressModeU  = VkSamplerAddressMode.Repeat,
            AddressModeV  = VkSamplerAddressMode.Repeat,
            AddressModeW  = VkSamplerAddressMode.Repeat,
            BorderColor   = VkBorderColor.IntOpaqueBlack,
            CompareOp     = VkCompareOp.Always,
            MagFilter     = VkFilter.Linear,
            MaxAnisotropy = properties.Limits.MaxSamplerAnisotropy,
            MaxLod        = 1,
            MinFilter     = VkFilter.Linear,
            MipmapMode    = VkSamplerMipmapMode.Linear,
        };

        return new(this.context.Device.CreateSampler(createInfo));
    }

    public Surface CreateSurface(nint handle, Size<uint> clientSize) =>
        this.context.CreateSurface(handle, clientSize);

    public Texture CreateTexture(in TextureCreateInfo textureCreateInfo)
    {
        var samples = VkSampleCountFlags.N1;
        var tiling  = VkImageTiling.Optimal;
        var usage   = VkImageUsageFlags.TransferSrc | VkImageUsageFlags.TransferDst | VkImageUsageFlags.Sampled;

        var imageCreateInfo = new VkImageCreateInfo
        {
            ArrayLayers   = 1,
            Extent        = new()
            {
                Width  = textureCreateInfo.Width,
                Height = textureCreateInfo.Height,
                Depth  = textureCreateInfo.Depth
            },
            Format        = textureCreateInfo.Format,
            ImageType     = textureCreateInfo.ImageType,
            InitialLayout = VkImageLayout.Undefined,
            MipLevels     = 1,
            Samples       = samples,
            Tiling        = tiling,
            Usage         = usage,
        };

        var image = this.CreateImage(imageCreateInfo);

        image.TransitionImageLayout(
            VkImageLayout.Undefined,
            VkImageLayout.TransferDstOptimal,
            default,
            VkAccessFlags.TransferWrite,
            VkPipelineStageFlags.TopOfPipe,
            VkPipelineStageFlags.Transfer
        );

        var imageView = this.CreateImageView(image, VkImageAspectFlags.Color);

        var texture = new Texture(true)
        {
            Image     = image,
            ImageView = imageView,
        };

        return texture;
    }

    public Texture CreateTexture(Image image, bool owner)
    {
        var imageView = this.CreateImageView(image, VkImageAspectFlags.Color);

        var texture = new Texture(owner)
        {
            Image     = image,
            ImageView = imageView,
        };

        return texture;
    }

    public Texture CreateTexture(in TextureCreateInfo textureCreate, Span<byte> data)
    {
        var texture = this.CreateTexture(textureCreate);

        texture.Update(data);

        return texture;
    }

    public VertexBuffer CreateVertexBuffer<T>(Span<T> data) where T : unmanaged
    {
        var size = (ulong)(data.Length * sizeof(T));
        var buffer = this.CreateBuffer(
            size,
            VkBufferUsageFlags.TransferDst | VkBufferUsageFlags.VertexBuffer,
            VkMemoryPropertyFlags.DeviceLocal
        );

        buffer.Update(data);

        return new()
        {
            Buffer = buffer,
            Size   = (uint)data.Length,
        };
    }

    public void DeferredDispose(IDisposable disposable)
    {
        this.framesUntilPendingDispose = FRAMES_BETWEEN_PENDING_DISPOSES;

        this.pendingDisposes.Enqueue(disposable);
    }

    public void DeferredDispose(IEnumerable<IDisposable> disposables)
    {
        this.framesUntilPendingDispose = FRAMES_BETWEEN_PENDING_DISPOSES;

        foreach (var disposable in disposables)
        {
            this.pendingDisposes.Enqueue(disposable);
        }
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void EndFrame()
    {
        if (this.context.Frame.BufferPrepared)
        {
            this.context.Frame.CommandBuffer.End();
        }

        this.context.SwapBuffers();
    }

    public void EndSingleTimeCommands(CommandBuffer commandBuffer)
    {
        commandBuffer.End();

        var commandBufferHandle = commandBuffer.Value.Handle;

        var submitInfo = new VkSubmitInfo
        {
            CommandBufferCount = 1,
            PCommandBuffers    = &commandBufferHandle
        };

        this.context.GraphicsQueue.Submit(submitInfo);
        this.context.GraphicsQueue.WaitIdle();

        commandBuffer.Dispose();
    }

    public VkFormat FindSupportedFormat(Span<VkFormat> candidates, VkImageTiling tiling, VkFormatFeatureFlags features) =>
        this.context.FindSupportedFormat(candidates, tiling, features);

    public void UpdateDescriptorSets(Span<VkWriteDescriptorSet> descriptorWrites, Span<VkCopyDescriptorSet> descriptorCopies) =>
        this.context.Device.UpdateDescriptorSets(descriptorWrites, descriptorCopies);

    public void WaitIdle() =>
        this.context.Device.WaitIdle();
}
