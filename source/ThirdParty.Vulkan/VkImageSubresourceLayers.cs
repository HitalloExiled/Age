using ThirdParty.Vulkan.Flags;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkImageSubresourceLayers.html">VkImageSubresourceLayers</see>
/// </summary>
public struct VkImageSubresourceLayers
{
    public VkImageAspectFlags AspectMask;
    public uint               MipLevel;
    public uint               BaseArrayLayer;
    public uint               LayerCount;
}
