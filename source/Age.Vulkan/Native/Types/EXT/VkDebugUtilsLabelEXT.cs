using Age.Vulkan.Native.Enums;

namespace Age.Vulkan.Native.Types.EXT;

/// <summary>
/// Specify parameters of a label region
/// </summary>
public unsafe struct VkDebugUtilsLabelEXT
{
    /// <summary>
    /// A <see cref="VkStructureType"/> value identifying this structure.
    /// </summary>
    public readonly VkStructureType sType;

    /// <summary>
    /// Null or a pointer to a structure extending this structure.
    /// </summary>
    public void* pNext;

    /// <summary>
    /// Is a pointer to a null-terminated UTF-8 string containing the name of the label.
    /// </summary>
    public byte* pLabelName;

    /// <summary>
    /// An optional RGBA color value that can be associated with the label. A particular implementation may choose to ignore this color value. The values contain RGBA values in order, in the range 0.0 to 1.0. If all elements in color are set to 0.0 then it is ignored.
    /// </summary>
    public fixed float color[4];

    public VkDebugUtilsLabelEXT() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_DEBUG_UTILS_LABEL_EXT;
}
