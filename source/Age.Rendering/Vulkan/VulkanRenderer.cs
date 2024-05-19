using System.Numerics;
using System.Runtime.InteropServices;
using Age.Core;
using Age.Core.Interop;
using Age.Numerics;
using Age.Rendering.Enums;
using Age.Rendering.Interfaces;
using Age.Rendering.Resources;
using Age.Rendering.Shaders;
using Age.Rendering.Vulkan.Uniforms;
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
    private const ushort MAX_DESCRIPTORS_PER_POOL        = 64;
    private const ushort FRAMES_BETWEEN_PENDING_DISPOSES = 2;

    private readonly object padlock = new();

    private readonly Queue<IDisposable> pendingDisposes = new();

    private ushort framesUntilPendingDispose;

    private bool disposed;

    public VulkanContext Context { get; } = new();

    private static VkDescriptorType ConvertToDescriptorType(UniformType type) =>
        type switch
        {
            UniformType.CombinedImageSampler => VkDescriptorType.CombinedImageSampler,
            UniformType.UniformBuffer        => VkDescriptorType.UniformBuffer,
            _ => throw new NotImplementedException(),
        };

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

    private DescriptorPool CreateDescriptorPool(VkDescriptorType descriptorType) =>
        DescriptorPool.CreateDescriptorPool(this.Context.Device, descriptorType);

    private void CreateImage(in VkImageCreateInfo createInfo, out VkImage image, out Allocation allocation)
    {
        image = this.Context.Device.CreateImage(createInfo);

        image.GetMemoryRequirements(out var memRequirements);

        var memoryType = this.Context.FindMemoryType(memRequirements.MemoryTypeBits, VkMemoryPropertyFlags.DeviceLocal);

        var memoryAllocateInfo = new VkMemoryAllocateInfo
        {
            AllocationSize  = memRequirements.Size,
            MemoryTypeIndex = memoryType,
        };

        var deviceMemory = this.Context.Device.AllocateMemory(memoryAllocateInfo);

        image.BindMemory(deviceMemory, 0);

        allocation = new()
        {
            Alignment  = memRequirements.Alignment,
            Memory     = deviceMemory,
            Memorytype = memoryType,
            Offset     = 0,
            Size       = memRequirements.Size,
        };
    }

    private VkImageView CreateImageView(VkImage image, VkFormat format, VkImageAspectFlags aspect)
    {
        var imageViewCreateInfo = new VkImageViewCreateInfo
        {
            Format           = format,
            Image            = image.Handle,
            SubresourceRange = new()
            {
                AspectMask = aspect,
                LayerCount = 1,
                LevelCount = 1,
            },
            ViewType = VkImageViewType.N2D,
        };

        var imageView = this.Context.Device.CreateImageView(imageViewCreateInfo);

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

            return this.Context.Device.CreateShaderModule(createInfo);
        }
    }

    public void CopyBuffer(VkBuffer srcBuffer, VkBuffer dstBuffer, ulong size)
    {
        var commandBuffer = this.Context.BeginSingleTimeCommands();

        var copyRegion = new VkBufferCopy
        {
            Size = size
        };

        commandBuffer.CopyBuffer(srcBuffer, dstBuffer, copyRegion);

        this.Context.EndSingleTimeCommands(commandBuffer);
    }

    public void CopyBufferToImage(VkBuffer buffer, VkImage image, uint width, uint height)
    {
        var commandBuffer = this.Context.BeginSingleTimeCommands();

        var bufferImageCopy = new VkBufferImageCopy
        {
            ImageExtent = new()
            {
                Depth  = 1,
                Height = height,
                Width  = width,
            },
            ImageSubresource = new()
            {
                AspectMask = VkImageAspectFlags.Color,
                LayerCount = 1,
            }
        };

        commandBuffer.CopyBufferToImage(buffer, image, VkImageLayout.TransferDstOptimal, [bufferImageCopy]);

        this.Context.EndSingleTimeCommands(commandBuffer);
    }

    public void CopyImageToBuffer(Image image, Buffer buffer, VkExtent3D extent)
    {
        var commandBuffer = this.Context.BeginSingleTimeCommands();

        var bufferImageCopy = new VkBufferImageCopy
        {
            ImageExtent      = extent,
            ImageSubresource = new()
            {
                AspectMask = VkImageAspectFlags.Color,
                LayerCount = 1,
            }
        };

        commandBuffer.CopyImageToBuffer(image.Value, VkImageLayout.TransferSrcOptimal, buffer.Value, [bufferImageCopy]);

        this.Context.EndSingleTimeCommands(commandBuffer);
    }

    private void CreateShader<TShaderResources, TVertexInput, TPushConstant>(TShaderResources shaderResources, RenderPass renderPass, out VkPipeline pipeline, out VkPipelineLayout pipelineLayout, out VkDescriptorSetLayout descriptorSetLayout)
    where TShaderResources : ShaderResources<TVertexInput, TPushConstant>
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

                descriptorSetLayout = this.Context.Device.CreateDescriptorSetLayout(descriptorSetLayoutCreateInfo);

                var descriptorSetLayoutHandle = descriptorSetLayout.Handle;

                var pipelineLayoutCreateInfo = new VkPipelineLayoutCreateInfo
                {
                    PSetLayouts            = &descriptorSetLayoutHandle,
                    SetLayoutCount         = 1,
                    PPushConstantRanges    = pPushConstantRanges,
                    PushConstantRangeCount = (uint)pushConstantRanges.Count,
                };

                pipelineLayout = this.Context.Device.CreatePipelineLayout(pipelineLayoutCreateInfo);

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
                    RasterizationSamples = VkSampleCountFlags.N1,
                };

                var pipelineRasterizationStateCreateInfo = new VkPipelineRasterizationStateCreateInfo
                {
                    CullMode    = VkCullModeFlags.Back,
                    FrontFace   = VkFrontFace.Clockwise,
                    LineWidth   = 1,
                    PolygonMode = VkPolygonMode.Fill,
                };

                var pipelineViewportStateCreateInfo = new VkPipelineViewportStateCreateInfo
                {
                    ViewportCount = 1,
                    ScissorCount  = 1,
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
                };

                pipeline = this.Context.Device.CreateGraphicsPipelines(graphicsPipelineCreateInfo);
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

    public void TransitionImageLayout(
        VkImage              image,
        VkImageLayout        oldLayout,
        VkImageLayout        newLayout,
        VkAccessFlags        srcAccessMask,
        VkAccessFlags        dstAccessMask,
        VkPipelineStageFlags sourceStage,
        VkPipelineStageFlags destinationStage
    )
    {
        var commandBuffer = this.Context.BeginSingleTimeCommands();

        var imageMemoryBarrier = new VkImageMemoryBarrier
        {
            DstAccessMask       = dstAccessMask,
            DstQueueFamilyIndex = VkConstants.VK_QUEUE_FAMILY_IGNORED,
            Image               = image.Handle,
            NewLayout           = newLayout,
            OldLayout           = oldLayout,
            SrcAccessMask       = srcAccessMask,
            SrcQueueFamilyIndex = VkConstants.VK_QUEUE_FAMILY_IGNORED,
            SubresourceRange    = new()
            {
                AspectMask = VkImageAspectFlags.Color,
                LayerCount = 1,
                LevelCount = 1,
            }
        };

        commandBuffer.PipelineBarrier(sourceStage, destinationStage, default, [], [], [imageMemoryBarrier]);

        this.Context.EndSingleTimeCommands(commandBuffer);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            this.Context.Device.WaitIdle();

            this.DisposePendingResources(true);

            DescriptorPool.Clear();

            this.Context.Dispose();

            this.disposed = true;
        }
    }

    public void BeginFrame()
    {
        this.DisposePendingResources();

        this.Context.PrepareBuffers();

        if (this.Context.Frame.BufferPrepared)
        {
            this.Context.Frame.CommandBuffer.Begin();
        }
    }

    public void BeginRenderPass(RenderPass renderPass, uint framebufferIndex, Color clearColor) =>
        this.BeginRenderPass(this.Context.Frame.CommandBuffer, renderPass, framebufferIndex, clearColor);

    public void BeginRenderPass(VkCommandBuffer commandBuffer, RenderPass renderPass, uint framebufferIndex, Color clearColor)
    {
        var clearValues = new VkClearValue[2];

        clearValues[0].Color.Float32[0] = clearColor.R;
        clearValues[0].Color.Float32[1] = clearColor.G;
        clearValues[0].Color.Float32[2] = clearColor.B;
        clearValues[0].Color.Float32[3] = clearColor.A;

        clearValues[1].DepthStencil = new()
        {
            Depth   = 1.0f,
            Stencil = 1
        };

        fixed (VkClearValue* pClearValues = clearValues)
        {
            var renderPassBeginInfo = new VkRenderPassBeginInfo
            {
                ClearValueCount = (uint)clearValues.Length,
                Framebuffer     = renderPass.Framebuffers[framebufferIndex].Value.Handle,
                PClearValues    = pClearValues,
                RenderArea      = new()
                {
                    Offset = new()
                    {
                        X = 0,
                        Y = 0
                    },
                    Extent = renderPass.Extent,
                },
                RenderPass = renderPass.Value.Handle,
            };

            commandBuffer.BeginRenderPass(renderPassBeginInfo, VkSubpassContents.Inline);
        }
    }

    public void BindIndexBuffer(IndexBuffer indexBuffer) =>
        this.BindIndexBuffer(this.Context.Frame.CommandBuffer, indexBuffer);

    public void BindIndexBuffer(VkCommandBuffer commandBuffer, IndexBuffer indexBuffer) =>
        commandBuffer.BindIndexBuffer(indexBuffer.Buffer.Value, 0, indexBuffer.Type);

    public void BindPipeline(Shader shader) =>
        this.BindPipeline(this.Context.Frame.CommandBuffer, shader);

    public void BindPipeline(VkCommandBuffer commandBuffer, Shader shader) =>
        commandBuffer.BindPipeline(shader.PipelineBindPoint, shader.Pipeline);

    public void BindVertexBuffers(VertexBuffer vertexBuffer) =>
        this.BindVertexBuffers(this.Context.Frame.CommandBuffer, vertexBuffer);

    public void BindVertexBuffers(VkCommandBuffer commandBuffer, VertexBuffer vertexBuffer) =>
        commandBuffer.BindVertexBuffers(0, 1, [vertexBuffer.Buffer.Value], [0]);

    public void BindVertexBuffer(VkCommandBuffer commandBuffer, VertexBuffer[] vertexBuffers) =>
        commandBuffer.BindVertexBuffers(0, 1, [.. vertexBuffers.Select(x => x.Buffer.Value)], new ulong[vertexBuffers.Length]);

    public void BindUniformSet(UniformSet uniformSet) =>
        this.BindUniformSet(this.Context.Frame.CommandBuffer, uniformSet);

    public void BindUniformSet(VkCommandBuffer commandBuffer, UniformSet uniformSet) =>
        commandBuffer.BindDescriptorSets(uniformSet.Shader.PipelineBindPoint, uniformSet.Shader.PipelineLayout, 0, uniformSet.DescriptorSets, []);

    public Buffer CreateBuffer(ulong size, VkBufferUsageFlags usage, VkMemoryPropertyFlags properties)
    {
        var bufferCreateInfo = new VkBufferCreateInfo
        {
            Size  = size,
            Usage = usage,
        };

        var device = this.Context.Device;

        var buffer = device.CreateBuffer(bufferCreateInfo);

        buffer.GetMemoryRequirements(out var memRequirements);

        var memoryType = this.Context.FindMemoryType(memRequirements.MemoryTypeBits, properties);

        var memoryAllocateInfo = new VkMemoryAllocateInfo
        {
            AllocationSize  = memRequirements.Size,
            MemoryTypeIndex = memoryType
        };

        var memory = device.AllocateMemory(memoryAllocateInfo);

        buffer.BindMemory(memory, 0);

        return new()
        {
            Allocation = new()
            {
                Alignment  = memRequirements.Alignment,
                Memory     = memory,
                Memorytype = memoryType,
                Offset     = 0,
                Size       = size,
            },
            Value = buffer,
            Usage = usage,
        };
    }

    public Image CreateImage(in VkImageCreateInfo createInfo)
    {
        this.CreateImage(createInfo, out var image, out var allocation);

        return new()
        {
            Value      = image,
            Allocation = allocation,
            Extent     = createInfo.Extent,
        };
    }

    public IndexBuffer CreateIndexBuffer(IList<ushort> indices) =>
        this.CreateIndexBuffer(indices, VkIndexType.Uint16);

    public IndexBuffer CreateIndexBuffer(IList<uint> indices) =>
        this.CreateIndexBuffer(indices, VkIndexType.Uint32);

    public IndexBuffer CreateIndexBuffer<T>(IList<T> indices, VkIndexType indexType) where T : unmanaged, INumber<T>
    {
        var bufferSize = (ulong)(sizeof(T) * indices.Count);

        var buffer = this.CreateBuffer(
            bufferSize,
            VkBufferUsageFlags.TransferDst | VkBufferUsageFlags.IndexBuffer,
            VkMemoryPropertyFlags.DeviceLocal
        );

        this.UpdateBuffer(buffer, [.. indices]);

        return new()
        {
            Buffer = buffer,
            Type   = indexType,
            Size   = (uint)indices.Count,
        };
    }

    public Shader CreateShader<TShaderResources, TVertexInput, TPushConstant>(TShaderResources shaderResources, RenderPass renderPass)
    where TShaderResources : ShaderResources<TVertexInput, TPushConstant>
    where TVertexInput     : IVertexInput
    where TPushConstant    : IPushConstant
    {
        this.CreateShader<TShaderResources, TVertexInput, TPushConstant>(shaderResources, renderPass, out var pipeline, out var pipelineLayout, out var descriptorSetLayout);

        return new(shaderResources, VkPipelineBindPoint.Graphics, pipeline, pipelineLayout, descriptorSetLayout, renderPass);
    }

    public Shader CreateShaderAndWatch<TShaderResources, TVertexInput, TPushConstant>(TShaderResources shaderResources, RenderPass renderPass)
    where TShaderResources : ShaderResources<TVertexInput, TPushConstant>
    where TVertexInput     : IVertexInput
    where TPushConstant    : IPushConstant
    {
        var shader = this.CreateShader<TShaderResources, TVertexInput, TPushConstant>(shaderResources, renderPass);

        void action()
        {
            this.CreateShader<TShaderResources, TVertexInput, TPushConstant>(shaderResources, shader.RenderPass, out var pipeline, out var pipelineLayout, out var descriptorSetLayout);

            lock (this.padlock)
            {
                this.DeferredDispose(shader.Update(pipeline, pipelineLayout, descriptorSetLayout));
            }
        }

        shader.Changed += action;

        return shader;
    }

    public VkSampler CreateSampler()
    {
        this.Context.GetPhysicalDeviceProperties(out var properties);

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

        return this.Context.Device.CreateSampler(createInfo);
    }

    public Texture CreateTexture(TextureCreate textureCreate)
    {
        var samples = VkSampleCountFlags.N1;
        var tiling  = VkImageTiling.Optimal;
        var usage   = VkImageUsageFlags.TransferSrc | VkImageUsageFlags.TransferDst | VkImageUsageFlags.Sampled;
        var format  = textureCreate.ColorMode == ColorMode.Grayscale ? VkFormat.R8G8Unorm : VkFormat.B8G8R8A8Unorm;

        var imageCreateInfo = new VkImageCreateInfo
        {
            ArrayLayers   = 1,
            Extent        = new()
            {
                Width  = textureCreate.Width,
                Height = textureCreate.Height,
                Depth  = textureCreate.Depth
            },
            Format        = format,
            ImageType     = VkImageType.N2D,
            InitialLayout = VkImageLayout.Undefined,
            MipLevels     = 1,
            Samples       = samples,
            Tiling        = tiling,
            Usage         = usage,
        };

        this.CreateImage(imageCreateInfo, out var image, out var allocation);

        this.TransitionImageLayout(
            image,
            VkImageLayout.Undefined,
            VkImageLayout.TransferDstOptimal,
            default,
            VkAccessFlags.TransferWrite,
            VkPipelineStageFlags.TopOfPipe,
            VkPipelineStageFlags.Transfer
        );

        var imageView = this.CreateImageView(image, format, VkImageAspectFlags.Color);

        var texture = new Texture
        {
            Allocation = allocation,
            Extent     = new()
            {
                Depth  = textureCreate.Depth,
                Height = textureCreate.Height,
                Width  = textureCreate.Width,
            },
            Image       = image,
            ImageView   = imageView,
            TextureType = textureCreate.TextureType,
        };

        return texture;
    }

    public UniformSet CreateUniformSet(Shader shader, Uniform[] uniforms)
    {
        using var disposables = new Disposables();

        var poolKey = uniforms.Select(x => ConvertToDescriptorType(x.Type)).Aggregate((previous, current) => previous | current);

        var writes = new List<VkWriteDescriptorSet>();

        var descriptorPool = this.CreateDescriptorPool(poolKey);

        var descriptorSetLayoutHandle = shader.DescriptorSetLayout.Handle;

        var descriptorSetAllocateInfo = new VkDescriptorSetAllocateInfo
        {
            DescriptorSetCount = 1,
            PSetLayouts        = &descriptorSetLayoutHandle,
        };

        var descriptorSets = descriptorPool.Value.AllocateDescriptorSets(descriptorSetAllocateInfo);

        foreach (var uniform in uniforms)
        {
            switch (uniform)
            {
                case CombinedImageSamplerUniform combinedImageSampler:
                {
                    var pImageInfo = new NativeArray<VkDescriptorImageInfo>(combinedImageSampler.Images.Count);

                    disposables.Add(pImageInfo);

                    for (var i = 0; i < combinedImageSampler.Images.Count; i++)
                    {
                        var image = combinedImageSampler.Images[i];

                        var descriptorImageInfo = new VkDescriptorImageInfo
                        {
                            Sampler     = image.Sampler.Value.Handle,
                            ImageView   = image.Texture.ImageView.Handle,
                            ImageLayout = VkImageLayout.ShaderReadOnlyOptimal,
                        };

                        pImageInfo[i] = descriptorImageInfo;
                    }

                    var writeDescriptorSet = new VkWriteDescriptorSet
                    {
                        DescriptorCount = 1,
                        DescriptorType  = VkDescriptorType.CombinedImageSampler,
                        DstBinding      = uniform.Binding,
                        DstSet          = descriptorSets[0].Handle,
                        PImageInfo      = pImageInfo,
                    };

                    writes.Add(writeDescriptorSet);

                    break;
                }
                case UniformBufferUniform uniformBuffer:
                {
                    var descriptorBufferInfo = new VkDescriptorBufferInfo
                    {
                        Buffer = uniformBuffer.Buffer.Value.Handle,
                        Offset = uniformBuffer.Buffer.Allocation.Offset,
                        Range  = uniformBuffer.Buffer.Allocation.Size,
                    };

                    var pBufferInfo = new NativeArray<VkDescriptorBufferInfo>([descriptorBufferInfo]);

                    disposables.Add(pBufferInfo);

                    var writeDescriptorSet = new VkWriteDescriptorSet
                    {
                        DescriptorCount = 1,
                        DescriptorType  = VkDescriptorType.UniformBuffer,
                        DstBinding      = uniform.Binding,
                        DstSet          = descriptorSets[0].Handle,
                        PBufferInfo     = pBufferInfo,
                    };

                    writes.Add(writeDescriptorSet);

                    break;
                }
                default:
                    throw new Exception();
            }

        }

        this.Context.Device.UpdateDescriptorSets([..writes], []);

        var uniformSet = new UniformSet()
        {
            DescriptorPool = descriptorPool,
            DescriptorSets = descriptorSets,
            Shader         = shader,
        };

        return uniformSet;
    }

    public VertexBuffer CreateVertexBuffer<T>(T[] data) where T : unmanaged
    {
        var size = (ulong)(data.Length * sizeof(T));
        var buffer = this.CreateBuffer(
            size,
            VkBufferUsageFlags.TransferDst | VkBufferUsageFlags.VertexBuffer,
            VkMemoryPropertyFlags.DeviceLocal
        );

        this.UpdateBuffer(buffer, data);

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

    public void Draw(VertexBuffer vertexBuffer, uint instanceCount = 1, uint firstVertex = 0, uint firstInstance = 0) =>
        this.Draw(this.Context.Frame.CommandBuffer, vertexBuffer, instanceCount, firstVertex, firstInstance);

    public void Draw(VkCommandBuffer commandBuffer, VertexBuffer vertexBuffer, uint instanceCount = 1, uint firstVertex = 0, uint firstInstance = 0) =>
        commandBuffer.Draw(vertexBuffer.Size, instanceCount, firstVertex, firstInstance);

    public void DrawIndexed(IndexBuffer indexBuffer, uint instanceCount = 1, uint firstIndex = 0, int vertexOffset = 0, uint firstInstance = 0) =>
        this.DrawIndexed(this.Context.Frame.CommandBuffer, indexBuffer, instanceCount, firstIndex, vertexOffset, firstInstance);

    public void DrawIndexed(VkCommandBuffer commandBuffer, IndexBuffer indexBuffer, uint instanceCount = 1, uint firstIndex = 0, int vertexOffset = 0, uint firstInstance = 0) =>
        commandBuffer.DrawIndexed(indexBuffer.Size, instanceCount, firstIndex, vertexOffset, firstInstance);

    public void EndFrame()
    {
        if (this.Context.Frame.BufferPrepared)
        {
            this.Context.Frame.CommandBuffer.End();
        }

        this.Context.SwapBuffers();
    }

    public void EndRenderPass() =>
        this.Context.Frame.CommandBuffer.EndRenderPass();

    public void EndRenderPass(VkCommandBuffer commandBuffer) =>
        commandBuffer.EndRenderPass();

    public void PushConstant<T>(Shader shader, in T constant) where T : unmanaged, IPushConstant =>
        this.Context.Frame.CommandBuffer.PushConstants(shader.PipelineLayout, T.Stages, constant);

    public void PushConstant<T>(VkCommandBuffer commandBuffer, Shader shader, in T constant) where T : unmanaged, IPushConstant =>
        commandBuffer.PushConstants(shader.PipelineLayout, T.Stages, constant);

    public void SetViewport(Surface surface) =>
        this.SetViewport(this.Context.Frame.CommandBuffer, surface);

    public void SetViewport(VkCommandBuffer commandBuffer, Surface surface)
    {
        var viewport = new VkViewport
        {
            X        = 0,
            Y        = 0,
            Width    = surface.Size.Width,
            Height   = surface.Size.Height,
            MinDepth = 0,
            MaxDepth = 1
        };

        commandBuffer.SetViewport(0, viewport);

        var scissor = new VkRect2D
        {
            Offset = new()
            {
                X = 0,
                Y = 0
            },
            Extent = surface.Swapchain.Extent,
        };

        commandBuffer.SetScissor(0, scissor);
    }

    public void UpdateBuffer<T>(Buffer buffer, T data) where T : unmanaged =>
        this.UpdateBuffer(buffer, [data]);

    public void UpdateBuffer<T>(Buffer buffer, T[] data) where T : unmanaged
    {
        var stagingBuffer = this.CreateBuffer(buffer.Allocation.Size, VkBufferUsageFlags.TransferSrc, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent);

        stagingBuffer.Allocation.Memory.Write(0, 0, data);

        this.CopyBuffer(stagingBuffer.Value, buffer.Value, buffer.Allocation.Size);

        stagingBuffer.Dispose();
    }

    public void UpdateIndexBuffer<T>(VertexBuffer indexBuffer, T data) where T : unmanaged =>
        this.UpdateBuffer(indexBuffer.Buffer, data);

    public void UpdateIndexBuffer<T>(VertexBuffer indexBuffer, T[] data) where T : unmanaged =>
        this.UpdateBuffer(indexBuffer.Buffer, data);

    public void UpdateVertexBuffer<T>(VertexBuffer vertexBuffer, T data) where T : unmanaged =>
        this.UpdateBuffer(vertexBuffer.Buffer, data);

    public void UpdateVertexBuffer<T>(VertexBuffer vertexBuffer, T[] data) where T : unmanaged =>
        this.UpdateBuffer(vertexBuffer.Buffer, data);

    public void UpdateTexture(Texture texture, Span<byte> data)
    {
        var buffer = this.CreateBuffer((ulong)data.Length, VkBufferUsageFlags.TransferSrc, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent);

        buffer.Allocation.Memory.Write(0, 0, data);

        this.TransitionImageLayout(
            texture.Image,
            VkImageLayout.Undefined,
            VkImageLayout.TransferDstOptimal,
            default,
            VkAccessFlags.TransferWrite,
            VkPipelineStageFlags.TopOfPipe,
            VkPipelineStageFlags.Transfer
        );

        this.CopyBufferToImage(buffer.Value, texture.Image, texture.Extent.Width, texture.Extent.Height);

        this.TransitionImageLayout(
            texture.Image,
            VkImageLayout.TransferDstOptimal,
            VkImageLayout.ShaderReadOnlyOptimal,
            VkAccessFlags.TransferWrite,
            VkAccessFlags.ShaderRead,
            VkPipelineStageFlags.Transfer,
            VkPipelineStageFlags.FragmentShader
        );

        buffer.Dispose();
    }

    public void WaitIdle() =>
        this.Context.Device.WaitIdle();
}
