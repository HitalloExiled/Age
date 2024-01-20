using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Age.Core.Unsafe;
using Age.Numerics;
using Age.Rendering.Vulkan.Handlers;
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

using static Age.Core.Unsafe.UnmanagedUtils;

namespace Age.Rendering.Vulkan;

public unsafe partial class VulkanContext : IDisposable
{
    public const ushort MAX_FRAMES_IN_FLIGHT = 2;

    private static readonly HashSet<string> validationLayers = [Vk.VK_LAYER_KHRONOS_VALIDATION];

    private readonly VkExtDebugUtilsExtension? debugUtilsExtension;
    private readonly VkDebugUtilsMessengerEXT  debugUtilsMessenger;
    private readonly bool                      enableValidationLayers = Debugger.IsAttached;
    private readonly VkFence[]                 fences = new VkFence[MAX_FRAMES_IN_FLIGHT];
    private readonly Frame[]                   frames = new Frame[MAX_FRAMES_IN_FLIGHT];
    private readonly VkInstance                instance;
    private readonly VkSemaphore[]             renderingFinishedSemaphores = new VkSemaphore[MAX_FRAMES_IN_FLIGHT];
    private readonly VkKhrSurfaceExtension     surfaceExtension;
    private readonly Vk                        vk = new();
    private readonly List<SurfaceContext>      surfaceContexts = [];

    private ushort                   currentFrame;
    private VkDevice                 device;
    private bool                     deviceInitialized;
    private bool                     disposed;
    private VkQueue                  graphicsQueue;
    private uint                     graphicsQueueIndex;
    private VkPhysicalDevice         physicalDevice;
    private VkQueue                  presentationQueue;
    private uint                     presentationQueueIndex;
    private VkSurfaceFormatKHR       surfaceFormat;
    private VkKhrSwapchainExtension  swapchainExtension = null!;

    private IList<string> RequiredExtensions
    {
        get
        {
            var extensions = new List<string>
            {
                VkKhrSurfaceExtension.Name,
            };

            extensions.AddRange(this.platformExtensions);

            if (this.enableValidationLayers)
            {
                extensions.Add(VkExtDebugUtilsExtension.Name);
            }

            return extensions;
        }
    }

    public Frame Frame => this.frames[this.currentFrame];

    public VkDevice Device       => this.device;
    public VkFormat ScreenFormat => this.surfaceFormat.format;
    public Vk       Vk           => this.vk;

    public unsafe VulkanContext()
    {
        if (this.enableValidationLayers && !this.CheckValidationLayerSupport())
        {
            throw new Exception("validation layers requested, but not available!");
        }

        fixed (byte* pName = "Age"u8)
        {
            var applicationInfo = new VkApplicationInfo
            {
                apiVersion         = Vk.ApiVersion_1_0,
                applicationVersion = Vk.MakeApiVersion(0, 0, 1, 0),
                engineVersion      = Vk.MakeApiVersion(0, 0, 1, 0),
                pApplicationName   = pName,
                pEngineName        = pName,
            };

            var debugUtilsMessengerCreateInfo = this.enableValidationLayers
                ?  new VkDebugUtilsMessengerCreateInfoEXT
                {
                    messageType = VkDebugUtilsMessageTypeFlagBitsEXT.VK_DEBUG_UTILS_MESSAGE_TYPE_DEVICE_ADDRESS_BINDING_BIT_EXT
                        | VkDebugUtilsMessageTypeFlagBitsEXT.VK_DEBUG_UTILS_MESSAGE_TYPE_GENERAL_BIT_EXT
                        | VkDebugUtilsMessageTypeFlagBitsEXT.VK_DEBUG_UTILS_MESSAGE_TYPE_PERFORMANCE_BIT_EXT
                        | VkDebugUtilsMessageTypeFlagBitsEXT.VK_DEBUG_UTILS_MESSAGE_TYPE_VALIDATION_BIT_EXT,
                    messageSeverity = VkDebugUtilsMessageSeverityFlagBitsEXT.VK_DEBUG_UTILS_MESSAGE_SEVERITY_ERROR_BIT_EXT
                        | VkDebugUtilsMessageSeverityFlagBitsEXT.VK_DEBUG_UTILS_MESSAGE_SEVERITY_WARNING_BIT_EXT
                        | VkDebugUtilsMessageSeverityFlagBitsEXT.VK_DEBUG_UTILS_MESSAGE_SEVERITY_INFO_BIT_EXT,
                    pfnUserCallback = new(DebugCallback),
                }
                : default;

            using var ppEnabledLayerNames     = new StringArrayPtr(validationLayers.ToArray());
            using var ppEnabledExtensionNames = new StringArrayPtr(this.RequiredExtensions);

            var instanceCreateInfo = new VkInstanceCreateInfo
            {
                enabledExtensionCount   = (uint)ppEnabledExtensionNames.Length,
                enabledLayerCount       = (uint)ppEnabledLayerNames.Length,
                pApplicationInfo        = &applicationInfo,
                ppEnabledExtensionNames = ppEnabledExtensionNames,
                ppEnabledLayerNames     = ppEnabledLayerNames,
                pNext                   = NullIfDefault(debugUtilsMessengerCreateInfo, &debugUtilsMessengerCreateInfo),
            };

            VkCheck(this.vk.CreateInstance(instanceCreateInfo, default, out this.instance));
            this.surfaceExtension = this.vk.GetInstanceExtension<VkKhrSurfaceExtension>(this.instance);

            this.debugUtilsExtension = default;
            this.debugUtilsMessenger = default;

            if (this.enableValidationLayers)
            {
                this.debugUtilsExtension = this.vk.GetInstanceExtension<VkExtDebugUtilsExtension>(this.instance);

                VkCheck(this.debugUtilsExtension.CreateDebugUtilsMessenger(this.instance, debugUtilsMessengerCreateInfo, default, out this.debugUtilsMessenger));
            }
        }

        this.PlatformInitialize();
    }

    private static VkExtent2D ChooseSwapExtent(Size<uint> size, VkSurfaceCapabilitiesKHR capabilities)
    {
        if (capabilities.currentExtent.width != uint.MaxValue)
        {
            return capabilities.currentExtent;
        }
        else
        {
            var actualExtent = new VkExtent2D
            {
                width  = size.Width,
                height = size.Height,
            };

            actualExtent.width  = Math.Clamp(actualExtent.width, capabilities.minImageExtent.width, capabilities.maxImageExtent.width);
            actualExtent.height = Math.Clamp(actualExtent.height, capabilities.minImageExtent.height, capabilities.maxImageExtent.height);

            return actualExtent;
        }
    }

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

        var message = Marshal.PtrToStringAnsi((nint)pCallbackData->pMessage);

        Console.WriteLine(message);

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

    private bool CheckValidationLayerSupport()
    {
        this.vk.EnumerateInstanceLayerProperties(out VkLayerProperties[] properties);

        return validationLayers.Overlaps(properties.Select(x => Marshal.PtrToStringAnsi((nint)x.layerName)!));
    }

    private void SetupFrames()
    {
        for (ushort i = 0; i < this.frames.Length; i++)
        {
            var createInfo = new VkCommandPoolCreateInfo
            {
                flags            = VkCommandPoolCreateFlagBits.VK_COMMAND_POOL_CREATE_RESET_COMMAND_BUFFER_BIT,
                queueFamilyIndex = this.graphicsQueueIndex,
            };

            VkCheck(this.vk.CreateCommandPool(this.device, createInfo, default, out var commandPool));

            var commandBufferAllocateInfo = new VkCommandBufferAllocateInfo
            {
                commandPool        = commandPool,
                commandBufferCount = 1,
                level              = VkCommandBufferLevel.VK_COMMAND_BUFFER_LEVEL_PRIMARY
            };

            VkCheck(this.vk.AllocateCommandBuffers(this.device, commandBufferAllocateInfo, out VkCommandBuffer commandBuffer));

            this.frames[i].CommandBuffer = commandBuffer;
            this.frames[i].CommandPool   = commandPool;
            this.frames[i].Index         = i;
        }
    }

    private void CreateDevice(out VkDevice device, out VkKhrSwapchainExtension swapchainExtension, out VkQueue graphicsQueue, out VkQueue presentationQueue)
    {
        var queuePriorities  = 1f;
        var pQueuePriorities = &queuePriorities;

        var queueCreateInfos = new HashSet<uint> { this.graphicsQueueIndex, this.presentationQueueIndex }
            .Select(
                x => new VkDeviceQueueCreateInfo
                {
                    queueFamilyIndex = this.graphicsQueueIndex,
                    queueCount       = 1,
                    pQueuePriorities = pQueuePriorities,
                }
            )
            .ToArray();

        using var ppEnabledExtensionNames = new StringArrayPtr([VkKhrSwapchainExtension.Name]);

        fixed (VkDeviceQueueCreateInfo* pQueueCreateInfos = queueCreateInfos)
        {
            var deviceCreateInfo = new VkDeviceCreateInfo
            {
                enabledExtensionCount   = (uint)ppEnabledExtensionNames.Length,
                ppEnabledExtensionNames = ppEnabledExtensionNames,
                pQueueCreateInfos       = pQueueCreateInfos,
                queueCreateInfoCount    = (uint)queueCreateInfos.Length,
            };

            VkCheck(this.vk.CreateDevice(this.physicalDevice, deviceCreateInfo, default, out device));

            if (!this.vk.TryGetDeviceExtension(this.physicalDevice, device, out swapchainExtension!))
            {
                throw new Exception($"Cannot found required extension {VkKhrSwapchainExtension.Name}");
            }

            this.vk.GetDeviceQueue(device, this.graphicsQueueIndex, 0, out graphicsQueue);
            this.vk.GetDeviceQueue(device, this.presentationQueueIndex, 0, out presentationQueue);
        }
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

        VkCheck(this.vk.CreateImageView(this.device, imageViewCreateInfo, default, out var imageView));

        return imageView;
    }

    private SwapchainHandler CreateSwapchain(VkSurfaceKHR surface, Size<uint> size)
    {
        this.surfaceExtension.GetPhysicalDeviceSurfaceCapabilities(this.physicalDevice, surface, out var surfaceCapabilities);
        var extent = ChooseSwapExtent(size, surfaceCapabilities);

        fixed (uint* pQueueFamilyIndices = &this.presentationQueueIndex)
        {
            var swapchainCreateInfo = new VkSwapchainCreateInfoKHR
            {
                compositeAlpha        = VkCompositeAlphaFlagBitsKHR.VK_COMPOSITE_ALPHA_OPAQUE_BIT_KHR,
                imageArrayLayers      = 1,
                imageColorSpace       = this.surfaceFormat.colorSpace,
                imageExtent           = extent,
                imageFormat           = this.surfaceFormat.format,
                imageUsage            = VkImageUsageFlagBits.VK_IMAGE_USAGE_COLOR_ATTACHMENT_BIT | VkImageUsageFlagBits.VK_IMAGE_USAGE_TRANSFER_DST_BIT,
                minImageCount         = surfaceCapabilities.minImageCount,
                pQueueFamilyIndices   = pQueueFamilyIndices,
                preTransform          = VkSurfaceTransformFlagBitsKHR.VK_SURFACE_TRANSFORM_IDENTITY_BIT_KHR,
                queueFamilyIndexCount = 1,
                surface               = surface,
            };

            VkCheck(this.swapchainExtension.CreateSwapchain(this.device, swapchainCreateInfo, default, out var swapchain));
            VkCheck(this.swapchainExtension.GetSwapchainImages(this.device, swapchain, out VkImage[] images));

            var imageViews = new VkImageView[images.Length];
            var framebuffers = new VkFramebuffer[images.Length];

            var renderPass = this.CreateRenderPass(this.surfaceFormat.format);

            for (var i = 0; i < images.Length; i++)
            {
                imageViews[i]   = this.CreateImageView(images[i], this.surfaceFormat.format, VkImageAspectFlagBits.VK_IMAGE_ASPECT_COLOR_BIT);
                framebuffers[i] = this.CreateFrameBuffer(renderPass, imageViews[i], extent);
            }

            return new SwapchainHandler
            {
                Extent        = extent,
                Format        = this.surfaceFormat.format,
                Framebuffers  = framebuffers,
                Handler       = swapchain,
                Images        = images,
                ImageViews    = imageViews,
                RenderPass    = renderPass,
            };
        }
    }

    private void GetSurfaceCapabilities(VkSurfaceKHR surface, out VkSurfaceFormatKHR surfaceFormat)
    {
        this.surfaceExtension.GetPhysicalDeviceSurfaceFormats(this.physicalDevice, surface, out VkSurfaceFormatKHR[] surfaceFormats);

        foreach (var availableFormat in surfaceFormats)
        {
            if (availableFormat.format == VkFormat.VK_FORMAT_B8G8R8A8_SRGB && availableFormat.colorSpace == VkColorSpaceKHR.VK_COLOR_SPACE_SRGB_NONLINEAR_KHR)
            {
                surfaceFormat = availableFormat;
            }
        }

        surfaceFormat = surfaceFormats[0];
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

        VkCheck(this.vk.CreateFramebuffer(this.device, framebufferCreateInfo, default, out var framebuffer));

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

        VkCheck(this.vk.CreateRenderPass(this.device, renderPassCreateInfo, default, out var renderPass));

        return renderPass;
    }

    private void CreateSyncObjects()
    {
        for (var i = 0; i < MAX_FRAMES_IN_FLIGHT; i++)
        {
            VkCheck(this.vk.CreateSemaphore(this.device, new VkSemaphoreCreateInfo(), default, out this.renderingFinishedSemaphores[i]));

            var fenceCreateInfo = new VkFenceCreateInfo
            {
                flags = VkFenceCreateFlagBits.VK_FENCE_CREATE_SIGNALED_BIT,
            };

            VkCheck(this.vk.CreateFence(this.device, fenceCreateInfo, default, out this.fences[i]));
        }
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

    private void CopyBufferToImage(VkBuffer buffer, VkImage image, uint width, uint height)
    {
        var commandBuffer = this.BeginSingleTimeCommands();

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

        this.EndSingleTimeCommands(commandBuffer);
    }

    private void DestroySwapchain(SwapchainHandler swapchain)
    {
        this.vk.DeviceWaitIdle(this.device);

        for (var i = 0; i < swapchain.ImageViews.Length; i++)
        {
            this.vk.DestroyFramebuffer(this.device, swapchain.Framebuffers[i], null);
            this.vk.DestroyImageView(this.device, swapchain.ImageViews[i], null);
        }

        this.vk.DestroyRenderPass(this.device, swapchain.RenderPass, default);
        this.swapchainExtension.DestroySwapchain(this.device, swapchain.Handler, null);
    }

    public uint FindMemoryType(uint typeFilter, VkMemoryPropertyFlagBits properties)
    {
        this.vk.GetPhysicalDeviceMemoryProperties(this.physicalDevice, out var memProperties);

        for (var i = 0u; i < memProperties.memoryTypeCount; i++)
        {
            if ((typeFilter & (1 << (int)i)) != 0 && memProperties.GetMemoryTypes(i).propertyFlags.HasFlag(properties))
            {
                return i;
            }
        }

        throw new Exception("Failed to find suitable memory type");
    }

    private void InitializeDevice(VkSurfaceKHR surface)
    {
        this.PickPhysicalDevice(surface, out this.physicalDevice, out this.graphicsQueueIndex, out this.presentationQueueIndex);
        this.CreateDevice(out this.device, out this.swapchainExtension, out this.graphicsQueue, out this.presentationQueue);
        this.SetupFrames();
        this.CreateSyncObjects();
        this.GetSurfaceCapabilities(surface, out this.surfaceFormat);
    }

    private void PickPhysicalDevice(VkSurfaceKHR surface, out VkPhysicalDevice physicalDevice, out uint graphicsQueueIndex, out uint presentationQueueIndex)
    {
        this.vk.EnumeratePhysicalDevices(this.instance, out VkPhysicalDevice[] physicalDevices);

        var graphicsQueueFounded = -1;
        var presentationQueueFounded = -1;

        foreach (var device in physicalDevices)
        {
            this.vk.GetPhysicalDeviceFeatures(device, out var supportedFeatures);
            this.vk.GetPhysicalDeviceQueueFamilyProperties(device, out VkQueueFamilyProperties[] queueFamilyProperties);

            for (var i = 0u; i < queueFamilyProperties.Length; i++)
            {
                var queue = queueFamilyProperties[i];

                if (queue.queueFlags.HasFlag(VkQueueFlagBits.VK_QUEUE_GRAPHICS_BIT | VkQueueFlagBits.VK_QUEUE_TRANSFER_BIT))
                {
                    graphicsQueueFounded = (int)i;
                }

                this.surfaceExtension.GetPhysicalDeviceSurfaceSupport(device, i, surface, out var supported);

                if (supported)
                {
                    presentationQueueFounded = (int)i;
                }

                if (graphicsQueueFounded > -1 && presentationQueueFounded > -1 && supportedFeatures.samplerAnisotropy)
                {
                    graphicsQueueIndex     = (uint)graphicsQueueFounded;
                    presentationQueueIndex = (uint)presentationQueueFounded;
                    physicalDevice         = device;

                    return;
                }
            }
        }

        throw new Exception("Failed to find a suitable GPU!");
    }

    private void RecreateSwapchain(SurfaceContext context)
    {
        if (!context.Hidden)
        {
            this.DestroySwapchain(context.Swapchain);

            context.Swapchain  = this.CreateSwapchain(context.Surface, context.Size);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            this.vk.DeviceWaitIdle(this.device);

            for (var i = 0; i < MAX_FRAMES_IN_FLIGHT; i++)
            {
                this.vk.DestroySemaphore(this.device, this.renderingFinishedSemaphores[i], null);
                this.vk.DestroyFence(this.device, this.fences[i], null);
            }

            foreach (var frame in this.frames)
            {
                this.vk.FreeCommandBuffers(this.device, frame.CommandPool, frame.CommandBuffer);
                this.vk.DestroyCommandPool(this.device, frame.CommandPool, null);
            }

            this.vk.DestroyDevice(this.device, null);

            this.debugUtilsExtension?.DestroyDebugUtilsMessenger(this.instance, this.debugUtilsMessenger, null);

            this.vk.DestroyInstance(this.instance, null);
            this.vk.Dispose();

            this.disposed = true;
        }
    }

    public VkCommandBuffer BeginSingleTimeCommands()
    {
        var allocInfo = new VkCommandBufferAllocateInfo
        {
            level              = VkCommandBufferLevel.VK_COMMAND_BUFFER_LEVEL_PRIMARY,
            commandPool        = this.frames[this.currentFrame].CommandPool,
            commandBufferCount = 1
        };

        VkCheck(this.vk.AllocateCommandBuffers(this.device, allocInfo, out VkCommandBuffer commandBuffer));

        var beginInfo = new VkCommandBufferBeginInfo
        {
            flags = VkCommandBufferUsageFlagBits.VK_COMMAND_BUFFER_USAGE_ONE_TIME_SUBMIT_BIT
        };

        VkCheck(this.vk.BeginCommandBuffer(commandBuffer, beginInfo));

        return commandBuffer;
    }

    public virtual SurfaceContext CreateSurfaceContext(VkSurfaceKHR surface, Size<uint> size)
    {
        if (!this.deviceInitialized)
        {
            this.InitializeDevice(surface);

            this.deviceInitialized = true;
        }

        var swapchain = this.CreateSwapchain(surface, size);

        var semaphores = new VkSemaphore[MAX_FRAMES_IN_FLIGHT];

        for (var i = 0; i < MAX_FRAMES_IN_FLIGHT; i++)
        {
            VkCheck(this.vk.CreateSemaphore(this.device, new VkSemaphoreCreateInfo(), default, out semaphores[i]));
        }

        var surfaceContext = new SurfaceContext
        {
            Semaphores  = semaphores,
            Size        = size,
            Surface     = surface,
            Swapchain   = swapchain,
        };

        this.surfaceContexts.Add(surfaceContext);

        return surfaceContext;
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void DestroySurfaceContext(SurfaceContext context)
    {
        this.vk.DeviceWaitIdle(this.device);

        for (var i = 0; i < MAX_FRAMES_IN_FLIGHT; i++)
        {
            this.vk.DestroySemaphore(this.device, context.Semaphores[i], null);
        }

        this.DestroySwapchain(context.Swapchain);

        this.surfaceExtension.DestroySurface(this.instance, context.Surface, null);

        this.surfaceContexts.Remove(context);
    }

    public void EndSingleTimeCommands(VkCommandBuffer commandBuffer)
    {
        VkCheck(this.vk.EndCommandBuffer(commandBuffer));

        var submitInfo = new VkSubmitInfo
        {
            commandBufferCount = 1,
            pCommandBuffers    = &commandBuffer
        };

        VkCheck(this.vk.QueueSubmit(this.graphicsQueue, submitInfo, default));
        VkCheck(this.vk.QueueWaitIdle(this.graphicsQueue));

        this.vk.FreeCommandBuffers(this.device, this.frames[this.currentFrame].CommandPool, commandBuffer);
    }

    public void PrepareBuffers()
    {
        if (this.surfaceContexts.Count == 0 || this.surfaceContexts.All(x => x.Hidden))
        {
            return;
        }

        var fence = this.fences[this.currentFrame];

        this.vk.WaitForFences(this.device, fence, true, ulong.MaxValue);
        this.vk.ResetFences(this.device, fence);

        foreach (var context in this.surfaceContexts)
        {
            if (!context.Hidden)
            {
                var result = this.swapchainExtension.AcquireNextImage(this.device, context.Swapchain.Handler, ulong.MaxValue, context.Semaphores[this.currentFrame], default, out var imageIndex);

                context.CurrentBuffer = imageIndex;

                if (result == VkResult.VK_ERROR_OUT_OF_DATE_KHR)
                {
                    this.RecreateSwapchain(context);
                }
                else if (result is not VkResult.VK_SUCCESS and not VkResult.VK_SUBOPTIMAL_KHR)
                {
                    throw new Exception("failed to acquire swapchain image!");
                }
            }
        }
    }

    public void GetPhysicalDeviceProperties(out VkPhysicalDeviceProperties properties) =>
        this.vk.GetPhysicalDeviceProperties(this.physicalDevice, out properties);

    public void SwapBuffers()
    {
        var visibleSurfaces = this.surfaceContexts.Where(x => !x.Hidden).ToArray();
        var fence = this.fences[this.currentFrame];

        if (visibleSurfaces.Length == 0)
        {
            return;
        }

        var imageIndices   = new uint[visibleSurfaces.Length];
        var swapchains     = new VkSwapchainKHR[visibleSurfaces.Length];
        var waitSemaphores = new VkSemaphore[visibleSurfaces.Length];
        var waitStages     = new VkPipelineStageFlags[visibleSurfaces.Length];
        var results        = new VkResult[visibleSurfaces.Length];

        for (var i = 0; i < visibleSurfaces.Length; i++)
        {
            var context = visibleSurfaces[i];

            imageIndices[i]   = context.CurrentBuffer;
            swapchains[i]     = context.Swapchain.Handler;
            waitSemaphores[i] = context.Semaphores[this.currentFrame];
            waitStages[i]     = VkPipelineStageFlagBits.VK_PIPELINE_STAGE_COLOR_ATTACHMENT_OUTPUT_BIT;
        }

        fixed (VkSemaphore*          pWaitSemaphores   = waitSemaphores)
        fixed (VkPipelineStageFlags* pWaitDstStageMask = waitStages)
        fixed (VkCommandBuffer*      pCommandBuffers   = &this.frames[this.currentFrame].CommandBuffer)
        fixed (VkSemaphore*          pSignalSemaphores = &this.renderingFinishedSemaphores[this.currentFrame])
        fixed (VkSwapchainKHR*       pSwapchains       = swapchains)
        fixed (uint*                 pImageIndices     = imageIndices)
        fixed (VkResult*             pResults          = results)
        {
            var submitInfo = new VkSubmitInfo
            {
                commandBufferCount   = 1,
                pCommandBuffers      = pCommandBuffers,
                pSignalSemaphores    = pSignalSemaphores,
                pWaitDstStageMask    = pWaitDstStageMask,
                pWaitSemaphores      = pWaitSemaphores,
                signalSemaphoreCount = 1,
                waitSemaphoreCount   = (uint)waitSemaphores.Length,
            };

            if (this.vk.QueueSubmit(this.graphicsQueue, submitInfo, fence) != VkResult.VK_SUCCESS)
            {
                throw new Exception("failed to submit draw command buffer!");
            }

            var presentInfo = new VkPresentInfoKHR
            {
                waitSemaphoreCount = 1,
                pWaitSemaphores    = pSignalSemaphores,
                swapchainCount     = (uint)swapchains.Length,
                pSwapchains        = pSwapchains,
                pImageIndices      = pImageIndices,
                pResults           = pResults,
            };

            this.swapchainExtension.QueuePresent(this.presentationQueue, presentInfo);

            for (var i = 0; i < results.Length; i++)
            {
                var result  = results[i];
                var context = this.surfaceContexts[i];

                if (result is VkResult.VK_ERROR_OUT_OF_DATE_KHR)
                {
                    Console.WriteLine("Vulkan queue submit: Swapchain is out of date, recreating.");

                    this.RecreateSwapchain(context);

                }
                else if (result == VkResult.VK_SUBOPTIMAL_KHR)
                {
                    Console.WriteLine("Vulkan queue submit: Swapchain is suboptimal.");
                }
                else if (result != VkResult.VK_SUCCESS)
                {
                    throw new Exception("failed to present swap chain image!");
                }
            }
        }

        this.currentFrame = (ushort)((this.currentFrame + 1) % MAX_FRAMES_IN_FLIGHT);
    }
}
