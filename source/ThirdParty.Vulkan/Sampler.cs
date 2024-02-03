namespace ThirdParty.Vulkan;

public unsafe partial class Sampler : DeviceResource
{
    internal Sampler(Device device, CreateInfo createInfo) : base(device)
    {
        fixed (VkSampler* pHandle = &this.Handle)
        {
            VulkanException.Check(PInvoke.vkCreateSampler(device, createInfo, device.PhysicalDevice.Instance.Allocator, pHandle));
        }
    }

    protected override void OnDispose() =>
        PInvoke.vkDestroySampler(this.Device, this, this.Device.PhysicalDevice.Instance.Allocator);
}
