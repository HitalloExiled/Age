namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkImageMemoryBarrier.html">VkImageMemoryBarrier</see>
/// </summary>
public unsafe struct VkImageMemoryBarrier
{
    public readonly VkStructureType sType;

    public void*                   pNext;
    public VkAccessFlags           srcAccessMask;
    public VkAccessFlags           dstAccessMask;
    public VkImageLayout           oldLayout;
    public VkImageLayout           newLayout;
    public uint                    srcQueueFamilyIndex;
    public uint                    dstQueueFamilyIndex;
    public VkImage                 image;
    public VkImageSubresourceRange subresourceRange;

    public VkImageMemoryBarrier() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_IMAGE_MEMORY_BARRIER;
}
