namespace Age.Vulkan.Types;

/// <summary>
/// Structure specifying specialization information.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public unsafe struct VkSpecializationInfo
{
    /// <summary>
    /// The number of entries in the pMapEntries array.
    /// </summary>
    public uint mapEntryCount;

    /// <summary>
    /// A pointer to an array of <see cref="VkSpecializationMapEntry"/> structures, which map constant IDs to offsets in pData.
    /// </summary>
    public VkSpecializationMapEntry* pMapEntries;

    /// <summary>
    /// The byte size of the pData buffer.
    /// </summary>
    public ulong dataSize;

    /// <summary>
    /// Contains the actual constant values to specialize with.
    /// </summary>
    public void* pData;
}
