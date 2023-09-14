using System.Runtime.InteropServices;
using Age.Core.Extensions;
using Age.Core.Unsafe;
using Age.Platform.Windows.Display;
using Age.Platform.Windows.Vulkan;
using Age.Vulkan.Native;
using Age.Vulkan.Native.Enums;

namespace Age;

public unsafe class SimpleEngine : IDisposable
{
    private readonly HashSet<string> validationLayers = new()
    {
        "VK_LAYER_KHRONOS_validation"
    };
    private readonly VulkanWindows   vk               = new();
    private readonly WindowManager   windowManager    = new();

    #if DEBUG
    private readonly bool enableValidationLayers = true;
    #else
    private readonly bool enableValidationLayers;
    #endif

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

    private void Cleanup() =>
        this.vk.DestroyInstance(this.instance, null);

    private void CreateInstance()
    {
        if (this.enableValidationLayers && !this.CheckValidationLayerSupport())
        {
            throw new InvalidOperationException("validation layers requested, but not available!");
        }

        fixed (byte* pApplicationName = "Hello Triangle".ToUTF8Bytes())
        fixed (byte* pEngineName      = "No Engine".ToUTF8Bytes())
        {
            var appInfo = new VkApplicationInfo
            {
                sType              = VkStructureType.VK_STRUCTURE_TYPE_APPLICATION_INFO,
                pApplicationName   = pApplicationName,
                applicationVersion = VK.MakeApiVersion(1, 0, 0),
                pEngineName        = pEngineName,
                engineVersion      = VK.MakeApiVersion(1, 0, 0),
                apiVersion         = VK.ApiVersion_1_0,
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
                this.instance = instance;
            }
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
            extensions.Add("VK_EXT_debug_utils");
        }

        return extensions;
    }

    private void InitVulkan()
    {
        this.CreateInstance();
        this.SetupDebugMessenger();
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

    private void SetupDebugMessenger()
    {
        if (!this.enableValidationLayers)
        {
            return;
        }

        var createInfo = new VkDebugUtilsMessengerCreateInfoEXT
        {
            sType           = VkStructureType.VK_STRUCTURE_TYPE_DEBUG_UTILS_MESSENGER_CREATE_INFO_EXT,
            messageSeverity = VkDebugUtilsMessageSeverityFlagBitsEXT.VK_DEBUG_UTILS_MESSAGE_SEVERITY_VERBOSE_BIT_EXT | VkDebugUtilsMessageSeverityFlagBitsEXT.VK_DEBUG_UTILS_MESSAGE_SEVERITY_WARNING_BIT_EXT | VkDebugUtilsMessageSeverityFlagBitsEXT.VK_DEBUG_UTILS_MESSAGE_SEVERITY_ERROR_BIT_EXT,
            messageType     = VkDebugUtilsMessageTypeFlagBitsEXT.VK_DEBUG_UTILS_MESSAGE_TYPE_GENERAL_BIT_EXT | VkDebugUtilsMessageTypeFlagBitsEXT.VK_DEBUG_UTILS_MESSAGE_TYPE_VALIDATION_BIT_EXT | VkDebugUtilsMessageTypeFlagBitsEXT.VK_DEBUG_UTILS_MESSAGE_TYPE_PERFORMANCE_BIT_EXT,
            pfnUserCallback = new(DebugCallback),
            pUserData       = null
        };
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
