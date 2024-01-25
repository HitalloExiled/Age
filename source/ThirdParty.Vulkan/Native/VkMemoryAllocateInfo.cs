namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkMemoryAllocateInfo.html">VkMemoryAllocateInfo</see>
/// </summary>
public unsafe struct VkMemoryAllocateInfo
{
    public readonly VkStructureType sType;

    public void*        pNext;
    public VkDeviceSize allocationSize;
    public uint         memoryTypeIndex;

    public VkMemoryAllocateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_MEMORY_ALLOCATE_INFO;
}
