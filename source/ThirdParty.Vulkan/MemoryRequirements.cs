using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

/// <summary>
/// Structure specifying memory requirements.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public readonly struct MemoryRequirements
{
    /// <summary>
    /// The size, in bytes, of the memory allocation required for the resource.
    /// </summary>
    public ulong Size { get; }

    /// <summary>
    /// The alignment, in bytes, of the offset within the allocation required for the resource.
    /// </summary>
    public ulong Alignment { get; }

    /// <summary>
    /// A bitmask and contains one bit set for every supported memory type for the resource. Bit i is set if and only if the memory type i in the VkPhysicalDeviceMemoryProperties structure for the physical device is supported for the resource.
    /// </summary>
    public uint MemoryTypeBits { get; }

    internal MemoryRequirements(in VkMemoryRequirements memoryRequirements)
    {
        this.Size           = memoryRequirements.size;
        this.Alignment      = memoryRequirements.alignment;
        this.MemoryTypeBits = memoryRequirements.memoryTypeBits;
    }
}
