#define SIMPLE_ENGINE
#if SIMPLE_ENGINE
using System.Diagnostics;
using System.Runtime.InteropServices;
using Age.Core.Interop;
using Age.Numerics;
using Age.Platforms.Abstractions;
using Age.Vulkan;
using Age.Vulkan.Enums;
using Age.Vulkan.Enums.KHR;
using Age.Vulkan.Extensions.EXT;
using Age.Vulkan.Extensions.KHR;
using Age.Vulkan.Flags;
using Age.Vulkan.Flags.EXT;
using Age.Vulkan.Flags.KHR;
using Age.Vulkan.Types;
using Age.Vulkan.Types.EXT;
using Age.Vulkan.Types.KHR;
using SkiaSharp;

using PlatformWindow = Age.Platforms.Display.Window;

using WavefrontLoader = Age.Resources.Loaders.Wavefront.Loader;

namespace Age.Playground;

public unsafe partial class SimpleEngine : IDisposable
{
    private const int MAX_FRAMES_IN_FLIGHT = 2;

    private static readonly DateTime startTime = DateTime.UtcNow;

    private readonly HashSet<string>        deviceExtensions         = [VkKhrSwapchainExtension.Name];
    private readonly bool                   enableValidationLayers   = Debugger.IsAttached;
    private readonly VkSemaphore[]          imageAvailableSemaphores = new VkSemaphore[MAX_FRAMES_IN_FLIGHT];
    private readonly List<uint>             indices                  = [];
    private readonly VkFence[]              inFlightFences           = new VkFence[MAX_FRAMES_IN_FLIGHT];
    private readonly WavefrontLoader        wavefrontLoader          = new(new FileSystem());
    private readonly VkSemaphore[]          renderFinishedSemaphores = new VkSemaphore[MAX_FRAMES_IN_FLIGHT];
    private readonly VkBuffer[]             uniformBuffers           = new VkBuffer[MAX_FRAMES_IN_FLIGHT];
    private readonly UniformBufferObject*[] uniformBuffersMapped     = new UniformBufferObject*[MAX_FRAMES_IN_FLIGHT];
    private readonly VkDeviceMemory[]       uniformBuffersMemory     = new VkDeviceMemory[MAX_FRAMES_IN_FLIGHT];
    private readonly HashSet<string>        validationLayers         = ["VK_LAYER_KHRONOS_validation"];
    private readonly List<Vertex>           vertices                 = [];
    private readonly Vk                     vk;

    private VkImage                   colorImage;
    private VkDeviceMemory            colorImageMemory;
    private VkImageView               colorImageView;
    private VkCommandBuffer[]         commandBuffers = [];
    private VkCommandPool             commandPool;
    private uint                      currentFrame;
    private VkDebugUtilsMessengerEXT  debugMessenger;
    private VkImage                   depthImage;
    private VkDeviceMemory            depthImageMemory;
    private VkImageView               depthImageView;
    private VkDescriptorPool          descriptorPool;
    private VkDescriptorSetLayout     descriptorSetLayout;
    private VkDescriptorSet[]         descriptorSets = [];
    private VkDevice                  device;
    private bool                      disposed;
    private bool                      framebufferResized;
    private VkPipeline                graphicsPipeline;
    private VkQueue                   graphicsQueue;
    private VkBuffer                  indexBuffer;
    private VkDeviceMemory            indexBufferMemory;
    private VkInstance                instance;
    private uint                      mipLevels;
    private VkSampleCountFlagBits     msaaSamples = VkSampleCountFlagBits.VK_SAMPLE_COUNT_1_BIT;
    private VkPhysicalDevice          physicalDevice;
    private VkPipelineLayout          pipelineLayout;
    private VkQueue                   presentQueue;
    private VkRenderPass              renderPass;
    private VkSurfaceKHR              surface;
    private VkSwapchainKHR            swapChain;
    private VkExtent2D                swapChainExtent;
    private VkFramebuffer[]           swapChainFramebuffers = [];
    private VkFormat                  swapChainImageFormat;
    private VkImage[]                 swapChainImages     = [];
    private VkImageView[]             swapChainImageViews = [];
    private VkImage                   textureImage;
    private VkDeviceMemory            textureImageMemory;
    private VkImageView               textureImageView;
    private VkSampler                 textureSampler;
    private VkBuffer                  vertexBuffer;
    private VkDeviceMemory            vertexBufferMemory;
    private VkExtDebugUtilsExtension? vkExtDebugUtils;
    private VkKhrSurfaceExtension     vkKhrSurface   = null!;
    private VkKhrSwapchainExtension   vkKhrSwapchain = null!;
    private PlatformWindow            window         = null!;

    public SimpleEngine()
    {
        this.vk = new();

        for (var i = 0; i < MAX_FRAMES_IN_FLIGHT; i++)
        {
            this.uniformBuffersMapped[i] = (UniformBufferObject*)NativeMemory.Alloc((uint)sizeof(UniformBufferObject));
        }
    }

    private static VkPresentModeKHR ChooseSwapPresentMode(VkPresentModeKHR[] availablePresentModes)
    {
        foreach (var availablePresentMode in availablePresentModes)
        {
            if (availablePresentMode == VkPresentModeKHR.VK_PRESENT_MODE_MAILBOX_KHR)
            {
                return availablePresentMode;
            }
        }

        return VkPresentModeKHR.VK_PRESENT_MODE_FIFO_KHR;
    }

    private static VkSurfaceFormatKHR ChooseSwapSurfaceFormat(VkSurfaceFormatKHR[] availableFormats)
    {
        foreach (var availableFormat in availableFormats)
        {
            if (availableFormat.format == VkFormat.VK_FORMAT_B8G8R8A8_SRGB && availableFormat.colorSpace == VkColorSpaceKHR.VK_COLOR_SPACE_SRGB_NONLINEAR_KHR)
            {
                return availableFormat;
            }
        }

        return availableFormats[0];
    }

    private static VkBool32 DebugCallback(
        VkDebugUtilsMessageSeverityFlagBitsEXT messageSeverity,
        VkDebugUtilsMessageTypeFlagsEXT        messageType,
        VkDebugUtilsMessengerCallbackDataEXT*  pCallbackData,
        void*                                  pUserData
    )
    {
        Console.WriteLine("validation layer: " + Marshal.PtrToStringAnsi((nint)pCallbackData->pMessage));

        return false;
    }

    private static bool HasStencilComponent(VkFormat format) =>
        format == VkFormat.VK_FORMAT_D32_SFLOAT_S8_UINT || format == VkFormat.VK_FORMAT_D24_UNORM_S8_UINT;

    private static void PopulateDebugMessengerCreateInfo(out VkDebugUtilsMessengerCreateInfoEXT createInfo) =>
        createInfo = new VkDebugUtilsMessengerCreateInfoEXT
        {
            messageSeverity = VkDebugUtilsMessageSeverityFlagBitsEXT.VK_DEBUG_UTILS_MESSAGE_SEVERITY_VERBOSE_BIT_EXT | VkDebugUtilsMessageSeverityFlagBitsEXT.VK_DEBUG_UTILS_MESSAGE_SEVERITY_WARNING_BIT_EXT | VkDebugUtilsMessageSeverityFlagBitsEXT.VK_DEBUG_UTILS_MESSAGE_SEVERITY_ERROR_BIT_EXT,
            messageType     = VkDebugUtilsMessageTypeFlagBitsEXT.VK_DEBUG_UTILS_MESSAGE_TYPE_GENERAL_BIT_EXT | VkDebugUtilsMessageTypeFlagBitsEXT.VK_DEBUG_UTILS_MESSAGE_TYPE_VALIDATION_BIT_EXT | VkDebugUtilsMessageTypeFlagBitsEXT.VK_DEBUG_UTILS_MESSAGE_TYPE_PERFORMANCE_BIT_EXT,
            pfnUserCallback = new(DebugCallback),
            pUserData       = null
        };

    private VkCommandBuffer BeginSingleTimeCommands()
    {
        var allocInfo = new VkCommandBufferAllocateInfo
        {
            level              = VkCommandBufferLevel.VK_COMMAND_BUFFER_LEVEL_PRIMARY,
            commandPool        = this.commandPool,
            commandBufferCount = 1
        };

        this.vk.AllocateCommandBuffers(this.device, allocInfo, out VkCommandBuffer commandBuffer);

        var beginInfo = new VkCommandBufferBeginInfo
        {
            flags = VkCommandBufferUsageFlagBits.VK_COMMAND_BUFFER_USAGE_ONE_TIME_SUBMIT_BIT
        };

        this.vk.BeginCommandBuffer(commandBuffer, beginInfo);

        return commandBuffer;
    }

    private bool CheckDeviceExtensionSupport(VkPhysicalDevice device)
    {
        this.vk.EnumerateDeviceExtensionProperties(device, null, out VkExtensionProperties[] extensions);

        return this.deviceExtensions.Overlaps(extensions.Select(x => Marshal.PtrToStringAnsi((nint)x.extensionName)!));
    }

    private bool CheckValidationLayerSupport()
    {
        this.vk.EnumerateInstanceLayerProperties(out VkLayerProperties[] availableLayers);

        return this.validationLayers.Overlaps(availableLayers.Select(x => Marshal.PtrToStringAnsi((nint)x.layerName)!));
    }

    private VkExtent2D ChooseSwapExtent(VkSurfaceCapabilitiesKHR capabilities)
    {
        if (capabilities.currentExtent.width != uint.MaxValue)
        {
            return capabilities.currentExtent;
        }
        else
        {
            var actualExtent = new VkExtent2D
            {
                width  = this.window.Size.Width,
                height = this.window.Size.Height,
            };

            actualExtent.width  = Math.Clamp(actualExtent.width, capabilities.minImageExtent.width, capabilities.maxImageExtent.width);
            actualExtent.height = Math.Clamp(actualExtent.height, capabilities.minImageExtent.height, capabilities.maxImageExtent.height);

            return actualExtent;
        }
    }

    private void Cleanup()
    {
        this.CleanupSwapChain();

        this.vk.DestroySampler(this.device, this.textureSampler, null);
        this.vk.DestroyImageView(this.device, this.textureImageView, null);
        this.vk.DestroyImage(this.device, this.textureImage, null);
        this.vk.FreeMemory(this.device, this.textureImageMemory, null);
        this.vk.DestroyPipeline(this.device, this.graphicsPipeline, null);
        this.vk.DestroyPipelineLayout(this.device, this.pipelineLayout, null);
        this.vk.DestroyRenderPass(this.device, this.renderPass, null);

        for (var i = 0; i < MAX_FRAMES_IN_FLIGHT; i++)
        {
            this.vk.DestroyBuffer(this.device, this.uniformBuffers[i], null);
            this.vk.FreeMemory(this.device, this.uniformBuffersMemory[i], null);
        }

        this.vk.DestroyDescriptorPool(this.device, this.descriptorPool, null);
        this.vk.DestroyDescriptorSetLayout(this.device, this.descriptorSetLayout, null);
        this.vk.DestroyBuffer(this.device, this.indexBuffer, null);
        this.vk.FreeMemory(this.device, this.indexBufferMemory, null);
        this.vk.DestroyBuffer(this.device, this.vertexBuffer, null);
        this.vk.FreeMemory(this.device, this.vertexBufferMemory, null);

        for (var i = 0; i < MAX_FRAMES_IN_FLIGHT; i++)
        {
            this.vk.DestroySemaphore(this.device, this.imageAvailableSemaphores[i], null);
            this.vk.DestroySemaphore(this.device, this.renderFinishedSemaphores[i], null);
            this.vk.DestroyFence(this.device, this.inFlightFences[i], null);
        }

        this.vk.DestroyCommandPool(this.device, this.commandPool, null);
        this.vk.DestroyDevice(this.device, null);

        if (this.enableValidationLayers && this.vkExtDebugUtils != null)
        {
            this.vkExtDebugUtils.DestroyDebugUtilsMessenger(this.instance, this.debugMessenger, default);
        }

        this.vkKhrSurface.DestroySurface(this.instance, this.surface, null);
        this.vk.DestroyInstance(this.instance, null);
    }

    private void CleanupSwapChain()
    {
        this.vk.DestroyImageView(this.device, this.depthImageView, null);
        this.vk.DestroyImage(this.device, this.depthImage, null);
        this.vk.FreeMemory(this.device, this.depthImageMemory, null);

        this.vk.DestroyImageView(this.device, this.colorImageView, null);
        this.vk.DestroyImage(this.device, this.colorImage, null);
        this.vk.FreeMemory(this.device, this.colorImageMemory, null);

        for (var i = 0; i < this.swapChainFramebuffers.Length; i++)
        {
            this.vk.DestroyFramebuffer(this.device, this.swapChainFramebuffers[i], null);
        }

        for (var i = 0; i < this.swapChainImageViews.Length; i++)
        {
            this.vk.DestroyImageView(this.device, this.swapChainImageViews[i], null);
        }

        this.vkKhrSwapchain.DestroySwapchain(this.device, this.swapChain, null);
    }

    private void CopyBuffer(VkBuffer srcBuffer, VkBuffer dstBuffer, VkDeviceSize size)
    {
        var commandBuffer = this.BeginSingleTimeCommands();

        var copyRegion = new VkBufferCopy
        {
            size = size
        };

        this.vk.CmdCopyBuffer(commandBuffer, srcBuffer, dstBuffer, copyRegion);

        this.EndSingleTimeCommands(commandBuffer);
    }

    private void CopyBufferToImage(VkBuffer buffer, VkImage image, int width, int height)
    {
        var commandBuffer = this.BeginSingleTimeCommands();

        var region = new VkBufferImageCopy
        {
            bufferOffset      = 0,
            bufferRowLength   = 0,
            bufferImageHeight = 0,
            imageSubresource  = new()
            {
                aspectMask     = VkImageAspectFlagBits.VK_IMAGE_ASPECT_COLOR_BIT,
                mipLevel       = 0,
                baseArrayLayer = 0,
                layerCount     = 1,
            },
            imageOffset = new() { x = 0, y = 0, z = 0 },
            imageExtent = new()
            {
                width  = (uint)width,
                height = (uint)height,
                depth  = 1,
            }
        };

        this.vk.CmdCopyBufferToImage(
            commandBuffer,
            buffer,
            image,
            VkImageLayout.VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL,
            region
        );

        this.EndSingleTimeCommands(commandBuffer);
    }

    private void CreateBuffer(VkDeviceSize size, VkBufferUsageFlags usage, VkMemoryPropertyFlags properties, out VkBuffer buffer, out VkDeviceMemory bufferMemory)
    {
        var bufferInfo = new VkBufferCreateInfo
        {
            size        = size,
            usage       = usage,
            sharingMode = VkSharingMode.VK_SHARING_MODE_EXCLUSIVE
        };

        if (this.vk.CreateBuffer(this.device, bufferInfo, default, out buffer) != VkResult.VK_SUCCESS)
        {
            throw new Exception("failed to create buffer!");
        }

        this.vk.GetBufferMemoryRequirements(this.device, buffer, out var memRequirements);

        var allocInfo = new VkMemoryAllocateInfo
        {
            allocationSize = memRequirements.size,
            memoryTypeIndex = this.FindMemoryType(memRequirements.memoryTypeBits, properties)
        };

        if (this.vk.AllocateMemory(this.device, allocInfo, default, out bufferMemory) != VkResult.VK_SUCCESS)
        {
            throw new Exception("failed to allocate buffer memory!");
        }

        this.vk.BindBufferMemory(this.device, buffer, bufferMemory, 0);
    }

    private void CreateCommandBuffers()
    {
        var allocInfo = new VkCommandBufferAllocateInfo
        {
            commandPool        = this.commandPool,
            level              = VkCommandBufferLevel.VK_COMMAND_BUFFER_LEVEL_PRIMARY,
            commandBufferCount = MAX_FRAMES_IN_FLIGHT,
        };

        if (this.vk.AllocateCommandBuffers(this.device, allocInfo, out this.commandBuffers) != VkResult.VK_SUCCESS)
        {
            throw new Exception("failed to allocate command buffers!");
        }
    }

    private void CreateCommandPool()
    {
        var queueFamilyIndices = this.FindQueueFamilies(this.physicalDevice);

        var poolInfo = new VkCommandPoolCreateInfo
        {
            flags            = VkCommandPoolCreateFlagBits.VK_COMMAND_POOL_CREATE_RESET_COMMAND_BUFFER_BIT,
            queueFamilyIndex = queueFamilyIndices.GraphicsFamily!.Value,
        };

        if (this.vk.CreateCommandPool(this.device, poolInfo, default, out this.commandPool) != VkResult.VK_SUCCESS)
        {
            throw new Exception("failed to create command pool!");
        }
    }

    private void CreateColorResources()
    {
        var colorFormat = this.swapChainImageFormat;

        this.CreateImage(
            this.swapChainExtent.width,
            this.swapChainExtent.height,
            1,
            this.msaaSamples,
            colorFormat,
            VkImageTiling.VK_IMAGE_TILING_OPTIMAL,
            VkImageUsageFlagBits.VK_IMAGE_USAGE_TRANSIENT_ATTACHMENT_BIT | VkImageUsageFlagBits.VK_IMAGE_USAGE_COLOR_ATTACHMENT_BIT,
            VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT,
            out this.colorImage,
            out this.colorImageMemory
        );
        this.colorImageView = this.CreateImageView(this.colorImage, colorFormat, VkImageAspectFlagBits.VK_IMAGE_ASPECT_COLOR_BIT, 1);
    }

    private void CreateDepthResources()
    {
        var depthFormat = this.FindDepthFormat();

        this.CreateImage(
            this.swapChainExtent.width,
            this.swapChainExtent.height,
            1,
            this.msaaSamples,
            depthFormat,
            VkImageTiling.VK_IMAGE_TILING_OPTIMAL,
            VkImageUsageFlagBits.VK_IMAGE_USAGE_DEPTH_STENCIL_ATTACHMENT_BIT,
            VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT,
            out this.depthImage,
            out this.depthImageMemory
        );

        this.depthImageView = this.CreateImageView(this.depthImage, depthFormat, VkImageAspectFlagBits.VK_IMAGE_ASPECT_DEPTH_BIT, 1);
    }

    private void CreateDescriptorPool()
    {
        var poolSizes = new VkDescriptorPoolSize[]
        {
            new()
            {
                type            = VkDescriptorType.VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER,
                descriptorCount = MAX_FRAMES_IN_FLIGHT,
            },
            new()
            {
                type            = VkDescriptorType.VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER,
                descriptorCount = MAX_FRAMES_IN_FLIGHT,
            },
        };

        fixed (VkDescriptorPoolSize* pPoolSizes = poolSizes)
        {
            var poolInfo = new VkDescriptorPoolCreateInfo
            {
                poolSizeCount = (uint)poolSizes.Length,
                pPoolSizes    = pPoolSizes,
                maxSets       = MAX_FRAMES_IN_FLIGHT,
            };

            if (this.vk.CreateDescriptorPool(this.device, poolInfo, default, out this.descriptorPool) != VkResult.VK_SUCCESS)
            {
                throw new Exception("failed to create descriptor pool!");
            }
        }
    }

    private void CreateDescriptorSets()
    {
        var layouts = new VkDescriptorSetLayout[MAX_FRAMES_IN_FLIGHT]
        {
            this.descriptorSetLayout,
            this.descriptorSetLayout,
        };

        fixed (VkDescriptorSetLayout* pSetLayouts = layouts)
        {
            var allocInfo = new VkDescriptorSetAllocateInfo
            {
                descriptorPool     = this.descriptorPool,
                descriptorSetCount = MAX_FRAMES_IN_FLIGHT,
                pSetLayouts        = pSetLayouts
            };

            if (this.vk.AllocateDescriptorSets(this.device, allocInfo, out this.descriptorSets) != VkResult.VK_SUCCESS)
            {
                throw new Exception("failed to allocate descriptor sets!");
            }

            for (var i = 0; i < MAX_FRAMES_IN_FLIGHT; i++)
            {
                var bufferInfo = new VkDescriptorBufferInfo
                {
                    buffer = this.uniformBuffers[i],
                    offset = 0,
                    range  = (uint)Marshal.SizeOf<UniformBufferObject>()
                };

                var imageInfo = new VkDescriptorImageInfo
                {
                    imageLayout = VkImageLayout.VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL,
                    imageView   = this.textureImageView,
                    sampler     = this.textureSampler
                };

                var descriptorWrites = new VkWriteDescriptorSet[]
                {
                    new()
                    {
                        dstSet          = this.descriptorSets[i],
                        dstBinding      = 0,
                        dstArrayElement = 0,
                        descriptorType  = VkDescriptorType.VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER,
                        descriptorCount = 1,
                        pBufferInfo     = &bufferInfo,
                    },
                    new()
                    {
                        dstSet          = this.descriptorSets[i],
                        dstBinding      = 1,
                        dstArrayElement = 0,
                        descriptorType  = VkDescriptorType.VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER,
                        descriptorCount = 1,
                        pImageInfo      = &imageInfo,
                    }
                };

                this.vk.UpdateDescriptorSets(this.device, descriptorWrites, null);
            }
        }
    }

    private void CreateDescriptorSetLayout()
    {
        var uboLayoutBinding = new VkDescriptorSetLayoutBinding
        {
            binding            = 0,
            descriptorType     = VkDescriptorType.VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER,
            descriptorCount    = 1,
            stageFlags         = VkShaderStageFlagBits.VK_SHADER_STAGE_VERTEX_BIT,
            pImmutableSamplers = default, // Optional
        };

        var samplerLayoutBinding = new VkDescriptorSetLayoutBinding
        {
            binding            = 1,
            descriptorCount    = 1,
            descriptorType     = VkDescriptorType.VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER,
            pImmutableSamplers = null,
            stageFlags         = VkShaderStageFlagBits.VK_SHADER_STAGE_FRAGMENT_BIT
        };

        var bindings = new[]
        {
            uboLayoutBinding,
            samplerLayoutBinding
        };

        fixed (VkDescriptorSetLayoutBinding* pBindings = bindings)
        {
            var layoutInfo = new VkDescriptorSetLayoutCreateInfo
            {
                bindingCount = (uint)bindings.Length,
                pBindings    = pBindings
            };

            if (this.vk.CreateDescriptorSetLayout(this.device, layoutInfo, default, out this.descriptorSetLayout) != VkResult.VK_SUCCESS)
            {
                throw new Exception("failed to create descriptor set layout!");
            }
        }
    }

    private void CreateFramebuffers()
    {
        this.swapChainFramebuffers = new VkFramebuffer[this.swapChainImageViews.Length];

        for (var i = 0; i < this.swapChainImageViews.Length; i++)
        {
            var attachments = new[]
            {
                this.colorImageView,
                this.depthImageView,
                this.swapChainImageViews[i]
            };

            fixed (VkImageView* pAttachments = attachments)
            {
                var framebufferInfo = new VkFramebufferCreateInfo
                {
                    renderPass      = this.renderPass,
                    attachmentCount = (uint)attachments.Length,
                    pAttachments    = pAttachments,
                    width           = this.swapChainExtent.width,
                    height          = this.swapChainExtent.height,
                    layers          = 1
                };

                if (this.vk.CreateFramebuffer(this.device, framebufferInfo, default, out this.swapChainFramebuffers[i]) != VkResult.VK_SUCCESS)
                {
                    throw new Exception("failed to create framebuffer!");
                }
            }
        }
    }

    private void CreateGraphicsPipeline()
    {
        var vertShaderCode = File.ReadAllBytes(Path.Join(AppContext.BaseDirectory, "Shaders/vert.spv"))!;
        var fragShaderCode = File.ReadAllBytes(Path.Join(AppContext.BaseDirectory, "Shaders/frag.spv"))!;

        var vertShaderModule = this.CreateShaderModule(vertShaderCode);
        var fragShaderModule = this.CreateShaderModule(fragShaderCode);

        fixed (byte* pName = "main"u8)
        {
            var vertShaderStageInfo = new VkPipelineShaderStageCreateInfo
            {
                stage  = VkShaderStageFlagBits.VK_SHADER_STAGE_VERTEX_BIT,
                module = vertShaderModule,
                pName  = pName
            };

            var fragShaderStageInfo = new VkPipelineShaderStageCreateInfo
            {
                stage  = VkShaderStageFlagBits.VK_SHADER_STAGE_FRAGMENT_BIT,
                module = fragShaderModule,
                pName  = pName
            };

            var shaderStages = new[]
            {
                vertShaderStageInfo,
                fragShaderStageInfo
            };

            var bindingDescription    = Vertex.GetBindingDescription();
            var attributeDescriptions = Vertex.GetAttributeDescriptions();

            fixed (VkVertexInputAttributeDescription* pVertexAttributeDescriptions = attributeDescriptions)
            {
                var vertexInputInfo = new VkPipelineVertexInputStateCreateInfo
                {
                    vertexBindingDescriptionCount   = 1,
                    vertexAttributeDescriptionCount = (uint)attributeDescriptions.Length,
                    pVertexBindingDescriptions      = &bindingDescription,
                    pVertexAttributeDescriptions    = pVertexAttributeDescriptions
                };

                var inputAssembly = new VkPipelineInputAssemblyStateCreateInfo
                {
                    topology               = VkPrimitiveTopology.VK_PRIMITIVE_TOPOLOGY_TRIANGLE_LIST,
                    primitiveRestartEnable = false
                };

                var viewportState = new VkPipelineViewportStateCreateInfo
                {
                    viewportCount = 1,
                    scissorCount  = 1
                };

                var rasterizer = new VkPipelineRasterizationStateCreateInfo
                {
                    depthClampEnable        = false,
                    rasterizerDiscardEnable = false,
                    polygonMode             = VkPolygonMode.VK_POLYGON_MODE_FILL,
                    lineWidth               = 1,
                    cullMode                = VkCullModeFlagBits.VK_CULL_MODE_BACK_BIT,
                    frontFace               = VkFrontFace.VK_FRONT_FACE_COUNTER_CLOCKWISE,
                    depthBiasEnable         = false,
                };

                var multisampling = new VkPipelineMultisampleStateCreateInfo
                {
                    sampleShadingEnable  = true,
                    rasterizationSamples = this.msaaSamples,
                    minSampleShading     = 1,
                };

                var depthStencil = new VkPipelineDepthStencilStateCreateInfo
                {
                    depthTestEnable       = true,
                    depthWriteEnable      = true,
                    depthCompareOp        = VkCompareOp.VK_COMPARE_OP_LESS,
                    depthBoundsTestEnable = false,
                    stencilTestEnable     = false
                };

                var colorBlendAttachment = new VkPipelineColorBlendAttachmentState
                {
                    colorWriteMask = VkColorComponentFlagBits.VK_COLOR_COMPONENT_R_BIT | VkColorComponentFlagBits.VK_COLOR_COMPONENT_G_BIT | VkColorComponentFlagBits.VK_COLOR_COMPONENT_B_BIT | VkColorComponentFlagBits.VK_COLOR_COMPONENT_A_BIT,
                    blendEnable    = false
                };

                var colorBlending = new VkPipelineColorBlendStateCreateInfo
                {
                    logicOpEnable   = false,
                    logicOp         = VkLogicOp.VK_LOGIC_OP_COPY,
                    attachmentCount = 1,
                    pAttachments    = &colorBlendAttachment
                };

                colorBlending.blendConstants[0] = 0;
                colorBlending.blendConstants[1] = 0;
                colorBlending.blendConstants[2] = 0;
                colorBlending.blendConstants[3] = 0;

                var dynamicStates = new[]
                {
                    VkDynamicState.VK_DYNAMIC_STATE_VIEWPORT,
                    VkDynamicState.VK_DYNAMIC_STATE_SCISSOR
                };

                fixed (VkDynamicState*                  pDynamicStates = dynamicStates)
                fixed (VkPipelineShaderStageCreateInfo* pStages        = shaderStages)
                fixed (VkDescriptorSetLayout*           pSetLayouts    = &this.descriptorSetLayout)
                {
                    var dynamicState = new VkPipelineDynamicStateCreateInfo
                    {
                        dynamicStateCount = (uint)dynamicStates.Length,
                        pDynamicStates    = pDynamicStates
                    };

                    var pipelineLayoutInfo = new VkPipelineLayoutCreateInfo
                    {
                        setLayoutCount = 1,
                        pSetLayouts    = pSetLayouts
                    };

                    if (this.vk.CreatePipelineLayout(this.device, pipelineLayoutInfo, default, out this.pipelineLayout) != VkResult.VK_SUCCESS)
                    {
                        throw new Exception("failed to create pipeline layout!");
                    }

                    var pipelineInfo = new VkGraphicsPipelineCreateInfo
                    {
                        stageCount          = 2,
                        pStages             = pStages,
                        pVertexInputState   = &vertexInputInfo,
                        pInputAssemblyState = &inputAssembly,
                        pViewportState      = &viewportState,
                        pRasterizationState = &rasterizer,
                        pMultisampleState   = &multisampling,
                        pColorBlendState    = &colorBlending,
                        pDynamicState       = &dynamicState,
                        layout              = this.pipelineLayout,
                        renderPass          = this.renderPass,
                        subpass             = 0,
                        basePipelineHandle  = default,
                        pDepthStencilState  = &depthStencil,
                    };

                    if (this.vk.CreateGraphicsPipelines(this.device, default, pipelineInfo, default, out this.graphicsPipeline) != VkResult.VK_SUCCESS)
                    {
                        throw new Exception("failed to create graphics pipeline!");
                    }

                    this.vk.DestroyShaderModule(this.device, fragShaderModule, null);
                    this.vk.DestroyShaderModule(this.device, vertShaderModule, null);
                }
            }
        }
    }

    private void CreateImage(uint width, uint height, uint mipLevels, VkSampleCountFlagBits numSamples, VkFormat format, VkImageTiling tiling, VkImageUsageFlagBits usage, VkMemoryPropertyFlagBits properties, out VkImage image, out VkDeviceMemory imageMemory)
    {
        var imageInfo = new VkImageCreateInfo
        {
            arrayLayers = 1,
            extent      = new()
            {
                width  = width,
                height = height,
                depth  = 1,
            },
            format        = format,
            imageType     = VkImageType.VK_IMAGE_TYPE_2D,
            initialLayout = VkImageLayout.VK_IMAGE_LAYOUT_UNDEFINED,
            mipLevels     = mipLevels,
            samples       = numSamples,
            sharingMode   = VkSharingMode.VK_SHARING_MODE_EXCLUSIVE,
            tiling        = tiling,
            usage         = usage,
        };

        if (this.vk.CreateImage(this.device, imageInfo, default, out image) != VkResult.VK_SUCCESS)
        {
            throw new Exception("failed to create image!");
        }

        this.vk.GetImageMemoryRequirements(this.device, image, out var memRequirements);

        var allocInfo = new VkMemoryAllocateInfo
        {
            allocationSize  = memRequirements.size,
            memoryTypeIndex = this.FindMemoryType(memRequirements.memoryTypeBits, properties)
        };

        if (this.vk.AllocateMemory(this.device, allocInfo, default, out imageMemory) != VkResult.VK_SUCCESS)
        {
            throw new Exception("failed to allocate image memory!");
        }

        this.vk.BindImageMemory(this.device, image, imageMemory, 0);
    }

    private VkImageView CreateImageView(VkImage image, VkFormat format, VkImageAspectFlagBits aspectFlags, uint mipLevels)
    {
        var viewInfo = new VkImageViewCreateInfo
        {
            image            = image,
            viewType         = VkImageViewType.VK_IMAGE_VIEW_TYPE_2D,
            format           = format,
            subresourceRange = new()
            {
                aspectMask     = aspectFlags,
                baseMipLevel   = 0,
                levelCount     = mipLevels,
                baseArrayLayer = 0,
                layerCount     = 1
            }
        };

        return this.vk.CreateImageView(this.device, viewInfo, default, out var imageView) != VkResult.VK_SUCCESS
            ? throw new Exception("failed to create texture image view!")
            : imageView;
    }

    private void CreateImageViews()
    {
        this.swapChainImageViews = new VkImageView[this.swapChainImages.Length];

        for (var i = 0; i < this.swapChainImages.Length; i++)
        {
            this.swapChainImageViews[i] = this.CreateImageView(this.swapChainImages[i], this.swapChainImageFormat, VkImageAspectFlagBits.VK_IMAGE_ASPECT_COLOR_BIT, 1);
        }
    }

    private void CreateIndexBuffer()
    {
        VkDeviceSize bufferSize = (ulong)(sizeof(int) * this.indices.Count);

        this.CreateBuffer(
            bufferSize,
            VkBufferUsageFlagBits.VK_BUFFER_USAGE_TRANSFER_SRC_BIT,
            VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT | VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_COHERENT_BIT,
            out var stagingBuffer,
            out var stagingBufferMemory
        );

        this.vk.MapMemory(this.device, stagingBufferMemory, 0, 0, this.indices.ToArray());
        this.vk.UnmapMemory(this.device, stagingBufferMemory);

        this.CreateBuffer(
            bufferSize,
            VkBufferUsageFlagBits.VK_BUFFER_USAGE_TRANSFER_DST_BIT | VkBufferUsageFlagBits.VK_BUFFER_USAGE_INDEX_BUFFER_BIT,
            VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT,
            out this.indexBuffer,
            out this.indexBufferMemory
        );

        this.CopyBuffer(stagingBuffer, this.indexBuffer, bufferSize);

        this.vk.DestroyBuffer(this.device, stagingBuffer, null);
        this.vk.FreeMemory(this.device, stagingBufferMemory, null);
    }

    private void CreateInstance()
    {
        if (this.enableValidationLayers && !this.CheckValidationLayerSupport())
        {
            throw new Exception("validation layers requested, but not available!");
        }

        fixed (byte* pApplicationName = "Hello Triangle"u8)
        fixed (byte* pEngineName      = "No Engine"u8)
        {
            var appInfo = new VkApplicationInfo
            {
                pApplicationName   = pApplicationName,
                applicationVersion = Vk.MakeApiVersion(1, 0, 0),
                pEngineName        = pEngineName,
                engineVersion      = Vk.MakeApiVersion(1, 0, 0),
                apiVersion         = Vk.ApiVersion_1_0,
            };

            var createInfo = new VkInstanceCreateInfo
            {
                pApplicationInfo = &appInfo,
            };

            using var ppEnabledLayerNames = new StringArrayPtr(this.validationLayers.ToArray());

            if (this.enableValidationLayers)
            {
                createInfo.enabledLayerCount   = (uint)ppEnabledLayerNames.Length;
                createInfo.ppEnabledLayerNames = ppEnabledLayerNames;

                PopulateDebugMessengerCreateInfo(out var debugCreateInfo);

                createInfo.pNext = &debugCreateInfo;
            }
            else
            {
                createInfo.enabledLayerCount = 0;
            }

            using var ppEnabledExtensionNames = new StringArrayPtr(this.GetRequiredExtensions());

            createInfo.enabledExtensionCount   = (uint)ppEnabledExtensionNames.Length;
            createInfo.ppEnabledExtensionNames = ppEnabledExtensionNames;

            if (this.vk.CreateInstance(createInfo, default, out var instance) == VkResult.VK_SUCCESS)
            {
                if (this.enableValidationLayers && !this.vk.TryGetInstanceExtension(instance, out this.vkExtDebugUtils))
                {
                    throw new Exception($"Cannot found required extension {VkExtDebugUtilsExtension.Name}");
                }

                if (!this.vk.TryGetInstanceExtension<VkKhrSurfaceExtension>(instance, out var vkKhrSurface))
                {
                    throw new Exception($"Cannot found required extension {VkKhrSurfaceExtension.Name}");
                }

                this.instance     = instance;
                this.vkKhrSurface = vkKhrSurface;
            }
        }
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

        var queuePriority = 1f;

        foreach (var queueFamily in uniqueQueueFamilies)
        {
            var queueCreateInfo = new VkDeviceQueueCreateInfo
            {
                queueFamilyIndex = indices.GraphicsFamily!.Value,
                queueCount       = 1,
                pQueuePriorities = &queuePriority,
            };

            queueCreateInfos.Add(queueCreateInfo);
        }

        var deviceFeatures = new VkPhysicalDeviceFeatures
        {
            samplerAnisotropy = true,
            sampleRateShading = true,
        };

        fixed (VkDeviceQueueCreateInfo* pQueueCreateInfos = queueCreateInfos.ToArray())
        {
            var createInfo = new VkDeviceCreateInfo
            {
                queueCreateInfoCount  = (uint)queueCreateInfos.Count,
                pQueueCreateInfos     = pQueueCreateInfos,
                pEnabledFeatures      = &deviceFeatures,
                enabledExtensionCount = 0,
            };

            using var ppEnabledLayerNames = new StringArrayPtr(this.validationLayers.ToArray());

            // Compatibility
            #pragma warning disable CS0618
            if (this.enableValidationLayers)
            {
                createInfo.enabledLayerCount   = (uint)ppEnabledLayerNames.Length;
                createInfo.ppEnabledLayerNames = ppEnabledLayerNames;
            }
            else
            {
                createInfo.enabledLayerCount = 0;
            }
            #pragma warning restore CS0618

            using var ppEnabledExtensionNames = new StringArrayPtr(this.deviceExtensions.ToArray());

            createInfo.enabledExtensionCount   = (uint)ppEnabledExtensionNames.Length;
            createInfo.ppEnabledExtensionNames = ppEnabledExtensionNames;

            if (this.vk.CreateDevice(this.physicalDevice, createInfo, default, out this.device) != VkResult.VK_SUCCESS)
            {
                throw new Exception("failed to create logical device!");
            }

            if (!this.vk.TryGetDeviceExtension<VkKhrSwapchainExtension>(this.physicalDevice, this.device, out var vkKhrSwapchain))
            {
                throw new Exception($"Cannot found required extension {VkKhrSwapchainExtension.Name}");
            }

            this.vkKhrSwapchain = vkKhrSwapchain;

            this.vk.GetDeviceQueue(this.device, indices.GraphicsFamily.Value, 0, out this.graphicsQueue);
            this.vk.GetDeviceQueue(this.device, indices.PresentFamily.Value, 0, out this.presentQueue);
        }
    }

    private void CreateRenderPass()
    {
        var colorAttachment = new VkAttachmentDescription
        {
            format         = this.swapChainImageFormat,
            samples        = this.msaaSamples,
            loadOp         = VkAttachmentLoadOp.VK_ATTACHMENT_LOAD_OP_CLEAR,
            storeOp        = VkAttachmentStoreOp.VK_ATTACHMENT_STORE_OP_STORE,
            stencilLoadOp  = VkAttachmentLoadOp.VK_ATTACHMENT_LOAD_OP_DONT_CARE,
            stencilStoreOp = VkAttachmentStoreOp.VK_ATTACHMENT_STORE_OP_DONT_CARE,
            initialLayout  = VkImageLayout.VK_IMAGE_LAYOUT_UNDEFINED,
            finalLayout    = VkImageLayout.VK_IMAGE_LAYOUT_COLOR_ATTACHMENT_OPTIMAL
        };

        var depthAttachment = new VkAttachmentDescription
        {
            format         = this.FindDepthFormat(),
            samples        = this.msaaSamples,
            loadOp         = VkAttachmentLoadOp.VK_ATTACHMENT_LOAD_OP_CLEAR,
            storeOp        = VkAttachmentStoreOp.VK_ATTACHMENT_STORE_OP_DONT_CARE,
            stencilLoadOp  = VkAttachmentLoadOp.VK_ATTACHMENT_LOAD_OP_DONT_CARE,
            stencilStoreOp = VkAttachmentStoreOp.VK_ATTACHMENT_STORE_OP_DONT_CARE,
            initialLayout  = VkImageLayout.VK_IMAGE_LAYOUT_UNDEFINED,
            finalLayout    = VkImageLayout.VK_IMAGE_LAYOUT_DEPTH_STENCIL_ATTACHMENT_OPTIMAL
        };

        var colorAttachmentResolve = new VkAttachmentDescription()
        {
            format         = this.swapChainImageFormat,
            samples        = VkSampleCountFlagBits.VK_SAMPLE_COUNT_1_BIT,
            loadOp         = VkAttachmentLoadOp.VK_ATTACHMENT_LOAD_OP_DONT_CARE,
            storeOp        = VkAttachmentStoreOp.VK_ATTACHMENT_STORE_OP_STORE,
            stencilLoadOp  = VkAttachmentLoadOp.VK_ATTACHMENT_LOAD_OP_DONT_CARE,
            stencilStoreOp = VkAttachmentStoreOp.VK_ATTACHMENT_STORE_OP_DONT_CARE,
            initialLayout  = VkImageLayout.VK_IMAGE_LAYOUT_UNDEFINED,
            finalLayout    = VkImageLayout.VK_IMAGE_LAYOUT_PRESENT_SRC_KHR
        };

        var colorAttachmentResolveRef = new VkAttachmentReference
        {
            attachment = 2,
            layout     = VkImageLayout.VK_IMAGE_LAYOUT_COLOR_ATTACHMENT_OPTIMAL
        };

        var colorAttachmentRef = new VkAttachmentReference
        {
            attachment = 0,
            layout     = VkImageLayout.VK_IMAGE_LAYOUT_COLOR_ATTACHMENT_OPTIMAL
        };

        var depthAttachmentRef = new VkAttachmentReference
        {
            attachment = 1,
            layout     = VkImageLayout.VK_IMAGE_LAYOUT_DEPTH_STENCIL_ATTACHMENT_OPTIMAL
        };

        var subpass = new VkSubpassDescription
        {
            pipelineBindPoint       = VkPipelineBindPoint.VK_PIPELINE_BIND_POINT_GRAPHICS,
            colorAttachmentCount    = 1,
            pColorAttachments       = &colorAttachmentRef,
            pDepthStencilAttachment = &depthAttachmentRef,
            pResolveAttachments     = &colorAttachmentResolveRef,
        };

        var dependency = new VkSubpassDependency
        {
            srcSubpass    = Vk.VK_SUBPASS_EXTERNAL,
            dstSubpass    = 0,
            srcStageMask  = VkPipelineStageFlagBits.VK_PIPELINE_STAGE_COLOR_ATTACHMENT_OUTPUT_BIT | VkPipelineStageFlagBits.VK_PIPELINE_STAGE_EARLY_FRAGMENT_TESTS_BIT,
            srcAccessMask = default,
            dstStageMask  = VkPipelineStageFlagBits.VK_PIPELINE_STAGE_COLOR_ATTACHMENT_OUTPUT_BIT | VkPipelineStageFlagBits.VK_PIPELINE_STAGE_EARLY_FRAGMENT_TESTS_BIT,
            dstAccessMask = VkAccessFlagBits.VK_ACCESS_COLOR_ATTACHMENT_WRITE_BIT | VkAccessFlagBits.VK_ACCESS_DEPTH_STENCIL_ATTACHMENT_WRITE_BIT
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
                attachmentCount = (uint)attachments.Length,
                pAttachments    = pAttachments,
                subpassCount    = 1,
                pSubpasses      = &subpass
            };

            if (this.vk.CreateRenderPass(this.device, renderPassInfo, default, out this.renderPass) != VkResult.VK_SUCCESS)
            {
                throw new Exception("failed to create render pass!");
            }
        }
    }

    private VkShaderModule CreateShaderModule(byte[] code)
    {
        fixed (byte* pCode = code)
        {
            var createInfo = new VkShaderModuleCreateInfo
            {
                codeSize = (uint)code.Length,
                pCode    = (uint*)pCode
            };

            return this.vk.CreateShaderModule(this.device, createInfo, default, out var shaderModule) != VkResult.VK_SUCCESS
                ? throw new Exception("failed to create shader module!")
                : shaderModule;
        }
    }

    private void CreateSurface()
    {
        if (!this.vk.TryGetInstanceExtension<VkKhrWin32SurfaceExtension>(this.instance, null, out var vkKhrWin32Surface))
        {
            throw new Exception($"Cannot found required extension {VkKhrWin32SurfaceExtension.Name}");
        }

        var createInfo = new VkWin32SurfaceCreateInfoKHR
        {
            hwnd      = this.window!.Handle,
            hinstance = Process.GetCurrentProcess().Handle,
        };

        vkKhrWin32Surface.CreateWin32Surface(this.instance, createInfo, default, out this.surface);
    }

    private void CreateSwapChain()
    {
        var swapChainSupport = this.QuerySwapChainSupport(this.physicalDevice);
        var surfaceFormat    = ChooseSwapSurfaceFormat(swapChainSupport.Formats);
        var presentMode      = ChooseSwapPresentMode(swapChainSupport.PresentModes);
        var extent           = this.ChooseSwapExtent(swapChainSupport.Capabilities);

        var imageCount = swapChainSupport.Capabilities.minImageCount + 1;

        if (swapChainSupport.Capabilities.maxImageCount > 0 && imageCount > swapChainSupport.Capabilities.maxImageCount)
        {
            imageCount = swapChainSupport.Capabilities.maxImageCount;
        }

        var createInfo = new VkSwapchainCreateInfoKHR
        {
            imageArrayLayers = 1,
            imageColorSpace  = surfaceFormat.colorSpace,
            imageExtent      = extent,
            imageFormat      = surfaceFormat.format,
            imageUsage       = VkImageUsageFlagBits.VK_IMAGE_USAGE_COLOR_ATTACHMENT_BIT,
            minImageCount    = imageCount,
            surface          = this.surface,
        };

        var indices = this.FindQueueFamilies(this.physicalDevice);

        var queueFamilyIndices = new[]
        {
            indices.GraphicsFamily!.Value,
            indices.PresentFamily!.Value
        };

        fixed (uint* pQueueFamilyIndices = queueFamilyIndices)
        {
            if (indices.GraphicsFamily != indices.PresentFamily)
            {
                createInfo.imageSharingMode      = VkSharingMode.VK_SHARING_MODE_CONCURRENT;
                createInfo.queueFamilyIndexCount = 2;
                createInfo.pQueueFamilyIndices   = pQueueFamilyIndices;
            }
            else
            {
                createInfo.imageSharingMode      = VkSharingMode.VK_SHARING_MODE_EXCLUSIVE;
                createInfo.queueFamilyIndexCount = 0; // Optional
                createInfo.pQueueFamilyIndices   = null; // Optional
            }

            createInfo.preTransform   = swapChainSupport.Capabilities.currentTransform;
            createInfo.compositeAlpha = VkCompositeAlphaFlagBitsKHR.VK_COMPOSITE_ALPHA_OPAQUE_BIT_KHR;
            createInfo.presentMode    = presentMode;
            createInfo.clipped        = true;
            createInfo.oldSwapchain   = default;
        }

        if (this.vkKhrSwapchain.CreateSwapchain(this.device, createInfo, default, out this.swapChain) != VkResult.VK_SUCCESS)
        {
            throw new Exception("failed to create swap chain!");
        }

        this.vkKhrSwapchain.GetSwapchainImages(this.device, this.swapChain, out this.swapChainImages);

        this.swapChainImageFormat = surfaceFormat.format;
        this.swapChainExtent      = extent;
    }

    private void CreateSyncObjects()
    {
        var semaphoreInfo = new VkSemaphoreCreateInfo();
        var fenceInfo     = new VkFenceCreateInfo
        {
            flags = VkFenceCreateFlagBits.VK_FENCE_CREATE_SIGNALED_BIT
        };

        for (var i = 0; i < MAX_FRAMES_IN_FLIGHT; i++)
        {
            if (
                this.vk.CreateSemaphore(this.device, semaphoreInfo, default, out this.imageAvailableSemaphores[i]) != VkResult.VK_SUCCESS ||
                this.vk.CreateSemaphore(this.device, semaphoreInfo, default, out this.renderFinishedSemaphores[i]) != VkResult.VK_SUCCESS ||
                this.vk.CreateFence(this.device, fenceInfo, default, out this.inFlightFences[i]) != VkResult.VK_SUCCESS
            )
            {
                throw new Exception("failed to create semaphores!");
            }
        }
    }

    private void CreateTextureImage()
    {
        using var stream = File.OpenRead(Path.Join(AppContext.BaseDirectory, "Textures/viking_room.png"));
        var bitmap = SKBitmap.Decode(stream);
        var pixels = bitmap.Pixels.Select(x => (uint)x).ToArray();

        this.mipLevels = (uint)Math.Floor(Math.Log2(Math.Max(bitmap.Width, bitmap.Height))) + 1;

        VkDeviceSize imageSize = (ulong)(pixels.Length * 4);

        this.CreateBuffer(
            imageSize,
            VkBufferUsageFlagBits.VK_BUFFER_USAGE_TRANSFER_SRC_BIT,
            VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT | VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_COHERENT_BIT,
            out var stagingBuffer,
            out var stagingBufferMemory
        );

        this.vk.MapMemory(this.device, stagingBufferMemory, 0, 0, pixels);
        this.vk.UnmapMemory(this.device, stagingBufferMemory);

        this.CreateImage(
            (uint)bitmap.Width,
            (uint)bitmap.Height,
            this.mipLevels,
            VkSampleCountFlagBits.VK_SAMPLE_COUNT_1_BIT,
            VkFormat.VK_FORMAT_B8G8R8A8_SRGB,
            VkImageTiling.VK_IMAGE_TILING_OPTIMAL,
            VkImageUsageFlagBits.VK_IMAGE_USAGE_TRANSFER_SRC_BIT | VkImageUsageFlagBits.VK_IMAGE_USAGE_TRANSFER_DST_BIT | VkImageUsageFlagBits.VK_IMAGE_USAGE_SAMPLED_BIT,
            VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT,
            out this.textureImage,
            out this.textureImageMemory
        );

        this.TransitionImageLayout(this.textureImage, VkFormat.VK_FORMAT_B8G8R8A8_SRGB, VkImageLayout.VK_IMAGE_LAYOUT_UNDEFINED, VkImageLayout.VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL, this.mipLevels);
        this.CopyBufferToImage(stagingBuffer, this.textureImage, bitmap.Width, bitmap.Height);
        //transitioned to VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL while generating mipmaps

        this.vk.DestroyBuffer(this.device, stagingBuffer, null);
        this.vk.FreeMemory(this.device, stagingBufferMemory, null);

        this.GenerateMipmaps(this.textureImage, VkFormat.VK_FORMAT_B8G8R8A8_SRGB, bitmap.Width, bitmap.Height, this.mipLevels);
    }

    private void CreateTextureSampler()
    {
        this.vk.GetPhysicalDeviceProperties(this.physicalDevice, out var properties);

        var samplerInfo = new VkSamplerCreateInfo
        {
            addressModeU            = VkSamplerAddressMode.VK_SAMPLER_ADDRESS_MODE_REPEAT,
            addressModeV            = VkSamplerAddressMode.VK_SAMPLER_ADDRESS_MODE_REPEAT,
            addressModeW            = VkSamplerAddressMode.VK_SAMPLER_ADDRESS_MODE_REPEAT,
            anisotropyEnable        = true,
            borderColor             = VkBorderColor.VK_BORDER_COLOR_INT_OPAQUE_BLACK,
            compareEnable           = false,
            compareOp               = VkCompareOp.VK_COMPARE_OP_ALWAYS,
            magFilter               = VkFilter.VK_FILTER_LINEAR,
            maxAnisotropy           = properties.limits.maxSamplerAnisotropy,
            maxLod                  = this.mipLevels,
            minFilter               = VkFilter.VK_FILTER_LINEAR,
            minLod                  = 0,
            mipLodBias              = 0,
            mipmapMode              = VkSamplerMipmapMode.VK_SAMPLER_MIPMAP_MODE_LINEAR,
            unnormalizedCoordinates = false,
        };

        if (this.vk.CreateSampler(this.device, samplerInfo, default, out this.textureSampler) != VkResult.VK_SUCCESS)
        {
            throw new Exception("failed to create texture sampler!");
        }
    }

    private void CreateTextureImageView() =>
        this.textureImageView = this.CreateImageView(this.textureImage, VkFormat.VK_FORMAT_B8G8R8A8_SRGB, VkImageAspectFlagBits.VK_IMAGE_ASPECT_COLOR_BIT, this.mipLevels);

    private void CreateUniformBuffers()
    {
        VkDeviceSize bufferSize = (ulong)Marshal.SizeOf<UniformBufferObject>();

        for (var i = 0; i < MAX_FRAMES_IN_FLIGHT; i++)
        {
            this.CreateBuffer(
                bufferSize,
                VkBufferUsageFlagBits.VK_BUFFER_USAGE_UNIFORM_BUFFER_BIT,
                VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT | VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_COHERENT_BIT,
                out this.uniformBuffers[i],
                out this.uniformBuffersMemory[i]
            );

            fixed (UniformBufferObject** ppData = &this.uniformBuffersMapped[i])
            {
                this.vk.MapMemory(this.device, this.uniformBuffersMemory[i], 0, 0, ppData);
            }
        }
    }

    private void CreateVertexBuffer()
    {
        VkDeviceSize bufferSize = (ulong)(Marshal.SizeOf<Vertex>() * this.vertices.Count);

        this.CreateBuffer(
            bufferSize,
            VkBufferUsageFlagBits.VK_BUFFER_USAGE_TRANSFER_SRC_BIT,
            VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT | VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_COHERENT_BIT,
            out var stagingBuffer,
            out var stagingBufferMemory
        );

        this.vk.MapMemory(this.device, stagingBufferMemory, 0, 0, this.vertices.ToArray());
        this.vk.UnmapMemory(this.device, stagingBufferMemory);

        this.CreateBuffer(
            bufferSize,
            VkBufferUsageFlagBits.VK_BUFFER_USAGE_TRANSFER_DST_BIT | VkBufferUsageFlagBits.VK_BUFFER_USAGE_VERTEX_BUFFER_BIT,
            VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT,
            out this.vertexBuffer,
            out this.vertexBufferMemory
        );

        this.CopyBuffer(stagingBuffer, this.vertexBuffer, bufferSize);

        this.vk.DestroyBuffer(this.device, stagingBuffer, null);
        this.vk.FreeMemory(this.device, stagingBufferMemory, null);
    }

    private void DrawFrame()
    {
        this.vk.WaitForFences(this.device, this.inFlightFences[this.currentFrame], true, ulong.MaxValue);

        var result = this.vkKhrSwapchain.AcquireNextImage(this.device, this.swapChain, ulong.MaxValue, this.imageAvailableSemaphores[this.currentFrame], default, out var imageIndex);

        if (result == VkResult.VK_ERROR_OUT_OF_DATE_KHR)
        {
            this.RecreateSwapChain();

            return;
        }
        else if (result is not VkResult.VK_SUCCESS and not VkResult.VK_SUBOPTIMAL_KHR)
        {
            throw new Exception("failed to acquire swap chain image!");
        }

        this.UpdateUniformBuffer(this.currentFrame);

        this.vk.ResetFences(this.device, this.inFlightFences[this.currentFrame]);
        this.vk.ResetCommandBuffer(this.commandBuffers[this.currentFrame], default);

        this.RecordCommandBuffer(this.commandBuffers[this.currentFrame], imageIndex);

        var waitSemaphores = new[]
        {
            this.imageAvailableSemaphores[this.currentFrame]
        };

        var waitStages = new VkPipelineStageFlags[]
        {
            VkPipelineStageFlagBits.VK_PIPELINE_STAGE_COLOR_ATTACHMENT_OUTPUT_BIT
        };

        var commandBuffers = new[]
        {
            this.commandBuffers[this.currentFrame]
        };

        var signalSemaphores = new[]
        {
            this.renderFinishedSemaphores[this.currentFrame]
        };

        fixed (VkSemaphore*          pWaitSemaphores   = waitSemaphores)
        fixed (VkPipelineStageFlags* pWaitDstStageMask = waitStages)
        fixed (VkCommandBuffer*      pCommandBuffers   = commandBuffers)
        fixed (VkSemaphore*          pSignalSemaphores = signalSemaphores)
        {
            var submitInfo = new VkSubmitInfo
            {
                commandBufferCount   = 1,
                pCommandBuffers      = pCommandBuffers,
                pSignalSemaphores    = pSignalSemaphores,
                pWaitDstStageMask    = pWaitDstStageMask,
                pWaitSemaphores      = pWaitSemaphores,
                signalSemaphoreCount = 1,
                waitSemaphoreCount   = 1,
            };

            if (this.vk.QueueSubmit(this.graphicsQueue, submitInfo, this.inFlightFences[this.currentFrame]) != VkResult.VK_SUCCESS)
            {
                throw new Exception("failed to submit draw command buffer!");
            }

            var dependency = new VkSubpassDependency
            {
                srcSubpass    = Vk.VK_SUBPASS_EXTERNAL,
                dstSubpass    = 0,
                srcStageMask  = VkPipelineStageFlagBits.VK_PIPELINE_STAGE_COLOR_ATTACHMENT_OUTPUT_BIT,
                srcAccessMask = default,
                dstStageMask  = VkPipelineStageFlagBits.VK_PIPELINE_STAGE_COLOR_ATTACHMENT_OUTPUT_BIT,
                dstAccessMask = VkAccessFlagBits.VK_ACCESS_COLOR_ATTACHMENT_WRITE_BIT
            };

            var presentInfo = new VkPresentInfoKHR
            {
                waitSemaphoreCount = 1,
                pWaitSemaphores    = pSignalSemaphores
            };

            var swapChains = new[]
            {
                this.swapChain
            };

            fixed (VkSwapchainKHR* pSwapchains = swapChains)
            {
                presentInfo.swapchainCount = 1;
                presentInfo.pSwapchains    = pSwapchains;
                presentInfo.pImageIndices  = &imageIndex;

                result = this.vkKhrSwapchain.QueuePresent(this.presentQueue, presentInfo);

                if (result is VkResult.VK_ERROR_OUT_OF_DATE_KHR or VkResult.VK_SUBOPTIMAL_KHR || this.framebufferResized)
                {
                    this.framebufferResized = false;

                    this.RecreateSwapChain();
                }
                else if (result != VkResult.VK_SUCCESS)
                {
                    throw new Exception("failed to present swap chain image!");
                }
            }
        }

        this.currentFrame = (this.currentFrame + 1) % MAX_FRAMES_IN_FLIGHT;
    }

    private void EndSingleTimeCommands(VkCommandBuffer commandBuffer)
    {
        this.vk.EndCommandBuffer(commandBuffer);

        var submitInfo = new VkSubmitInfo
        {
            commandBufferCount = 1,
            pCommandBuffers    = &commandBuffer
        };

        this.vk.QueueSubmit(this.graphicsQueue, submitInfo, default);
        this.vk.QueueWaitIdle(this.graphicsQueue);

        this.vk.FreeCommandBuffers(this.device, this.commandPool, commandBuffer);
    }

    private VkSampleCountFlagBits GetMaxUsableSampleCount()
    {
        this.vk.GetPhysicalDeviceProperties(this.physicalDevice, out var physicalDeviceProperties);

        var counts = (VkSampleCountFlagBits)physicalDeviceProperties.limits.framebufferColorSampleCounts & physicalDeviceProperties.limits.framebufferDepthSampleCounts;

        return counts.HasFlag(VkSampleCountFlagBits.VK_SAMPLE_COUNT_64_BIT)
            ? VkSampleCountFlagBits.VK_SAMPLE_COUNT_64_BIT
            : counts.HasFlag(VkSampleCountFlagBits.VK_SAMPLE_COUNT_32_BIT)
                ? VkSampleCountFlagBits.VK_SAMPLE_COUNT_32_BIT
                : counts.HasFlag(VkSampleCountFlagBits.VK_SAMPLE_COUNT_16_BIT)
                    ? VkSampleCountFlagBits.VK_SAMPLE_COUNT_16_BIT
                    : counts.HasFlag(VkSampleCountFlagBits.VK_SAMPLE_COUNT_8_BIT)
                        ? VkSampleCountFlagBits.VK_SAMPLE_COUNT_8_BIT
                        : counts.HasFlag(VkSampleCountFlagBits.VK_SAMPLE_COUNT_4_BIT)
                            ? VkSampleCountFlagBits.VK_SAMPLE_COUNT_4_BIT
                            : counts.HasFlag(VkSampleCountFlagBits.VK_SAMPLE_COUNT_2_BIT)
                                ? VkSampleCountFlagBits.VK_SAMPLE_COUNT_2_BIT
                                : VkSampleCountFlagBits.VK_SAMPLE_COUNT_1_BIT;
    }

    private List<string> GetRequiredExtensions()
    {
        var extensions = new List<string>();

        if (this.enableValidationLayers)
        {
            extensions.Add(VkExtDebugUtilsExtension.Name);
        }

        extensions.Add(VkKhrSurfaceExtension.Name);
        extensions.Add(VkKhrWin32SurfaceExtension.Name);

        return extensions;
    }

    private VkFormat FindDepthFormat() =>
        this.FindSupportedFormat(
            [VkFormat.VK_FORMAT_D32_SFLOAT, VkFormat.VK_FORMAT_D32_SFLOAT_S8_UINT, VkFormat.VK_FORMAT_D24_UNORM_S8_UINT],
            VkImageTiling.VK_IMAGE_TILING_OPTIMAL,
            VkFormatFeatureFlagBits.VK_FORMAT_FEATURE_DEPTH_STENCIL_ATTACHMENT_BIT
        );

    private uint FindMemoryType(uint typeFilter, VkMemoryPropertyFlagBits properties)
    {
        this.vk.GetPhysicalDeviceMemoryProperties(this.physicalDevice, out var memProperties);

        for (var i = 0u; i < memProperties.memoryTypeCount; i++)
        {
            if ((typeFilter & (1 << (int)i)) != 0 && memProperties.GetMemoryTypes(i).propertyFlags.HasFlag(properties))
            {
                return i;
            }
        }

        throw new Exception("failed to find suitable memory type!");
    }

    private QueueFamilyIndices FindQueueFamilies(VkPhysicalDevice device)
    {
        var indices = new QueueFamilyIndices();

        this.vk.GetPhysicalDeviceQueueFamilyProperties(device, out VkQueueFamilyProperties[] queueFamilies);

        var i = 0u;

        foreach (var queueFamily in queueFamilies)
        {
            if (queueFamily.queueFlags.HasFlag(VkQueueFlagBits.VK_QUEUE_GRAPHICS_BIT))
            {
                indices.GraphicsFamily = i;
            }

            this.vkKhrSurface.GetPhysicalDeviceSurfaceSupport(device, i, this.surface, out var presentSupport);

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
            this.vk.GetPhysicalDeviceFormatProperties(this.physicalDevice, format, out var props);

            if (tiling == VkImageTiling.VK_IMAGE_TILING_LINEAR && props.linearTilingFeatures.HasFlag(features))
            {
                return format;
            }
            else if (tiling == VkImageTiling.VK_IMAGE_TILING_OPTIMAL && props.optimalTilingFeatures.HasFlag(features))
            {
                return format;
            }
        }

        throw new Exception("failed to find supported format!");
    }

    private void GenerateMipmaps(VkImage image, VkFormat imageFormat, int texWidth, int texHeight, uint mipLevels)
    {
        // Check if image format supports linear blitting
        this.vk.GetPhysicalDeviceFormatProperties(this.physicalDevice, imageFormat, out var formatProperties);

        if (!formatProperties.optimalTilingFeatures.HasFlag(VkFormatFeatureFlagBits.VK_FORMAT_FEATURE_SAMPLED_IMAGE_FILTER_LINEAR_BIT))
        {
            throw new Exception("texture image format does not support linear blitting!");
        }

        var commandBuffer = this.BeginSingleTimeCommands();

        var barrier = new VkImageMemoryBarrier
        {
            image               = image,
            srcQueueFamilyIndex = Vk.VK_QUEUE_FAMILY_IGNORED,
            dstQueueFamilyIndex = Vk.VK_QUEUE_FAMILY_IGNORED,
            subresourceRange    = new()
            {
                aspectMask     = VkImageAspectFlagBits.VK_IMAGE_ASPECT_COLOR_BIT,
                baseArrayLayer = 0,
                layerCount     = 1,
                levelCount     = 1,
            }
        };

        var mipWidth  = texWidth;
        var mipHeight = texHeight;

        for (var i = 1u; i < mipLevels; i++)
        {
            barrier.subresourceRange.baseMipLevel = i - 1;
            barrier.oldLayout                     = VkImageLayout.VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL;
            barrier.newLayout                     = VkImageLayout.VK_IMAGE_LAYOUT_TRANSFER_SRC_OPTIMAL;
            barrier.srcAccessMask                 = VkAccessFlagBits.VK_ACCESS_TRANSFER_WRITE_BIT;
            barrier.dstAccessMask                 = VkAccessFlagBits.VK_ACCESS_TRANSFER_READ_BIT;

            this.vk.CmdPipelineBarrier(
                commandBuffer,
                VkPipelineStageFlagBits.VK_PIPELINE_STAGE_TRANSFER_BIT,
                VkPipelineStageFlagBits.VK_PIPELINE_STAGE_TRANSFER_BIT,
                default,
                null,
                null,
                [barrier]
            );

            var blit = new VkImageBlit
            {
                srcSubresource = new()
                {
                    aspectMask     = VkImageAspectFlagBits.VK_IMAGE_ASPECT_COLOR_BIT,
                    mipLevel       = i - 1,
                    baseArrayLayer = 0,
                    layerCount     = 1
                },
                dstSubresource = new()
                {
                    aspectMask     = VkImageAspectFlagBits.VK_IMAGE_ASPECT_COLOR_BIT,
                    mipLevel       = i,
                    baseArrayLayer = 0,
                    layerCount     = 1
                }
            };

            blit.SetSrcOffsets(0, new VkOffset3D());
            blit.SetSrcOffsets(1, new VkOffset3D { x = mipWidth, y = mipHeight, z = 1 });

            blit.SetDstOffsets(0, new VkOffset3D());
            blit.SetDstOffsets(1, new VkOffset3D { x = mipWidth > 1 ? mipWidth / 2 : 1, y = mipHeight > 1 ? mipHeight / 2 : 1, z = 1 });

            this.vk.CmdBlitImage(
                commandBuffer,
                image,
                VkImageLayout.VK_IMAGE_LAYOUT_TRANSFER_SRC_OPTIMAL,
                image,
                VkImageLayout.VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL,
                [blit],
                VkFilter.VK_FILTER_LINEAR
            );

            barrier.oldLayout     = VkImageLayout.VK_IMAGE_LAYOUT_TRANSFER_SRC_OPTIMAL;
            barrier.newLayout     = VkImageLayout.VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL;
            barrier.srcAccessMask = VkAccessFlagBits.VK_ACCESS_TRANSFER_READ_BIT;
            barrier.dstAccessMask = VkAccessFlagBits.VK_ACCESS_SHADER_READ_BIT;

            this.vk.CmdPipelineBarrier(
                commandBuffer,
                VkPipelineStageFlagBits.VK_PIPELINE_STAGE_TRANSFER_BIT,
                VkPipelineStageFlagBits.VK_PIPELINE_STAGE_FRAGMENT_SHADER_BIT,
                default,
                null,
                null,
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

        barrier.subresourceRange.baseMipLevel = mipLevels - 1;
        barrier.oldLayout     = VkImageLayout.VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL;
        barrier.newLayout     = VkImageLayout.VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL;
        barrier.srcAccessMask = VkAccessFlagBits.VK_ACCESS_TRANSFER_WRITE_BIT;
        barrier.dstAccessMask = VkAccessFlagBits.VK_ACCESS_SHADER_READ_BIT;

        this.vk.CmdPipelineBarrier(
            commandBuffer,
            VkPipelineStageFlagBits.VK_PIPELINE_STAGE_TRANSFER_BIT,
            VkPipelineStageFlagBits.VK_PIPELINE_STAGE_FRAGMENT_SHADER_BIT,
            default,
            null,
            null,
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

    private bool IsDeviceSuitable(VkPhysicalDevice device)
    {
        var indices             = this.FindQueueFamilies(device);
        var extensionsSupported = this.CheckDeviceExtensionSupport(device);
        var swapChainAdequate   = false;

        if (extensionsSupported)
        {
            var swapChainSupport = this.QuerySwapChainSupport(device);

            swapChainAdequate = swapChainSupport.Formats.Length != 0 && swapChainSupport.PresentModes.Length != 0;
        }

        this.vk.GetPhysicalDeviceFeatures(device, out var supportedFeatures);

        return indices.IsComplete && extensionsSupported && swapChainAdequate && supportedFeatures.samplerAnisotropy;
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

        this.vk.DeviceWaitIdle(this.device);
    }

    private void PickPhysicalDevice()
    {
        this.vk.EnumeratePhysicalDevices(this.instance, out VkPhysicalDevice[] devices);

        foreach (var device in devices)
        {
            if (this.IsDeviceSuitable(device))
            {
                this.physicalDevice = device;
                this.msaaSamples    = this.GetMaxUsableSampleCount();

                break;
            }
        }

        if (this.physicalDevice == default)
        {
            throw new Exception("failed to find a suitable GPU!");
        }
    }

    private SwapChainSupportDetails QuerySwapChainSupport(VkPhysicalDevice device)
    {
        this.vkKhrSurface.GetPhysicalDeviceSurfaceCapabilities(device, this.surface, out var capabilities);
        this.vkKhrSurface.GetPhysicalDeviceSurfaceFormats(device, this.surface, out VkSurfaceFormatKHR[] formats);
        this.vkKhrSurface.GetPhysicalDeviceSurfacePresentModes(device, this.surface, out VkPresentModeKHR[] presentModes);

        return new SwapChainSupportDetails
        {
            Capabilities = capabilities,
            Formats      = formats,
            PresentModes = presentModes
        };
    }

    private void RecordCommandBuffer(VkCommandBuffer commandBuffer, uint imageIndex)
    {
        if (this.vk.BeginCommandBuffer(commandBuffer) != VkResult.VK_SUCCESS)
        {
            throw new Exception("failed to begin recording command buffer!");
        }

        var renderPassInfo = new VkRenderPassBeginInfo
        {
            renderPass  = this.renderPass,
            framebuffer = this.swapChainFramebuffers[imageIndex],
            renderArea  = new()
            {
                offset = new()
                {
                    x = 0,
                    y = 0
                },
                extent = this.swapChainExtent,
            }
        };

        var clearValues = new VkClearValue[2];

        clearValues[0].color.float32[0] = 0;
        clearValues[0].color.float32[1] = 0;
        clearValues[0].color.float32[2] = 0;
        clearValues[0].color.float32[3] = 0;

        clearValues[1].depthStencil = new()
        {
            depth   = 1.0f,
            stencil = 0
        };

        fixed (VkClearValue* pClearValues = clearValues)
        {
            renderPassInfo.clearValueCount = (uint)clearValues.Length;
            renderPassInfo.pClearValues    = pClearValues;

            this.vk.CmdBeginRenderPass(commandBuffer, renderPassInfo, VkSubpassContents.VK_SUBPASS_CONTENTS_INLINE);

            #region RenderPass
            this.vk.CmdBindPipeline(commandBuffer, VkPipelineBindPoint.VK_PIPELINE_BIND_POINT_GRAPHICS, this.graphicsPipeline);

            var viewport = new VkViewport
            {
                x        = 0,
                y        = 0,
                width    = this.swapChainExtent.width,
                height   = this.swapChainExtent.height,
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
                extent = this.swapChainExtent
            };

            this.vk.CmdSetScissor(commandBuffer, 0, scissor);

            var vertexBuffers = new[]
            {
                this.vertexBuffer
            };

            var offsets = new VkDeviceSize[] { 0 };

            this.vk.CmdBindVertexBuffers(commandBuffer, 0, vertexBuffers, offsets);
            this.vk.CmdBindIndexBuffer(commandBuffer, this.indexBuffer, 0, VkIndexType.VK_INDEX_TYPE_UINT32);
            this.vk.CmdBindDescriptorSets(commandBuffer, VkPipelineBindPoint.VK_PIPELINE_BIND_POINT_GRAPHICS, this.pipelineLayout, 0, [this.descriptorSets[this.currentFrame]], null);
            this.vk.CmdDrawIndexed(commandBuffer, (uint)this.indices.Count, 1, 0, 0, 0);
            #endregion RenderPass

            this.vk.CmdEndRenderPass(commandBuffer);

            if (this.vk.EndCommandBuffer(commandBuffer) != VkResult.VK_SUCCESS)
            {
                throw new Exception("failed to record command buffer!");
            }
        }
    }

    private void RecreateSwapChain()
    {
        while (this.window.Minimized)
        {
            this.window.DoEvents();
        }

        this.vk.DeviceWaitIdle(this.device);

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

        if (this.vkExtDebugUtils!.CreateDebugUtilsMessenger(this.instance, createInfo, default, out this.debugMessenger) != VkResult.VK_SUCCESS)
        {
            throw new Exception("failed to set up debug messenger!");
        }
    }

    private void TransitionImageLayout(VkImage image, VkFormat _, VkImageLayout oldLayout, VkImageLayout newLayout, uint mipLevels)
    {
        var commandBuffer = this.BeginSingleTimeCommands();

        var barrier = new VkImageMemoryBarrier
        {
            oldLayout           = oldLayout,
            newLayout           = newLayout,
            srcQueueFamilyIndex = Vk.VK_QUEUE_FAMILY_IGNORED,
            dstQueueFamilyIndex = Vk.VK_QUEUE_FAMILY_IGNORED,
            image               = image,
            srcAccessMask       = default, // TODO
            dstAccessMask       = default, // TODO
            subresourceRange    = new()
            {
                aspectMask     = VkImageAspectFlagBits.VK_IMAGE_ASPECT_COLOR_BIT,
                baseMipLevel   = 0,
                levelCount     = mipLevels,
                baseArrayLayer = 0,
                layerCount     = 1,
            }
        };

        VkPipelineStageFlags sourceStage;
        VkPipelineStageFlags destinationStage;

        if (oldLayout == VkImageLayout.VK_IMAGE_LAYOUT_UNDEFINED && newLayout ==  VkImageLayout.VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL)
        {
            barrier.srcAccessMask = default;
            barrier.dstAccessMask = VkAccessFlagBits.VK_ACCESS_TRANSFER_WRITE_BIT;

            sourceStage      = VkPipelineStageFlagBits.VK_PIPELINE_STAGE_TOP_OF_PIPE_BIT;
            destinationStage = VkPipelineStageFlagBits.VK_PIPELINE_STAGE_TRANSFER_BIT;
        }
        else if (oldLayout ==  VkImageLayout.VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL && newLayout ==  VkImageLayout.VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL)
        {
            barrier.srcAccessMask = VkAccessFlagBits.VK_ACCESS_TRANSFER_WRITE_BIT;
            barrier.dstAccessMask = VkAccessFlagBits.VK_ACCESS_SHADER_READ_BIT;

            sourceStage      = VkPipelineStageFlagBits.VK_PIPELINE_STAGE_TRANSFER_BIT;
            destinationStage = VkPipelineStageFlagBits.VK_PIPELINE_STAGE_FRAGMENT_SHADER_BIT;
        }
        else
        {
            throw new Exception("unsupported layout transition!");
        }

        this.vk.CmdPipelineBarrier(
            commandBuffer,
            sourceStage,
            destinationStage,
            default,
            null,
            null,
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
            Proj  = Matrix4x4<float>.PerspectiveFov((float)(45 * RADIANS), this.swapChainExtent.width / (float)this.swapChainExtent.height, 0.1f, 10)
        };

        ubo.Proj[1, 1] *= -1;

        Marshal.StructureToPtr(ubo, (nint)this.uniformBuffersMapped[currentImage], true);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            for (var i = 0; i < MAX_FRAMES_IN_FLIGHT; i++)
            {
                NativeMemory.Free(this.uniformBuffersMapped[i]);
            }

            this.window.Dispose();

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
