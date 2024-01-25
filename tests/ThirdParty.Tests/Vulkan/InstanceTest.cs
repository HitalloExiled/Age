using ThirdParty.Vulkan;
using ThirdParty.Vulkan.EXT;
using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan.Flags.EXT;
using Version = ThirdParty.Vulkan.Version;

namespace ThirdParty.Tests.Vulkan;

public class InstanceTest
{
    [Fact]
    public void CreateInstance()
    {
        unsafe static bool callback(DebugUtilsMessageSeverityFlags messageSeverity, DebugUtilsMessageTypeFlags messageTypes, DebugUtilsMessenger.CallbackData callbackData)
        {
            var defaultColor = Console.ForegroundColor;

            var color = messageSeverity switch
            {
                DebugUtilsMessageSeverityFlags.Error => ConsoleColor.DarkRed,
                DebugUtilsMessageSeverityFlags.Warning => ConsoleColor.DarkYellow,
                DebugUtilsMessageSeverityFlags.Info => ConsoleColor.DarkBlue,
                _ => defaultColor
            };

            Console.ForegroundColor = color;

            Console.WriteLine(callbackData.Message);

            Console.ForegroundColor = defaultColor;

            return true;
        }

        var applicationInfo = new ApplicationInfo
        {
            ApiVersion         = Version.V1_0,
            ApplicationName    = "Engine",
            ApplicationVersion = new Version(0, 0, 1, 0),
            EngineName         = "Engine",
            EngineVersion      = new Version(0, 0, 1, 0),
        };

        var debugUtilsMessengerCreateInfo = new DebugUtilsMessenger.CreateInfo
        {
            UserCallback = callback,
            MessageSeverity = DebugUtilsMessageSeverityFlags.Error
                | DebugUtilsMessageSeverityFlags.Warning
                | DebugUtilsMessageSeverityFlags.Info,
            MessageType = DebugUtilsMessageTypeFlags.DeviceAddressBinding
                | DebugUtilsMessageTypeFlags.General
                | DebugUtilsMessageTypeFlags.Performance
                | DebugUtilsMessageTypeFlags.Validation,
        };

        var instanceCreateInfo = new Instance.CreateInfo
        {
            ApplicationInfo   = applicationInfo,
            EnabledExtensions = ["VK_EXT_debug_utils"],
            EnabledLayers     = [Constants.VK_LAYER_KHRONOS_VALIDATION],
            Next              = debugUtilsMessengerCreateInfo,
        };

        using var instance = new Instance(instanceCreateInfo);

        var physicalDevice = instance.EnumeratePhysicalDevices()[0];
        var enabledFeatures = physicalDevice.GetDeviceFeatures();

        var deviceCreateInfo = new Device.CreateInfo
        {
            EnabledFeatures   = enabledFeatures,
            EnabledExtensions = [],
            QueueCreateInfos  =
            [
                new DeviceQueue.CreateInfo
                {
                    QueueFamilyIndex = 0,
                    QueuePriorities  = [1],
                }
            ],
        };

        var properties = physicalDevice.GetProperties();
        using var device = physicalDevice.CreateDevice(deviceCreateInfo);

        var queue = device.GetQueue(0, 0);

        var commandPoolCreateInfo = new CommandPool.CreateInfo
        {
            Flags = CommandPoolCreateFlags.ResetCommandBuffer,
        };

        using var commandPool   = device.CreateCommandPool(commandPoolCreateInfo);
        using var commandBuffer = commandPool.AllocateCommand(CommandBufferLevelFlags.Primary);

        Assert.True(true);
    }
}
