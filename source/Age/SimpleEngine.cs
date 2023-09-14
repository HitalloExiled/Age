using System.Diagnostics;
using System.Runtime.InteropServices;
using Age.Core.Unsafe;
using Age.Platform.Windows.Display;
using Age.Platform.Windows.Vulkan;
using Age.Vulkan.Native;
using Age.Vulkan.Native.Enums;
using Age.Vulkan.Native.Extensions.EXT;
using Age.Vulkan.Native.Extensions.EXT.Enums;
using Age.Vulkan.Native.Extensions.EXT.Types;
using Age.Vulkan.Native.Extensions.EXT.Flags;
using Age.Vulkan.Native.Extensions.KHR;
using Age.Vulkan.Native.Extensions.KHR.Types;
using Age.Vulkan.Native.Types;

namespace Age;

public unsafe class SimpleEngine : IDisposable
{
    public record QueueFamilyIndices
    {
        public uint? GraphicsFamily { get; set; }
        public uint? PresentFamily  { get; set; }

        public bool IsComplete => this.GraphicsFamily.HasValue && this.PresentFamily.HasValue;
    }

    public record SwapChainSupportDetails
    {
        public required VkSurfaceCapabilitiesKHR Capabilities { get; init; }
        public required VkSurfaceFormatKHR[]     Formats      { get; init; }
        public required VkPresentModeKHR[]       PresentModes { get; init; }
    }

    private readonly HashSet<string> deviceExtensions = new()
    {
        VkKhrSwapchain.Name
    };

    private readonly HashSet<string> validationLayers = new()
    {
        "VK_LAYER_KHRONOS_validation"
    };

    private readonly VulkanWindows vk            = new();
    private readonly WindowManager windowManager = new();

    private readonly bool enableValidationLayers = Debugger.IsAttached;

    private VkDebugUtilsMessengerEXT debugMessenger;
    private VkDevice                 device;
    private VkQueue                  graphicsQueue;
    private VkPhysicalDevice         physicalDevice;
    private VkQueue                  presentQueue;
    private VkSurfaceKHR             surface;
    private VkExtDebugUtils?         vkExtDebugUtils;
    private VkKhrSurface             vkKhrSurface = null!;

    private bool       disposed;
    private VkInstance instance;
    private Window     window = null!;

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
            sType           = VkStructureType.VK_STRUCTURE_TYPE_DEBUG_UTILS_MESSENGER_CREATE_INFO_EXT,
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

    private void Cleanup()
    {
        this.vk.DestroyDevice(this.device, null);

        if (this.enableValidationLayers && this.vkExtDebugUtils != null)
        {
            this.vkExtDebugUtils.DestroyDebugUtilsMessenger(this.instance, this.debugMessenger, default);
        }

        this.vkKhrSurface.DestroySurface(this.instance, this.surface, null);
        this.vk.DestroyInstance(this.instance, null);
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
                sType              = VkStructureType.VK_STRUCTURE_TYPE_APPLICATION_INFO,
                pApplicationName   = pApplicationName,
                applicationVersion = Vk.MakeApiVersion(1, 0, 0),
                pEngineName        = pEngineName,
                engineVersion      = Vk.MakeApiVersion(1, 0, 0),
                apiVersion         = Vk.ApiVersion_1_0,
            };

            var createInfo = new VkInstanceCreateInfo
            {
                sType             = VkStructureType.VK_STRUCTURE_TYPE_INSTANCE_CREATE_INFO,
                pApplicationInfo  = &appInfo,
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
                    throw new Exception($"Cannot found required extension {VkExtDebugUtils.Name}");
                }

                this.instance     = instance;
                this.vkKhrSurface = vkKhrSurface;
            }
        }
    }

    private unsafe void CreateLogicalDevice()
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
                sType            = VkStructureType.VK_STRUCTURE_TYPE_DEVICE_QUEUE_CREATE_INFO,
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
                sType                 = VkStructureType.VK_STRUCTURE_TYPE_DEVICE_CREATE_INFO,
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

            this.vk.GetDeviceQueue(this.device, indices.GraphicsFamily.Value, 0, out this.graphicsQueue);
            this.vk.GetDeviceQueue(this.device, indices.PresentFamily.Value, 0, out this.presentQueue);
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
            sType     = VkStructureType.VK_STRUCTURE_TYPE_WIN32_SURFACE_CREATE_INFO_KHR,
            hwnd      = this.window!.Handle,
            hinstance = Process.GetCurrentProcess().Handle,
        };

        vkKhrWin32Surface.CreateWin32Surface(this.instance, createInfo, default, out this.surface);
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

            this.vk.Dispose();
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
