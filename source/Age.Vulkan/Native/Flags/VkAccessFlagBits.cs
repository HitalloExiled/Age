namespace Age.Vulkan.Native.Flags;

/// <summary>
/// <para>Bitmask specifying memory access types that will participate in a memory dependency</para>
/// <para>Certain access types are only performed by a subset of pipeline stages. Any synchronization command that takes both stage masks and access masks uses both to define the access scopes - only the specified access types performed by the specified stages are included in the access scope. An application must not specify an access flag in a synchronization command if it does not include a pipeline stage in the corresponding stage mask that is able to perform accesses of that type. The following table lists, for each access flag, which pipeline stages can perform that type of access.</para>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
[Flags]
public enum VkAccessFlagBits
{
    /// <summary>
    /// Specifies read access to indirect command data read as part of an indirect build, trace, drawing or dispatching command. Such access occurs in the <see cref="VkPipelineStageFlagBits.VK_PIPELINE_STAGE_DRAW_INDIRECT_BIT"/> pipeline stage.
    /// </summary>
    VK_ACCESS_INDIRECT_COMMAND_READ_BIT = 0x00000001,

    /// <summary>
    /// Specifies read access to an index buffer as part of an indexed drawing command, bound by <see cref="VkKhrMaintenance5.CmdBindIndexBuffer2"/> and <see cref="Vk.CmdBindIndexBuffer"/>. Such access occurs in the <see cref="VkPipelineStageFlagBits.VK_PIPELINE_STAGE_VERTEX_INPUT_BIT"/> pipeline stage.
    /// </summary>
    VK_ACCESS_INDEX_READ_BIT = 0x00000002,

    /// <summary>
    /// Specifies read access to a vertex buffer as part of a drawing command, bound by <see cref="Vk.CmdBindVertexBuffers"/>. Such access occurs in the <see cref="VkPipelineStageFlagBits.VK_PIPELINE_STAGE_VERTEX_INPUT_BIT"/> pipeline stage.
    /// </summary>
    VK_ACCESS_VERTEX_ATTRIBUTE_READ_BIT = 0x00000004,

    /// <summary>
    /// Specifies read access to a uniform buffer in any shader pipeline stage.
    /// </summary>
    VK_ACCESS_UNIFORM_READ_BIT = 0x00000008,

    /// <summary>
    /// Specifies read access to an input attachment within a render pass during subpass shading or fragment shading. Such access occurs in the <see cref="VkPipelineStageFlagBits.VK_PIPELINE_STAGE_2_SUBPASS_SHADER_BIT_HUAWEI"/> or <see cref="VkPipelineStageFlagBits.VK_PIPELINE_STAGE_FRAGMENT_SHADER_BIT"/> pipeline stage.
    /// </summary>
    VK_ACCESS_INPUT_ATTACHMENT_READ_BIT = 0x00000010,

    /// <summary>
    /// Specifies read access to a uniform texel buffer, sampled image, storage buffer, physical storage buffer, shader binding table, storage texel buffer, or storage image in any shader pipeline stage.
    /// </summary>
    VK_ACCESS_SHADER_READ_BIT = 0x00000020,

    /// <summary>
    /// Specifies write access to a storage buffer, physical storage buffer, storage texel buffer, or storage image in any shader pipeline stage.
    /// </summary>
    VK_ACCESS_SHADER_WRITE_BIT = 0x00000040,

    /// <summary>
    /// Specifies read access to a color attachment, such as via blending (other than advanced blend operations), logic operations or certain render pass load operations in the <see cref="VkPipelineStageFlagBits.VK_PIPELINE_STAGE_COLOR_ATTACHMENT_OUTPUT_BIT"/> pipeline stage or via fragment shader tile image reads in the <see cref="VkPipelineStageFlagBits.VK_PIPELINE_STAGE_FRAGMENT_SHADER_BIT"/> pipeline stage.
    /// </summary>
    VK_ACCESS_COLOR_ATTACHMENT_READ_BIT = 0x00000080,

    /// <summary>
    /// Specifies write access to a color, resolve, or depth/stencil resolve attachment during a render pass or via certain render pass load and store operations. Such access occurs in the <see cref="VkPipelineStageFlagBits.VK_PIPELINE_STAGE_COLOR_ATTACHMENT_OUTPUT_BIT"/> pipeline stage.
    /// </summary>
    VK_ACCESS_COLOR_ATTACHMENT_WRITE_BIT = 0x00000100,

    /// <summary>
    /// Specifies read access to a depth/stencil attachment, via depth or stencil operations or certain render pass load operations in the <see cref="VkPipelineStageFlagBits.VK_PIPELINE_STAGE_EARLY_FRAGMENT_TESTS_BIT"/> or <see cref="VkPipelineStageFlagBits.VK_PIPELINE_STAGE_LATE_FRAGMENT_TESTS_BIT"/> pipeline stages or via fragment shader tile image reads in the <see cref="VkPipelineStageFlagBits.VK_PIPELINE_STAGE_FRAGMENT_SHADER_BIT"/> pipeline stage.
    /// </summary>
    VK_ACCESS_DEPTH_STENCIL_ATTACHMENT_READ_BIT = 0x00000200,

    /// <summary>
    /// Specifies write access to a depth/stencil attachment, via depth or stencil operations or certain render pass load and store operations. Such access occurs in the <see cref="VkPipelineStageFlagBits.VK_PIPELINE_STAGE_EARLY_FRAGMENT_TESTS_BIT"/> or <see cref="VkPipelineStageFlagBits.VK_PIPELINE_STAGE_LATE_FRAGMENT_TESTS_BIT"/> pipeline stages.
    /// </summary>
    VK_ACCESS_DEPTH_STENCIL_ATTACHMENT_WRITE_BIT = 0x00000400,

    /// <summary>
    /// Specifies read access to an image or buffer in a copy operation. Such access occurs in the <see cref="VkPipelineStageFlagBits.VK_PIPELINE_STAGE_2_ALL_TRANSFER_BIT"/> pipeline stage.
    /// </summary>
    VK_ACCESS_TRANSFER_READ_BIT = 0x00000800,

    /// <summary>
    /// Specifies write access to an image or buffer in a clear or copy operation. Such access occurs in the <see cref="VkPipelineStageFlagBits.VK_PIPELINE_STAGE_2_ALL_TRANSFER_BIT"/> pipeline stage.
    /// </summary>
    VK_ACCESS_TRANSFER_WRITE_BIT = 0x00001000,

    /// <summary>
    /// Specifies read access by a host operation. Accesses of this type are not performed through a resource, but directly on memory. Such access occurs in the <see cref="VkPipelineStageFlagBits.VK_PIPELINE_STAGE_HOST_BIT"/> pipeline stage.
    /// </summary>
    VK_ACCESS_HOST_READ_BIT = 0x00002000,

    /// <summary>
    /// Specifies write access by a host operation. Accesses of this type are not performed through a resource, but directly on memory. Such access occurs in the <see cref="VkPipelineStageFlagBits.VK_PIPELINE_STAGE_HOST_BIT"/> pipeline stage.
    /// </summary>
    VK_ACCESS_HOST_WRITE_BIT = 0x00004000,

    /// <summary>
    /// Specifies all read accesses. It is always valid in any access mask, and is treated as equivalent to setting all READ access flags that are valid where it is used.
    /// </summary>
    VK_ACCESS_MEMORY_READ_BIT = 0x00008000,

    /// <summary>
    /// Specifies all write accesses. It is always valid in any access mask, and is treated as equivalent to setting all WRITE access flags that are valid where it is used.
    /// </summary>
    VK_ACCESS_MEMORY_WRITE_BIT = 0x00010000,

    /// <summary>
    /// Specifies no accesses.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_ACCESS_NONE = 0,

    /// <summary>
    /// Specifies write access to a transform feedback buffer made when transform feedback is active. Such access occurs in the <see cref="VkPipelineStageFlagBits.VK_PIPELINE_STAGE_TRANSFORM_FEEDBACK_BIT_EXT"/> pipeline stage.
    /// </summary>
    /// <remarks>Provided by VK_EXT_transform_feedback</remarks>
    VK_ACCESS_TRANSFORM_FEEDBACK_WRITE_BIT_EXT = 0x02000000,

    /// <summary>
    /// Specifies read access to a transform feedback counter buffer which is read when <see cref="VkExtTransformFeedback.vkCmdBeginTransformFeedback"/> executes. Such access occurs in the <see cref="VkPipelineStageFlagBits.VK_PIPELINE_STAGE_TRANSFORM_FEEDBACK_BIT_EXT"/> pipeline stage.
    /// </summary>
    /// <remarks>Provided by VK_EXT_transform_feedback</remarks>
    VK_ACCESS_TRANSFORM_FEEDBACK_COUNTER_READ_BIT_EXT = 0x04000000,

    /// <summary>
    /// Specifies write access to a transform feedback counter buffer which is written when <see cref="VK_EXT_transform_feedback.CmdEndTransformFeedback"/> executes. Such access occurs in the <see cref="VkPipelineStageFlagBits.VK_PIPELINE_STAGE_TRANSFORM_FEEDBACK_BIT_EXT"/> pipeline stage.
    /// </summary>
    /// <remarks>Provided by VK_EXT_transform_feedback</remarks>
    VK_ACCESS_TRANSFORM_FEEDBACK_COUNTER_WRITE_BIT_EXT = 0x08000000,

    /// <summary>
    /// Specifies read access to a predicate as part of conditional rendering. Such access occurs in the <see cref="VkPipelineStageFlagBits.VK_PIPELINE_STAGE_CONDITIONAL_RENDERING_BIT_EXT"/> pipeline stage.
    /// </summary>
    /// <remarks>Provided by VK_EXT_conditional_rendering</remarks>
    VK_ACCESS_CONDITIONAL_RENDERING_READ_BIT_EXT = 0x00100000,

    /// <summary>
    /// Specifies read access to color attachments, including advanced blend operations. Such access occurs in the <see cref="VkPipelineStageFlagBits.VK_PIPELINE_STAGE_COLOR_ATTACHMENT_OUTPUT_BIT"/> pipeline stage.
    /// </summary>
    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_ACCESS_COLOR_ATTACHMENT_READ_NONCOHERENT_BIT_EXT = 0x00080000,

    /// <summary>
    /// Specifies read access to an acceleration structure as part of a trace, build, or copy command, or to an acceleration structure scratch buffer as part of a build command. Such access occurs in the <see cref="VkPipelineStageFlagBits.VK_PIPELINE_STAGE_RAY_TRACING_SHADER_BIT_KHR"/> pipeline stage or <see cref="VkPipelineStageFlagBits.VK_PIPELINE_STAGE_ACCELERATION_STRUCTURE_BUILD_BIT_KHR"/> pipeline stage.
    /// </summary>
    /// <remarks>Provided by VK_KHR_acceleration_structure</remarks>
    VK_ACCESS_ACCELERATION_STRUCTURE_READ_BIT_KHR = 0x00200000,

    /// <summary>
    /// Specifies write access to an acceleration structure or acceleration structure scratch buffer as part of a build or copy command. Such access occurs in the <see cref="VkPipelineStageFlagBits.VK_PIPELINE_STAGE_ACCELERATION_STRUCTURE_BUILD_BIT_KHR"/> pipeline stage.
    /// </summary>
    /// <remarks>Provided by VK_KHR_acceleration_structure</remarks>
    VK_ACCESS_ACCELERATION_STRUCTURE_WRITE_BIT_KHR = 0x00400000,

    /// <summary>
    /// Specifies read access to a fragment density map attachment during dynamic fragment density map operations Such access occurs in the <see cref="VkPipelineStageFlagBits.VK_PIPELINE_STAGE_FRAGMENT_DENSITY_PROCESS_BIT_EXT"/> pipeline stage.
    /// </summary>
    /// <remarks>Provided by VK_EXT_fragment_density_map</remarks>
    VK_ACCESS_FRAGMENT_DENSITY_MAP_READ_BIT_EXT = 0x01000000,

    /// <summary>
    /// Specifies read access to a fragment shading rate attachment during rasterization. Such access occurs in the <see cref="VkPipelineStageFlagBits.VK_PIPELINE_STAGE_FRAGMENT_SHADING_RATE_ATTACHMENT_BIT_KHR"/> pipeline stage.
    /// </summary>
    /// <remarks>Provided by VK_KHR_fragment_shading_rate</remarks>
    VK_ACCESS_FRAGMENT_SHADING_RATE_ATTACHMENT_READ_BIT_KHR = 0x00800000,

    /// <summary>
    /// Specifies reads from buffer inputs to <see cref="VK_NV_device_generated_commands.CmdPreprocessGeneratedCommands"/>. Such access occurs in the <see cref="VkPipelineStageFlagBits.VK_PIPELINE_STAGE_COMMAND_PREPROCESS_BIT_NV"/> pipeline stage.
    /// </summary>
    /// <remarks>Provided by VK_NV_device_generated_commands</remarks>
    VK_ACCESS_COMMAND_PREPROCESS_READ_BIT_NV = 0x00020000,

    /// <summary>
    /// Specifies writes to the target command buffer preprocess outputs in <see cref="VK_NV_device_generated_commands.CmdPreprocessGeneratedCommands"/>. Such access occurs in the <see cref="VkPipelineStageFlagBits.VK_PIPELINE_STAGE_COMMAND_PREPROCESS_BIT_NV"/> pipeline stage.
    /// </summary>
    /// <remarks>Provided by VK_NV_device_generated_commands</remarks>
    VK_ACCESS_COMMAND_PREPROCESS_WRITE_BIT_NV = 0x00040000,

    /// <summary>
    /// Specifies read access to a shading rate image during rasterization. Such access occurs in the <see cref="VkPipelineStageFlagBits.VK_PIPELINE_STAGE_SHADING_RATE_IMAGE_BIT_NV"/> pipeline stage. It is equivalent to <see cref="VkPipelineStageFlagBits.VK_PIPELINE_STAGE_FRAGMENT_SHADING_RATE_ATTACHMENT_BIT_KHR"/>.
    /// </summary>
    /// <remarks>Provided by VK_NV_shading_rate_image</remarks>
    VK_ACCESS_SHADING_RATE_IMAGE_READ_BIT_NV = VK_ACCESS_FRAGMENT_SHADING_RATE_ATTACHMENT_READ_BIT_KHR,

    /// <inheritdoc cref="VK_ACCESS_ACCELERATION_STRUCTURE_READ_BIT_KHR" />
    /// <remarks>Provided by VK_NV_ray_tracing</remarks>
    VK_ACCESS_ACCELERATION_STRUCTURE_READ_BIT_NV = VK_ACCESS_ACCELERATION_STRUCTURE_READ_BIT_KHR,

    /// <inheritdoc cref="VK_ACCESS_ACCELERATION_STRUCTURE_WRITE_BIT_KHR" />
    /// <remarks>Provided by VK_NV_ray_tracing</remarks>
    VK_ACCESS_ACCELERATION_STRUCTURE_WRITE_BIT_NV = VK_ACCESS_ACCELERATION_STRUCTURE_WRITE_BIT_KHR,

    /// <inheritdoc cref="VK_ACCESS_NONE" />
    /// <remarks>Provided by VK_KHR_synchronization2</remarks>
    VK_ACCESS_NONE_KHR = VK_ACCESS_NONE,
}
