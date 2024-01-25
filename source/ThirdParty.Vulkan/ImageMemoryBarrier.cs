using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

/// <inheritdoc cref="VkImageMemoryBarrier" />
public unsafe record ImageMemoryBarrier : NativeReference<VkImageMemoryBarrier>
{
    private Image?                 image;
    private ImageSubresourceRange? subresourceRange;

    public nint Next
    {
        get => (nint)this.PNative->pNext;
        init => this.PNative->pNext = value.ToPointer();
    }

    public AccessFlags SrcAccessMask
    {
        get => this.PNative->srcAccessMask;
        init => this.PNative->srcAccessMask = value;
    }

    public AccessFlags DstAccessMask
    {
        get => this.PNative->dstAccessMask;
        init => this.PNative->dstAccessMask = value;
    }

    public ImageLayout OldLayout
    {
        get => this.PNative->oldLayout;
        init => this.PNative->oldLayout = value;
    }

    public ImageLayout NewLayout
    {
        get => this.PNative->newLayout;
        init => this.PNative->newLayout = value;
    }

    public uint SrcQueueFamilyIndex
    {
        get => this.PNative->srcQueueFamilyIndex;
        init => this.PNative->srcQueueFamilyIndex = value;
    }

    public uint DstQueueFamilyIndex
    {
        get => this.PNative->dstQueueFamilyIndex;
        init => this.PNative->dstQueueFamilyIndex = value;
    }

    public Image? Image
    {
        get => this.image;
        init => this.PNative->image = this.image = value;
    }

    public ImageSubresourceRange? SubresourceRange
    {
        get => this.subresourceRange;
        init => this.PNative->subresourceRange = this.subresourceRange = value;
    }
}
