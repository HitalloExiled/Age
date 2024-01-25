namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkImageSubresourceLayers.html">VkImageSubresourceLayers</see>
/// </summary>
public struct VkImageSubresourceLayers
{
    public VkImageAspectFlags aspectMask;
    public uint               mipLevel;
    public uint               baseArrayLayer;
    public uint               layerCount;
}
