using Age.Vulkan.Enums;

namespace Age.Vulkan.Types;

/// <summary>
/// Structure specifying vertex input binding description.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public struct VkVertexInputBindingDescription
{
    /// <summary>
    /// The binding number that this structure describes.
    /// </summary>
    public uint binding;

    /// <summary>
    /// The byte stride between consecutive elements within the buffer.
    /// </summary>
    public uint stride;

    /// <summary>
    /// A VkVertexInputRate value specifying whether vertex attribute addressing is a function of the vertex index or of the instance index.
    /// </summary>
    public VkVertexInputRate inputRate;
}
