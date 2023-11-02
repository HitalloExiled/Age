using Age.Vulkan.Native.Enums;
using Age.Vulkan.Native.Flags;

namespace Age.Vulkan.Native.Types;

/// <summary>
/// Structure specifying parameters of a newly created pipeline color blend state.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public unsafe struct VkPipelineColorBlendStateCreateInfo
{
    /// <summary>
    /// A VkStructureType value identifying this structure.
    /// </summary>
    public readonly VkStructureType sType;

    /// <summary>
    /// Null or a pointer to a structure extending this structure.
    /// </summary>
    public void* pNext;

    /// <summary>
    /// A bitmask of VkPipelineColorBlendStateCreateFlagBits specifying additional color blending information.
    /// </summary>
    public VkPipelineColorBlendStateCreateFlags flags;

    /// <summary>
    /// Whether to apply Logical Operations.
    /// </summary>
    public VkBool32 logicOpEnable;

    /// <summary>
    /// Which logical operation to apply.
    /// </summary>
    public VkLogicOp logicOp;

    /// <summary>
    /// The number of <see cref="VkPipelineColorBlendAttachmentState"/> elements in pAttachments. It is ignored if the pipeline is created with <see cref="VkDynamicState.VK_DYNAMIC_STATE_COLOR_BLEND_ENABLE_EXT"/>, <see cref="VkDynamicState.VK_DYNAMIC_STATE_COLOR_BLEND_EQUATION_EXT"/>, and <see cref="VkDynamicState.VK_DYNAMIC_STATE_COLOR_WRITE_MASK_EXT"/> dynamic states set, and either <see cref="VkDynamicState.VK_DYNAMIC_STATE_COLOR_BLEND_ADVANCED_EXT"/> set or advancedBlendCoherentOperations is not enabled on the device.
    /// </summary>
    public uint attachmentCount;

    /// <summary>
    /// A pointer to an array of <see cref="VkPipelineColorBlendAttachmentState"/> structures defining blend state for each color attachment. It is ignored if the pipeline is created with <see cref="VkDynamicState.VK_DYNAMIC_STATE_COLOR_BLEND_ENABLE_EXT"/>, <see cref="VkDynamicState.VK_DYNAMIC_STATE_COLOR_BLEND_EQUATION_EXT"/>, and <see cref="VkDynamicState.VK_DYNAMIC_STATE_COLOR_WRITE_MASK_EXT"/> dynamic states set, and either <see cref="VkDynamicState.VK_DYNAMIC_STATE_COLOR_BLEND_ADVANCED_EXT"/> set or advancedBlendCoherentOperations is not enabled on the device.
    /// </summary>
    public VkPipelineColorBlendAttachmentState* pAttachments;

    /// <summary>
    /// A pointer to an array of four values used as the R, G, B, and A components of the blend constant that are used in blending, depending on the blend factor.
    /// </summary>
    public fixed float blendConstants[4];

    public VkPipelineColorBlendStateCreateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_COLOR_BLEND_STATE_CREATE_INFO;
}
