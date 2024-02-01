namespace ThirdParty.Vulkan;

public unsafe partial class DescriptorSetLayout : DisposableNativeHandle
{
    private readonly Device device;

    internal DescriptorSetLayout(Device device, CreateInfo createInfo)
    {
        this.device = device;

        fixed (VkDescriptorSetLayout* pHandle = &this.Handle)
        {
            VulkanException.Check(PInvoke.vkCreateDescriptorSetLayout(device, createInfo, device.PhysicalDevice.Instance.Allocator, pHandle));
        }
    }

    protected override void OnDispose() =>
        PInvoke.vkDestroyDescriptorSetLayout(this.device, this, this.device.PhysicalDevice.Instance.Allocator);
}
