namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkImageBlit.html">VkImageBlit</see>
/// </summary>
public unsafe struct VkImageBlit
{
    public VkImageSubresourceLayers SrcSubresource;
    public fixed byte               SrcOffsets[2 * 12];
    public VkImageSubresourceLayers DstSubresource;
    public fixed byte               DstOffsets[2 * 12];
}
