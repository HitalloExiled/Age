using Silk.NET.Vulkan;

namespace Age.Drivers.Vulkan;

internal abstract class VulkanHooks
{
    public virtual bool CreateVulkanInstance(ref InstanceCreateInfo vulkan_create_info, out Instance instance)
    {
        instance = default;

        return false;
    }

	public virtual bool GetPhysicalDevice(out PhysicalDevice device)
    {
        device = default;

        return false;
    }

	public virtual bool CreateVulkanDevice(ref DeviceCreateInfo deviceCreateInfo, out Device device)
    {
        device = default;

        return false;
    }
}
