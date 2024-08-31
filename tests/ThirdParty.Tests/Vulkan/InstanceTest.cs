#if TEST_VULKAN
using System.Runtime.InteropServices;
using Age.Core.Interop;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace ThirdParty.Tests.Vulkan;

public class InstanceTest
{
    [Fact]
    public unsafe void NativeCreateInstance()
    {
        static unsafe VkBool32 callback(VkDebugUtilsMessageSeverityFlagsEXT messageSeverity, VkDebugUtilsMessageTypeFlagsEXT messageTypes, VkDebugUtilsMessengerCallbackDataEXT* pCallbackData, void* pUserData)
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

            Console.WriteLine(Marshal.PtrToStringAnsi((nint)pCallbackData->PMessage));

            Console.ForegroundColor = defaultColor;

            return true;
        }

        VkApplicationInfo applicationInfo;

        fixed (byte* pName = "Engine"u8)
        {
            applicationInfo = new VkApplicationInfo
            {
                ApiVersion         = VkVersion.V1_0,
                PApplicationName   = pName,
                ApplicationVersion = new VkVersion(0, 0, 1, 0),
                PEngineName        = pName,
                EngineVersion      = new VkVersion(0, 0, 1, 0),
            };
        }

        var debugUtilsMessengerCreateInfo = new VkDebugUtilsMessengerCreateInfoEXT
        {
            PfnUserCallback = new(callback),
            MessageSeverity = VkDebugUtilsMessageSeverityFlagsEXT.Error
                | VkDebugUtilsMessageSeverityFlagsEXT.Warning
                | VkDebugUtilsMessageSeverityFlagsEXT.Info,
            MessageType = VkDebugUtilsMessageTypeFlagsEXT.DeviceAddressBinding
                | VkDebugUtilsMessageTypeFlagsEXT.General
                | VkDebugUtilsMessageTypeFlagsEXT.Performance
                | VkDebugUtilsMessageTypeFlagsEXT.Validation,
        };

        using var ppEnabledExtensionNames = new NativeStringArray(["VK_EXT_debug_utils"]);
        using var ppEnabledLayerNames     = new NativeStringArray([VkConstants.VK_LAYER_KHRONOS_VALIDATION]);

        var instanceCreateInfo = new VkInstanceCreateInfo
        {
            PApplicationInfo        = &applicationInfo,
            PpEnabledExtensionNames = ppEnabledExtensionNames,
            EnabledExtensionCount   = (uint)ppEnabledExtensionNames.Length,
            PpEnabledLayerNames     = ppEnabledLayerNames,
            EnabledLayerCount       = (uint)ppEnabledLayerNames.Length,
            PNext                   = &debugUtilsMessengerCreateInfo,
        };

        using var instance = new VkInstance(instanceCreateInfo);

        var physicalDevice = instance.EnumeratePhysicalDevices()[0];

        var qeuePriorities = 1f;

        var queueCreateInfo = new VkDeviceQueueCreateInfo
        {
            QueueFamilyIndex = 0,
            QueueCount       = 1,
            PQueuePriorities = &qeuePriorities,
        };

        physicalDevice.GetDeviceFeatures(out var enabledFeatures);

        var deviceCreateInfo = new VkDeviceCreateInfo
        {
            PEnabledFeatures     = &enabledFeatures,
            PQueueCreateInfos    = &queueCreateInfo,
            QueueCreateInfoCount = 1,
        };

        physicalDevice.GetProperties(out var properties);

        using var device = physicalDevice.CreateDevice(deviceCreateInfo);

        var queue = device.GetQueue(0, 0);

        var commandPoolCreateInfo = new VkCommandPoolCreateInfo
        {
            Flags = VkCommandPoolCreateFlags.ResetCommandBuffer,
        };

        using var commandPool   = device.CreateCommandPool(commandPoolCreateInfo);
        using var commandBuffer = commandPool.AllocateCommand(VkCommandBufferLevel.Primary);

        Assert.True(true);
    }
}
#endif
