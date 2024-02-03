namespace ThirdParty.Vulkan;

public unsafe partial class ImageView : DeviceResource
{
    public ImageView(Device device, CreateInfo createInfo) : base(device)
    {
        fixed (VkImageView* pHandle = &this.Handle)
        {
            VulkanException.Check(PInvoke.vkCreateImageView(device, createInfo, device.PhysicalDevice.Instance.Allocator, pHandle));
        }
    }

    protected override void OnDispose() =>
        PInvoke.vkDestroyImageView(this.Device, this, this.Device.PhysicalDevice.Instance.Allocator);
}
