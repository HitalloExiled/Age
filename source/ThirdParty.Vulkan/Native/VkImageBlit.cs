namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkImageBlit.html">VkImageBlit</see>
/// </summary>
public unsafe struct VkImageBlit
{
    public VkImageSubresourceLayers srcSubresource;
    public fixed byte               srcOffsets[2 * 12];
    public VkImageSubresourceLayers dstSubresource;
    public fixed byte               dstOffsets[2 * 12];
}
