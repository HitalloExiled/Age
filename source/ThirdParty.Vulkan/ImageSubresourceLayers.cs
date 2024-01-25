using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkImageSubresourceLayers.html">VkImageSubresourceLayers</see>
/// </summary>
public unsafe record ImageSubresourceLayers : NativeReference<VkImageSubresourceLayers>
{
    public ImageAspectFlags AspectMask
    {
        get => this.PNative->aspectMask;
        init => this.PNative->aspectMask = value;
    }

    public uint MipLevel
    {
        get => this.PNative->mipLevel;
        init => this.PNative->mipLevel = value;
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

