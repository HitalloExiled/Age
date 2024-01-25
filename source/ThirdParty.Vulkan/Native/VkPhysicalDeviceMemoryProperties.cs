namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPhysicalDeviceMemoryProperties.html">VkPhysicalDeviceMemoryProperties</see>
/// </summary>
public unsafe struct VkPhysicalDeviceMemoryProperties
{
    public uint       memoryTypeCount;
    public fixed byte memoryTypes[8 * (int)Constants.VK_MAX_MEMORY_TYPES];
    public uint       memoryHeapCount;
    public fixed byte memoryHeaps[16 * (int)Constants.VK_MAX_MEMORY_HEAPS];
}
