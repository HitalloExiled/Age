namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPhysicalDeviceMemoryProperties.html">VkPhysicalDeviceMemoryProperties</see>
/// </summary>
public unsafe struct VkPhysicalDeviceMemoryProperties
{
    public uint       MemoryTypeCount;
    public fixed byte MemoryTypes[8 * (int)VkConstants.VK_MAX_MEMORY_TYPES];
    public uint       MemoryHeapCount;
    public fixed byte MemoryHeaps[16 * (int)VkConstants.VK_MAX_MEMORY_HEAPS];
}
