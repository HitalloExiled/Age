using System.Numerics;
using System.Runtime.InteropServices;
using Age.Core;
using Age.Core.Interop;
using Age.Rendering.Enums;
using Age.Rendering.Resources;
using Age.Rendering.Vulkan.Uniforms;
using ThirdParty.Shaderc.Enums;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

using Buffer        = Age.Rendering.Resources.Buffer;
using SpirvCompiler = ThirdParty.Shaderc.Compiler;

namespace Age.Rendering.Vulkan;

public unsafe partial class VulkanRenderer : IDisposable
{
    private const ushort MAX_DESCRIPTORS_PER_POOL = 64;

    private readonly Dictionary<VkDescriptorType, List<DescriptorPool>> descriptorPools = [];
    private readonly SpirvCompiler                                      spirvCompiler   = new();

    private bool disposed;

    public VulkanContext Context { get; } = new();

    private static VkDescriptorType ConvertToDescriptorType(UniformType type) =>
        type switch
        {
            UniformType.CombinedImageSampler => VkDescriptorType.CombinedImageSampler,
            UniformType.UniformBuffer        => VkDescriptorType.UniformBuffer,
            _ => throw new NotImplementedException(),
        };

    private static VkImageType ConvertToImageType(TextureType type) =>
        type switch
        {
            TextureType.N1D or TextureType.N1DArray                                              => VkImageType.N1D,
            TextureType.N2D or TextureType.N2DArray or TextureType.Cube or TextureType.CubeArray => VkImageType.N2D,
            TextureType.N3D                                                                      => VkImageType.N3D,
            _ => throw new NotImplementedException(),
        };

    private static VkImageViewType ConvertToImageViewType(TextureType type) =>
        type switch
        {
            TextureType.N1D       => VkImageViewType.N1D,
            TextureType.N1DArray  => VkImageViewType.N1DArray,
            TextureType.N2D       => VkImageViewType.N2D,
            TextureType.N2DArray  => VkImageViewType.N2DArray,
            TextureType.N3D       => VkImageViewType.N3D,
            TextureType.Cube      => VkImageViewType.Cube,
            TextureType.CubeArray => VkImageViewType.CubeArray,
            _ => throw new NotImplementedException(),
        };

    private unsafe static VkBool32 DebugCallback(VkDebugUtilsMessageSeverityFlagsEXT messageSeverity, VkDebugUtilsMessageTypeFlagsEXT messageTypes, VkDebugUtilsMessengerCallbackDataEXT* pCallbackData, void* pUserData)
    {
        var defaultColor = Console.ForegroundColor;

        var color = messageSeverity switch
        {
            VkDebugUtilsMessageSeverityFlagsEXT.Error => ConsoleColor.DarkRed,
            VkDebugUtilsMessageSeverityFlagsEXT.Warning => ConsoleColor.DarkYellow,
            VkDebugUtilsMessageSeverityFlagsEXT.Info => ConsoleColor.DarkBlue,
            _ => defaultColor
        };

        Console.ForegroundColor = color;

        Console.WriteLine("validation layer: " + Marshal.PtrToStringAnsi((nint)pCallbackData->PMessage));

        Console.ForegroundColor = defaultColor;

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

    private VkDescriptorSetLayout CreateDescriptorSetLayout()
    {
        var descriptorSetLayoutBindings = new VkDescriptorSetLayoutBinding[]
        {
            new()
            {
                Binding         = 0,
                DescriptorCount = 1,
                DescriptorType  = VkDescriptorType.CombinedImageSampler,
                StageFlags      = VkShaderStageFlags.Fragment,
            },
        };

        fixed (VkDescriptorSetLayoutBinding* pBindings = descriptorSetLayoutBindings)
        {
            var descriptorSetLayoutCreateInfo = new VkDescriptorSetLayoutCreateInfo
            {
                PBindings    = pBindings,
                BindingCount = (uint)descriptorSetLayoutBindings.Length,
            };

            var descriptorSetLayout = this.Context.Device.CreateDescriptorSetLayout(descriptorSetLayoutCreateInfo);

            return descriptorSetLayout;
        }
    }

    private void CreateImage(uint width, uint height, VkSampleCountFlags samples, VkFormat format, VkImageTiling tiling, VkImageUsageFlags usage, VkMemoryPropertyFlags properties, out VkImage image, out Allocation allocation)
    {
        var imageCreateInfo = new VkImageCreateInfo
        {
            ArrayLayers = 1,
            Extent      = new()
            {
                Depth  = 1,
                Height = height,
                Width  = width,
            },
            Format        = format,
            ImageType     = VkImageType.N2D,
            InitialLayout = VkImageLayout.Undefined,
            MipLevels     = 1,
            Samples       = samples,
            Tiling        = tiling,
            Usage         = usage,
        };

        image = this.Context.Device.CreateImage(imageCreateInfo);

        image.GetMemoryRequirements(out var memRequirements);

        var memoryType = this.Context.FindMemoryType(memRequirements.MemoryTypeBits, properties);

        var memoryAllocateInfo = new VkMemoryAllocateInfo
        {
            AllocationSize  = memRequirements.Size,
            MemoryTypeIndex = memoryType,
        };

        var deviceMemory = this.Context.Device.AllocateMemory(memoryAllocateInfo);

        image.BindMemory(deviceMemory, 0);

        allocation = new()
        {
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

    private byte[] CompileShader(string path)
    {
        var shaderKind = Path.GetExtension(path) switch
        {
            ".frag" => ShaderKind.FragmentShader,
            ".vert" => ShaderKind.VertexShader,
            _ => throw new Exception("Unsupported shader type"),
        };

        var fileName = Path.GetFileName(path);

        var result = this.spirvCompiler.CompileIntoSpv(File.ReadAllText(path), shaderKind, fileName, "main");

        return result.CompilationStatus != CompilationStatus.Success
            ? throw new Exception($"Error compiling shader: {result.ErrorMessage}")
            : result.Bytes;
    }

    private VkPipeline CreatePipeline(VkPipelineLayout pipelineLayout, VkRenderPass renderPass)
    {
        fixed (byte* pName = "main"u8)
        {
            using var compiler = new SpirvCompiler();

            var vertShaderCode = this.CompileShader(Path.Join(AppContext.BaseDirectory, "Shaders/shader.vert"));
            var fragShaderCode = this.CompileShader(Path.Join(AppContext.BaseDirectory, "Shaders/shader.frag"));

            var vertShaderModule = this.CreateShaderModule(vertShaderCode);
            var fragShaderModule = this.CreateShaderModule(fragShaderCode);

            var pipelineShaderStageCreateInfos = new VkPipelineShaderStageCreateInfo[]
            {
                new()
                {
                    Module = vertShaderModule.Handle,
                    PName  = pName,
                    Stage  = VkShaderStageFlags.Vertex,
                },
                new()
                {
                    Module = fragShaderModule.Handle,
                    PName  = pName,
                    Stage  = VkShaderStageFlags.Fragment,
                },
            };

            var vertexInputBindingDescription = new VkVertexInputBindingDescription
            {
                Binding   = 0,
                Stride    = (uint)Marshal.SizeOf<Vertex>(),
                InputRate = VkVertexInputRate.Vertex,
            };

            var vertexInputAttributeDescription = new VkVertexInputAttributeDescription[]
            {
                new()
                {
                    Binding  = 0,
                    Format   = VkFormat.R32G32Sfloat,
                    Location = 0,
                    Offset   = (uint)Marshal.OffsetOf<Vertex>(nameof(Vertex.Position)),
                },
                new()
                {
                    Binding  = 0,
                    Format   = VkFormat.R32G32B32A32Sfloat,
                    Location = 1,
                    Offset   = (uint)Marshal.OffsetOf<Vertex>(nameof(Vertex.Color)),
                },
                new()
                {
                    Binding  = 0,
                    Format   = VkFormat.R32G32Sfloat,
                    Location = 2,
                    Offset   = (uint)Marshal.OffsetOf<Vertex>(nameof(Vertex.UV)),
                },
            };

            var dynamicStates = new VkDynamicState[]
            {
                VkDynamicState.Viewport,
                VkDynamicState.Scissor,
            };

            fixed (VkVertexInputAttributeDescription* pVertexAttributeDescriptions    = vertexInputAttributeDescription)
            fixed (VkDynamicState*                    pDynamicStates                  = dynamicStates)
            fixed (VkPipelineShaderStageCreateInfo*   pPipelineShaderStageCreateInfos = pipelineShaderStageCreateInfos)
            {
                var inputAssembly = new VkPipelineInputAssemblyStateCreateInfo
                {
                    Topology = VkPrimitiveTopology.TriangleList,
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
                    StageCount          = (uint)pipelineShaderStageCreateInfos.Length,
                    RenderPass          = renderPass.Handle,
                };

                var pipeline = this.Context.Device.CreateGraphicsPipelines(graphicsPipelineCreateInfo);

                fragShaderModule.Dispose();
                vertShaderModule.Dispose();

                return pipeline;
            }
        }
    }

    private VkPipelineLayout CreatePipelineLayout(VkDescriptorSetLayout descriptorSetLayout)
    {
        var descriptorSetLayoutHandle = descriptorSetLayout.Handle;

        var createInfo = new VkPipelineLayoutCreateInfo
        {
            PSetLayouts    = &descriptorSetLayoutHandle,
            SetLayoutCount = 1,
        };

        return this.Context.Device.CreatePipelineLayout(createInfo);
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

    private VkFramebuffer CreateFrameBuffer(VkRenderPass renderPass, VkImageView imageView, VkExtent2D extent)
    {
        var imageViewHandle = imageView.Handle;

        var createInfo = new VkFramebufferCreateInfo
        {
            AttachmentCount = 1,
            Height          = extent.Height,
            Layers          = 1,
            PAttachments    = &imageViewHandle,
            Width           = extent.Width,
            RenderPass      = renderPass.Handle,
        };

        return this.Context.Device.CreateFramebuffer(createInfo);
    }

    private VkRenderPass CreateRenderPass(VkFormat format)
    {
        var attachmentDescription = new VkAttachmentDescription
        {
            Samples        = VkSampleCountFlags.N1,
            FinalLayout    = VkImageLayout.PresentSrcKHR,
            Format         = format,
            InitialLayout  = VkImageLayout.Undefined,
            LoadOp         = VkAttachmentLoadOp.Clear,
            StencilLoadOp  = VkAttachmentLoadOp.Clear,
            StencilStoreOp = VkAttachmentStoreOp.DontCare,
        };

        var attachmentReference = new VkAttachmentReference
        {
            Layout = VkImageLayout.ColorAttachmentOptimal,
        };

        var subpass = new VkSubpassDescription
        {
            ColorAttachmentCount = 1,
            PColorAttachments    = &attachmentReference,
            PipelineBindPoint    = VkPipelineBindPoint.Graphics,
        };

        var renderPassCreateInfo = new VkRenderPassCreateInfo
        {
            AttachmentCount = 1,
            PAttachments    = &attachmentDescription,
            PSubpasses      = &subpass,
            SubpassCount    = 1,
        };

        return this.Context.Device.CreateRenderPass(renderPassCreateInfo);
    }

    private void CopyBuffer(VkBuffer srcBuffer, VkBuffer dstBuffer, ulong size)
    {
        var commandBuffer = this.Context.BeginSingleTimeCommands();

        var copyRegion = new VkBufferCopy
        {
            Size = size
        };

        commandBuffer.CopyBuffer(srcBuffer, dstBuffer, copyRegion);

        this.Context.EndSingleTimeCommands(commandBuffer);
    }

    private void CopyBufferToImage(VkBuffer buffer, VkImage image, uint width, uint height)
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

        commandBuffer.CopyBufferToImage(buffer, image, VkImageLayout.TransferDstOptimal, bufferImageCopy);

        this.Context.EndSingleTimeCommands(commandBuffer);
    }

    private void TransitionImageLayout(
        VkImage                 image,
        VkImageLayout           oldLayout,
        VkImageLayout           newLayout,
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

            foreach (var descriptorPool in this.descriptorPools.Values.SelectMany(x => x))
            {
                descriptorPool.Value.Dispose();
            }

            this.Context.Dispose();
            this.spirvCompiler.Dispose();

            this.disposed = true;
        }
    }

    public void BeginFrame()
    {
        this.Context.PrepareBuffers();

        this.Context.Frame.CommandBuffer.Begin();
    }

    public void BeginRenderPass(Surface surface)
    {
        var clearValues = new VkClearValue[2];

        clearValues[0].Color.Float32[0] = 1;
        clearValues[0].Color.Float32[1] = 1;
        clearValues[0].Color.Float32[2] = 1;
        clearValues[0].Color.Float32[3] = 1;

        clearValues[1].DepthStencil = new()
        {
            Depth   = 1.0f,
            Stencil = 0
        };

        fixed (VkClearValue* pClearValues = clearValues)
        {
            var renderPassBeginInfo = new VkRenderPassBeginInfo
            {
                ClearValueCount = (uint)clearValues.Length,
                Framebuffer     = surface.Swapchain.Framebuffers[surface.CurrentBuffer].Handle,
                PClearValues    = pClearValues,
                RenderArea      = new()
                {
                    Offset = new()
                    {
                        X = 0,
                        Y = 0
                    },
                    Extent = surface.Swapchain.Extent,
                },
                RenderPass = surface.Swapchain.RenderPass.Handle,
            };

            this.Context.Frame.CommandBuffer.BeginRenderPass(renderPassBeginInfo, VkSubpassContents.Inline);
        }
    }

    public void BindIndexBuffer(IndexBuffer indexBuffer) =>
        this.Context.Frame.CommandBuffer.BindIndexBuffer(indexBuffer.Buffer.Value, 0, indexBuffer.Type);

    public void BindPipeline(Shader shader) =>
        this.Context.Frame.CommandBuffer.BindPipeline(shader.PipelineBindPoint, shader.Pipeline);

    public void BindVertexBuffer(VertexBuffer vertexBuffer) =>
        this.Context.Frame.CommandBuffer.BindVertexBuffers(0, 1, [vertexBuffer.Buffer.Value], [0]);

    public void BindVertexBuffer(VertexBuffer[] vertexBuffers) =>
        this.Context.Frame.CommandBuffer.BindVertexBuffers(0, 1, [.. vertexBuffers.Select(x => x.Buffer.Value)], new ulong[vertexBuffers.Length]);

    public void BindUniformSet(UniformSet uniformSet) =>
        this.Context.Frame.CommandBuffer.BindDescriptorSets(uniformSet.Shader.PipelineBindPoint, uniformSet.Shader.PipelineLayout, 0, uniformSet.DescriptorSets, []);

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
                Memory     = memory,
                Memorytype = memoryType,
                Offset     = 0,
                Size       = size,
            },
            Value = buffer,
            Usage = usage,
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

    public Shader CreateShader()
    {
        var renderPass          = this.CreateRenderPass(this.Context.ScreenFormat);
        var descriptorSetLayout = this.CreateDescriptorSetLayout();
        var pipelineLayout      = this.CreatePipelineLayout(descriptorSetLayout);
        var pipeline            = this.CreatePipeline(pipelineLayout, renderPass);

        return new()
        {
            DescriptorSetLayout = descriptorSetLayout,
            Pipeline            = pipeline,
            PipelineBindPoint   = VkPipelineBindPoint.Graphics,
            PipelineLayout      = pipelineLayout,
            RenderPass          = renderPass,
        };
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
        var format  = VkFormat.B8G8R8A8Srgb;

        this.CreateImage(
            textureCreate.Width,
            textureCreate.Height,
            samples,
            format,
            tiling,
            usage,
            VkMemoryPropertyFlags.DeviceLocal,
            out var image,
            out var allocation
        );

        this.TransitionImageLayout(
            image,
            VkImageLayout.Undefined,
            VkImageLayout.TransferDstOptimal,
            default,
            VkAccessFlags.TransferWrite,
            VkPipelineStageFlags.TopOfPipe,
            VkPipelineStageFlags.Transfer
        );

        var imageView = this.CreateImageView(image, VkFormat.B8G8R8A8Srgb, VkImageAspectFlags.Color);

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

        this.UpdateTexture(texture, textureCreate.Data);

        return texture;
    }

    public UniformSet CreateUniformSet(IList<Uniform> uniforms, Shader shader)
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
            Size   = size,
        };
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void DrawIndexed(IndexBuffer indexBuffer) =>
        this.Context.Frame.CommandBuffer.DrawIndexed(indexBuffer.Size, 1, 0, 0, 0);

    public void EndFrame()
    {
        this.Context.Frame.CommandBuffer.End();
        this.Context.SwapBuffers();
    }

    public void EndRenderPass() =>
        this.Context.Frame.CommandBuffer.EndRenderPass();

    public void SetViewport(Surface surface)
    {
        var commandBuffer = this.Context.Frame.CommandBuffer;

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

    public void UpdateTexture(Texture texture, uint[] data)
    {
        var imageSize = (ulong)data.Length * 4;

        var buffer = this.CreateBuffer(imageSize, VkBufferUsageFlags.TransferSrc, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent);

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
