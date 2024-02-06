using ThirdParty.Vulkan.Flags;

namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkImageSubresourceRange.html">VkImageSubresourceRange</see>
/// </summary>
public struct VkImageSubresourceRange
{
    public VkImageAspectFlags AspectMask;
    public uint               BaseMipLevel;
    public uint               LevelCount;
    public uint               BaseArrayLayer;
    public uint               LayerCount;
}
