using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Age.Core;
using Age.Core.Interop;
using Age.Rendering.Enums;
using Age.Rendering.Vulkan.Handlers;
using Age.Rendering.Vulkan.Uniforms;
using Age.Vulkan;
using Age.Vulkan.Enums;
using Age.Vulkan.Flags;
using Age.Vulkan.Flags.EXT;
using Age.Vulkan.Types;
using Age.Vulkan.Types.EXT;
using ThirdParty.Shaderc.Enums;
using SpirvCompiler = ThirdParty.Shaderc.Compiler;

namespace Age.Rendering.Vulkan;

public unsafe partial class VulkanRenderer : IDisposable
{
    private const ushort MAX_DESCRIPTORS_PER_POOL = 64;
    private readonly Dictionary<VkDescriptorType, List<DescriptorPoolHandler>> descriptorPools = [];
    private readonly Vk                                                        vk;
    private readonly SpirvCompiler                                             spirvCompiler = new();

    private bool disposed;

    public VulkanContext Context { get; }

    public VulkanRenderer()
    {
        this.Context = new();
        this.vk      = this.Context.Vk;
    }

    private static VkDescriptorType ConvertToDescriptorType(UniformType type) =>
        type switch
        {
            UniformType.CombinedImageSampler => VkDescriptorType.VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER,
            UniformType.UniformBuffer        => VkDescriptorType.VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER,
            _ => throw new NotImplementedException(),
        };

    private static VkImageType ConvertToImageType(TextureType type) =>
        type switch
        {
            TextureType.T1D or TextureType.T1DArray                                              => VkImageType.VK_IMAGE_TYPE_1D,
            TextureType.T2D or TextureType.T2DArray or TextureType.Cube or TextureType.CubeArray => VkImageType.VK_IMAGE_TYPE_2D,
            TextureType.T3D                                                                      => VkImageType.VK_IMAGE_TYPE_3D,
            _ => throw new NotImplementedException(),
        };

    private static VkImageViewType ConvertToImageViewType(TextureType type) =>
        type switch
        {
            TextureType.T1D       => VkImageViewType.VK_IMAGE_VIEW_TYPE_1D,
            TextureType.T1DArray  => VkImageViewType.VK_IMAGE_VIEW_TYPE_1D_ARRAY,
            TextureType.T2D       => VkImageViewType.VK_IMAGE_VIEW_TYPE_2D,
            TextureType.T2DArray  => VkImageViewType.VK_IMAGE_VIEW_TYPE_2D_ARRAY,
            TextureType.T3D       => VkImageViewType.VK_IMAGE_VIEW_TYPE_3D,
            TextureType.Cube      => VkImageViewType.VK_IMAGE_VIEW_TYPE_CUBE,
            TextureType.CubeArray => VkImageViewType.VK_IMAGE_VIEW_TYPE_CUBE_ARRAY,
            _ => throw new NotImplementedException(),
        };

    private unsafe static VkBool32 DebugCallback(VkDebugUtilsMessageSeverityFlagBitsEXT messageSeverity, VkDebugUtilsMessageTypeFlagsEXT messageTypes, VkDebugUtilsMessengerCallbackDataEXT* pCallbackData, void* pUserData)
    {
        var defaultColor = Console.ForegroundColor;

        var color = messageSeverity switch
        {
            VkDebugUtilsMessageSeverityFlagBitsEXT.VK_DEBUG_UTILS_MESSAGE_SEVERITY_ERROR_BIT_EXT => ConsoleColor.DarkRed,
            VkDebugUtilsMessageSeverityFlagBitsEXT.VK_DEBUG_UTILS_MESSAGE_SEVERITY_WARNING_BIT_EXT => ConsoleColor.DarkYellow,
            VkDebugUtilsMessageSeverityFlagBitsEXT.VK_DEBUG_UTILS_MESSAGE_SEVERITY_INFO_BIT_EXT => ConsoleColor.DarkBlue,
            _ => defaultColor
        };

        Console.ForegroundColor = color;

        Console.WriteLine("validation layer: " + Marshal.PtrToStringAnsi((nint)pCallbackData->pMessage));

        Console.ForegroundColor = defaultColor;

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void VkCheck(in VkResult result)
    {
        if (result != VkResult.VK_SUCCESS)
        {
            throw new Exception($"Vulkan Error: {result}");
        }
    }

    private void CreateBuffer(VkDeviceSize size, VkBufferUsageFlagBits usage, VkMemoryPropertyFlagBits properties, out VkBuffer buffer, out Allocation allocation)
    {
        var bufferCreateInfo = new VkBufferCreateInfo
        {
            size  = size,
            usage = usage,
        };

        var device = this.Context.Device;

        VkCheck(this.vk.CreateBuffer(device, bufferCreateInfo, default, out buffer));

        this.vk.GetBufferMemoryRequirements(device, buffer, out var memRequirements);

        var memoryType = this.Context.FindMemoryType(memRequirements.memoryTypeBits, properties);

        var memoryAllocateInfo = new VkMemoryAllocateInfo
        {
            allocationSize  = memRequirements.size,
            memoryTypeIndex = memoryType
        };

        VkCheck(this.vk.AllocateMemory(device, memoryAllocateInfo, default, out var memory));
        VkCheck(this.vk.BindBufferMemory(device, buffer, memory, 0));

        allocation = new()
        {
            Memory     = memory,
            Memorytype = memoryType,
            Offset     = 0,
            Size       = size,
        };
    }

    private VkDescriptorSet[] CreateDescriptorSets(VkDescriptorPool descriptorPool, VkDescriptorSetLayout descriptorSetLayout)
    {
        var descriptorSetLayouts = new VkDescriptorSetLayout[VulkanContext.MAX_FRAMES_IN_FLIGHT]
        {
            descriptorSetLayout,
            descriptorSetLayout,
        };

        fixed (VkDescriptorSetLayout* pSetLayouts = descriptorSetLayouts)
        {
            var descriptorSetAllocateInfo = new VkDescriptorSetAllocateInfo
            {
                descriptorPool     = descriptorPool,
                descriptorSetCount = VulkanContext.MAX_FRAMES_IN_FLIGHT,
                pSetLayouts        = pSetLayouts,
            };

            VkCheck(this.vk.AllocateDescriptorSets(this.Context.Device, descriptorSetAllocateInfo, out var descriptorSets));

            return descriptorSets;
        }
    }

    private DescriptorPoolHandler CreateDescriptorPool(VkDescriptorType descriptorType)
    {
        if (this.descriptorPools.TryGetValue(descriptorType, out var descriptorPools))
        {
            if (descriptorPools.FirstOrDefault(x => x.Usage < MAX_DESCRIPTORS_PER_POOL) is DescriptorPoolHandler descriptorPool)
            {
                descriptorPool.Usage++;

                return descriptorPool;
            }
        }
        else
        {
            this.descriptorPools[descriptorType] = descriptorPools = [];
        }

        var sizes = new List<VkDescriptorPoolSize>();

        if (descriptorType.HasFlag(VkDescriptorType.VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER))
        {
            sizes.Add(new() { type = VkDescriptorType.VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER, descriptorCount = MAX_DESCRIPTORS_PER_POOL });
        }

        if (descriptorType.HasFlag(VkDescriptorType.VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER))
        {
            sizes.Add(new() { type = VkDescriptorType.VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER, descriptorCount = MAX_DESCRIPTORS_PER_POOL });
        }

        fixed (VkDescriptorPoolSize* pPoolSizes = sizes.ToArray())
        {
            var descriptorPoolCreateInfo = new VkDescriptorPoolCreateInfo
            {
                flags         = VkDescriptorPoolCreateFlagBits.VK_DESCRIPTOR_POOL_CREATE_FREE_DESCRIPTOR_SET_BIT,
                maxSets       = MAX_DESCRIPTORS_PER_POOL,
                poolSizeCount = (uint)sizes.Count,
                pPoolSizes    = pPoolSizes,
            };

            VkCheck(this.vk.CreateDescriptorPool(this.Context.Device, descriptorPoolCreateInfo, default, out var descriptorPool));

            var descriptorPoolHandler = new DescriptorPoolHandler
            {
                DescriptorType = descriptorType,
                Handler        = descriptorPool,
                Usage          = 1,
            };

            descriptorPools.Add(descriptorPoolHandler);

            return descriptorPoolHandler;
        }
    }

    private VkDescriptorSetLayout CreateDescriptorSetLayout()
    {
        var descriptorSetLayoutBindings = new VkDescriptorSetLayoutBinding[]
        {
            new()
            {
                binding         = 0,
                descriptorCount = 1,
                descriptorType  = VkDescriptorType.VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER,
                stageFlags      = VkShaderStageFlagBits.VK_SHADER_STAGE_FRAGMENT_BIT,
            },
        };

        fixed (VkDescriptorSetLayoutBinding* pBindings = descriptorSetLayoutBindings)
        {
            var descriptorSetLayoutCreateInfo = new VkDescriptorSetLayoutCreateInfo
            {
                pBindings    = pBindings,
                bindingCount = (uint)descriptorSetLayoutBindings.Length,
            };

            VkCheck(this.vk.CreateDescriptorSetLayout(this.Context.Device, descriptorSetLayoutCreateInfo, default, out var descriptorSetLayout));

            return descriptorSetLayout;
        }
    }

    private void CreateImage(uint width, uint height, VkSampleCountFlagBits samples, VkFormat format, VkImageTiling tiling, VkImageUsageFlagBits usage, VkMemoryPropertyFlagBits properties, out VkImage image, out Allocation allocation)
    {
        var imageCreateInfo = new VkImageCreateInfo
        {
            arrayLayers = 1,
            extent      = new()
            {
                depth  = 1,
                height = height,
                width  = width,
            },
            format        = format,
            imageType     = VkImageType.VK_IMAGE_TYPE_2D,
            initialLayout = VkImageLayout.VK_IMAGE_LAYOUT_UNDEFINED,
            mipLevels     = 1,
            samples       = samples,
            tiling        = tiling,
            usage         = usage,
        };

        VkCheck(this.vk.CreateImage(this.Context.Device, imageCreateInfo, default, out image));

        this.vk.GetImageMemoryRequirements(this.Context.Device, image, out var memRequirements);

        var memoryType = this.Context.FindMemoryType(memRequirements.memoryTypeBits, properties);

        var memoryAllocateInfo = new VkMemoryAllocateInfo
        {
            allocationSize  = memRequirements.size,
            memoryTypeIndex = memoryType,
        };

        VkCheck(this.vk.AllocateMemory(this.Context.Device, memoryAllocateInfo, default, out var deviceMemory));
        VkCheck(this.vk.BindImageMemory(this.Context.Device, image, deviceMemory, 0));

        allocation = new()
        {
            Memory     = deviceMemory,
            Memorytype = memoryType,
            Offset     = 0,
            Size       = memRequirements.size,
        };
    }

    private VkImageView CreateImageView(VkImage image, VkFormat format, VkImageAspectFlagBits aspect)
    {
        var imageViewCreateInfo = new VkImageViewCreateInfo
        {
            format           = format,
            image            = image,
            subresourceRange = new()
            {
                aspectMask = aspect,
                layerCount = 1,
                levelCount = 1,
            },
            viewType = VkImageViewType.VK_IMAGE_VIEW_TYPE_2D,
        };

        VkCheck(this.vk.CreateImageView(this.Context.Device, imageViewCreateInfo, default, out var imageView));

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
                    module = vertShaderModule,
                    pName  = pName,
                    stage  = VkShaderStageFlagBits.VK_SHADER_STAGE_VERTEX_BIT,
                },
                new()
                {
                    module = fragShaderModule,
                    pName  = pName,
                    stage  = VkShaderStageFlagBits.VK_SHADER_STAGE_FRAGMENT_BIT,
                },
            };

            var vertexInputBindingDescription = new VkVertexInputBindingDescription
            {
                binding   = 0,
                stride    = (uint)Marshal.SizeOf<Vertex>(),
                inputRate = VkVertexInputRate.VK_VERTEX_INPUT_RATE_VERTEX,
            };

            var vertexInputAttributeDescription = new VkVertexInputAttributeDescription[]
            {
                new()
                {
                    binding  = 0,
                    format   = VkFormat.VK_FORMAT_R32G32_SFLOAT,
                    location = 0,
                    offset   = (uint)Marshal.OffsetOf<Vertex>(nameof(Vertex.Position)),
                },
                new()
                {
                    binding  = 0,
                    format   = VkFormat.VK_FORMAT_R32G32B32A32_SFLOAT,
                    location = 1,
                    offset   = (uint)Marshal.OffsetOf<Vertex>(nameof(Vertex.Color)),
                },
                new()
                {
                    binding  = 0,
                    format   = VkFormat.VK_FORMAT_R32G32_SFLOAT,
                    location = 2,
                    offset   = (uint)Marshal.OffsetOf<Vertex>(nameof(Vertex.UV)),
                },
            };

            var dynamicStates = new VkDynamicState[]
            {
                VkDynamicState.VK_DYNAMIC_STATE_VIEWPORT,
                VkDynamicState.VK_DYNAMIC_STATE_SCISSOR,
            };

            fixed (VkVertexInputAttributeDescription* pVertexAttributeDescriptions    = vertexInputAttributeDescription)
            fixed (VkDynamicState*                    pDynamicStates                  = dynamicStates)
            fixed (VkPipelineShaderStageCreateInfo*   pPipelineShaderStageCreateInfos = pipelineShaderStageCreateInfos)
            {
                var inputAssembly = new VkPipelineInputAssemblyStateCreateInfo
                {
                    topology = VkPrimitiveTopology.VK_PRIMITIVE_TOPOLOGY_TRIANGLE_LIST,
                };

                var pipelineVertexInputStateCreateInfo = new VkPipelineVertexInputStateCreateInfo
                {
                    pVertexAttributeDescriptions    = pVertexAttributeDescriptions,
                    pVertexBindingDescriptions      = &vertexInputBindingDescription,
                    vertexAttributeDescriptionCount = (uint)vertexInputAttributeDescription.Length,
                    vertexBindingDescriptionCount   = 1,
                };

                var pipelineDynamicStateCreateInfo = new VkPipelineDynamicStateCreateInfo
                {
                    dynamicStateCount = (uint)dynamicStates.Length,
                    pDynamicStates    = pDynamicStates,
                };

                var pipelineColorBlendAttachmentState = new VkPipelineColorBlendAttachmentState
                {
                    blendEnable    = true,
                    colorWriteMask = VkColorComponentFlagBits.VK_COLOR_COMPONENT_R_BIT
                        | VkColorComponentFlagBits.VK_COLOR_COMPONENT_G_BIT
                        | VkColorComponentFlagBits.VK_COLOR_COMPONENT_B_BIT
                        | VkColorComponentFlagBits.VK_COLOR_COMPONENT_A_BIT,
                    dstColorBlendFactor = VkBlendFactor.VK_BLEND_FACTOR_ONE_MINUS_SRC_ALPHA,
                    srcAlphaBlendFactor = VkBlendFactor.VK_BLEND_FACTOR_ONE,
                    srcColorBlendFactor = VkBlendFactor.VK_BLEND_FACTOR_SRC_ALPHA,
                };

                var pipelineColorBlendStateCreateInfo = new VkPipelineColorBlendStateCreateInfo
                {
                    attachmentCount = 1,
                    pAttachments    = &pipelineColorBlendAttachmentState,
                };

                var pipelineMultisampleStateCreateInfo = new VkPipelineMultisampleStateCreateInfo
                {
                    rasterizationSamples = VkSampleCountFlagBits.VK_SAMPLE_COUNT_1_BIT,
                };

                var pipelineRasterizationStateCreateInfo = new VkPipelineRasterizationStateCreateInfo
                {
                    cullMode    = VkCullModeFlagBits.VK_CULL_MODE_BACK_BIT,
                    frontFace   = VkFrontFace.VK_FRONT_FACE_CLOCKWISE,
                    lineWidth   = 1,
                    polygonMode = VkPolygonMode.VK_POLYGON_MODE_FILL,
                };

                var pipelineViewportStateCreateInfo = new VkPipelineViewportStateCreateInfo
                {
                    viewportCount = 1,
                    scissorCount  = 1,
                };

                var graphicsPipelineCreateInfo = new VkGraphicsPipelineCreateInfo
                {
                    layout              = pipelineLayout,
                    pColorBlendState    = &pipelineColorBlendStateCreateInfo,
                    pDynamicState       = &pipelineDynamicStateCreateInfo,
                    pInputAssemblyState = &inputAssembly,
                    pMultisampleState   = &pipelineMultisampleStateCreateInfo,
                    pRasterizationState = &pipelineRasterizationStateCreateInfo,
                    pStages             = pPipelineShaderStageCreateInfos,
                    pVertexInputState   = &pipelineVertexInputStateCreateInfo,
                    pViewportState      = &pipelineViewportStateCreateInfo,
                    stageCount          = (uint)pipelineShaderStageCreateInfos.Length,
                    renderPass          = renderPass,
                };

                VkCheck(this.vk.CreateGraphicsPipelines(this.Context.Device, default, graphicsPipelineCreateInfo, default, out var pipeline));

                this.vk.DestroyShaderModule(this.Context.Device, fragShaderModule, null);
                this.vk.DestroyShaderModule(this.Context.Device, vertShaderModule, null);

                return pipeline;
            }
        }
    }

    private VkPipelineLayout CreatePipelineLayout(VkDescriptorSetLayout descriptorSetLayout)
    {
        var pipelineLayoutCreateInfo = new VkPipelineLayoutCreateInfo
        {
            pSetLayouts    = &descriptorSetLayout,
            setLayoutCount = 1,
        };

        VkCheck(this.vk.CreatePipelineLayout(this.Context.Device, pipelineLayoutCreateInfo, default, out var pipelineLayout));

        return pipelineLayout;
    }

    private VkShaderModule CreateShaderModule(byte[] buffer)
    {
        fixed (byte* pCode = buffer)
        {
            var shaderModuleCreateInfo = new VkShaderModuleCreateInfo
            {
                codeSize = (uint)buffer.Length,
                pCode    = (uint*)pCode,
            };

            VkCheck(this.vk.CreateShaderModule(this.Context.Device, shaderModuleCreateInfo, default, out var shaderModule));

            return shaderModule;
        }
    }

    private VkFramebuffer CreateFrameBuffer(VkRenderPass renderPass, VkImageView imageView, VkExtent2D extent)
    {
        var framebufferCreateInfo = new VkFramebufferCreateInfo
        {
            attachmentCount = 1,
            height          = extent.height,
            layers          = 1,
            pAttachments    = &imageView,
            width           = extent.width,
            renderPass      = renderPass,
        };

        VkCheck(this.vk.CreateFramebuffer(this.Context.Device, framebufferCreateInfo, default, out var framebuffer));

        return framebuffer;
    }

    private VkRenderPass CreateRenderPass(VkFormat format)
    {
        var attachmentDescription = new VkAttachmentDescription
        {
            samples        = VkSampleCountFlagBits.VK_SAMPLE_COUNT_1_BIT,
            finalLayout    = VkImageLayout.VK_IMAGE_LAYOUT_PRESENT_SRC_KHR,
            format         = format,
            initialLayout  = VkImageLayout.VK_IMAGE_LAYOUT_UNDEFINED,
            loadOp         = VkAttachmentLoadOp.VK_ATTACHMENT_LOAD_OP_CLEAR,
            stencilLoadOp  = VkAttachmentLoadOp.VK_ATTACHMENT_LOAD_OP_CLEAR,
            stencilStoreOp = VkAttachmentStoreOp.VK_ATTACHMENT_STORE_OP_DONT_CARE,
        };

        var attachmentReference = new VkAttachmentReference
        {
            layout = VkImageLayout.VK_IMAGE_LAYOUT_COLOR_ATTACHMENT_OPTIMAL,
        };

        var subpass = new VkSubpassDescription
        {
            colorAttachmentCount = 1,
            pColorAttachments    = &attachmentReference,
            pipelineBindPoint    = VkPipelineBindPoint.VK_PIPELINE_BIND_POINT_GRAPHICS,
        };

        var renderPassCreateInfo = new VkRenderPassCreateInfo
        {
            attachmentCount = 1,
            pAttachments    = &attachmentDescription,
            pSubpasses      = &subpass,
            subpassCount    = 1,
        };

        VkCheck(this.vk.CreateRenderPass(this.Context.Device, renderPassCreateInfo, default, out var renderPass));

        return renderPass;
    }

    private void CopyBuffer(VkBuffer srcBuffer, VkBuffer dstBuffer, VkDeviceSize size)
    {
        var commandBuffer = this.Context.BeginSingleTimeCommands();

        var copyRegion = new VkBufferCopy
        {
            size = size
        };

        this.vk.CmdCopyBuffer(commandBuffer, srcBuffer, dstBuffer, copyRegion);

        this.Context.EndSingleTimeCommands(commandBuffer);
    }

    private void CopyBufferToImage(VkBuffer buffer, VkImage image, uint width, uint height)
    {
        var commandBuffer = this.Context.BeginSingleTimeCommands();

        var bufferImageCopy = new VkBufferImageCopy
        {
            imageExtent = new()
            {
                depth  = 1,
                height = height,
                width  = width,
            },
            imageSubresource = new()
            {
                aspectMask = VkImageAspectFlagBits.VK_IMAGE_ASPECT_COLOR_BIT,
                layerCount = 1,
            }
        };

        this.vk.CmdCopyBufferToImage(commandBuffer, buffer, image, VkImageLayout.VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL, bufferImageCopy);

        this.Context.EndSingleTimeCommands(commandBuffer);
    }

    private void RemoveFromDescriptorPool(DescriptorPoolHandler descriptorPool)
    {
        descriptorPool.Usage--;

        if (descriptorPool.Usage == 0)
        {
            this.vk.DestroyDescriptorPool(this.Context.Device, descriptorPool.Handler, null);

            var entries = this.descriptorPools[descriptorPool.DescriptorType];

            entries.Remove(descriptorPool);

            if (entries.Count == 0)
            {
                this.descriptorPools.Remove(descriptorPool.DescriptorType);
            }
        }
    }

    private void TransitionImageLayout(
        VkImage                 image,
        VkImageLayout           oldLayout,
        VkImageLayout           newLayout,
        VkAccessFlagBits        srcAccessMask,
        VkAccessFlagBits        dstAccessMask,
        VkPipelineStageFlagBits sourceStage,
        VkPipelineStageFlagBits destinationStage
    )
    {
        var commandBuffer = this.Context.BeginSingleTimeCommands();

        var imageMemoryBarrier = new VkImageMemoryBarrier
        {
            dstAccessMask       = dstAccessMask,
            dstQueueFamilyIndex = Vk.VK_QUEUE_FAMILY_IGNORED,
            image               = image,
            newLayout           = newLayout,
            oldLayout           = oldLayout,
            srcAccessMask       = srcAccessMask,
            srcQueueFamilyIndex = Vk.VK_QUEUE_FAMILY_IGNORED,
            subresourceRange    = new()
            {
                aspectMask = VkImageAspectFlagBits.VK_IMAGE_ASPECT_COLOR_BIT,
                layerCount = 1,
                levelCount = 1,
            }
        };

        this.vk.CmdPipelineBarrier(commandBuffer, sourceStage, destinationStage, default, null, null, [imageMemoryBarrier]);

        this.Context.EndSingleTimeCommands(commandBuffer);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            this.vk.DeviceWaitIdle(this.Context.Device);

            foreach (var descriptorPool in this.descriptorPools.Values.SelectMany(x => x))
            {
                this.vk.DestroyDescriptorPool(this.Context.Device, descriptorPool.Handler, null);
            }

            this.Context.Dispose();
            this.spirvCompiler.Dispose();

            this.disposed = true;
        }
    }

    public void BeginFrame()
    {
        this.Context.PrepareBuffers();

        this.vk.BeginCommandBuffer(this.Context.Frame.CommandBuffer);
    }

    public void BeginRenderPass(SurfaceContext windowContext)
    {
        var clearValues = new VkClearValue[2];

        clearValues[0].color.float32[0] = 1;
        clearValues[0].color.float32[1] = 1;
        clearValues[0].color.float32[2] = 1;
        clearValues[0].color.float32[3] = 1;

        clearValues[1].depthStencil = new()
        {
            depth   = 1.0f,
            stencil = 0
        };

        fixed (VkClearValue* pClearValues = clearValues)
        {
            var renderPassBeginInfo = new VkRenderPassBeginInfo
            {
                clearValueCount = (uint)clearValues.Length,
                framebuffer     = windowContext.Swapchain.Framebuffers[windowContext.CurrentBuffer],
                pClearValues    = pClearValues,
                renderArea      = new()
                {
                    offset = new()
                    {
                        x = 0,
                        y = 0
                    },
                    extent = windowContext.Swapchain.Extent,
                },
                renderPass = windowContext.Swapchain.RenderPass,
            };

            this.vk.CmdBeginRenderPass(this.Context.Frame.CommandBuffer, renderPassBeginInfo, VkSubpassContents.VK_SUBPASS_CONTENTS_INLINE);
        }
    }

    public void BindIndexBuffer(IndexBufferHandler indexBuffer) =>
        this.vk.CmdBindIndexBuffer(this.Context.Frame.CommandBuffer, indexBuffer.Buffer.Handler, 0, indexBuffer.Type);

    public void BindPipeline(ShaderHandler shader) =>
        this.vk.CmdBindPipeline(this.Context.Frame.CommandBuffer, shader.PipelineBindPoint, shader.Pipeline);

    public void BindVertexBuffer(VertexBufferHandler vertexBuffer) =>
        this.vk.CmdBindVertexBuffers(this.Context.Frame.CommandBuffer, 0, [vertexBuffer.Buffer.Handler], [0]);

    public void BindVertexBuffer(VertexBufferHandler[] vertexBuffers) =>
        this.vk.CmdBindVertexBuffers(this.Context.Frame.CommandBuffer, 0, [.. vertexBuffers.Select(x => x.Buffer.Handler)], new VkDeviceSize[vertexBuffers.Length]);

    public void BindUniformSet(UniformSet uniformSet) =>
        this.vk.CmdBindDescriptorSets(this.Context.Frame.CommandBuffer, uniformSet.Shader.PipelineBindPoint, uniformSet.Shader.PipelineLayout, 0, uniformSet.DescriptorSets, null);

    public BufferHandler CreateBuffer(ulong size, VkBufferUsageFlagBits usage, VkMemoryPropertyFlagBits properties)
    {
        this.CreateBuffer(size, usage, properties, out var buffer, out var allocation);

        return new()
        {
            Allocation = allocation,
            Handler    = buffer,
            Usage      = usage,
        };
    }

    public IndexBufferHandler CreateIndexBuffer(IList<ushort> indices) =>
        this.CreateIndexBuffer(indices, VkIndexType.VK_INDEX_TYPE_UINT16);

    public IndexBufferHandler CreateIndexBuffer(IList<uint> indices) =>
        this.CreateIndexBuffer(indices, VkIndexType.VK_INDEX_TYPE_UINT32);

    public IndexBufferHandler CreateIndexBuffer<T>(IList<T> indices, VkIndexType indexType) where T : unmanaged, INumber<T>
    {
        VkDeviceSize bufferSize = (ulong)(sizeof(T) * indices.Count);

        var buffer = this.CreateBuffer(
            bufferSize,
            VkBufferUsageFlagBits.VK_BUFFER_USAGE_TRANSFER_DST_BIT | VkBufferUsageFlagBits.VK_BUFFER_USAGE_INDEX_BUFFER_BIT,
            VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT
        );

        this.UpdateBuffer(buffer, [.. indices]);

        return new()
        {
            Buffer = buffer,
            Type   = indexType,
            Size   = (uint)indices.Count,
        };
    }

    public ShaderHandler CreateShader()
    {
        var renderPass          = this.CreateRenderPass(this.Context.ScreenFormat);
        var descriptorSetLayout = this.CreateDescriptorSetLayout();
        var pipelineLayout      = this.CreatePipelineLayout(descriptorSetLayout);
        var pipeline            = this.CreatePipeline(pipelineLayout, renderPass);

        return new()
        {
            DescriptorSetLayout = descriptorSetLayout,
            Pipeline            = pipeline,
            PipelineBindPoint   = VkPipelineBindPoint.VK_PIPELINE_BIND_POINT_GRAPHICS,
            PipelineLayout      = pipelineLayout,
            RenderPass          = renderPass,
        };
    }

    public VkSampler CreateSampler()
    {
        this.Context.GetPhysicalDeviceProperties(out var properties);

        var samplerCreateInfo = new VkSamplerCreateInfo
        {
            addressModeU  = VkSamplerAddressMode.VK_SAMPLER_ADDRESS_MODE_REPEAT,
            addressModeV  = VkSamplerAddressMode.VK_SAMPLER_ADDRESS_MODE_REPEAT,
            addressModeW  = VkSamplerAddressMode.VK_SAMPLER_ADDRESS_MODE_REPEAT,
            borderColor   = VkBorderColor.VK_BORDER_COLOR_INT_OPAQUE_BLACK,
            compareOp     = VkCompareOp.VK_COMPARE_OP_ALWAYS,
            magFilter     = VkFilter.VK_FILTER_LINEAR,
            maxAnisotropy = properties.limits.maxSamplerAnisotropy,
            maxLod        = 1,
            minFilter     = VkFilter.VK_FILTER_LINEAR,
            mipmapMode    = VkSamplerMipmapMode.VK_SAMPLER_MIPMAP_MODE_LINEAR,
        };

        VkCheck(this.vk.CreateSampler(this.Context.Device, samplerCreateInfo, default, out var sampler));

        return sampler;
    }

    public TextureHandler CreateTexture(TextureCreate textureCreate)
    {
        var imageType = ConvertToImageType(textureCreate.TextureType);

        var samples = VkSampleCountFlagBits.VK_SAMPLE_COUNT_1_BIT;
        var tiling  = VkImageTiling.VK_IMAGE_TILING_OPTIMAL;
        var usage   = VkImageUsageFlagBits.VK_IMAGE_USAGE_TRANSFER_SRC_BIT | VkImageUsageFlagBits.VK_IMAGE_USAGE_TRANSFER_DST_BIT | VkImageUsageFlagBits.VK_IMAGE_USAGE_SAMPLED_BIT;
        var format  = VkFormat.VK_FORMAT_B8G8R8A8_SRGB;

        this.CreateImage(
            textureCreate.Width,
            textureCreate.Height,
            samples,
            format,
            tiling,
            usage,
            VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT,
            out var image,
            out var allocation
        );

        this.TransitionImageLayout(
            image,
            VkImageLayout.VK_IMAGE_LAYOUT_UNDEFINED,
            VkImageLayout.VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL,
            default,
            VkAccessFlagBits.VK_ACCESS_TRANSFER_WRITE_BIT,
            VkPipelineStageFlagBits.VK_PIPELINE_STAGE_TOP_OF_PIPE_BIT,
            VkPipelineStageFlagBits.VK_PIPELINE_STAGE_TRANSFER_BIT
        );

        var imageView = this.CreateImageView(image, VkFormat.VK_FORMAT_B8G8R8A8_SRGB, VkImageAspectFlagBits.VK_IMAGE_ASPECT_COLOR_BIT);

        var texture = new TextureHandler
        {
            Allocation   = allocation,
            Depth        = textureCreate.Depth,
            Format       = format,
            Height       = textureCreate.Height,
            Image        = image,
            ImageTiling  = tiling,
            ImageType    = imageType,
            ImageUsage   = usage,
            ImageView    = imageView,
            SampleCount  = samples,
            Samples      = samples,
            Width        = textureCreate.Width,
        };

        this.UpdateTexture(texture, textureCreate.Data);

        return texture;
    }

    public UniformSet CreateUniformSet(IList<Uniform> uniforms, ShaderHandler shader)
    {
        using var disposables = new Disposables();

        var poolKey = uniforms.Select(x => ConvertToDescriptorType(x.Type)).Aggregate((previous, current) => previous | current);

        var writes = new List<VkWriteDescriptorSet>();

        var descriptorPool = this.CreateDescriptorPool(poolKey);

        var descriptorSetLayout = shader.DescriptorSetLayout;

        var descriptorSetAllocateInfo = new VkDescriptorSetAllocateInfo
        {
            descriptorPool     = descriptorPool.Handler,
            descriptorSetCount = 1,
            pSetLayouts        = &descriptorSetLayout,
        };

        this.vk.AllocateDescriptorSets(this.Context.Device, descriptorSetAllocateInfo, out var descriptorSets);

        foreach (var uniform in uniforms)
        {
            switch (uniform)
            {
                case CombinedImageSamplerUniform combinedImageSampler:
                {
                    var pImageInfo = new PointerArray<VkDescriptorImageInfo>(combinedImageSampler.Images.Count);

                    disposables.Add(pImageInfo);

                    for (var i = 0; i < combinedImageSampler.Images.Count; i++)
                    {
                        var image = combinedImageSampler.Images[i];

                        var descriptorImageInfo = new VkDescriptorImageInfo
                        {
                            sampler     = image.Sampler,
                            imageView   = image.Texture.ImageView,
                            imageLayout = VkImageLayout.VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL,
                        };

                        pImageInfo[i] = descriptorImageInfo;
                    }

                    var writeDescriptorSet = new VkWriteDescriptorSet
                    {
                        descriptorCount = 1,
                        descriptorType  = VkDescriptorType.VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER,
                        dstBinding      = uniform.Binding,
                        dstSet          = descriptorSets[0],
                        pImageInfo      = pImageInfo,
                    };

                    writes.Add(writeDescriptorSet);

                    break;
                }
                case UniformBufferUniform uniformBuffer:
                {
                    var descriptorBufferInfo = new VkDescriptorBufferInfo
                    {
                        buffer = uniformBuffer.Buffer.Handler,
                        offset = uniformBuffer.Buffer.Allocation.Offset,
                        range  = uniformBuffer.Buffer.Allocation.Size,
                    };

                    var pBufferInfo = new PointerArray<VkDescriptorBufferInfo>([descriptorBufferInfo]);

                    disposables.Add(pBufferInfo);

                    var writeDescriptorSet = new VkWriteDescriptorSet
                    {
                        descriptorCount = 1,
                        descriptorType  = VkDescriptorType.VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER,
                        dstBinding      = uniform.Binding,
                        dstSet          = descriptorSets[0],
                        pBufferInfo     = pBufferInfo,
                    };

                    writes.Add(writeDescriptorSet);

                    break;
                }
                default:
                    throw new Exception();
            }

        }

        this.vk.UpdateDescriptorSets(this.Context.Device, [..writes], null);

        var uniformSet = new UniformSet()
        {
            DescriptorPool = descriptorPool,
            DescriptorSets = descriptorSets,
            Shader         = shader,
        };

        return uniformSet;
    }

    public VertexBufferHandler CreateVertexBuffer<T>(T[] data) where T : unmanaged
    {
        var size = (ulong)(data.Length * sizeof(T));
        var buffer = this.CreateBuffer(
            size,
            VkBufferUsageFlagBits.VK_BUFFER_USAGE_TRANSFER_DST_BIT | VkBufferUsageFlagBits.VK_BUFFER_USAGE_VERTEX_BUFFER_BIT,
            VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT
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

    public void DrawIndexed(IndexBufferHandler indexBuffer) =>
        this.vk.CmdDrawIndexed(this.Context.Frame.CommandBuffer, indexBuffer.Size, 1, 0, 0, 0);

    public void EndFrame()
    {
        this.vk.EndCommandBuffer(this.Context.Frame.CommandBuffer);
        this.Context.SwapBuffers();
    }

    public void EndRenderPass() =>
        this.vk.CmdEndRenderPass(this.Context.Frame.CommandBuffer);

    public void DestroyBuffer(BufferHandler buffer)
    {
        this.vk.DestroyBuffer(this.Context.Device, buffer.Handler, null);
        this.vk.FreeMemory(this.Context.Device, buffer.Allocation.Memory, null);
    }

    public void DestroyIndexBuffer(IndexBufferHandler indexBuffer) =>
        this.DestroyBuffer(indexBuffer.Buffer);

    public void DestroyVertexBuffer(VertexBufferHandler vertexBuffer) =>
        this.DestroyBuffer(vertexBuffer.Buffer);

    public void DestroySampler(VkSampler sampler) =>
        this.vk.DestroySampler(this.Context.Device, sampler, null);

    public void DestroyShader(Shader shader)
    {
        var device = this.Context.Device;

        this.vk.DestroyPipeline(device, shader.Handler.Pipeline, null);
        this.vk.DestroyPipelineLayout(device, shader.Handler.PipelineLayout, null);
        this.vk.DestroyDescriptorSetLayout(device, shader.Handler.DescriptorSetLayout, null);
        this.vk.DestroyRenderPass(device, shader.Handler.RenderPass, null);
    }

    public void DestroyTexture(TextureHandler texture)
    {
        var device = this.Context.Device;

        this.vk.FreeMemory(device, texture.Allocation.Memory, null);
        this.vk.DestroyImage(device, texture.Image, null);
        this.vk.DestroyImageView(device, texture.ImageView, null);
    }

    public void DestroyUniformSet(UniformSet uniformSet)
    {
        VkCheck(this.vk.FreeDescriptorSets(this.Context.Device, uniformSet.DescriptorPool.Handler, uniformSet.DescriptorSets));

        this.RemoveFromDescriptorPool(uniformSet.DescriptorPool);
    }

    public void SetViewport(SurfaceContext surfaceContext)
    {
        var commandBuffer = this.Context.Frame.CommandBuffer;

        var viewport = new VkViewport
        {
            x        = 0,
            y        = 0,
            width    = surfaceContext.Size.Width,
            height   = surfaceContext.Size.Height,
            minDepth = 0,
            maxDepth = 1
        };

        this.vk.CmdSetViewport(commandBuffer, 0, viewport);

        var scissor = new VkRect2D
        {
            offset = new()
            {
                x = 0,
                y = 0
            },
            extent = surfaceContext.Swapchain.Extent,
        };

        this.vk.CmdSetScissor(commandBuffer, 0, scissor);
    }

    public void UpdateBuffer<T>(BufferHandler buffer, T data) where T : unmanaged =>
        this.UpdateBuffer(buffer, [data]);

    public void UpdateBuffer<T>(BufferHandler buffer, T[] data) where T : unmanaged
    {
        this.CreateBuffer(
            buffer.Allocation.Size,
            VkBufferUsageFlagBits.VK_BUFFER_USAGE_TRANSFER_SRC_BIT,
            VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT | VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_COHERENT_BIT,
            out var stagingBuffer,
            out var allocation
        );

        var stagingMemory = allocation.Memory;

        this.vk.MapMemory(this.Context.Device, stagingMemory, 0, 0, data);
        this.vk.UnmapMemory(this.Context.Device, stagingMemory);

        this.CopyBuffer(stagingBuffer, buffer.Handler, buffer.Allocation.Size);

        this.vk.DestroyBuffer(this.Context.Device, stagingBuffer, null);
        this.vk.FreeMemory(this.Context.Device, stagingMemory, null);
    }

    public void UpdateIndexBuffer<T>(VertexBufferHandler indexBuffer, T data) where T : unmanaged =>
        this.UpdateBuffer(indexBuffer.Buffer, data);

    public void UpdateIndexBuffer<T>(VertexBufferHandler indexBuffer, T[] data) where T : unmanaged =>
        this.UpdateBuffer(indexBuffer.Buffer, data);

    public void UpdateVertexBuffer<T>(VertexBufferHandler vertexBuffer, T data) where T : unmanaged =>
        this.UpdateBuffer(vertexBuffer.Buffer, data);

    public void UpdateVertexBuffer<T>(VertexBufferHandler vertexBuffer, T[] data) where T : unmanaged =>
        this.UpdateBuffer(vertexBuffer.Buffer, data);

    public void UpdateTexture(TextureHandler textureData, uint[] data)
    {
        VkDeviceSize imageSize = (ulong)data.Length * 4;

        this.CreateBuffer(
            imageSize,
            VkBufferUsageFlagBits.VK_BUFFER_USAGE_TRANSFER_SRC_BIT,
            VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT | VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_COHERENT_BIT,
            out var stagingBuffer,
            out var allocation
        );

        var stagingMemory = allocation.Memory;

        VkCheck(this.vk.MapMemory(this.Context.Device, stagingMemory, 0, 0, data));
        this.vk.UnmapMemory(this.Context.Device, stagingMemory);

        this.TransitionImageLayout(
            textureData.Image,
            VkImageLayout.VK_IMAGE_LAYOUT_UNDEFINED,
            VkImageLayout.VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL,
            default,
            VkAccessFlagBits.VK_ACCESS_TRANSFER_WRITE_BIT,
            VkPipelineStageFlagBits.VK_PIPELINE_STAGE_TOP_OF_PIPE_BIT,
            VkPipelineStageFlagBits.VK_PIPELINE_STAGE_TRANSFER_BIT
        );

        this.CopyBufferToImage(stagingBuffer, textureData.Image, textureData.Width, textureData.Height);

        this.TransitionImageLayout(
            textureData.Image,
            VkImageLayout.VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL,
            VkImageLayout.VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL,
            VkAccessFlagBits.VK_ACCESS_TRANSFER_WRITE_BIT,
            VkAccessFlagBits.VK_ACCESS_SHADER_READ_BIT,
            VkPipelineStageFlagBits.VK_PIPELINE_STAGE_TRANSFER_BIT,
            VkPipelineStageFlagBits.VK_PIPELINE_STAGE_FRAGMENT_SHADER_BIT
        );

        this.vk.FreeMemory(this.Context.Device, stagingMemory, null);
        this.vk.DestroyBuffer(this.Context.Device, stagingBuffer, null);
    }

    public void WaitIdle() =>
        this.vk.DeviceWaitIdle(this.Context.Device);


}
