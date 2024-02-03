namespace ThirdParty.Vulkan;

public unsafe partial class DescriptorSetLayout : DeviceResource
{
    internal DescriptorSetLayout(Device device, CreateInfo createInfo) : base(device)
    {
        fixed (VkDescriptorSetLayout* pHandle = &this.Handle)
        {
            VulkanException.Check(PInvoke.vkCreateDescriptorSetLayout(device, createInfo, device.PhysicalDevice.Instance.Allocator, pHandle));
        }
    }

    protected override void OnDispose() =>
        PInvoke.vkDestroyDescriptorSetLayout(this.Device, this, this.Device.PhysicalDevice.Instance.Allocator);
}
