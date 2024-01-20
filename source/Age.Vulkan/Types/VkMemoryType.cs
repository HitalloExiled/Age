using Age.Vulkan.Flags;

namespace Age.Vulkan.Types;

/// <summary>
/// Structure specifying memory type.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public struct VkMemoryType
{
    /// <summary>
    /// Describes which memory heap this memory type corresponds to, and must be less than memoryHeapCount from the <see cref="VkPhysicalDeviceMemoryProperties"/> structure.
    /// </summary>
    public VkMemoryPropertyFlags propertyFlags;

    /// <summary>
    /// A bitmask of <see cref="VkMemoryPropertyFlagBits"/> of properties for this memory type.
    /// </summary>
    public uint heapIndex;
}
