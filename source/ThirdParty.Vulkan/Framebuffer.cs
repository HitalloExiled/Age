namespace ThirdParty.Vulkan;

public unsafe partial class Framebuffer : DeviceResource
{
    internal Framebuffer(Device device, CreateInfo createInfo) : base(device)
    {
        fixed (VkFramebuffer* pHandle = &this.Handle)
        {
            VulkanException.Check(PInvoke.vkCreateFramebuffer(device, createInfo, device.PhysicalDevice.Instance.Allocator, pHandle));
        }
    }

    protected override void OnDispose() =>
        PInvoke.vkDestroyFramebuffer(this.Device, this, this.Device.PhysicalDevice.Instance.Allocator);
}
