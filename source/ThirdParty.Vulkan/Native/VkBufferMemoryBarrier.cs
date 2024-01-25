namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkBufferMemoryBarrier.html">VkBufferMemoryBarrier</see>
/// </summary>
public unsafe struct VkBufferMemoryBarrier
{
    public readonly VkStructureType sType;

    public void*         pNext;
    public VkAccessFlags srcAccessMask;
    public VkAccessFlags dstAccessMask;
    public uint          srcQueueFamilyIndex;
    public uint          dstQueueFamilyIndex;
    public VkBuffer      buffer;
    public VkDeviceSize  offset;
    public VkDeviceSize  size;

    public VkBufferMemoryBarrier() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_BUFFER_MEMORY_BARRIER;
}
