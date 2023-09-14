using System.Diagnostics;
using System.Runtime.InteropServices;
using Age.Core.Unsafe;
using Age.Platform.Windows.Display;
using Age.Platform.Windows.Vulkan;
using Age.Vulkan.Native;
using Age.Vulkan.Native.Enums;
using Age.Vulkan.Native.Extensions.EXT;
using Age.Vulkan.Native.Extensions.EXT.Enums;
using Age.Vulkan.Native.Extensions.EXT.VkFlags;

namespace Age;

public unsafe class SimpleEngine : IDisposable
{
    public record QueueFamilyIndices
    {
        public uint? GraphicsFamily { get; set; }
    }

    private readonly HashSet<string> validationLayers = new()
    {
        "VK_LAYER_KHRONOS_validation"
    };
    private readonly VulkanWindows vk            = new();
    private readonly WindowManager windowManager = new();

    private readonly bool            enableValidationLayers = Debugger.IsAttached;

    private VkDebugUtilsMessengerEXT debugMessenger;
    private VkDevice                 device;
    private VkPhysicalDevice         physicalDevice;
    private VkExtDebugUtils?         vkExtDebugUtils;

    private bool       disposed;
    private VkInstance instance;
    private Window?    window;

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

    private void Cleanup()
    {
        if (this.enableValidationLayers && this.vkExtDebugUtils != null)
        {
            this.vkExtDebugUtils.DestroyDebugUtilsMessenger(this.instance, this.debugMessenger, default);
        }

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
                if (!this.vk.TryGetExtension<VkExtDebugUtils>(instance, out var vkExtDebugUtils))
                {
                    throw new Exception($"Cannot found required extension {VkExtDebugUtils.Name}");
                }

                this.instance        = instance;
                this.vkExtDebugUtils = vkExtDebugUtils;
            }
        }
    }

    private void CreateLogicalDevice()
    {
        var indices = this.FindQueueFamilies(this.physicalDevice);

        var queueCreateInfo = new VkDeviceQueueCreateInfo
        {
            sType            = VkStructureType.VK_STRUCTURE_TYPE_DEVICE_QUEUE_CREATE_INFO,
            queueFamilyIndex = indices.GraphicsFamily!.Value,
            queueCount       = 1
        };

        var queuePriority = 1.0f;

        queueCreateInfo.pQueuePriorities = &queuePriority;

        var deviceFeatures = new VkPhysicalDeviceFeatures();

        var createInfo = new VkDeviceCreateInfo
        {
            sType                 = VkStructureType.VK_STRUCTURE_TYPE_DEVICE_CREATE_INFO,
            pQueueCreateInfos     = &queueCreateInfo,
            queueCreateInfoCount  = 1,
            pEnabledFeatures      = &deviceFeatures,
            enabledExtensionCount = 0,
        };

        // Deprecated
        // if (this.enableValidationLayers)
        // {
        //     createInfo.enabledLayerCount   = (uint)ppEnabledLayerNames.Length;
        //     createInfo.ppEnabledLayerNames = ppEnabledLayerNames;
        // }
        // else
        // {
        //     createInfo.enabledLayerCount = 0;
        // }

        if (this.vk.CreateDevice(this.physicalDevice, createInfo, default, out this.device) != VkResult.VK_SUCCESS)
        {
            throw new Exception("failed to create logical device!");
        }
    }

    private bool CheckValidationLayerSupport()
    {
        this.vk.EnumerateInstanceLayerProperties(out VkLayerProperties[] availableLayers);

        return availableLayers.Any(x => this.validationLayers.Contains(Marshal.PtrToStringAnsi((nint)x.layerName)!));
    }

    private List<string> GetRequiredExtensions()
    {
        var extensions = new List<string>();

        if (this.enableValidationLayers)
        {
            extensions.Add(VkExtDebugUtils.Name);
        }

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

            if (indices.GraphicsFamily.HasValue)
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
        this.PickPhysicalDevice();
        this.CreateLogicalDevice();
    }

    private bool IsDeviceSuitable(VkPhysicalDevice device)
    {
        var indices = this.FindQueueFamilies(device);

        return indices.GraphicsFamily.HasValue;
    }

    private void MainLoop()
    {
        this.window = this.windowManager.CreateWindow("Age", 600, 400, 0, 0);

        this.window.Show();

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
        this.InitVulkan();
        this.MainLoop();
        this.Cleanup();
    }
}
