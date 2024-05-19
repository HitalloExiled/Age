#define SIMPLE_ENGINE_V2
#if SIMPLE_ENGINE_V2
using System.Diagnostics;
using System.Runtime.InteropServices;
using Age.Numerics;
using Age.Platforms.Abstractions;
using SkiaSharp;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Extensions;
using ThirdParty.Vulkan.Flags;

using PlatformWindow = Age.Platforms.Display.Window;
using WavefrontLoader = Age.Resources.Loaders.Wavefront.Loader;
using Age.Core.Interop;

namespace Age.Playground;

public unsafe partial class SimpleEngineV2 : IDisposable
{
    private const int MAX_FRAMES_IN_FLIGHT = 2;

    private static readonly DateTime startTime = DateTime.UtcNow;

    private readonly HashSet<string>        deviceExtensions         = [VkSwapchainExtensionKHR.Name];
    private readonly bool                   enableValidationLayers   = Debugger.IsAttached;
    private readonly VkSemaphore[]          imageAvailableSemaphores = new VkSemaphore[MAX_FRAMES_IN_FLIGHT];
    private readonly List<uint>             indices                  = [];
    private readonly VkFence[]              inFlightFences           = new VkFence[MAX_FRAMES_IN_FLIGHT];
    private readonly WavefrontLoader        wavefrontLoader          = new(new FileSystem());
    private readonly VkSemaphore[]          renderFinishedSemaphores = new VkSemaphore[MAX_FRAMES_IN_FLIGHT];
    private readonly VkBuffer[]             uniformBuffers           = new VkBuffer[MAX_FRAMES_IN_FLIGHT];
    private readonly nint[]                 uniformBuffersMapped     = new nint[MAX_FRAMES_IN_FLIGHT];
    private readonly VkDeviceMemory[]       uniformBuffersMemory     = new VkDeviceMemory[MAX_FRAMES_IN_FLIGHT];
    private readonly HashSet<string>        validationLayers         = ["VK_LAYER_KHRONOS_validation"];
    private readonly List<Vertex>           vertices                 = [];

    private VkImage                   colorImage            = null!;
    private VkDeviceMemory            colorImageMemory      = null!;
    private VkImageView               colorImageView        = null!;
    private VkCommandBuffer[]         commandBuffers        = [];
    private VkCommandPool             commandPool           = null!;
    private uint                      currentFrame;
    private VkDebugUtilsMessengerEXT  debugMessenger        = null!;
    private VkImage                   depthImage            = null!;
    private VkDeviceMemory            depthImageMemory      = null!;
    private VkImageView               depthImageView        = null!;
    private VkDescriptorPool          descriptorPool        = null!;
    private VkDescriptorSetLayout     descriptorSetLayout   = null!;
    private VkDescriptorSet[]         descriptorSets        = [];
    private VkDevice                  device                = null!;
    private bool                      disposed;
    private bool                      framebufferResized;
    private VkPipeline                graphicsPipeline      = null!;
    private VkQueue                   graphicsQueue         = null!;
    private VkBuffer                  indexBuffer           = null!;
    private VkDeviceMemory            indexBufferMemory     = null!;
    private VkInstance                instance              = null!;
    private uint                      mipLevels;
    private VkSampleCountFlags        msaaSamples           = VkSampleCountFlags.N1;
    private VkPhysicalDevice          physicalDevice        = null!;
    private VkPipelineLayout          pipelineLayout        = null!;
    private VkQueue                   presentQueue          = null!;
    private VkRenderPass              renderPass            = null!;
    private VkSurfaceKHR              surface               = null!;
    private VkSwapchainKHR            swapChain             = null!;
    private VkExtent2D                swapChainExtent;
    private VkFramebuffer[]           swapChainFramebuffers = [];
    private VkFormat                  swapChainImageFormat;
    private VkImage[]                 swapChainImages       = [];
    private VkImageView[]             swapChainImageViews   = [];
    private VkImage                   textureImage          = null!;
    private VkDeviceMemory            textureImageMemory    = null!;
    private VkImageView               textureImageView      = null!;
    private VkSampler                 textureSampler        = null!;
    private VkBuffer                  vertexBuffer          = null!;
    private VkDeviceMemory            vertexBufferMemory    = null!;
    private VkDebugUtilsExtensionEXT? debugUtilsExtension;
    private VkSurfaceExtensionKHR     surfaceExtension      = null!;
    private VkSwapchainExtensionKHR   swapchainExtension    = null!;
    private PlatformWindow            window                = null!;

    public SimpleEngineV2()
    {
        for (var i = 0; i < MAX_FRAMES_IN_FLIGHT; i++)
        {
            this.uniformBuffersMapped[i] = (nint)NativeMemory.Alloc((uint)sizeof(UniformBufferObject));
        }
    }

    private static VkPresentModeKHR ChooseSwapPresentMode(VkPresentModeKHR[] availablePresentModes)
    {
        foreach (var availablePresentMode in availablePresentModes)
        {
            if (availablePresentMode == VkPresentModeKHR.Mailbox)
            {
                return availablePresentMode;
            }
        }

        return VkPresentModeKHR.Fifo;
    }

    private static VkSurfaceFormatKHR ChooseSwapSurfaceFormat(VkSurfaceFormatKHR[] availableFormats)
    {
        foreach (var availableFormat in availableFormats)
        {
            if (availableFormat.Format == VkFormat.B8G8R8A8Srgb && availableFormat.ColorSpace == VkColorSpaceKHR.SrgbNonlinear)
            {
                return availableFormat;
            }
        }

        return availableFormats[0];
    }

    private static VkBool32 DebugCallback(VkDebugUtilsMessageSeverityFlagsEXT messageSeverity, VkDebugUtilsMessageTypeFlagsEXT messageTypes, VkDebugUtilsMessengerCallbackDataEXT* pCallbackData, void* pUserData)
    {
        Console.WriteLine("validation layer: " + Marshal.PtrToStringAnsi((nint)pCallbackData->PMessage));

        return false;
    }

    private static void PopulateDebugMessengerCreateInfo(out VkDebugUtilsMessengerCreateInfoEXT createInfo) =>
        createInfo = new VkDebugUtilsMessengerCreateInfoEXT
        {
            MessageSeverity = VkDebugUtilsMessageSeverityFlagsEXT.Verbose | VkDebugUtilsMessageSeverityFlagsEXT.Warning | VkDebugUtilsMessageSeverityFlagsEXT.Error,
            MessageType     = VkDebugUtilsMessageTypeFlagsEXT.General | VkDebugUtilsMessageTypeFlagsEXT.Validation | VkDebugUtilsMessageTypeFlagsEXT.Performance,
            PfnUserCallback = new(DebugCallback),
        };

    private VkCommandBuffer BeginSingleTimeCommands()
    {
        var commandBuffer = this.commandPool.AllocateCommand(VkCommandBufferLevel.Primary);

        commandBuffer.Begin();

        return commandBuffer;
    }

    private bool CheckDeviceExtensionSupport(VkPhysicalDevice physicalDevice)
    {
        var extensions = physicalDevice.EnumerateDeviceExtensionProperties();

        return this.deviceExtensions.Overlaps(extensions.Select(x => Marshal.PtrToStringAnsi((nint)x.ExtensionName)!));
    }

    private bool CheckValidationLayerSupport()
    {
        var availableLayers = VkInstance.EnumerateLayerProperties();

        return this.validationLayers.Overlaps(availableLayers.Select(x => Marshal.PtrToStringAnsi((nint)x.LayerName)!));
    }

    private VkExtent2D ChooseSwapExtent(VkSurfaceCapabilitiesKHR capabilities)
    {
        if (capabilities.CurrentExtent.Width != uint.MaxValue)
        {
            return capabilities.CurrentExtent;
        }
        else
        {
            var actualExtent = new VkExtent2D
            {
                Width  = Math.Clamp(this.window.Size.Width, capabilities.MinImageExtent.Width, capabilities.MaxImageExtent.Width),
                Height = Math.Clamp(this.window.Size.Height, capabilities.MinImageExtent.Height, capabilities.MaxImageExtent.Height),
            };

            return actualExtent;
        }
    }

    private void Cleanup()
    {
        this.CleanupSwapChain();

        this.textureSampler.Dispose();
        this.textureImageView.Dispose();
        this.textureImage.Dispose();
        this.textureImageMemory.Dispose();
        this.graphicsPipeline.Dispose();
        this.pipelineLayout.Dispose();
        this.renderPass.Dispose();

        for (var i = 0; i < MAX_FRAMES_IN_FLIGHT; i++)
        {
            this.uniformBuffers[i].Dispose();
            this.uniformBuffersMemory[i].Dispose();
        }

        this.descriptorPool.Dispose();
        this.descriptorSetLayout.Dispose();
        this.indexBuffer.Dispose();
        this.indexBufferMemory.Dispose();
        this.vertexBuffer.Dispose();
        this.vertexBufferMemory.Dispose();

        for (var i = 0; i < MAX_FRAMES_IN_FLIGHT; i++)
        {
            this.imageAvailableSemaphores[i].Dispose();
            this.renderFinishedSemaphores[i].Dispose();
            this.inFlightFences[i].Dispose();
        }

        this.commandPool.Dispose();
        this.device.Dispose();

        if (this.enableValidationLayers && this.debugUtilsExtension != null)
        {
            this.debugMessenger.Dispose();
        }

        this.surface.Dispose();
        this.instance.Dispose();
    }

    private void CleanupSwapChain()
    {
        this.depthImageView.Dispose();
        this.depthImage.Dispose();
        this.depthImageMemory.Dispose();

        this.colorImageView.Dispose();
        this.colorImage.Dispose();
        this.colorImageMemory.Dispose();

        for (var i = 0; i < this.swapChainFramebuffers.Length; i++)
        {
            this.swapChainFramebuffers[i].Dispose();
        }

        for (var i = 0; i < this.swapChainImageViews.Length; i++)
        {
            this.swapChainImageViews[i].Dispose();
        }

        this.swapChain.Dispose();
    }

    private void CopyBuffer(VkBuffer srcBuffer, VkBuffer dstBuffer, ulong size)
    {
        var commandBuffer = this.BeginSingleTimeCommands();

        var copyRegion = new VkBufferCopy
        {
            Size = size
        };

        commandBuffer.CopyBuffer(srcBuffer, dstBuffer, copyRegion);

        this.EndSingleTimeCommands(commandBuffer);
    }

    private void CopyBufferToImage(VkBuffer buffer, VkImage image, int width, int height)
    {
        var commandBuffer = this.BeginSingleTimeCommands();

        var region = new VkBufferImageCopy
        {
            BufferOffset      = 0,
            BufferRowLength   = 0,
            BufferImageHeight = 0,
            ImageSubresource  = new()
            {
                AspectMask     = VkImageAspectFlags.Color,
                MipLevel       = 0,
                BaseArrayLayer = 0,
                LayerCount     = 1,
            },
            ImageOffset = new(),
            ImageExtent = new()
            {
                Width  = (uint)width,
                Height = (uint)height,
                Depth  = 1
            },
        };

        commandBuffer.CopyBufferToImage(buffer, image, VkImageLayout.TransferDstOptimal, [region]);

        this.EndSingleTimeCommands(commandBuffer);
    }

    private void CreateBuffer(ulong size, VkBufferUsageFlags usage, VkMemoryPropertyFlags properties, out VkBuffer buffer, out VkDeviceMemory bufferMemory)
    {
        var createInfo = new VkBufferCreateInfo
        {
            Size        = size,
            Usage       = usage,
            SharingMode = VkSharingMode.Exclusive
        };

        buffer = this.device.CreateBuffer(createInfo);

        buffer.GetMemoryRequirements(out var memRequirements);

        var allocInfo = new VkMemoryAllocateInfo
        {
            AllocationSize  = memRequirements.Size,
            MemoryTypeIndex = this.FindMemoryType(memRequirements.MemoryTypeBits, properties)
        };

        bufferMemory = this.device.AllocateMemory(allocInfo);

        buffer.BindMemory(bufferMemory, 0);
    }

    private void CreateCommandBuffers() =>
        this.commandBuffers = this.commandPool.AllocateCommands(MAX_FRAMES_IN_FLIGHT, VkCommandBufferLevel.Primary);

    private void CreateCommandPool()
    {
        var queueFamilyIndices = this.FindQueueFamilies(this.physicalDevice);

        this.commandPool = this.device.CreateCommandPool(queueFamilyIndices.GraphicsFamily!.Value, VkCommandPoolCreateFlags.ResetCommandBuffer);
    }

    private void CreateColorResources()
    {
        var colorFormat = this.swapChainImageFormat;

        this.CreateImage(
            this.swapChainExtent.Width,
            this.swapChainExtent.Height,
            1,
            this.msaaSamples,
            colorFormat,
            VkImageTiling.Optimal,
            VkImageUsageFlags.TransientAttachment | VkImageUsageFlags.ColorAttachment,
            VkMemoryPropertyFlags.DeviceLocal,
            out this.colorImage,
            out this.colorImageMemory
        );
        this.colorImageView = this.CreateImageView(this.colorImage, colorFormat, VkImageAspectFlags.Color, 1);
    }

    private void CreateDepthResources()
    {
        var depthFormat = this.FindDepthFormat();

        this.CreateImage(
            this.swapChainExtent.Width,
            this.swapChainExtent.Height,
            1,
            this.msaaSamples,
            depthFormat,
            VkImageTiling.Optimal,
            VkImageUsageFlags.DepthStencilAttachment,
            VkMemoryPropertyFlags.DeviceLocal,
            out this.depthImage,
            out this.depthImageMemory
        );

        this.depthImageView = this.CreateImageView(this.depthImage, depthFormat, VkImageAspectFlags.Depth, 1);
    }

    private void CreateDescriptorPool()
    {
        var poolSizes = new VkDescriptorPoolSize[]
        {
            new()
            {
                Type            = VkDescriptorType.UniformBuffer,
                DescriptorCount = MAX_FRAMES_IN_FLIGHT,
            },
            new()
            {
                Type            = VkDescriptorType.CombinedImageSampler,
                DescriptorCount = MAX_FRAMES_IN_FLIGHT,
            },
        };

        fixed (VkDescriptorPoolSize* pPoolSizes = poolSizes)
        {
            var createInfo = new VkDescriptorPoolCreateInfo
            {
                PPoolSizes    = pPoolSizes,
                PoolSizeCount = (uint)poolSizes.Length,
                MaxSets       = MAX_FRAMES_IN_FLIGHT,
            };

            this.descriptorPool = this.device.CreateDescriptorPool(createInfo);
        }
    }

    private void CreateDescriptorSets()
    {
        var layouts = new VkHandle<VkDescriptorSetLayout>[MAX_FRAMES_IN_FLIGHT]
        {
            this.descriptorSetLayout.Handle,
            this.descriptorSetLayout.Handle,
        };

        fixed (VkHandle<VkDescriptorSetLayout>* pSetLayouts = layouts)
        {
            var allocInfo = new VkDescriptorSetAllocateInfo
            {
                DescriptorSetCount = (uint)layouts.Length,
                PSetLayouts        = pSetLayouts
            };

            this.descriptorSets = this.descriptorPool.AllocateDescriptorSets(allocInfo);

            for (var i = 0; i < MAX_FRAMES_IN_FLIGHT; i++)
            {
                var bufferInfo = new VkDescriptorBufferInfo
                {
                    Buffer = this.uniformBuffers[i].Handle,
                    Offset = 0,
                    Range  = (uint)Marshal.SizeOf<UniformBufferObject>()
                };

                var imageInfo = new VkDescriptorImageInfo
                {
                    ImageLayout = VkImageLayout.ShaderReadOnlyOptimal,
                    ImageView   = this.textureImageView.Handle,
                    Sampler     = this.textureSampler.Handle
                };

                var descriptorWrites = new VkWriteDescriptorSet[]
                {
                    new()
                    {
                        DescriptorCount = 1,
                        DescriptorType  = VkDescriptorType.UniformBuffer,
                        DstArrayElement = 0,
                        DstBinding      = 0,
                        DstSet          = this.descriptorSets[i].Handle,
                        PBufferInfo     = &bufferInfo,
                    },
                    new()
                    {
                        DescriptorCount = 1,
                        DescriptorType  = VkDescriptorType.CombinedImageSampler,
                        DstArrayElement = 0,
                        DstBinding      = 1,
                        DstSet          = this.descriptorSets[i].Handle,
                        PImageInfo      = &imageInfo,
                    }
                };

                this.device.UpdateDescriptorSets(descriptorWrites, []);
            }
        }
    }

    private void CreateDescriptorSetLayout()
    {
        var uboLayoutBinding = new VkDescriptorSetLayoutBinding
        {
            Binding         = 0,
            DescriptorType  = VkDescriptorType.UniformBuffer,
            DescriptorCount = 1,
            StageFlags      = VkShaderStageFlags.Vertex,
        };

        var samplerLayoutBinding = new VkDescriptorSetLayoutBinding
        {
            Binding         = 1,
            DescriptorCount = 1,
            DescriptorType  = VkDescriptorType.CombinedImageSampler,
            StageFlags      = VkShaderStageFlags.Fragment
        };

        var bindings = new[]
        {
            uboLayoutBinding,
            samplerLayoutBinding
        };

        fixed (VkDescriptorSetLayoutBinding* pBindings = bindings)
        {
            var createInfo = new VkDescriptorSetLayoutCreateInfo
            {
                PBindings    = pBindings,
                BindingCount = (uint)bindings.Length,
            };

            this.descriptorSetLayout = this.device.CreateDescriptorSetLayout(createInfo);
        }
    }

    private void CreateFramebuffers()
    {
        this.swapChainFramebuffers = new VkFramebuffer[this.swapChainImageViews.Length];

        for (var i = 0; i < this.swapChainImageViews.Length; i++)
        {
            var attachments = new VkHandle<VkImageView>[]
            {
                this.colorImageView.Handle,
                this.depthImageView.Handle,
                this.swapChainImageViews[i].Handle
            };

            fixed (VkHandle<VkImageView>* pAttachments = attachments)
            {
                var createInfo = new VkFramebufferCreateInfo
                {
                    RenderPass      = this.renderPass.Handle,
                    PAttachments    = pAttachments,
                    AttachmentCount = (uint)attachments.Length,
                    Width           = this.swapChainExtent.Width,
                    Height          = this.swapChainExtent.Height,
                    Layers          = 1
                };

                this.swapChainFramebuffers[i] = this.device.CreateFramebuffer(createInfo);
            }
        }
    }

    private void CreateGraphicsPipeline()
    {
        var vertShaderCode = File.ReadAllBytes(Path.Join(AppContext.BaseDirectory, "Shaders/shader.vert.spv"))!;
        var fragShaderCode = File.ReadAllBytes(Path.Join(AppContext.BaseDirectory, "Shaders/shader.frag.spv"))!;

        var vertShaderModule = this.CreateShaderModule(vertShaderCode);
        var fragShaderModule = this.CreateShaderModule(fragShaderCode);

        fixed (byte* pName = "main"u8)
        {
            var vertShaderStageInfo = new VkPipelineShaderStageCreateInfo
            {
                Module = vertShaderModule.Handle,
                PName  = pName,
                Stage  = VkShaderStageFlags.Vertex,
            };

            var fragShaderStageInfo = new VkPipelineShaderStageCreateInfo
            {
                Module = fragShaderModule.Handle,
                PName  = pName,
                Stage  = VkShaderStageFlags.Fragment,
            };

            var stages = new[]
            {
                vertShaderStageInfo,
                fragShaderStageInfo
            };

            var bindingDescription    = Vertex.GetBindingDescription();
            var attributeDescriptions = Vertex.GetAttributeDescriptions();

            var dynamicStates = new[]
            {
                VkDynamicState.Viewport,
                VkDynamicState.Scissor,
            };

            fixed (VkVertexInputAttributeDescription* pVertexAttributeDescriptions = attributeDescriptions)
            fixed (VkPipelineShaderStageCreateInfo*   pStages                      = stages)
            fixed (VkDynamicState*                    pDynamicStates               = dynamicStates)
            {
                var vertexInputState = new VkPipelineVertexInputStateCreateInfo
            {
                PVertexBindingDescriptions      = &bindingDescription,
                VertexBindingDescriptionCount   = 1,
                PVertexAttributeDescriptions    = pVertexAttributeDescriptions,
                VertexAttributeDescriptionCount = (uint)attributeDescriptions.Length,
            };

            var inputAssemblyState = new VkPipelineInputAssemblyStateCreateInfo
            {
                Topology               = VkPrimitiveTopology.TriangleList,
                PrimitiveRestartEnable = false
            };

            var viewportState = new VkPipelineViewportStateCreateInfo
            {
                ViewportCount = 1,
                ScissorCount  = 1
            };

            var rasterizationState = new VkPipelineRasterizationStateCreateInfo
            {
                DepthClampEnable        = false,
                RasterizerDiscardEnable = false,
                PolygonMode             = VkPolygonMode.Fill,
                LineWidth               = 1,
                CullMode                = VkCullModeFlags.Back,
                FrontFace               = VkFrontFace.CounterClockwise,
                DepthBiasEnable         = false,
            };

            var multisampleState = new VkPipelineMultisampleStateCreateInfo
            {
                SampleShadingEnable  = true,
                RasterizationSamples = this.msaaSamples,
                MinSampleShading     = 1,
            };

            var depthStencilState = new VkPipelineDepthStencilStateCreateInfo
            {
                DepthTestEnable       = true,
                DepthWriteEnable      = true,
                DepthCompareOp        = VkCompareOp.Less,
                DepthBoundsTestEnable = false,
                StencilTestEnable     = false
            };

            var colorBlendAttachment = new VkPipelineColorBlendAttachmentState
            {
                ColorWriteMask = VkColorComponentFlags.R | VkColorComponentFlags.G | VkColorComponentFlags.B | VkColorComponentFlags.A,
                BlendEnable    = false
            };

            var colorBlendState = new VkPipelineColorBlendStateCreateInfo
            {
                LogicOpEnable   = false,
                LogicOp         = VkLogicOp.Copy,
                PAttachments    = &colorBlendAttachment,
                AttachmentCount = 1,
            };

                var dynamicState = new VkPipelineDynamicStateCreateInfo
                {
                    PDynamicStates    = pDynamicStates,
                    DynamicStateCount = (uint)dynamicStates.Length
                };

                var setLaout = this.descriptorSetLayout.Handle;

                var pipelineLayoutInfo = new VkPipelineLayoutCreateInfo
                {
                    PSetLayouts    = &setLaout,
                    SetLayoutCount = 1,
                };

                this.pipelineLayout = this.device.CreatePipelineLayout(pipelineLayoutInfo);

                var pipelineInfo = new VkGraphicsPipelineCreateInfo
                {
                    PStages             = pStages,
                    StageCount          = (uint)stages.Length,
                    PVertexInputState   = &vertexInputState,
                    PInputAssemblyState = &inputAssemblyState,
                    PViewportState      = &viewportState,
                    PRasterizationState = &rasterizationState,
                    PMultisampleState   = &multisampleState,
                    PColorBlendState    = &colorBlendState,
                    PDynamicState       = &dynamicState,
                    Layout              = this.pipelineLayout.Handle,
                    RenderPass          = this.renderPass.Handle,
                    Subpass             = 0,
                    BasePipelineHandle  = default,
                    PDepthStencilState  = &depthStencilState,
                };

                this.graphicsPipeline =  this.device.CreateGraphicsPipelines(pipelineInfo);

                fragShaderModule.Dispose();
                vertShaderModule.Dispose();
            }
        }
    }

    private void CreateImage(uint width, uint height, uint mipLevels, VkSampleCountFlags numSamples, VkFormat format, VkImageTiling tiling, VkImageUsageFlags usage, VkMemoryPropertyFlags properties, out VkImage image, out VkDeviceMemory imageMemory)
    {
        var createInfo = new VkImageCreateInfo
        {
            ArrayLayers = 1,
            Extent      = new()
            {
                Width  = width,
                Height = height,
                Depth  = 1,
            },
            Format        = format,
            ImageType     = VkImageType.N2D,
            InitialLayout = VkImageLayout.Undefined,
            MipLevels     = mipLevels,
            Samples       = numSamples,
            SharingMode   = VkSharingMode.Exclusive,
            Tiling        = tiling,
            Usage         = usage,
        };

        image = this.device.CreateImage(createInfo);

        image.GetMemoryRequirements(out var memRequirements);

        var allocInfo = new VkMemoryAllocateInfo
        {
            AllocationSize  = memRequirements.Size,
            MemoryTypeIndex = this.FindMemoryType(memRequirements.MemoryTypeBits, properties)
        };

        imageMemory = this.device.AllocateMemory(allocInfo);

        image.BindMemory(imageMemory, 0);
    }

    private VkImageView CreateImageView(VkImage image, VkFormat format, VkImageAspectFlags aspectFlags, uint mipLevels)
    {
        var createInfo = new VkImageViewCreateInfo
        {
            Image            = image.Handle,
            ViewType         = VkImageViewType.N2D,
            Format           = format,
            SubresourceRange = new()
            {
                AspectMask     = aspectFlags,
                BaseMipLevel   = 0,
                LevelCount     = mipLevels,
                BaseArrayLayer = 0,
                LayerCount     = 1
            }
        };

        return this.device.CreateImageView(createInfo);
    }

    private void CreateImageViews()
    {
        this.swapChainImageViews = new VkImageView[this.swapChainImages.Length];

        for (var i = 0; i < this.swapChainImages.Length; i++)
        {
            this.swapChainImageViews[i] = this.CreateImageView(this.swapChainImages[i], this.swapChainImageFormat, VkImageAspectFlags.Color, 1);
        }
    }

    private void CreateIndexBuffer()
    {
        var bufferSize = (ulong)(sizeof(int) * this.indices.Count);

        this.CreateBuffer(
            bufferSize,
            VkBufferUsageFlags.TransferSrc,
            VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent,
            out var stagingBuffer,
            out var stagingBufferMemory
        );

        stagingBufferMemory.Write(0, 0, [.. this.indices]);
        stagingBufferMemory.Unmap();

        this.CreateBuffer(
            bufferSize,
            VkBufferUsageFlags.TransferDst | VkBufferUsageFlags.IndexBuffer,
            VkMemoryPropertyFlags.DeviceLocal,
            out this.indexBuffer,
            out this.indexBufferMemory
        );

        this.CopyBuffer(stagingBuffer, this.indexBuffer, bufferSize);

        stagingBuffer.Dispose();
        stagingBufferMemory.Dispose();
    }

    private void CreateInstance()
    {
        if (this.enableValidationLayers && !this.CheckValidationLayerSupport())
        {
            throw new Exception("validation layers requested, but not available!");
        }

        VkApplicationInfo appInfo;

        fixed (byte* pApplicationName = "Hello Triangle"u8)
        fixed (byte* pEngineName      = "No Engine"u8)
        {
            appInfo = new VkApplicationInfo
            {
                PApplicationName   = pApplicationName,
                ApplicationVersion = new VkVersion(0, 1, 0, 0),
                PEngineName        = pEngineName,
                EngineVersion      = new VkVersion(0, 1, 0, 0),
                ApiVersion         = VkVersion.V1_0,
            };
        }

        VkDebugUtilsMessengerCreateInfoEXT debugCreateInfo;
        string[]                           enabledLayerNames = [];

        if (this.enableValidationLayers)
        {
            enabledLayerNames = [.. this.validationLayers];
            PopulateDebugMessengerCreateInfo(out debugCreateInfo);
        }

        using var ppEnabledExtensionNames = new NativeStringArray(this.GetRequiredExtensions());
        using var ppEnabledLayerNames     = new NativeStringArray(enabledLayerNames);

        var createInfo = new VkInstanceCreateInfo
        {
            PApplicationInfo        = &appInfo,
            PpEnabledExtensionNames = ppEnabledExtensionNames,
            EnabledExtensionCount   = (uint)ppEnabledExtensionNames.Length,
            PpEnabledLayerNames     = ppEnabledLayerNames,
            EnabledLayerCount       = (uint)ppEnabledLayerNames.Length,
            PNext                   = &debugCreateInfo,
        };

        this.instance = new VkInstance(createInfo);

        if (this.enableValidationLayers && !this.instance.TryGetExtension(out this.debugUtilsExtension))
        {
            throw new Exception($"Cannot found required extension {VkDebugUtilsExtensionEXT.Name}");
        }

        if (!this.instance.TryGetExtension<VkSurfaceExtensionKHR>(out var surfaceExtension))
        {
            throw new Exception($"Cannot found required extension {VkSurfaceExtensionKHR.Name}");
        }

        this.surfaceExtension = surfaceExtension;
    }

    private void CreateLogicalDevice()
    {
        var indices = this.FindQueueFamilies(this.physicalDevice);

        var queueCreateInfos    = new List<VkDeviceQueueCreateInfo>();
        var uniqueQueueFamilies = new HashSet<uint>
        {
            indices.GraphicsFamily!.Value,
            indices.PresentFamily!.Value
        };

        var queuePriorities = 1f;

        foreach (var queueFamily in uniqueQueueFamilies)
        {
            var queueCreateInfo = new VkDeviceQueueCreateInfo
            {
                QueueFamilyIndex = queueFamily,
                PQueuePriorities = &queuePriorities,
                QueueCount       = 1,
            };

            queueCreateInfos.Add(queueCreateInfo);
        }

        var deviceFeatures = new VkPhysicalDeviceFeatures
        {
            SamplerAnisotropy = true,
            SampleRateShading = true,
        };

        using var ppEnabledExtensionNames = new NativeStringArray(this.deviceExtensions.ToArray());

        VkDeviceCreateInfo createInfo;

        fixed (VkDeviceQueueCreateInfo* pQueueCreateInfos = CollectionsMarshal.AsSpan(queueCreateInfos))
        {
            createInfo = new VkDeviceCreateInfo
            {
                PQueueCreateInfos       = pQueueCreateInfos,
                QueueCreateInfoCount    = (uint)queueCreateInfos.Count,
                PEnabledFeatures        = &deviceFeatures,
                PpEnabledExtensionNames = ppEnabledExtensionNames,
                EnabledExtensionCount   = (uint)ppEnabledExtensionNames.Length,
            };
        }

        this.device = this.physicalDevice.CreateDevice(createInfo);

        if (!this.device.TryGetExtension<VkSwapchainExtensionKHR>(out var vkKhrSwapchain))
        {
            throw new Exception($"Cannot found required extension {VkSwapchainExtensionKHR.Name}");
        }

        this.swapchainExtension = vkKhrSwapchain;
        this.graphicsQueue  = this.device.GetQueue(indices.GraphicsFamily.Value, 0);
        this.presentQueue   = this.device.GetQueue(indices.PresentFamily.Value, 0);
    }

    private void CreateRenderPass()
    {
        var colorAttachment = new VkAttachmentDescription
        {
            Format         = this.swapChainImageFormat,
            Samples        = this.msaaSamples,
            LoadOp         = VkAttachmentLoadOp.Clear,
            StoreOp        = VkAttachmentStoreOp.Store,
            StencilLoadOp  = VkAttachmentLoadOp.DontCare,
            StencilStoreOp = VkAttachmentStoreOp.DontCare,
            InitialLayout  = VkImageLayout.Undefined,
            FinalLayout    = VkImageLayout.ColorAttachmentOptimal
        };

        var depthAttachment = new VkAttachmentDescription
        {
            Format         = this.FindDepthFormat(),
            Samples        = this.msaaSamples,
            LoadOp         = VkAttachmentLoadOp.Clear,
            StoreOp        = VkAttachmentStoreOp.DontCare,
            StencilLoadOp  = VkAttachmentLoadOp.DontCare,
            StencilStoreOp = VkAttachmentStoreOp.DontCare,
            InitialLayout  = VkImageLayout.Undefined,
            FinalLayout    = VkImageLayout.DepthStencilAttachmentOptimal
        };

        var colorAttachmentResolve = new VkAttachmentDescription
        {
            Format         = this.swapChainImageFormat,
            Samples        = VkSampleCountFlags.N1,
            LoadOp         = VkAttachmentLoadOp.DontCare,
            StoreOp        = VkAttachmentStoreOp.Store,
            StencilLoadOp  = VkAttachmentLoadOp.DontCare,
            StencilStoreOp = VkAttachmentStoreOp.DontCare,
            InitialLayout  = VkImageLayout.Undefined,
            FinalLayout    = VkImageLayout.PresentSrcKHR
        };

        var colorAttachmentRef = new VkAttachmentReference
        {
            Attachment = 0,
            Layout     = VkImageLayout.ColorAttachmentOptimal
        };

        var depthAttachmentRef = new VkAttachmentReference
        {
            Attachment = 1,
            Layout     = VkImageLayout.DepthStencilAttachmentOptimal
        };

        var colorAttachmentResolveRef = new VkAttachmentReference
        {
            Attachment = 2,
            Layout     = VkImageLayout.ColorAttachmentOptimal
        };

        var subpass = new VkSubpassDescription
        {
            PipelineBindPoint       = VkPipelineBindPoint.Graphics,
            PColorAttachments       = &colorAttachmentRef,
            ColorAttachmentCount    = 1,
            PDepthStencilAttachment = &depthAttachmentRef,
            PResolveAttachments     = &colorAttachmentResolveRef,
        };

        _ = new VkSubpassDependency
        {
            SrcSubpass = VkConstants.VK_SUBPASS_EXTERNAL,
            DstSubpass = 0,
            SrcStageMask = VkPipelineStageFlags.ColorAttachmentOutput | VkPipelineStageFlags.EarlyFragmentTests,
            SrcAccessMask = default,
            DstStageMask = VkPipelineStageFlags.ColorAttachmentOutput | VkPipelineStageFlags.EarlyFragmentTests,
            DstAccessMask = VkAccessFlags.ColorAttachmentWrite | VkAccessFlags.DepthStencilAttachmentWrite
        };

        var attachments = new[]
        {
            colorAttachment,
            depthAttachment,
            colorAttachmentResolve,
        };

        fixed (VkAttachmentDescription* pAttachments = attachments)
        {
            var renderPassInfo = new VkRenderPassCreateInfo
            {
                PAttachments    = pAttachments,
                AttachmentCount = (uint)attachments.Length,
                PSubpasses      = &subpass,
                SubpassCount    = 1,
            };

            this.renderPass = this.device.CreateRenderPass(renderPassInfo);
        }
    }

    private VkShaderModule CreateShaderModule(byte[] code)
    {
        fixed (byte* pCode = code)
        {
            var createInfo = new VkShaderModuleCreateInfo
            {
                PCode    = (uint*)pCode,
                CodeSize = (uint)code.Length,
            };

            return this.device.CreateShaderModule(createInfo);
        }
    }

    private void CreateSurface()
    {
        if (!this.instance.TryGetExtension<VkWin32SurfaceExtensionKHR>(out var vkKhrWin32Surface))
        {
            throw new Exception($"Cannot found required extension {VkWin32SurfaceExtensionKHR.Name}");
        }

        var createInfo = new VkWin32SurfaceCreateInfoKHR
        {
            Hwnd      = this.window.Handle,
            Hinstance = Process.GetCurrentProcess().Handle,
        };

        this.surface = vkKhrWin32Surface.CreateSurface(createInfo);
    }

    private void CreateSwapChain()
    {
        var swapChainSupport = this.QuerySwapChainSupport(this.physicalDevice);
        var surfaceFormat    = ChooseSwapSurfaceFormat(swapChainSupport.Formats);
        var presentMode      = ChooseSwapPresentMode(swapChainSupport.PresentModes);
        var extent           = this.ChooseSwapExtent(swapChainSupport.Capabilities);

        var imageCount = swapChainSupport.Capabilities.MinImageCount + 1;

        if (swapChainSupport.Capabilities.MaxImageCount > 0 && imageCount > swapChainSupport.Capabilities.MaxImageCount)
        {
            imageCount = swapChainSupport.Capabilities.MaxImageCount;
        }

        var indices = this.FindQueueFamilies(this.physicalDevice);

        uint[] queueFamilyIndices;
        VkSharingMode sharingMode;

        if (indices.GraphicsFamily != indices.PresentFamily)
        {
            sharingMode        = VkSharingMode.Concurrent;
            queueFamilyIndices =
            [
                indices.GraphicsFamily!.Value,
                indices.PresentFamily!.Value
            ];
        }
        else
        {
            sharingMode        = VkSharingMode.Exclusive;
            queueFamilyIndices = [];
        }

        fixed (uint* pQueueFamilyIndices = queueFamilyIndices)
        {
            var createInfo = new VkSwapchainCreateInfoKHR
            {
                Clipped               = true,
                CompositeAlpha        = VkCompositeAlphaFlagsKHR.Opaque,
                ImageArrayLayers      = 1,
                ImageColorSpace       = surfaceFormat.ColorSpace,
                ImageExtent           = extent,
                ImageFormat           = surfaceFormat.Format,
                ImageSharingMode      = sharingMode,
                ImageUsage            = VkImageUsageFlags.ColorAttachment,
                MinImageCount         = imageCount,
                OldSwapchain          = default,
                PresentMode           = presentMode,
                PreTransform          = swapChainSupport.Capabilities.CurrentTransform,
                PQueueFamilyIndices   = pQueueFamilyIndices,
                QueueFamilyIndexCount = (uint)queueFamilyIndices.Length,
                Surface               = this.surface.Handle,
            };

            this.swapChain       = this.swapchainExtension.CreateSwapchain(createInfo);
            this.swapChainImages = this.swapChain.GetImages();

            this.swapChainImageFormat = surfaceFormat.Format;
            this.swapChainExtent      = extent;
        }
    }

    private void CreateSyncObjects()
    {
        var semaphoreInfo = new VkSemaphoreCreateInfo();
        var fenceInfo     = new VkFenceCreateInfo
        {
            Flags = VkFenceCreateFlags.Signaled
        };

        for (var i = 0; i < MAX_FRAMES_IN_FLIGHT; i++)
        {
            this.imageAvailableSemaphores[i] = this.device.CreateSemaphore(semaphoreInfo);
            this.renderFinishedSemaphores[i] = this.device.CreateSemaphore(semaphoreInfo);
            this.inFlightFences[i]           = this.device.CreateFence(fenceInfo);
        }
    }

    private void CreateTextureImage()
    {
        using var stream = File.OpenRead(Path.Join(AppContext.BaseDirectory, "Textures/viking_room.png"));
        var bitmap = SKBitmap.Decode(stream);
        var pixels = bitmap.Pixels.Select(x => (uint)x).ToArray();

        this.mipLevels = (uint)Math.Floor(Math.Log2(Math.Max(bitmap.Width, bitmap.Height))) + 1;

        var imageSize = (ulong)(pixels.Length * 4);

        this.CreateBuffer(
            imageSize,
            VkBufferUsageFlags.TransferSrc,
            VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent,
            out var stagingBuffer,
            out var stagingBufferMemory
        );

        stagingBufferMemory.Write(0, 0, pixels);
        stagingBufferMemory.Unmap();

        this.CreateImage(
            (uint)bitmap.Width,
            (uint)bitmap.Height,
            this.mipLevels,
            VkSampleCountFlags.N1,
            VkFormat.B8G8R8A8Srgb,
            VkImageTiling.Optimal,
            VkImageUsageFlags.TransferSrc | VkImageUsageFlags.TransferDst | VkImageUsageFlags.Sampled,
            VkMemoryPropertyFlags.DeviceLocal,
            out this.textureImage,
            out this.textureImageMemory
        );

        this.TransitionImageLayout(this.textureImage, VkFormat.B8G8R8A8Srgb, VkImageLayout.Undefined, VkImageLayout.TransferDstOptimal, this.mipLevels);
        this.CopyBufferToImage(stagingBuffer, this.textureImage, bitmap.Width, bitmap.Height);
        //transitioned to VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL while generating mipmaps

        stagingBuffer.Dispose();
        stagingBufferMemory.Dispose();

        this.GenerateMipmaps(this.textureImage, VkFormat.B8G8R8A8Srgb, bitmap.Width, bitmap.Height, this.mipLevels);
    }

    private void CreateTextureSampler()
    {
        this.physicalDevice.GetProperties(out var properties);

        var samplerInfo = new VkSamplerCreateInfo
        {
            AddressModeU            = VkSamplerAddressMode.Repeat,
            AddressModeV            = VkSamplerAddressMode.Repeat,
            AddressModeW            = VkSamplerAddressMode.Repeat,
            AnisotropyEnable        = true,
            BorderColor             = VkBorderColor.IntOpaqueBlack,
            CompareEnable           = false,
            CompareOp               = VkCompareOp.Always,
            MagFilter               = VkFilter.Linear,
            MaxAnisotropy           = properties.Limits.MaxSamplerAnisotropy,
            MaxLod                  = this.mipLevels,
            MinFilter               = VkFilter.Linear,
            MinLod                  = 0,
            MipLodBias              = 0,
            MipmapMode              = VkSamplerMipmapMode.Linear,
            UnnormalizedCoordinates = false,
        };

        this.textureSampler = this.device.CreateSampler(samplerInfo);
    }

    private void CreateTextureImageView() =>
        this.textureImageView = this.CreateImageView(this.textureImage, VkFormat.B8G8R8A8Srgb, VkImageAspectFlags.Color, this.mipLevels);

    private void CreateUniformBuffers()
    {
        var bufferSize = (ulong)Marshal.SizeOf<UniformBufferObject>();

        for (var i = 0; i < MAX_FRAMES_IN_FLIGHT; i++)
        {
            this.CreateBuffer(
                bufferSize,
                VkBufferUsageFlags.UniformBuffer,
                VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent,
                out this.uniformBuffers[i],
                out this.uniformBuffersMemory[i]
            );

            this.uniformBuffersMemory[i].Map(0, (ulong)sizeof(UniformBufferObject), 0, out this.uniformBuffersMapped[i]);
        }
    }

    private void CreateVertexBuffer()
    {
        var bufferSize = (ulong)(Marshal.SizeOf<Vertex>() * this.vertices.Count);

        this.CreateBuffer(
            bufferSize,
            VkBufferUsageFlags.TransferSrc,
            VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent,
            out var stagingBuffer,
            out var stagingBufferMemory
        );

        stagingBufferMemory.Write(0, 0, this.vertices.ToArray());
        stagingBufferMemory.Unmap();

        this.CreateBuffer(
            bufferSize,
            VkBufferUsageFlags.TransferDst | VkBufferUsageFlags.VertexBuffer,
            VkMemoryPropertyFlags.DeviceLocal,
            out this.vertexBuffer,
            out this.vertexBufferMemory
        );

        this.CopyBuffer(stagingBuffer, this.vertexBuffer, bufferSize);

        stagingBuffer.Dispose();
        stagingBufferMemory.Dispose();
    }

    private void DrawFrame()
    {
        this.inFlightFences[this.currentFrame].Wait(true, ulong.MaxValue);

        var imageIndex = 0u;

        try
        {
            imageIndex = this.swapChain.AcquireNextImage(ulong.MaxValue, this.imageAvailableSemaphores[this.currentFrame], default);
        }
        catch (VkException exception)
        {
            if (exception.Result == VkResult.ErrorOutOfDateKHR)
            {
                this.RecreateSwapChain();

                return;
            }
            else if (exception.Result != VkResult.SuboptimalKHR)
            {
                throw new Exception("failed to acquire swap chain image!");
            }
        }

        this.UpdateUniformBuffer(this.currentFrame);

        this.inFlightFences[this.currentFrame].Reset();
        this.commandBuffers[this.currentFrame].Reset();

        this.RecordCommandBuffer(this.commandBuffers[this.currentFrame], imageIndex);

        var waitStage = VkPipelineStageFlags.ColorAttachmentOutput;

        var waitSemaphore   = this.imageAvailableSemaphores[this.currentFrame].Handle;
        var commandBuffer   = this.commandBuffers[this.currentFrame].Handle;
        var signalSemaphore = this.renderFinishedSemaphores[this.currentFrame].Handle;

        var submitInfo = new VkSubmitInfo
        {
            PCommandBuffers      = &commandBuffer,
            CommandBufferCount   = 1,
            PSignalSemaphores    = &signalSemaphore,
            SignalSemaphoreCount = 1,
            PWaitDstStageMask    = &waitStage,
            PWaitSemaphores      = &waitSemaphore,
            WaitSemaphoreCount   = 1,
        };

        this.graphicsQueue.Submit(submitInfo, this.inFlightFences[this.currentFrame]);

        _ = new VkSubpassDependency
        {
            SrcSubpass    = VkConstants.VK_SUBPASS_EXTERNAL,
            DstSubpass    = 0,
            SrcStageMask  = VkPipelineStageFlags.ColorAttachmentOutput,
            SrcAccessMask = default,
            DstStageMask  = VkPipelineStageFlags.ColorAttachmentOutput,
            DstAccessMask = VkAccessFlags.ColorAttachmentWrite
        };

        var swapchain = this.swapChain.Handle;

        var presentInfo = new VkPresentInfoKHR
        {
            PWaitSemaphores    = &signalSemaphore,
            WaitSemaphoreCount = 1,
            PSwapchains        = &swapchain,
            SwapchainCount     = 1,
            PImageIndices      = &imageIndex,
        };

        try
        {
            this.swapchainExtension.QueuePresent(this.presentQueue, presentInfo);
        }
        catch (VkException exception)
        {
            if (exception.Result is VkResult.ErrorOutOfDateKHR or VkResult.SuboptimalKHR || this.framebufferResized)
            {
                this.framebufferResized = false;

                this.RecreateSwapChain();
            }
            else
            {
                throw new Exception("failed to present swap chain image!");
            }
        }


        this.currentFrame = (this.currentFrame + 1) % MAX_FRAMES_IN_FLIGHT;
    }

    private void EndSingleTimeCommands(VkCommandBuffer commandBuffer)
    {
        commandBuffer.End();

        var handle = commandBuffer.Handle;

        var submitInfo = new VkSubmitInfo
        {
            CommandBufferCount = 1,
            PCommandBuffers    = &handle
        };

        this.graphicsQueue.Submit([submitInfo]);

        this.graphicsQueue.WaitIdle();

        commandBuffer.Dispose();
    }

    private VkSampleCountFlags GetMaxUsableSampleCount()
    {
        this.physicalDevice.GetProperties(out var physicalDeviceProperties);

        var counts = physicalDeviceProperties.Limits.FramebufferColorSampleCounts & physicalDeviceProperties.Limits.FramebufferDepthSampleCounts;

        return counts.HasFlag(VkSampleCountFlags.N64)
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

    private List<string> GetRequiredExtensions()
    {
        var extensions = new List<string>();

        if (this.enableValidationLayers)
        {
            extensions.Add(VkDebugUtilsExtensionEXT.Name);
        }

        extensions.Add(VkSurfaceExtensionKHR.Name);
        extensions.Add(VkWin32SurfaceExtensionKHR.Name);

        return extensions;
    }

    private VkFormat FindDepthFormat() =>
        this.FindSupportedFormat(
            [VkFormat.D32Sfloat, VkFormat.D32SfloatS8Uint, VkFormat.D24UnormS8Uint],
            VkImageTiling.Optimal,
            VkFormatFeatureFlags.DepthStencilAttachment
        );

    private uint FindMemoryType(uint typeFilter, VkMemoryPropertyFlags properties)
    {
        this.physicalDevice.GetMemoryProperties(out var memProperties);

        for (var i = 0u; i < VkConstants.VK_MAX_MEMORY_TYPES; i++)
        {
            if ((typeFilter & (1 << (int)i)) != 0 && ((VkMemoryType*)memProperties.MemoryTypes)[i].PropertyFlags.HasFlag(properties))
            {
                return i;
            }
        }

        throw new Exception("failed to find suitable memory type!");
    }

    private QueueFamilyIndices FindQueueFamilies(VkPhysicalDevice physicalDevice)
    {
        var indices = new QueueFamilyIndices();

        var queueFamilies = physicalDevice.GetQueueFamilyProperties();

        var i = 0u;

        foreach (var queueFamily in queueFamilies)
        {
            if (queueFamily.QueueFlags.HasFlag(VkQueueFlags.Graphics))
            {
                indices.GraphicsFamily = i;
            }

            var presentSupport = this.surfaceExtension.GetPhysicalDeviceSurfaceSupport(physicalDevice, i, this.surface);

            if (presentSupport)
            {
                indices.PresentFamily = i;
            }

            if (indices.IsComplete)
            {
                break;
            }

            i++;
        }

        return indices;
    }

    private VkFormat FindSupportedFormat(VkFormat[] candidates, VkImageTiling tiling, VkFormatFeatureFlags features)
    {
        foreach (var format in candidates)
        {
            this.physicalDevice.GetFormatProperties(format, out var props);

            if (tiling == VkImageTiling.Linear && props.LinearTilingFeatures.HasFlag(features))
            {
                return format;
            }
            else if (tiling == VkImageTiling.Optimal && props.OptimalTilingFeatures.HasFlag(features))
            {
                return format;
            }
        }

        throw new Exception("failed to find supported format!");
    }

    private void GenerateMipmaps(VkImage image, VkFormat imageFormat, int texWidth, int texHeight, uint mipLevels)
    {
        // Check if image format supports linear blitting

        this.physicalDevice.GetFormatProperties(imageFormat, out var formatProperties);

        if (!formatProperties.OptimalTilingFeatures.HasFlag(VkFormatFeatureFlags.SampledImageFilterLinear))
        {
            throw new Exception("texture image format does not support linear blitting!");
        }

        var commandBuffer = this.BeginSingleTimeCommands();

        var barrier = new VkImageMemoryBarrier
        {
            Image               = image.Handle,
            SrcQueueFamilyIndex = VkConstants.VK_QUEUE_FAMILY_IGNORED,
            DstQueueFamilyIndex = VkConstants.VK_QUEUE_FAMILY_IGNORED,
            SubresourceRange    = new()
            {
                AspectMask     = VkImageAspectFlags.Color,
                BaseArrayLayer = 0,
                LayerCount     = 1,
                LevelCount     = 1,
            }
        };

        var mipWidth  = texWidth;
        var mipHeight = texHeight;

        for (var i = 1u; i < mipLevels; i++)
        {
            barrier = barrier with
            {
                SubresourceRange = barrier.SubresourceRange with { BaseMipLevel = i - 1 },
                OldLayout        = VkImageLayout.TransferDstOptimal,
                NewLayout        = VkImageLayout.TransferSrcOptimal,
                SrcAccessMask    = VkAccessFlags.TransferWrite,
                DstAccessMask    = VkAccessFlags.TransferRead,
            };

            commandBuffer.PipelineBarrier(
                VkPipelineStageFlags.Transfer,
                VkPipelineStageFlags.Transfer,
                default,
                [],
                [],
                [barrier]
            );

            var blit = new VkImageBlit
            {
                SrcSubresource = new()
                {
                    AspectMask     = VkImageAspectFlags.Color,
                    MipLevel       = i - 1,
                    BaseArrayLayer = 0,
                    LayerCount     = 1
                },
                DstSubresource = new()
                {
                    AspectMask     = VkImageAspectFlags.Color,
                    MipLevel       = i,
                    BaseArrayLayer = 0,
                    LayerCount     = 1
                },
            };

            ((VkOffset3D*)blit.SrcOffsets)[1] = new() { X = mipWidth, Y = mipHeight, Z = 1 };
            ((VkOffset3D*)blit.DstOffsets)[1] = new() { X = mipWidth > 1 ? mipWidth / 2 : 1, Y = mipHeight > 1 ? mipHeight / 2 : 1, Z = 1 };

            commandBuffer.BlitImage(
                image,
                VkImageLayout.TransferSrcOptimal,
                image,
                VkImageLayout.TransferDstOptimal,
                [blit],
                VkFilter.Linear
            );

            barrier = barrier with
            {
                OldLayout     = VkImageLayout.TransferSrcOptimal,
                NewLayout     = VkImageLayout.ShaderReadOnlyOptimal,
                SrcAccessMask = VkAccessFlags.TransferRead,
                DstAccessMask = VkAccessFlags.ShaderRead,
            };

            commandBuffer.PipelineBarrier(
                VkPipelineStageFlags.Transfer,
                VkPipelineStageFlags.FragmentShader,
                default,
                [],
                [],
                [barrier]
            );

            if (mipWidth > 1)
            {
                mipWidth /= 2;
            }

            if (mipHeight > 1)
            {
                mipHeight /= 2;
            }
        }

        barrier = barrier with
        {
            SubresourceRange = barrier.SubresourceRange with { BaseMipLevel = mipLevels - 1 },
            OldLayout        = VkImageLayout.TransferDstOptimal,
            NewLayout        = VkImageLayout.ShaderReadOnlyOptimal,
            SrcAccessMask    = VkAccessFlags.TransferWrite,
            DstAccessMask    = VkAccessFlags.ShaderRead,
        };

        commandBuffer.PipelineBarrier(
            VkPipelineStageFlags.Transfer,
            VkPipelineStageFlags.FragmentShader,
            default,
            [],
            [],
            [barrier]
        );

        this.EndSingleTimeCommands(commandBuffer);
    }

    private void InitVulkan()
    {
        this.CreateInstance();
        this.SetupDebugMessenger();
        this.CreateSurface();
        this.PickPhysicalDevice();
        this.CreateLogicalDevice();
        this.CreateSwapChain();
        this.CreateImageViews();
        this.CreateRenderPass();
        this.CreateDescriptorSetLayout();
        this.CreateGraphicsPipeline();
        this.CreateCommandPool();
        this.CreateColorResources();
        this.CreateDepthResources();
        this.CreateFramebuffers();
        this.CreateTextureImage();
        this.CreateTextureImageView();
        this.CreateTextureSampler();
        this.LoadModel();
        this.CreateVertexBuffer();
        this.CreateIndexBuffer();
        this.CreateUniformBuffers();
        this.CreateDescriptorPool();
        this.CreateDescriptorSets();
        this.CreateCommandBuffers();
        this.CreateSyncObjects();
    }

    private void InitWindow()
    {
        this.window = new PlatformWindow("Age*", new(600, 400), new());

        this.window.SizeChanged += () => this.framebufferResized = true;
    }

    private bool IsDeviceSuitable(VkPhysicalDevice physicalDevice)
    {
        var indices             = this.FindQueueFamilies(physicalDevice);
        var extensionsSupported = this.CheckDeviceExtensionSupport(physicalDevice);
        var swapChainAdequate   = false;

        if (extensionsSupported)
        {
            var swapChainSupport = this.QuerySwapChainSupport(physicalDevice);

            swapChainAdequate = swapChainSupport.Formats.Length != 0 && swapChainSupport.PresentModes.Length != 0;
        }

        physicalDevice.GetDeviceFeatures(out var supportedFeatures);

        return indices.IsComplete && extensionsSupported && swapChainAdequate && supportedFeatures.SamplerAnisotropy;
    }

    private void LoadModel()
    {
        var data = this.wavefrontLoader.Load(Path.Join(AppContext.BaseDirectory, "Models", "viking_room.obj"));

        var uniqueVertices = new Dictionary<Vertex, uint>();

        foreach (var obj in data.Objects)
        {
            foreach (var item in obj.Mesh.Faces.SelectMany(x => x.Indices))
            {
                var pos      = data.Attributes.Vertices[item.Index];
                var color    = item.ColorIndex > -1 ? data.Attributes.Colors[item.ColorIndex] : new(1, 0, 0);
                var texCoord = data.Attributes.TexCoords[item.TexCoordIndex];

                texCoord.Y = 1 - texCoord.Y;

                var vertex = new Vertex(pos, color, texCoord);

                if (!uniqueVertices.TryGetValue(vertex, out var index))
                {
                    uniqueVertices[vertex] = index = (uint)this.vertices.Count;
                    this.vertices.Add(vertex);
                }

                this.indices.Add(index);
            }
        }
    }

    private void MainLoop()
    {
        this.window.Show();

        do
        {
            this.window.DoEvents();

            if (!this.window.Closed)
            {
                this.DrawFrame();
            }
        }
        while (!this.window.Closed);

        this.device.WaitIdle();
    }

    private void PickPhysicalDevice()
    {
        var physicalDevices = this.instance.EnumeratePhysicalDevices();

        foreach (var physicalDevice in physicalDevices)
        {
            if (this.IsDeviceSuitable(physicalDevice))
            {
                this.physicalDevice = physicalDevice;
                this.msaaSamples    = this.GetMaxUsableSampleCount();

                break;
            }
        }

        if (this.physicalDevice == default)
        {
            throw new Exception("failed to find a suitable GPU!");
        }
    }

    private SwapChainSupportDetails QuerySwapChainSupport(VkPhysicalDevice physicalDevice)
    {
        this.surface.GetCapabilities(physicalDevice, out var capabilities);
        var formats      = this.surface.GetFormats(physicalDevice);
        var presentModes = this.surface.GetPresentModes(physicalDevice);

        return new SwapChainSupportDetails
        {
            Capabilities = capabilities,
            Formats      = formats,
            PresentModes = presentModes
        };
    }

    private void RecordCommandBuffer(VkCommandBuffer commandBuffer, uint imageIndex)
    {
        commandBuffer.Begin();

        var clearValues = new VkClearValue[]
        {
            new(),
            new()
            {
                DepthStencil = new()
                {
                    Depth = 1,
                }
            }
        };

        fixed (VkClearValue* pClearValues = clearValues)
        {
            var renderPassInfo = new VkRenderPassBeginInfo
            {
                RenderPass  = this.renderPass.Handle,
                Framebuffer = this.swapChainFramebuffers[imageIndex].Handle,
                RenderArea  = new()
                {
                    Offset = new()
                    {
                        X = 0,
                        Y = 0
                    },
                    Extent = this.swapChainExtent,
                },
                PClearValues    = pClearValues,
                ClearValueCount = (uint)clearValues.Length,
            };

            commandBuffer.BeginRenderPass(renderPassInfo, VkSubpassContents.Inline);
        }

        #region RenderPass
        commandBuffer.BindPipeline(VkPipelineBindPoint.Graphics, this.graphicsPipeline);

        var viewport = new VkViewport
        {
            X        = 0,
            Y        = 0,
            Width    = this.swapChainExtent.Width,
            Height   = this.swapChainExtent.Height,
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
            Extent = this.swapChainExtent
        };

        commandBuffer.SetScissor(0, scissor);

        commandBuffer.BindVertexBuffers(0, 1, this.vertexBuffer, 0);
        commandBuffer.BindIndexBuffer(this.indexBuffer, 0, VkIndexType.Uint32);
        commandBuffer.BindDescriptorSets(VkPipelineBindPoint.Graphics, this.pipelineLayout, 0, this.descriptorSets[this.currentFrame], []);
        commandBuffer.DrawIndexed((uint)this.indices.Count, 1, 0, 0, 0);
        #endregion RenderPass

        commandBuffer.EndRenderPass();

        commandBuffer.End();
    }

    private void RecreateSwapChain()
    {
        while (this.window.Minimized)
        {
            this.window.DoEvents();
        }

        this.device.WaitIdle();

        this.CleanupSwapChain();

        this.CreateSwapChain();
        this.CreateImageViews();
        this.CreateColorResources();
        this.CreateDepthResources();
        this.CreateFramebuffers();
    }

    private void SetupDebugMessenger()
    {
        if (!this.enableValidationLayers)
        {
            return;
        }

        PopulateDebugMessengerCreateInfo(out var createInfo);

        this.debugMessenger = this.debugUtilsExtension!.CreateDebugUtilsMessenger(createInfo);
    }

    private void TransitionImageLayout(VkImage image, VkFormat _, VkImageLayout oldLayout, VkImageLayout newLayout, uint mipLevels)
    {
        var commandBuffer = this.BeginSingleTimeCommands();

        VkPipelineStageFlags sourceStage;
        VkPipelineStageFlags destinationStage;
        VkAccessFlags        srcAccessMask;
        VkAccessFlags        dstAccessMask;

        if (oldLayout == VkImageLayout.Undefined && newLayout == VkImageLayout.TransferDstOptimal)
        {
            srcAccessMask = default;
            dstAccessMask = VkAccessFlags.TransferWrite;

            sourceStage      = VkPipelineStageFlags.TopOfPipe;
            destinationStage = VkPipelineStageFlags.Transfer;
        }
        else if (oldLayout ==  VkImageLayout.TransferDstOptimal && newLayout ==  VkImageLayout.ShaderReadOnlyOptimal)
        {
            srcAccessMask = VkAccessFlags.TransferWrite;
            dstAccessMask = VkAccessFlags.ShaderRead;

            sourceStage      = VkPipelineStageFlags.Transfer;
            destinationStage = VkPipelineStageFlags.FragmentShader;
        }
        else
        {
            throw new Exception("unsupported layout transition!");
        }

        var barrier = new VkImageMemoryBarrier
        {
            OldLayout           = oldLayout,
            NewLayout           = newLayout,
            SrcQueueFamilyIndex = VkConstants.VK_QUEUE_FAMILY_IGNORED,
            DstQueueFamilyIndex = VkConstants.VK_QUEUE_FAMILY_IGNORED,
            Image               = image.Handle,
            SrcAccessMask       = srcAccessMask,
            DstAccessMask       = dstAccessMask,
            SubresourceRange    = new()
            {
                AspectMask     = VkImageAspectFlags.Color,
                BaseMipLevel   = 0,
                LevelCount     = mipLevels,
                BaseArrayLayer = 0,
                LayerCount     = 1,
            }
        };

        commandBuffer.PipelineBarrier(
            sourceStage,
            destinationStage,
            default,
            [],
            [],
            [barrier]
        );

        this.EndSingleTimeCommands(commandBuffer);
    }

    private void UpdateUniformBuffer(uint currentImage)
    {
        const double RADIANS = 0.017453292519943295;

        var now = DateTime.UtcNow;

        var time = Math.Max(0, (float)(now - startTime).TotalMilliseconds / 1000);

        var ubo = new UniformBufferObject
        {
            Model = Matrix4x4<float>.Rotate(new(0, 0, 1), time * (float)(90 * RADIANS)),
            View  = Matrix4x4<float>.LookAt(new(2), new(0), new(0, 0, 1)),
            Proj  = Matrix4x4<float>.PerspectiveFov((float)(45 * RADIANS), this.swapChainExtent.Width / (float)this.swapChainExtent.Height, 0.1f, 10)
        };

        ubo.Proj[1, 1] *= -1;

        Marshal.StructureToPtr(ubo, this.uniformBuffersMapped[currentImage], true);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            this.disposed = true;
        }
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void Run()
    {
        this.InitWindow();
        this.InitVulkan();
        this.MainLoop();
        this.Cleanup();
    }
}
#endif
