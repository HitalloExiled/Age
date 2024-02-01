namespace ThirdParty.Vulkan;

public unsafe partial class ImageView : DisposableNativeHandle
{
    private readonly Device device;

    public ImageView(Device device, CreateInfo createInfo)
    {
        this.device = device;

        fixed (VkImageView* pHandle = &this.Handle)
        {
            VulkanException.Check(PInvoke.vkCreateImageView(device, createInfo, device.PhysicalDevice.Instance.Allocator, pHandle));
        }
    }

    protected override void OnDispose() =>
        PInvoke.vkDestroyImageView(this.device, this, this.device.PhysicalDevice.Instance.Allocator);
}
