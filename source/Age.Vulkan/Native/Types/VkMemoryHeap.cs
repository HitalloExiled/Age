using Age.Vulkan.Native.Flags;

namespace Age.Vulkan.Native.Types;

/// <summary>
/// Structure specifying a memory heap.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public struct VkMemoryHeap
{
    /// <summary>
    /// The total memory size in bytes in the heap.
    /// </summary>
    public VkDeviceSize size;

    /// <summary>
    /// A bitmask of <see cref="VkMemoryHeapFlagBits"/> specifying attribute flags for the heap.
    /// </summary>
    public VkMemoryHeapFlags flags;
}
