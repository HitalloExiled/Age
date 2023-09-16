using Age.Vulkan.Native.Enums;

namespace Age.Vulkan.Native.Types;

/// <summary>
/// Structure specifying vertex input attribute description.
/// </summary>
public struct VkVertexInputAttributeDescription
{
    /// <summary>
    /// The shader input location number for this attribute.
    /// </summary>
    public uint location;

    /// <summary>
    /// The binding number which this attribute takes its data from.
    /// </summary>
    public uint binding;

    /// <summary>
    /// The size and type of the vertex attribute data.
    /// </summary>
    public VkFormat format;

    /// <summary>
    /// A byte offset of this attribute relative to the start of an element in the vertex input binding.
    /// </summary>
    public uint offset;
}
