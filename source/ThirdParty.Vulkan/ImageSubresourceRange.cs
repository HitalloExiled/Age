using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

/// <inheritdoc cref="VkImageSubresourceRange" />
public unsafe record ImageSubresourceRange : NativeReference<VkImageSubresourceRange>
{
    public ImageAspectFlags AspectMask
    {
        get => this.PNative->aspectMask;
        init => this.PNative->aspectMask = value;
    }

    public uint BaseMipLevel
    {
        get => this.PNative->baseMipLevel;
        init => this.PNative->baseMipLevel = value;
    }

    public uint LevelCount
    {
        get => this.PNative->levelCount;
        init => this.PNative->levelCount = value;
    }

    public uint BaseArrayLayer
    {
        get => this.PNative->baseArrayLayer;
        init => this.PNative->baseArrayLayer = value;
    }

    public uint LayerCount
    {
        get => this.PNative->layerCount;
        init => this.PNative->layerCount = value;
    }
}
