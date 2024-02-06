namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkMemoryAllocateInfo.html">VkMemoryAllocateInfo</see>
/// </summary>
public unsafe struct VkMemoryAllocateInfo
{
    public readonly VkStructureType SType;

    public void*        PNext;
    public VkDeviceSize AllocationSize;
    public uint         MemoryTypeIndex;

    public VkMemoryAllocateInfo() =>
        this.SType = VkStructureType.MemoryAllocateInfo;
}
