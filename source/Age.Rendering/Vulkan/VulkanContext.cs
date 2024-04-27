using System.Diagnostics;
using System.Runtime.InteropServices;
using Age.Core;
using Age.Core.Interop;
using Age.Numerics;
using Age.Rendering.Resources;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Extensions;
using ThirdParty.Vulkan.Flags;
using static Age.Core.Interop.PointerHelper;

namespace Age.Rendering.Vulkan;

public unsafe partial class VulkanContext : IDisposable
{
    public event Action? SwapchainRecreated;

    public const ushort MAX_FRAMES_IN_FLIGHT = 2;

    private static readonly HashSet<string> validationLayers = [VkConstants.VK_LAYER_KHRONOS_VALIDATION];

    private readonly VkDebugUtilsExtensionEXT? debugUtilsExtension;
    private readonly VkDebugUtilsMessengerEXT? debugUtilsMessenger;
    private readonly bool                      enableValidationLayers = Debugger.IsAttached;
    private readonly VkFence[]                 fences = new VkFence[MAX_FRAMES_IN_FLIGHT];
    private readonly Frame[]                   frames = new Frame[MAX_FRAMES_IN_FLIGHT];
    private readonly VkInstance                instance;
    private readonly VkSemaphore[]             renderingFinishedSemaphores = new VkSemaphore[MAX_FRAMES_IN_FLIGHT];
    private readonly VkSurfaceExtensionKHR     surfaceExtension;

    private ushort                  currentFrame;
    private VkDevice                device = null!;
    private bool                    deviceInitialized;
    private bool                    disposed;
    private VkQueue                 graphicsQueue = null!;
    private uint                    graphicsQueueIndex;
    private VkPhysicalDevice        physicalDevice = null!;
    private VkQueue                 presentationQueue = null!;
    private uint                    presentationQueueIndex;
    private VkSurfaceFormatKHR      surfaceFormat;
    private VkSwapchainExtensionKHR swapchainExtension = null!;

    private IList<string> RequiredExtensions
    {
        get
        {
            var extensions = new List<string>
            {
                VkSurfaceExtensionKHR.Name,
            };

            extensions.AddRange(this.platformExtensions);

            if (this.enableValidationLayers)
            {
                extensions.Add(VkDebugUtilsExtensionEXT.Name);
            }

            return extensions;
        }
    }

    public ref Frame Frame => ref this.frames[this.currentFrame];

    public VkDevice Device       => this.device;
    public VkFormat ScreenFormat => this.surfaceFormat.Format;

    public unsafe VulkanContext()
    {
        if (this.enableValidationLayers && !CheckValidationLayerSupport())
        {
            throw new Exception("validation layers requested, but not available!");
        }

        fixed (byte* pName = "Age"u8)
        {
            var applicationInfo = new VkApplicationInfo
            {
                ApiVersion         = VkVersion.V1_0,
                ApplicationVersion = new VkVersion(0, 0, 1, 0),
                EngineVersion      = new VkVersion(0, 0, 1, 0),
                PApplicationName   = pName,
                PEngineName        = pName,
            };

            var debugUtilsMessengerCreateInfo = this.enableValidationLayers
                ?  new VkDebugUtilsMessengerCreateInfoEXT
                {
                    MessageType = VkDebugUtilsMessageTypeFlagsEXT.DeviceAddressBinding
                        | VkDebugUtilsMessageTypeFlagsEXT.General
                        | VkDebugUtilsMessageTypeFlagsEXT.Performance
                        | VkDebugUtilsMessageTypeFlagsEXT.Validation,
                    MessageSeverity = VkDebugUtilsMessageSeverityFlagsEXT.Error
                        | VkDebugUtilsMessageSeverityFlagsEXT.Warning
                        | VkDebugUtilsMessageSeverityFlagsEXT.Info,
                    PfnUserCallback = new(DebugCallback),
                }
                : default;

            using var ppEnabledLayerNames     = new NativeStringArray(validationLayers.ToArray());
            using var ppEnabledExtensionNames = new NativeStringArray(this.RequiredExtensions);

            var instanceCreateInfo = new VkInstanceCreateInfo
            {
                EnabledExtensionCount   = (uint)ppEnabledExtensionNames.Length,
                EnabledLayerCount       = (uint)ppEnabledLayerNames.Length,
                PApplicationInfo        = &applicationInfo,
                PpEnabledExtensionNames = ppEnabledExtensionNames,
                PpEnabledLayerNames     = ppEnabledLayerNames,
                PNext                   = NullIfDefault(&debugUtilsMessengerCreateInfo),
            };

            this.instance         = new VkInstance(instanceCreateInfo);
            this.surfaceExtension = this.instance.GetExtension<VkSurfaceExtensionKHR>();

            this.debugUtilsExtension = default;
            this.debugUtilsMessenger = default;

            if (this.enableValidationLayers)
            {
                this.debugUtilsExtension = this.instance.GetExtension<VkDebugUtilsExtensionEXT>();

                this.debugUtilsMessenger = this.debugUtilsExtension.CreateDebugUtilsMessenger(debugUtilsMessengerCreateInfo);
            }
        }

        this.PlatformInitialize();
    }

    private static VkExtent2D ChooseSwapExtent(Size<uint> size, VkSurfaceCapabilitiesKHR capabilities)
    {
        if (capabilities.CurrentExtent.Width != uint.MaxValue)
        {
            return capabilities.CurrentExtent;
        }
        else
        {
            var actualExtent = new VkExtent2D
            {
                Width  = size.Width,
                Height = size.Height,
            };

            actualExtent.Width  = Math.Clamp(actualExtent.Width, capabilities.MinImageExtent.Width, capabilities.MaxImageExtent.Width);
            actualExtent.Height = Math.Clamp(actualExtent.Height, capabilities.MinImageExtent.Height, capabilities.MaxImageExtent.Height);

            return actualExtent;
        }
    }

    private unsafe static VkBool32 DebugCallback(VkDebugUtilsMessageSeverityFlagsEXT messageSeverity, VkDebugUtilsMessageTypeFlagsEXT messageTypes, VkDebugUtilsMessengerCallbackDataEXT* pCallbackData, void* pUserData)
    {
        var logLevel = messageSeverity switch
        {
            VkDebugUtilsMessageSeverityFlagsEXT.Error   => LogLevel.Error,
            VkDebugUtilsMessageSeverityFlagsEXT.Warning => LogLevel.Warning,
            VkDebugUtilsMessageSeverityFlagsEXT.Info    => LogLevel.Info,
            _ => LogLevel.None
        };

        var message = Marshal.PtrToStringAnsi((nint)pCallbackData->PMessage);

        Logger.Log(message!, logLevel);

        return true;
    }

    private static bool CheckValidationLayerSupport()
    {
        var properties = VkInstance.EnumerateLayerProperties();

        return validationLayers.Overlaps(properties.Select(x => Marshal.PtrToStringAnsi((nint)x.LayerName)!));
    }

    private void SetupFrames()
    {
        for (ushort i = 0; i < this.frames.Length; i++)
        {
            var createInfo = new VkCommandPoolCreateInfo
            {
                Flags            = VkCommandPoolCreateFlags.ResetCommandBuffer,
                QueueFamilyIndex = this.graphicsQueueIndex,
            };

            var commandPool   = this.device.CreateCommandPool(createInfo);
            var commandBuffer = commandPool.AllocateCommand(VkCommandBufferLevel.Primary);

            this.frames[i].Fence         = this.fences[i];
            this.frames[i].CommandBuffer = commandBuffer;
            this.frames[i].CommandPool   = commandPool;
            this.frames[i].Index         = i;
        }
    }

    private void CreateDevice(out VkDevice device, out VkSwapchainExtensionKHR swapchainExtension, out VkQueue graphicsQueue, out VkQueue presentationQueue)
    {
        var queuePriorities  = 1f;
        var pQueuePriorities = &queuePriorities;

        var queueCreateInfos = new HashSet<uint> { this.graphicsQueueIndex, this.presentationQueueIndex }
            .Select(
                x => new VkDeviceQueueCreateInfo
                {
                    QueueFamilyIndex = this.graphicsQueueIndex,
                    QueueCount       = 1,
                    PQueuePriorities = pQueuePriorities,
                }
            )
            .ToArray();

        using var ppEnabledExtensionNames = new NativeStringArray([VkSwapchainExtensionKHR.Name]);

        var enabledFeatures = new VkPhysicalDeviceFeatures
        {
            GeometryShader = true,
        };

        fixed (VkDeviceQueueCreateInfo* pQueueCreateInfos = queueCreateInfos)
        {
            var deviceCreateInfo = new VkDeviceCreateInfo
            {
                EnabledExtensionCount   = (uint)ppEnabledExtensionNames.Length,
                PpEnabledExtensionNames = ppEnabledExtensionNames,
                PQueueCreateInfos       = pQueueCreateInfos,
                PEnabledFeatures        = &enabledFeatures,
                QueueCreateInfoCount    = (uint)queueCreateInfos.Length,
            };

            device             = this.physicalDevice.CreateDevice(deviceCreateInfo);
            swapchainExtension = this.device.GetExtension<VkSwapchainExtensionKHR>();

            graphicsQueue     = device.GetQueue(this.graphicsQueueIndex, 0);
            presentationQueue = device.GetQueue(this.presentationQueueIndex, 0);
        }
    }

    private VkImageView CreateImageView(VkImage image, VkFormat format, VkImageAspectFlags aspect)
    {
        var createInfo = new VkImageViewCreateInfo
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

        var imageView = this.device.CreateImageView(createInfo);

        return imageView;
    }

    private Swapchain CreateSwapchain(VkSurfaceKHR surface, Size<uint> size)
    {
        this.surfaceExtension.GetPhysicalDeviceSurfaceCapabilities(this.physicalDevice, surface, out var surfaceCapabilities);
        var extent = ChooseSwapExtent(size, surfaceCapabilities);

        fixed (uint* pQueueFamilyIndices = &this.presentationQueueIndex)
        {
            var swapchainCreateInfo = new VkSwapchainCreateInfoKHR
            {
                CompositeAlpha        = VkCompositeAlphaFlagsKHR.Opaque,
                ImageArrayLayers      = 1,
                ImageColorSpace       = this.surfaceFormat.ColorSpace,
                ImageExtent           = extent,
                PresentMode           = VkPresentModeKHR.Fifo,
                ImageFormat           = this.surfaceFormat.Format,
                ImageUsage            = VkImageUsageFlags.ColorAttachment | VkImageUsageFlags.TransferDst,
                ImageSharingMode      = VkSharingMode.Exclusive,
                MinImageCount         = surfaceCapabilities.MinImageCount,
                PQueueFamilyIndices   = pQueueFamilyIndices,
                PreTransform          = VkSurfaceTransformFlagsKHR.Identity,
                QueueFamilyIndexCount = 1,
                Surface               = surface.Handle,
            };

            var swapchain = this.swapchainExtension.CreateSwapchain(swapchainCreateInfo);
            var images    = swapchain.GetImages();

            var imageViews   = new VkImageView[images.Length];
            var framebuffers = new VkFramebuffer[images.Length];

            var renderPassCreateInfo = new RenderPass.CreateInfo
            {
                ColorAttachments =
                [
                    new()
                    {
                        Layout = VkImageLayout.ColorAttachmentOptimal,
                        Color  = new VkAttachmentDescription
                        {
                            FinalLayout    = VkImageLayout.PresentSrcKHR,
                            Format         = this.surfaceFormat.Format,
                            InitialLayout  = VkImageLayout.Undefined,
                            LoadOp         = VkAttachmentLoadOp.Clear,
                            Samples        = VkSampleCountFlags.N1,
                            StencilLoadOp  = VkAttachmentLoadOp.Clear,
                            StencilStoreOp = VkAttachmentStoreOp.DontCare,
                        }
                    }
                ],
            };

            return new Swapchain
            {
                Extent = extent,
                Format = this.surfaceFormat.Format,
                Images = images,
                Value  = swapchain,
            };
        }
    }

    private void GetSurfaceCapabilities(VkSurfaceKHR surface, out VkSurfaceFormatKHR surfaceFormat)
    {
        var surfaceFormats = surface.GetFormats(this.physicalDevice);

        foreach (var availableFormat in surfaceFormats)
        {
            if (availableFormat.Format == VkFormat.B8G8R8A8Srgb && availableFormat.ColorSpace == VkColorSpaceKHR.SrgbNonlinear)
            {
                surfaceFormat = availableFormat;
            }
        }

        surfaceFormat = surfaceFormats[0];
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

        return this.device.CreateFramebuffer(createInfo);
    }

    private void CreateSyncObjects()
    {
        for (var i = 0; i < MAX_FRAMES_IN_FLIGHT; i++)
        {
            this.renderingFinishedSemaphores[i] = this.device.CreateSemaphore();

            var fenceCreateInfo = new VkFenceCreateInfo
            {
                Flags = VkFenceCreateFlags.Signaled,
            };

            this.fences[i] = this.device.CreateFence(fenceCreateInfo);
        }
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

    private void CopyBufferToImage(VkBuffer buffer, VkImage image, uint width, uint height)
    {
        var commandBuffer = this.BeginSingleTimeCommands();

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

        this.EndSingleTimeCommands(commandBuffer);
    }

    public uint FindMemoryType(uint typeFilter, VkMemoryPropertyFlags properties)
    {
        this.physicalDevice.GetMemoryProperties(out var memProperties);

        for (var i = 0u; i < memProperties.MemoryTypeCount; i++)
        {
            if ((typeFilter & (1 << (int)i)) != 0 && ((VkMemoryType*)memProperties.MemoryTypes)[i].PropertyFlags.HasFlag(properties))
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
        this.CreateSyncObjects();
        this.GetSurfaceCapabilities(surface, out this.surfaceFormat);
        this.SetupFrames();
    }

    private void PickPhysicalDevice(VkSurfaceKHR surface, out VkPhysicalDevice physicalDevice, out uint graphicsQueueIndex, out uint presentationQueueIndex)
    {
        var physicalDevices = this.instance.EnumeratePhysicalDevices();

        var graphicsQueueFounded = -1;
        var presentationQueueFounded = -1;

        foreach (var pd in physicalDevices)
        {
            pd.GetDeviceFeatures(out var supportedFeatures);
            var queueFamilyProperties = pd.GetQueueFamilyProperties();

            for (var i = 0u; i < queueFamilyProperties.Length; i++)
            {
                var queue = queueFamilyProperties[i];

                if (queue.QueueFlags.HasFlag(VkQueueFlags.Graphics | VkQueueFlags.Transfer))
                {
                    graphicsQueueFounded = (int)i;
                }

                var supported = surface.GetSupport(pd, i);

                if (supported)
                {
                    presentationQueueFounded = (int)i;
                }

                if (graphicsQueueFounded > -1 && presentationQueueFounded > -1 && supportedFeatures.SamplerAnisotropy)
                {
                    graphicsQueueIndex     = (uint)graphicsQueueFounded;
                    presentationQueueIndex = (uint)presentationQueueFounded;
                    physicalDevice         = pd;

                    return;
                }
            }
        }

        throw new Exception("Failed to find a suitable GPU!");
    }

    private void RecreateSwapchain(Surface context)
    {
        if (!context.Hidden)
        {
            this.Device.WaitIdle();

            context.Swapchain.Dispose();

            context.Swapchain = this.CreateSwapchain(context.Value, context.Size);

            SwapchainRecreated?.Invoke();
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            this.device.WaitIdle();

            for (var i = 0; i < MAX_FRAMES_IN_FLIGHT; i++)
            {
                this.renderingFinishedSemaphores[i].Dispose();
                this.fences[i].Dispose();
            }

            foreach (var frame in this.frames)
            {
                frame.CommandBuffer.Dispose();
                frame.CommandPool.Dispose();
            }

            this.device.Dispose();

            this.debugUtilsMessenger?.Dispose();

            this.instance.Dispose();

            this.disposed = true;
        }
    }

    public VkCommandBuffer BeginSingleTimeCommands()
    {
        var commandBuffer = this.Frame.CommandPool.AllocateCommand(VkCommandBufferLevel.Primary);

        commandBuffer.Begin(VkCommandBufferUsageFlags.OneTimeSubmit);

        return commandBuffer;
    }

    public RenderPass CreateRenderPass(VkFormat format)
    {
        var createInfo = new RenderPass.CreateInfo
        {
            ColorAttachments =
            [
                new()
                {
                    Layout = VkImageLayout.ColorAttachmentOptimal,
                    Color  = new VkAttachmentDescription
                    {
                        Samples        = VkSampleCountFlags.N1,
                        FinalLayout    = VkImageLayout.PresentSrcKHR,
                        Format         = format,
                        InitialLayout  = VkImageLayout.Undefined,
                        LoadOp         = VkAttachmentLoadOp.Clear,
                        StencilLoadOp  = VkAttachmentLoadOp.Clear,
                        StencilStoreOp = VkAttachmentStoreOp.DontCare,
                    }
                }
            ]
        };

        return this.CreateRenderPass(createInfo);
    }

    public RenderPass CreateRenderPass(in RenderPass.CreateInfo createInfo)
    {
        var attachmentDescription = new List<VkAttachmentDescription>();
        var colorAttachments      = new List<VkAttachmentReference>(createInfo.ColorAttachments.Length);
        var resolveAttachments    = new List<VkAttachmentReference>(createInfo.ColorAttachments.Length);

        foreach (var attachment in createInfo.ColorAttachments)
        {
            colorAttachments.Add(new() { Attachment = (uint)attachmentDescription.Count, Layout = attachment.Layout });
            attachmentDescription.Add(attachment.Color);

            if (!attachment.Resolve.Equals(default(VkAttachmentDescription)))
            {
                resolveAttachments.Add(new() { Attachment = (uint)attachmentDescription.Count, Layout = attachment.Layout });
                attachmentDescription.Add(attachment.Color);
            }
        }

        fixed (VkAttachmentDescription* pAttachments            = CollectionsMarshal.AsSpan(attachmentDescription))
        fixed (VkAttachmentReference*   pColorAttachments       = CollectionsMarshal.AsSpan(colorAttachments))
        fixed (VkAttachmentReference*   pResolveAttachments     = CollectionsMarshal.AsSpan(resolveAttachments))
        fixed (VkAttachmentReference*   pDepthStencilAttachment = &createInfo.DepthStencilAttachment)
        {
            var subpass = new VkSubpassDescription
            {
                PipelineBindPoint       = VkPipelineBindPoint.Graphics,
                PColorAttachments       = pColorAttachments,
                PResolveAttachments     = pResolveAttachments,
                ColorAttachmentCount    = (uint)colorAttachments.Count,
                PDepthStencilAttachment = NullIfDefault(pDepthStencilAttachment),
            };

            var renderPassCreateInfo = new VkRenderPassCreateInfo
            {
                AttachmentCount = (uint)attachmentDescription.Count,
                PAttachments    = pAttachments,
                PSubpasses      = &subpass,
                SubpassCount    = 1,
            };

            var renderPass = this.device.CreateRenderPass(renderPassCreateInfo);

            var framebuffers = new Framebuffer[MAX_FRAMES_IN_FLIGHT];
            var swapchain    = Surface.Entries[0].Swapchain;

            for (var i = 0; i < MAX_FRAMES_IN_FLIGHT; i++)
            {
                var imageView   = this.CreateImageView(swapchain.Images[i], swapchain.Format, VkImageAspectFlags.Color);
                var framebuffer = this.CreateFrameBuffer(renderPass, imageView, swapchain.Extent);

                framebuffers[i] = new()
                {
                    Value     = framebuffer,
                    ImageView = imageView,
                };
            }

            return new()
            {
                Value        = renderPass,
                Framebuffers = framebuffers,
            };
        }
    }

    public virtual Surface CreateSurface(VkSurfaceKHR surface, Size<uint> size)
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
            semaphores[i] = this.device.CreateSemaphore();
        }

        var surfaceContext = new Surface
        {
            Semaphores  = semaphores,
            Size        = size,
            Value     = surface,
            Swapchain   = swapchain,
        };

        return surfaceContext;
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void EndSingleTimeCommands(VkCommandBuffer commandBuffer)
    {
        commandBuffer.End();

        var commandBufferHandle = commandBuffer.Handle;

        var submitInfo = new VkSubmitInfo
        {
            CommandBufferCount = 1,
            PCommandBuffers    = &commandBufferHandle
        };

        this.graphicsQueue.Submit(submitInfo);
        this.graphicsQueue.WaitIdle();

        commandBuffer.Dispose();
    }

    public void PrepareBuffers()
    {
        if (Surface.Entries.Count == 0 || Surface.Entries.All(x => x.Hidden))
        {
            return;
        }

        var fence = this.fences[this.currentFrame];

        fence.Wait(true, ulong.MaxValue);

        foreach (var context in Surface.Entries)
        {
            if (!context.Hidden)
            {
                uint imageIndex = 0;
                try
                {
                    imageIndex = context.Swapchain.Value.AcquireNextImage(ulong.MaxValue, context.Semaphores[this.currentFrame], default);
                }
                catch (VkException exception)
                {
                    if (exception.Result == VkResult.ErrorOutOfDateKHR)
                    {
                        this.RecreateSwapchain(context);
                    }
                }
                catch (Exception)
                {
                    throw;
                }

                context.CurrentBuffer = imageIndex;
            }
        }

        fence.Reset();
        this.Frame.CommandBuffer.Reset();

        this.Frame.BufferPrepared = true;
    }

    public void GetPhysicalDeviceProperties(out VkPhysicalDeviceProperties properties) =>
        this.physicalDevice.GetProperties(out properties);

    public void SwapBuffers()
    {
        if (!this.Frame.BufferPrepared)
        {
            return;
        }

        var visibleSurfaces = Surface.Entries.Where(x => !x.Hidden).ToArray();

        var fence          = this.fences[this.currentFrame];
        var imageIndices   = new uint[visibleSurfaces.Length];
        var swapchains     = new VkHandle<VkSwapchainKHR>[visibleSurfaces.Length];
        var waitSemaphores = new VkHandle<VkSemaphore>[visibleSurfaces.Length];
        var waitStages     = new VkPipelineStageFlags[visibleSurfaces.Length];
        var results        = new VkResult[visibleSurfaces.Length];

        for (var i = 0; i < visibleSurfaces.Length; i++)
        {
            var context = visibleSurfaces[i];

            imageIndices[i]   = context.CurrentBuffer;
            swapchains[i]     = context.Swapchain.Value.Handle;
            waitSemaphores[i] = context.Semaphores[this.currentFrame].Handle;
            waitStages[i]     = VkPipelineStageFlags.ColorAttachmentOutput;
        }

        var commandBufferHandle   = this.Frame.CommandBuffer.Handle;
        var signalSemaphoreHandle = this.renderingFinishedSemaphores[this.currentFrame].Handle;

        fixed (VkHandle<VkSemaphore>*    pWaitSemaphores   = waitSemaphores)
        fixed (VkPipelineStageFlags*     pWaitDstStageMask = waitStages)
        fixed (VkHandle<VkSwapchainKHR>* pSwapchains       = swapchains)
        fixed (uint*                     pImageIndices     = imageIndices)
        fixed (VkResult*                 pResults          = results)
        {
            var submitInfo = new VkSubmitInfo
            {
                CommandBufferCount   = 1,
                PCommandBuffers      = &commandBufferHandle,
                PSignalSemaphores    = &signalSemaphoreHandle,
                PWaitDstStageMask    = pWaitDstStageMask,
                PWaitSemaphores      = pWaitSemaphores,
                SignalSemaphoreCount = 1,
                WaitSemaphoreCount   = (uint)waitSemaphores.Length,
            };

            this.graphicsQueue.Submit(submitInfo, fence);

            var presentInfo = new VkPresentInfoKHR
            {
                WaitSemaphoreCount = 1,
                PWaitSemaphores    = &signalSemaphoreHandle,
                SwapchainCount     = (uint)swapchains.Length,
                PSwapchains        = pSwapchains,
                PImageIndices      = pImageIndices,
                PResults           = pResults,
            };

            try
            {
                this.swapchainExtension.QueuePresent(this.presentationQueue, presentInfo);
            }
            catch (VkException)
            {
                for (var i = 0; i < results.Length; i++)
                {
                    var result  = results[i];
                    var context = Surface.Entries[i];

                    if (result is VkResult.ErrorOutOfDateKHR or VkResult.SuboptimalKHR)
                    {
                        Logger.Trace("Vulkan queue submit: Swapchain is out of date, recreating.");

                        this.RecreateSwapchain(context);
                    }
                    else
                    {
                        throw new Exception("failed to present swap chain image!");
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        this.currentFrame = (ushort)((this.currentFrame + 1) % MAX_FRAMES_IN_FLIGHT);

        this.Frame.BufferPrepared = false;
    }
}
