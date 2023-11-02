using Age.Vulkan.Native.Enums;
using Age.Vulkan.Native.Flags;

namespace Age.Vulkan.Native.Types;

/// <summary>
/// Structure specifying parameters of a newly created pipeline dynamic state.
/// </summary>
public unsafe struct VkPipelineDynamicStateCreateInfo
{
    /// <summary>
    /// A <see cref="VkStructureType"/> value identifying this structure.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public readonly VkStructureType sType;

    /// <summary>
    /// Null or a pointer to a structure extending this structure.
    /// </summary>
    public void* pNext;

    /// <summary>
    /// Reserved for future use.
    /// </summary>
    public VkPipelineDynamicStateCreateFlags flags;

    /// <summary>
    /// The number of elements in the pDynamicStates array.
    /// </summary>
    public uint dynamicStateCount;

    /// <summary>
    /// A pointer to an array of <see cref="VkDynamicState"/> values specifying which pieces of pipeline state will use the values from dynamic state commands rather than from pipeline state creation information.
    /// </summary>
    public VkDynamicState* pDynamicStates;

    public VkPipelineDynamicStateCreateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_DYNAMIC_STATE_CREATE_INFO;
}
