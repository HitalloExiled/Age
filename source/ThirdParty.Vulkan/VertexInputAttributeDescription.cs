using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

/// <summary>
/// Structure specifying vertex input attribute description.
/// </summary>
public unsafe record VertexInputAttributeDescription : NativeReference<VkVertexInputAttributeDescription>
{
    /// <summary>
    /// The shader input location number for this attribute.
    /// </summary>
    public uint Location
    {
        get => this.PNative->location;
        init => this.PNative->location = value;
    }

    /// <summary>
    /// The binding number which this attribute takes its data from.
    /// </summary>
    public uint Binding
    {
        get => this.PNative->binding;
        init => this.PNative->binding = value;
    }

    /// <summary>
    /// The size and type of the vertex attribute data.
    /// </summary>
    public Format Format
    {
        get => this.PNative->format;
        init => this.PNative->format = value;
    }

    /// <summary>
    /// A byte offset of this attribute relative to the start of an element in the vertex input binding.
    /// </summary>
    public uint Offset
    {
        get => this.PNative->offset;
        init => this.PNative->offset = value;
    }
}
