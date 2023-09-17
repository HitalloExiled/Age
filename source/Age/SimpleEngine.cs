using System.Diagnostics;
using System.Runtime.InteropServices;
using Age.Core.Unsafe;
using Age.Platform.Windows.Display;
using Age.Platform.Windows.Vulkan;
using Age.Vulkan.Native;
using Age.Vulkan.Native.Enums;
using Age.Vulkan.Native.Enums.KHR;
using Age.Vulkan.Native.Extensions.EXT;
using Age.Vulkan.Native.Extensions.KHR;
using Age.Vulkan.Native.Flags;
using Age.Vulkan.Native.Flags.EXT;
using Age.Vulkan.Native.Flags.KHR;
using Age.Vulkan.Native.Types;
using Age.Vulkan.Native.Types.EXT;
using Age.Vulkan.Native.Types.KHR;

namespace Age;

public unsafe partial class SimpleEngine : IDisposable
{
    private readonly HashSet<string> deviceExtensions = new()
    {
        VkKhrSwapchain.Name
    };

    private readonly bool                enableValidationLayers = Debugger.IsAttached;
    private readonly HashSet<string>     validationLayers       = new() { "VK_LAYER_KHRONOS_validation" };
    private readonly Vk                  vk;
    private readonly WindowManager       windowManager          = new();
    private readonly WindowsVulkanLoader windowsVulkanLoader;

    private VkCommandBuffer          commandBuffer;
    private VkCommandPool            commandPool;
    private VkDebugUtilsMessengerEXT debugMessenger;
    private VkDevice                 device;
    private bool                     disposed;
    private VkPipeline               graphicsPipeline;
    private VkQueue                  graphicsQueue;
    private VkInstance               instance;
    private VkPhysicalDevice         physicalDevice;
    private VkPipelineLayout         pipelineLayout;
    private VkQueue                  presentQueue;
    private VkRenderPass             renderPass;
    private VkSurfaceKHR             surface;
    private VkSwapchainKHR           swapChain;
    private VkExtent2D               swapChainExtent;
    private VkFramebuffer[]          swapChainFramebuffers = Array.Empty<VkFramebuffer>();
    private VkFormat                 swapChainImageFormat;
    private VkImage[]                swapChainImages     = Array.Empty<VkImage>();
    private VkImageView[]            swapChainImageViews = Array.Empty<VkImageView>();
    private VkExtDebugUtils?         vkExtDebugUtils;
    private VkKhrSurface             vkKhrSurface        = null!;
    private VkKhrSwapchain           vkKhrSwapchain      = null!;
    private Window                   window              = null!;

    public SimpleEngine()
    {
        this.windowsVulkanLoader = new();
        this.vk                  = new(this.windowsVulkanLoader);
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

    private static void PopulateDebugMessengerCreateInfo(out VkDebugUtilsMessengerCreateInfoEXT createInfo) =>
        createInfo = new VkDebugUtilsMessengerCreateInfoEXT
        {
            messageSeverity = VkDebugUtilsMessageSeverityFlagBitsEXT.VK_DEBUG_UTILS_MESSAGE_SEVERITY_VERBOSE_BIT_EXT | VkDebugUtilsMessageSeverityFlagBitsEXT.VK_DEBUG_UTILS_MESSAGE_SEVERITY_WARNING_BIT_EXT | VkDebugUtilsMessageSeverityFlagBitsEXT.VK_DEBUG_UTILS_MESSAGE_SEVERITY_ERROR_BIT_EXT,
            messageType     = VkDebugUtilsMessageTypeFlagBitsEXT.VK_DEBUG_UTILS_MESSAGE_TYPE_GENERAL_BIT_EXT | VkDebugUtilsMessageTypeFlagBitsEXT.VK_DEBUG_UTILS_MESSAGE_TYPE_VALIDATION_BIT_EXT | VkDebugUtilsMessageTypeFlagBitsEXT.VK_DEBUG_UTILS_MESSAGE_TYPE_PERFORMANCE_BIT_EXT,
            pfnUserCallback = new(DebugCallback),
            pUserData       = null
        };

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
                width  = (uint)this.window.Width,
                height = (uint)this.window.Height,
            };

            actualExtent.width  = Math.Clamp(actualExtent.width, capabilities.minImageExtent.width, capabilities.maxImageExtent.width);
            actualExtent.height = Math.Clamp(actualExtent.height, capabilities.minImageExtent.height, capabilities.maxImageExtent.height);

            return actualExtent;
        }
    }

    private void Cleanup()
    {
        this.vk.DestroyCommandPool(this.device, this.commandPool, null);

        foreach (var framebuffer in this.swapChainFramebuffers)
        {
            this.vk.DestroyFramebuffer(this.device, framebuffer, null);
        }

        this.vk.DestroyPipeline(this.device, this.graphicsPipeline, null);
        this.vk.DestroyPipelineLayout(this.device, this.pipelineLayout, null);
        this.vk.DestroyRenderPass(this.device, this.renderPass, null);

        foreach (var imageView in this.swapChainImageViews)
        {
            this.vk.DestroyImageView(this.device, imageView, null);
        }

        this.vkKhrSwapchain.DestroySwapchain(this.device, this.swapChain, null);
        this.vk.DestroyDevice(this.device, null);

        if (this.enableValidationLayers && this.vkExtDebugUtils != null)
        {
            this.vkExtDebugUtils.DestroyDebugUtilsMessenger(this.instance, this.debugMessenger, default);
        }

        this.vkKhrSurface.DestroySurface(this.instance, this.surface, null);
        this.vk.DestroyInstance(this.instance, null);
    }

    private void CreateCommandBuffer()
    {
        var allocInfo = new VkCommandBufferAllocateInfo
        {
            commandPool        = this.commandPool,
            level              = VkCommandBufferLevel.VK_COMMAND_BUFFER_LEVEL_PRIMARY,
            commandBufferCount = 1
        };

        if (this.vk.AllocateCommandBuffers(this.device, allocInfo, out this.commandBuffer) != VkResult.VK_SUCCESS)
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

    private void CreateFramebuffers()
    {
        this.swapChainFramebuffers = new VkFramebuffer[this.swapChainImageViews.Length];

        for (var i = 0; i < this.swapChainImageViews.Length; i++)
        {
            var attachments = new[]
            {
                this.swapChainImageViews[i]
            };

            fixed (VkImageView* pAttachments = attachments)
            {
                var framebufferInfo = new VkFramebufferCreateInfo
                {
                    renderPass      = this.renderPass,
                    attachmentCount = 1,
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

            var vertexInputInfo = new VkPipelineVertexInputStateCreateInfo
            {
                vertexBindingDescriptionCount   = 0,
                vertexAttributeDescriptionCount = 0
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
                lineWidth               = 1.0f,
                cullMode                = VkCullModeFlagBits.VK_CULL_MODE_BACK_BIT,
                frontFace               = VkFrontFace.VK_FRONT_FACE_CLOCKWISE,
                depthBiasEnable         = false
            };

            var multisampling = new VkPipelineMultisampleStateCreateInfo
            {
                sampleShadingEnable  = false,
                rasterizationSamples = VkSampleCountFlagBits.VK_SAMPLE_COUNT_1_BIT
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

            colorBlending.blendConstants[0] = 0.0f;
            colorBlending.blendConstants[1] = 0.0f;
            colorBlending.blendConstants[2] = 0.0f;
            colorBlending.blendConstants[3] = 0.0f;

            var dynamicStates = new[]
            {
                VkDynamicState.VK_DYNAMIC_STATE_VIEWPORT,
                VkDynamicState.VK_DYNAMIC_STATE_SCISSOR
            };

            fixed (VkDynamicState*                  pDynamicStates = dynamicStates)
            fixed (VkPipelineShaderStageCreateInfo* pStages        = shaderStages)
            {
                var dynamicState = new VkPipelineDynamicStateCreateInfo
                {
                    dynamicStateCount = (uint)dynamicStates.Length,
                    pDynamicStates    = pDynamicStates
                };

                var pipelineLayoutInfo = new VkPipelineLayoutCreateInfo
                {
                    setLayoutCount         = 0,
                    pushConstantRangeCount = 0
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
                };

                if (this.vk.CreateGraphicsPipelines(this.device, default, 1, pipelineInfo, default, out this.graphicsPipeline) != VkResult.VK_SUCCESS)
                {
                    throw new Exception("failed to create graphics pipeline!");
                }

                this.vk.DestroyShaderModule(this.device, fragShaderModule, null);
                this.vk.DestroyShaderModule(this.device, vertShaderModule, null);
            }
        }
    }

    private void CreateImageViews()
    {
        this.swapChainImageViews = new VkImageView[this.swapChainImages.Length];

        for (var i = 0; i < this.swapChainImages.Length; i++)
        {
            var createInfo = new VkImageViewCreateInfo
            {
                image      = this.swapChainImages[i],
                viewType   = VkImageViewType.VK_IMAGE_VIEW_TYPE_2D,
                format     = this.swapChainImageFormat,
                components = new()
                {
                    r = VkComponentSwizzle.VK_COMPONENT_SWIZZLE_IDENTITY,
                    g = VkComponentSwizzle.VK_COMPONENT_SWIZZLE_IDENTITY,
                    b = VkComponentSwizzle.VK_COMPONENT_SWIZZLE_IDENTITY,
                    a = VkComponentSwizzle.VK_COMPONENT_SWIZZLE_IDENTITY,
                },
                subresourceRange = new()
                {
                    aspectMask     = VkImageAspectFlagBits.VK_IMAGE_ASPECT_COLOR_BIT,
                    baseMipLevel   = 0,
                    levelCount     = 1,
                    baseArrayLayer = 0,
                    layerCount     = 1,
                }
            };

            if (this.vk.CreateImageView(this.device, createInfo, default, out this.swapChainImageViews[i]) != VkResult.VK_SUCCESS)
            {
                throw new Exception("failed to create image views!");
            }
        }
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
                    throw new Exception($"Cannot found required extension {VkExtDebugUtils.Name}");
                }

                if (!this.vk.TryGetInstanceExtension<VkKhrSurface>(instance, out var vkKhrSurface))
                {
                    throw new Exception($"Cannot found required extension {VkKhrSurface.Name}");
                }

                this.instance       = instance;
                this.vkKhrSurface   = vkKhrSurface;
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

        var queuePriority = 1.0f;

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

        var deviceFeatures = new VkPhysicalDeviceFeatures();

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

            if (!this.vk.TryGetDeviceExtension<VkKhrSwapchain>(this.physicalDevice, this.device, out var vkKhrSwapchain))
            {
                throw new Exception($"Cannot found required extension {VkKhrSwapchain.Name}");
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
            samples        = VkSampleCountFlagBits.VK_SAMPLE_COUNT_1_BIT,
            loadOp         = VkAttachmentLoadOp.VK_ATTACHMENT_LOAD_OP_CLEAR,
            storeOp        = VkAttachmentStoreOp.VK_ATTACHMENT_STORE_OP_STORE,
            stencilLoadOp  = VkAttachmentLoadOp.VK_ATTACHMENT_LOAD_OP_DONT_CARE,
            stencilStoreOp = VkAttachmentStoreOp.VK_ATTACHMENT_STORE_OP_DONT_CARE,
            initialLayout  = VkImageLayout.VK_IMAGE_LAYOUT_UNDEFINED,
            finalLayout    = VkImageLayout.VK_IMAGE_LAYOUT_PRESENT_SRC_KHR
        };

        var colorAttachmentRef = new VkAttachmentReference
        {
            attachment = 0,
            layout     = VkImageLayout.VK_IMAGE_LAYOUT_COLOR_ATTACHMENT_OPTIMAL
        };

        var subpass = new VkSubpassDescription
        {
            pipelineBindPoint    = VkPipelineBindPoint.VK_PIPELINE_BIND_POINT_GRAPHICS,
            colorAttachmentCount = 1,
            pColorAttachments    = &colorAttachmentRef
        };

        var renderPassInfo = new VkRenderPassCreateInfo
        {
            attachmentCount = 1,
            pAttachments    = &colorAttachment,
            subpassCount    = 1,
            pSubpasses      = &subpass
        };

        if (this.vk.CreateRenderPass(this.device, renderPassInfo, default, out this.renderPass) != VkResult.VK_SUCCESS)
        {
            throw new Exception("failed to create render pass!");
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
        if (!this.vk.TryGetInstanceExtension<VkKhrWin32Surface>(this.instance, null, out var vkKhrWin32Surface))
        {
            throw new Exception($"Cannot found required extension {VkKhrWin32Surface.Name}");
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
            surface          = this.surface,
            minImageCount    = imageCount,
            imageFormat      = surfaceFormat.format,
            imageColorSpace  = surfaceFormat.colorSpace,
            imageExtent      = extent,
            imageArrayLayers = 1,
            imageUsage       = VkImageUsageFlagBits.VK_IMAGE_USAGE_COLOR_ATTACHMENT_BIT
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

    private List<string> GetRequiredExtensions()
    {
        var extensions = new List<string>();

        if (this.enableValidationLayers)
        {
            extensions.Add(VkExtDebugUtils.Name);
        }

        extensions.Add(VkKhrSurface.Name);
        extensions.Add(VkKhrWin32Surface.Name);

        return extensions;
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
        this.CreateGraphicsPipeline();
        this.CreateFramebuffers();
        this.CreateCommandPool();
        this.CreateCommandBuffer();
    }

    private void InitWindow() =>
        this.window = this.windowManager.CreateWindow("Age", 600, 400, 0, 0);

    private bool IsDeviceSuitable(VkPhysicalDevice device)
    {
        var indices = this.FindQueueFamilies(device);

        var extensionsSupported = this.CheckDeviceExtensionSupport(device);

        var swapChainAdequate = false;

        if (extensionsSupported)
        {
            var swapChainSupport = this.QuerySwapChainSupport(device);

            swapChainAdequate = swapChainSupport.Formats.Length != 0 && swapChainSupport.PresentModes.Length != 0;
        }

        return indices.IsComplete && extensionsSupported && swapChainAdequate;
    }

    private void MainLoop()
    {
        this.window!.Show();

        while (!this.window.Closed)
        {
            this.window.DoEvents();
        }
    }

    private void PickPhysicalDevice()
    {
        this.vk.EnumeratePhysicalDevices(this.instance, out VkPhysicalDevice[] devices);

        foreach (var device in devices)
        {
            if (this.IsDeviceSuitable(device))
            {
                this.physicalDevice = device;
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
        var beginInfo = new VkCommandBufferBeginInfo();

        if (this.vk.BeginCommandBuffer(commandBuffer, ref beginInfo) != VkResult.VK_SUCCESS)
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

        var clearColor = new VkClearValue();

        clearColor.color.float32[0] = 0;
        clearColor.color.float32[1] = 0;
        clearColor.color.float32[2] = 0;
        clearColor.color.float32[3] = 1;

        renderPassInfo.clearValueCount = 1;
        renderPassInfo.pClearValues    = &clearColor;

        this.vk.CmdBeginRenderPass(commandBuffer, renderPassInfo, VkSubpassContents.VK_SUBPASS_CONTENTS_INLINE);

        #region RenderPass
        this.vk.CmdBindPipeline(commandBuffer, VkPipelineBindPoint.VK_PIPELINE_BIND_POINT_GRAPHICS, this.graphicsPipeline);

        var viewport = new VkViewport
        {
            x        = 0.0f,
            y        = 0.0f,
            width    = this.swapChainExtent.width,
            height   = this.swapChainExtent.height,
            minDepth = 0.0f,
            maxDepth = 1.0f
        };

        this.vk.CmdSetViewport(commandBuffer, 0, 1, viewport);

        var scissor = new VkRect2D
        {
            offset = new()
            {
                x = 0,
                y = 0
            },
            extent = this.swapChainExtent
        };

        this.vk.CmdSetScissor(commandBuffer, 0, 1, scissor);
        this.vk.CmdDraw(commandBuffer, 3, 1, 0, 0);
        #endregion RenderPass

        this.vk.CmdEndRenderPass(commandBuffer);

        if (this.vk.EndCommandBuffer(commandBuffer) != VkResult.VK_SUCCESS)
        {
            throw new Exception("failed to record command buffer!");
        }
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

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            this.windowsVulkanLoader.Dispose();
            this.windowManager.Dispose();

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
