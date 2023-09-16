namespace Age.Vulkan.Native.Flags;

/// <summary>
/// Pipeline layout creation flag bits.
/// </summary>
/// <remarks>Provided by VK_EXT_graphics_pipeline_library</remarks>
[Flags]
public enum VkPipelineLayoutCreateFlagBits
{
    /// <summary>
    /// Specifies that implementations must ensure that the properties and/or absence of a particular descriptor set do not influence any other properties of the pipeline layout. This allows pipelines libraries linked without <see cref="VkPipelineCreateFlagBits.VK_PIPELINE_CREATE_LINK_TIME_OPTIMIZATION_BIT_EXT"/> to be created with a subset of the total descriptor sets.
    /// </summary>
    /// <remarks>Provided by VK_EXT_graphics_pipeline_library</remarks>
    VK_PIPELINE_LAYOUT_CREATE_INDEPENDENT_SETS_BIT_EXT = 0x00000002,
}
