using Age.Vulkan.Native.Enums;
using Age.Vulkan.Native.Flags;

namespace Age.Vulkan.Native.Types;

/// <summary>
/// Structure specifying the parameters of a newly created pipeline layout object.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public unsafe struct VkPipelineLayoutCreateInfo
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
    /// A bitmask of <see cref="VkPipelineLayoutCreateFlagBits"/> specifying options for pipeline layout creation.
    /// </summary>
    public VkPipelineLayoutCreateFlags flags;

    /// <summary>
    /// The number of descriptor sets included in the pipeline layout.
    /// </summary>
    public uint setLayoutCount;

    /// <summary>
    /// A pointer to an array of <see cref="VkDescriptorSetLayout"/> objects.
    /// </summary>
    public VkDescriptorSetLayout* pSetLayouts;

    /// <summary>
    /// The number of push constant ranges included in the pipeline layout.
    /// </summary>
    public uint pushConstantRangeCount;

    /// <summary>
    /// a pointer to an array of <see cref="VkPushConstantRange"/> structures defining a set of push constant ranges for use in a single pipeline layout. In addition to descriptor set layouts, a pipeline layout also describes how many push constants can be accessed by each stage of the pipeline.
    /// </summary>
    /// <remarks>Push constants represent a high speed path to modify constant data in pipelines that is expected to outperform memory-backed resource updates.</remarks>
    public VkPushConstantRange* pPushConstantRanges;

    public VkPipelineLayoutCreateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_LAYOUT_CREATE_INFO;
}
