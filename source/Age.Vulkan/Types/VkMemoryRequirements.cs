namespace Age.Vulkan.Types;

/// <summary>
/// Structure specifying memory requirements.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public struct VkMemoryRequirements
{
    /// <summary>
    /// The size, in bytes, of the memory allocation required for the resource.
    /// </summary>
    public VkDeviceSize size;

    /// <summary>
    /// The alignment, in bytes, of the offset within the allocation required for the resource.
    /// </summary>
    public VkDeviceSize alignment;

    /// <summary>
    /// A bitmask and contains one bit set for every supported memory type for the resource. Bit i is set if and only if the memory type i in the VkPhysicalDeviceMemoryProperties structure for the physical device is supported for the resource.
    /// </summary>
    public uint memoryTypeBits;
}
