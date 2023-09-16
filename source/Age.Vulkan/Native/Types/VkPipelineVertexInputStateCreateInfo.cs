using Age.Vulkan.Native.Enums;
using Age.Vulkan.Native.Flags;

namespace Age.Vulkan.Native.Types;

/// <summary>
/// Structure specifying parameters of a newly created pipeline vertex input state.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public unsafe struct VkPipelineVertexInputStateCreateInfo
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
    /// Reserved for future use.
    /// </summary>
    public VkPipelineVertexInputStateCreateFlags flags;

    /// <summary>
    /// The number of vertex binding descriptions provided in pVertexBindingDescriptions.
    /// </summary>
    public uint vertexBindingDescriptionCount;

    /// <summary>
    /// A pointer to an array of <see cref="VkVertexInputBindingDescription"/> structures.
    /// </summary>
    public VkVertexInputBindingDescription* pVertexBindingDescriptions;

    /// <summary>
    /// The number of vertex attribute descriptions provided in pVertexAttributeDescriptions.
    /// </summary>
    public uint vertexAttributeDescriptionCount;

    /// <summary>
    /// A pointer to an array of <see cref="VkVertexInputAttributeDescription"/> structures.
    /// </summary>
    public VkVertexInputAttributeDescription* pVertexAttributeDescriptions;

    public VkPipelineVertexInputStateCreateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_VERTEX_INPUT_STATE_CREATE_INFO;
}
