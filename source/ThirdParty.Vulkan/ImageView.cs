namespace ThirdParty.Vulkan;

public unsafe partial class ImageView : DisposableNativeHandle
{
    private readonly AllocationCallbacks? allocator;
    private readonly Device               device;

    public ImageView(Device device, CreateInfo createInfo, AllocationCallbacks? allocator)
    {
        this.device    = device;
        this.allocator = allocator;

        fixed (VkImageView* pHandle = &this.Handle)
        {
            VulkanException.Check(PInvoke.vkCreateImageView(device, createInfo, allocator, pHandle));
        }
    }

    protected override void OnDispose() =>
        PInvoke.vkDestroyImageView(this.device, this, this.allocator);
}
