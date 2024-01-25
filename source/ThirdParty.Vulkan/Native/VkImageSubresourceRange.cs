namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkImageSubresourceRange.html">VkImageSubresourceRange</see>
/// </summary>
public struct VkImageSubresourceRange
{
    public VkImageAspectFlags aspectMask;
    public uint               baseMipLevel;
    public uint               levelCount;
    public uint               baseArrayLayer;
    public uint               layerCount;
}
