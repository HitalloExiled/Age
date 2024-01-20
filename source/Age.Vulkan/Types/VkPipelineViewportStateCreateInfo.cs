using Age.Vulkan.Enums;
using Age.Vulkan.Flags;

namespace Age.Vulkan.Types;

/// <summary>
/// Structure specifying parameters of a newly created pipeline viewport state.
/// </summary>
public unsafe struct VkPipelineViewportStateCreateInfo
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
    public VkPipelineViewportStateCreateFlags flags;

    /// <summary>
    /// The number of viewports used by the pipeline.
    /// </summary>
    public uint viewportCount;

    /// <summary>
    /// A pointer to an array of <see cref="VkViewport"/> structures, defining the viewport transforms. If the viewport state is dynamic, this member is ignored.
    /// </summary>
    public VkViewport* pViewports;

    /// <summary>
    /// The number of scissors and must match the number of viewports.
    /// </summary>
    public uint scissorCount;

    /// <summary>
    /// A pointer to an array of <see cref="VkRect2D"/> structures defining the rectangular bounds of the scissor for the corresponding viewport. If the scissor state is dynamic, this member is ignored.
    /// </summary>
    public VkRect2D* pScissors;

    public VkPipelineViewportStateCreateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_VIEWPORT_STATE_CREATE_INFO;
}
