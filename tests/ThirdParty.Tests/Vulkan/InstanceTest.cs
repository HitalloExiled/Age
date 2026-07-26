using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace ThirdParty.Tests.Vulkan;

[Collection("Vulkan")]
public class InstanceTest(VulkanFixture fixture)
{
    [Fact]
    public void Instance_Create_Succeeds()
    {
        Assert.NotNull(fixture.Instance);
    }

    [Fact]
    public void Instance_EnumeratePhysicalDevices_ReturnsDevices()
    {
        var devices = fixture.Instance.EnumeratePhysicalDevices();

        Assert.NotEmpty(devices);
    }

    [Fact]
    public unsafe void PhysicalDevice_GetProperties_ReturnsProperties()
    {
        var physicalDevice = fixture.Instance.EnumeratePhysicalDevices()[0];

        physicalDevice.GetProperties(out var properties);

        Assert.NotEqual(default, properties);
        Assert.NotEqual(0u, properties.ApiVersion);
    }

    [Fact]
    public unsafe void PhysicalDevice_CreateDevice_Succeeds()
    {
        var physicalDevice  = fixture.Instance.EnumeratePhysicalDevices()[0];
        var queuePriorities = 1f;

        physicalDevice.GetDeviceFeatures(out var enabledFeatures);

        var queueCreateInfo  = new VkDeviceQueueCreateInfo { QueueFamilyIndex = 0, QueueCount = 1, PQueuePriorities = &queuePriorities };
        var deviceCreateInfo = new VkDeviceCreateInfo { PQueueCreateInfos = &queueCreateInfo, QueueCreateInfoCount = 1, PEnabledFeatures = &enabledFeatures };

        using var device = physicalDevice.CreateDevice(deviceCreateInfo);

        Assert.NotNull(device);
    }

    [Fact]
    public unsafe void Device_GetQueue_ReturnsQueue()
    {
        var physicalDevice  = fixture.Instance.EnumeratePhysicalDevices()[0];
        var queuePriorities = 1f;

        physicalDevice.GetDeviceFeatures(out var enabledFeatures);

        var queueCreateInfo  = new VkDeviceQueueCreateInfo { QueueFamilyIndex = 0, QueueCount = 1, PQueuePriorities = &queuePriorities };
        var deviceCreateInfo = new VkDeviceCreateInfo { PQueueCreateInfos = &queueCreateInfo, QueueCreateInfoCount = 1, PEnabledFeatures = &enabledFeatures };

        using var device = physicalDevice.CreateDevice(deviceCreateInfo);
        var       queue  = device.GetQueue(0, 0);

        Assert.NotNull(queue);
    }

    [Fact]
    public unsafe void CommandPool_AllocateCommandBuffer_Succeeds()
    {
        var physicalDevice  = fixture.Instance.EnumeratePhysicalDevices()[0];
        var queuePriorities = 1f;

        physicalDevice.GetDeviceFeatures(out var enabledFeatures);

        var queueCreateInfo  = new VkDeviceQueueCreateInfo { QueueFamilyIndex = 0, QueueCount = 1, PQueuePriorities = &queuePriorities };
        var deviceCreateInfo = new VkDeviceCreateInfo { PQueueCreateInfos = &queueCreateInfo, QueueCreateInfoCount = 1, PEnabledFeatures = &enabledFeatures };

        using var device        = physicalDevice.CreateDevice(deviceCreateInfo);
        using var commandPool   = device.CreateCommandPool(new VkCommandPoolCreateInfo { Flags = VkCommandPoolCreateFlags.ResetCommandBuffer });
        using var commandBuffer = commandPool.AllocateCommand(VkCommandBufferLevel.Primary);

        Assert.NotNull(commandBuffer);
    }
}
