namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkMemoryBarrier.html">VkMemoryBarrier</see>
/// </summary>
public unsafe struct VkMemoryBarrier
{
    public readonly VkStructureType sType;

    public void*         pNext;
    public VkAccessFlags srcAccessMask;
    public VkAccessFlags dstAccessMask;

    public VkMemoryBarrier() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_MEMORY_BARRIER;
}
