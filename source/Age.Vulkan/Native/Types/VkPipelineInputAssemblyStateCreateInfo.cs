using Age.Vulkan.Native.Enums;
using Age.Vulkan.Native.Flags;

namespace Age.Vulkan.Native.Types;

/// <summary>
/// <para>Structure specifying parameters of a newly created pipeline input assembly state</para>
/// <para>Restarting the assembly of primitives discards the most recent index values if those elements formed an incomplete primitive, and restarts the primitive assembly using the subsequent indices, but only assembling the immediately following element through the end of the originally specified elements. The primitive restart index value comparison is performed before adding the vertexOffset value to the index value.</para>
/// </summary>
public unsafe struct VkPipelineInputAssemblyStateCreateInfo
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
    public VkPipelineInputAssemblyStateCreateFlags flags;

    /// <summary>
    /// A VkPrimitiveTopology defining the primitive topology, as described below.
    /// </summary>
    public VkPrimitiveTopology topology;

    /// <summary>
    /// Controls whether a special vertex index value is treated as restarting the assembly of primitives. This enable only applies to indexed draws (<see cref="Vk.CmdDrawIndexed"/>, <see cref="VkExtMultiDraw.CmdDrawMultiIndexed"/>, and <see cref="Vk.CmdDrawIndexedIndirect"/>), and the special index value is either 0xFFFFFFFF when the indexType parameter of <see cref="VkKhrMaintenance5.CmdBindIndexBuffer2"/> or <see cref="Vk.CmdBindIndexBuffer"/> is equal to <see cref="VkIndexType.VK_INDEX_TYPE_UINT32"/>, 0xFF when indexType is equal to <see cref="VkIndexType.VK_INDEX_TYPE_UINT8_EXT"/>, or 0xFFFF when indexType is equal to <see cref="VkIndexType.VK_INDEX_TYPE_UINT16"/>. Primitive restart is not allowed for “list” topologies, unless one of the features <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#features-primitiveTopologyPatchListRestart">primitiveTopologyPatchListRestart</see> (for <see cref="VkPrimitiveTopology.VK_PRIMITIVE_TOPOLOGY_PATCH_LIST"/>) or <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#features-primitiveTopologyListRestart">primitiveTopologyListRestart</see> (for all other list topologies) is enabled.
    /// </summary>
    public VkBool32 primitiveRestartEnable;

    public VkPipelineInputAssemblyStateCreateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_INPUT_ASSEMBLY_STATE_CREATE_INFO;
}
