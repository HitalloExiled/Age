
namespace Age.Vulkan.Flags;

/// <summary>
/// Bitmask specifying pipeline stages.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
[Flags]
public enum VkPipelineStageFlagBits
{
    /// <summary>
    /// Is equivalent to <see cref="VK_PIPELINE_STAGE_ALL_COMMANDS_BIT"/> with <see cref="VkAccessFlags"/> set to 0 when specified in the second synchronization scope, but specifies no stage of execution when specified in the first scope.
    /// </summary>
    VK_PIPELINE_STAGE_TOP_OF_PIPE_BIT = 0x00000001,

    /// <summary>
    /// Specifies the stage of the pipeline where <see cref="VkDrawIndirect"/>* / <see cref="VkDispatchIndirect"/>* / <see cref="VkTraceRaysIndirect"/>* data structures are consumed. This stage also includes reading commands written by <see cref="VkNvDeviceGeneratedCommands.CmdExecuteGeneratedCommands"/>.
    /// </summary>
    VK_PIPELINE_STAGE_DRAW_INDIRECT_BIT = 0x00000002,

    /// <summary>
    /// Specifies the stage of the pipeline where vertex and index buffers are consumed.
    /// </summary>
    VK_PIPELINE_STAGE_VERTEX_INPUT_BIT = 0x00000004,

    /// <summary>
    /// Specifies the vertex shader stage.
    /// </summary>
    VK_PIPELINE_STAGE_VERTEX_SHADER_BIT = 0x00000008,

    /// <summary>
    /// Specifies the tessellation control shader stage.
    /// </summary>
    VK_PIPELINE_STAGE_TESSELLATION_CONTROL_SHADER_BIT = 0x00000010,

    /// <summary>
    /// Specifies the tessellation evaluation shader stage.
    /// </summary>
    VK_PIPELINE_STAGE_TESSELLATION_EVALUATION_SHADER_BIT = 0x00000020,

    /// <summary>
    /// Specifies the geometry shader stage.
    /// </summary>
    VK_PIPELINE_STAGE_GEOMETRY_SHADER_BIT = 0x00000040,

    /// <summary>
    /// Specifies the fragment shader stage.
    /// </summary>
    VK_PIPELINE_STAGE_FRAGMENT_SHADER_BIT = 0x00000080,

    /// <summary>
    /// Specifies the stage of the pipeline where early fragment tests (depth and stencil tests before fragment shading) are performed. This stage also includes render pass load operations for framebuffer attachments with a depth/stencil format.
    /// </summary>
    VK_PIPELINE_STAGE_EARLY_FRAGMENT_TESTS_BIT = 0x00000100,

    /// <summary>
    /// Specifies the stage of the pipeline where late fragment tests (depth and stencil tests after fragment shading) are performed. This stage also includes render pass store operations for framebuffer attachments with a depth/stencil format.
    /// </summary>
    VK_PIPELINE_STAGE_LATE_FRAGMENT_TESTS_BIT = 0x00000200,

    /// <summary>
    /// Specifies the stage of the pipeline after blending where the final color values are output from the pipeline. This stage includes blending, logic operations, render pass load and store operations for color attachments, render pass multisample resolve operations, and <see cref="Vk.CmdClearAttachments"/>.
    /// </summary>
    VK_PIPELINE_STAGE_COLOR_ATTACHMENT_OUTPUT_BIT = 0x00000400,

    /// <summary>
    /// Specifies the execution of a compute shader.
    /// </summary>
    VK_PIPELINE_STAGE_COMPUTE_SHADER_BIT = 0x00000800,

    /// <summary>
    /// <para>Specifies the execution of all graphics pipeline stages, and is equivalent to the logical OR of:</para>
    /// <list type="bullet">
    /// <item><see cref="VK_PIPELINE_STAGE_DRAW_INDIRECT_BIT"/></item>
    /// <item><see cref="VK_PIPELINE_STAGE_TASK_SHADER_BIT_EXT"/></item>
    /// <item><see cref="VK_PIPELINE_STAGE_MESH_SHADER_BIT_EXT"/></item>
    /// <item><see cref="VK_PIPELINE_STAGE_VERTEX_INPUT_BIT"/></item>
    /// <item><see cref="VK_PIPELINE_STAGE_VERTEX_SHADER_BIT"/></item>
    /// <item><see cref="VK_PIPELINE_STAGE_TESSELLATION_CONTROL_SHADER_BIT"/></item>
    /// <item><see cref="VK_PIPELINE_STAGE_TESSELLATION_EVALUATION_SHADER_BIT"/></item>
    /// <item><see cref="VK_PIPELINE_STAGE_GEOMETRY_SHADER_BIT"/></item>
    /// <item><see cref="VK_PIPELINE_STAGE_FRAGMENT_SHADER_BIT"/></item>
    /// <item><see cref="VK_PIPELINE_STAGE_EARLY_FRAGMENT_TESTS_BIT"/></item>
    /// <item><see cref="VK_PIPELINE_STAGE_LATE_FRAGMENT_TESTS_BIT"/></item>
    /// <item><see cref="VK_PIPELINE_STAGE_COLOR_ATTACHMENT_OUTPUT_BIT"/></item>
    /// <item><see cref="VK_PIPELINE_STAGE_CONDITIONAL_RENDERING_BIT_EXT"/></item>
    /// <item><see cref="VK_PIPELINE_STAGE_TRANSFORM_FEEDBACK_BIT_EXT"/></item>
    /// <item><see cref="VK_PIPELINE_STAGE_FRAGMENT_SHADING_RATE_ATTACHMENT_BIT_KHR"/></item>
    /// <item><see cref="VK_PIPELINE_STAGE_FRAGMENT_DENSITY_PROCESS_BIT_EXT"/></item>
    /// </list>
    /// </summary>
    VK_PIPELINE_STAGE_TRANSFER_BIT = 0x00001000,

    /// <summary>
    /// Is equivalent to <see cref="VK_PIPELINE_STAGE_ALL_COMMANDS_BIT"/> with <see cref="VkAccessFlags"/> set to 0 when specified in the first synchronization scope, but specifies no stage of execution when specified in the second scope.
    /// </summary>
    VK_PIPELINE_STAGE_BOTTOM_OF_PIPE_BIT = 0x00002000,

    /// <summary>
    /// Specifies a pseudo-stage indicating execution on the host of reads/writes of device memory. This stage is not invoked by any commands recorded in a command buffer.
    /// </summary>
    VK_PIPELINE_STAGE_HOST_BIT = 0x00004000,

    /// <summary>
    /// <para>Specifies the execution of all graphics pipeline stages, and is equivalent to the logical OR of:</para>
    /// <list type="bullet">
    /// <item><see cref="VK_PIPELINE_STAGE_DRAW_INDIRECT_BIT"/></item>
    /// <item><see cref="VK_PIPELINE_STAGE_TASK_SHADER_BIT_EXT"/></item>
    /// <item><see cref="VK_PIPELINE_STAGE_MESH_SHADER_BIT_EXT"/></item>
    /// <item><see cref="VK_PIPELINE_STAGE_VERTEX_INPUT_BIT"/></item>
    /// <item><see cref="VK_PIPELINE_STAGE_VERTEX_SHADER_BIT"/></item>
    /// <item><see cref="VK_PIPELINE_STAGE_TESSELLATION_CONTROL_SHADER_BIT"/></item>
    /// <item><see cref="VK_PIPELINE_STAGE_TESSELLATION_EVALUATION_SHADER_BIT"/></item>
    /// <item><see cref="VK_PIPELINE_STAGE_GEOMETRY_SHADER_BIT"/></item>
    /// <item><see cref="VK_PIPELINE_STAGE_FRAGMENT_SHADER_BIT"/></item>
    /// <item><see cref="VK_PIPELINE_STAGE_EARLY_FRAGMENT_TESTS_BIT"/></item>
    /// <item><see cref="VK_PIPELINE_STAGE_LATE_FRAGMENT_TESTS_BIT"/></item>
    /// <item><see cref="VK_PIPELINE_STAGE_COLOR_ATTACHMENT_OUTPUT_BIT"/></item>
    /// <item><see cref="VK_PIPELINE_STAGE_CONDITIONAL_RENDERING_BIT_EXT"/></item>
    /// <item><see cref="VK_PIPELINE_STAGE_TRANSFORM_FEEDBACK_BIT_EXT"/></item>
    /// <item><see cref="VK_PIPELINE_STAGE_FRAGMENT_SHADING_RATE_ATTACHMENT_BIT_KHR"/></item>
    /// <item><see cref="VK_PIPELINE_STAGE_FRAGMENT_DENSITY_PROCESS_BIT_EXT"/></item>
    /// </list>
    /// </summary>
    VK_PIPELINE_STAGE_ALL_GRAPHICS_BIT = 0x00008000,

    /// <summary>
    /// Specifies all operations performed by all commands supported on the queue it is used with.
    /// </summary>
    VK_PIPELINE_STAGE_ALL_COMMANDS_BIT = 0x00010000,

    /// <summary>
    /// Specifies no stages of execution.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_PIPELINE_STAGE_NONE = 0,

    /// <summary>
    /// Specifies the stage of the pipeline where vertex attribute output values are written to the transform feedback buffers.
    /// </summary>
    /// <remarks>Provided by VK_EXT_transform_feedback</remarks>
    VK_PIPELINE_STAGE_TRANSFORM_FEEDBACK_BIT_EXT = 0x01000000,

    /// <summary>
    /// Specifies the stage of the pipeline where the predicate of conditional rendering is consumed.
    /// </summary>
    /// <remarks>Provided by VK_EXT_conditional_rendering</remarks>
    VK_PIPELINE_STAGE_CONDITIONAL_RENDERING_BIT_EXT = 0x00040000,

    /// <summary>
    /// Specifies the execution of <see cref="VkNvRayTracing.CmdBuildAccelerationStructure"/>, <see cref="VkNvRayTracing.CmdCopyAccelerationStructure"/>, <see cref="VkNvRayTracing.CmdWriteAccelerationStructuresProperties"/> , <see cref="VkKhrAccelerationStructure.CmdBuildAccelerationStructures"/>, <see cref="vkCmdBuildAccelerationStructuresIndirectKHR"/>, <see cref="vkCmdCopyAccelerationStructureKHR"/>, <see cref="vkCmdCopyAccelerationStructureToMemoryKHR"/>, <see cref="vkCmdCopyMemoryToAccelerationStructureKHR"/>, and <see cref="vkCmdWriteAccelerationStructuresPropertiesKHR"/>.
    /// </summary>
    /// <remarks>Provided by VK_KHR_acceleration_structure</remarks>
    VK_PIPELINE_STAGE_ACCELERATION_STRUCTURE_BUILD_BIT_KHR = 0x02000000,

    /// <summary>
    /// Specifies the execution of the ray tracing shader stages, via <see cref="VkNvRayTracing.CmdTraceRays"/>, <see cref="VkKhrRayTracingPipeline.CmdTraceRays"/>, or <see cref="VkKhrRayTracingPipeline.CmdTraceRaysIndirect"/>
    /// </summary>
    /// <remarks>Provided by VK_KHR_ray_tracing_pipeline</remarks>
    VK_PIPELINE_STAGE_RAY_TRACING_SHADER_BIT_KHR = 0x00200000,

    /// <summary>
    /// Specifies the stage of the pipeline where the fragment density map is read to generate the fragment areas.
    /// </summary>
    /// <remarks>Provided by VK_EXT_fragment_density_map</remarks>
    VK_PIPELINE_STAGE_FRAGMENT_DENSITY_PROCESS_BIT_EXT = 0x00800000,

    /// <summary>
    /// Specifies the stage of the pipeline where the fragment shading rate attachment or shading rate image is read to determine the fragment shading rate for portions of a rasterized primitive.
    /// </summary>
    /// <remarks>Provided by VK_KHR_fragment_shading_rate</remarks>
    VK_PIPELINE_STAGE_FRAGMENT_SHADING_RATE_ATTACHMENT_BIT_KHR = 0x00400000,

    /// <summary>
    /// Specifies the stage of the pipeline where device-side preprocessing for generated commands via <see cref="VkNvDeviceGeneratedCommands.CmdPreprocessGeneratedCommands"/> is handled.
    /// </summary>
    /// <remarks>Provided by VK_NV_device_generated_commands</remarks>
    VK_PIPELINE_STAGE_COMMAND_PREPROCESS_BIT_NV = 0x00020000,

    /// <summary>
    /// Specifies the task shader stage.
    /// </summary>
    /// <remarks>Provided by VK_EXT_mesh_shader</remarks>
    VK_PIPELINE_STAGE_TASK_SHADER_BIT_EXT = 0x00080000,

    /// <summary>
    /// Specifies the mesh shader stage.
    /// </summary>
    /// <remarks>Provided by VK_EXT_mesh_shader</remarks>
    VK_PIPELINE_STAGE_MESH_SHADER_BIT_EXT = 0x00100000,

    /// <inheritdoc cref="VK_PIPELINE_STAGE_FRAGMENT_SHADING_RATE_ATTACHMENT_BIT_KHR" />
    /// <remarks>Provided by VK_NV_shading_rate_image</remarks>
    VK_PIPELINE_STAGE_SHADING_RATE_IMAGE_BIT_NV = VK_PIPELINE_STAGE_FRAGMENT_SHADING_RATE_ATTACHMENT_BIT_KHR,

    /// <inheritdoc cref="VK_PIPELINE_STAGE_RAY_TRACING_SHADER_BIT_KHR" />
    /// <remarks>Provided by VK_NV_ray_tracing</remarks>
    VK_PIPELINE_STAGE_RAY_TRACING_SHADER_BIT_NV = VK_PIPELINE_STAGE_RAY_TRACING_SHADER_BIT_KHR,

    /// <inheritdoc cref="VK_PIPELINE_STAGE_ACCELERATION_STRUCTURE_BUILD_BIT_KHR" />
    /// <remarks>Provided by VK_NV_ray_tracing</remarks>
    VK_PIPELINE_STAGE_ACCELERATION_STRUCTURE_BUILD_BIT_NV = VK_PIPELINE_STAGE_ACCELERATION_STRUCTURE_BUILD_BIT_KHR,

    /// <inheritdoc cref="VK_PIPELINE_STAGE_TASK_SHADER_BIT_EXT" />
    /// <remarks>Provided by VK_NV_mesh_shader</remarks>
    VK_PIPELINE_STAGE_TASK_SHADER_BIT_NV = VK_PIPELINE_STAGE_TASK_SHADER_BIT_EXT,

    /// <inheritdoc cref="VK_PIPELINE_STAGE_MESH_SHADER_BIT_EXT" />
    /// <remarks>Provided by VK_NV_mesh_shader</remarks>
    VK_PIPELINE_STAGE_MESH_SHADER_BIT_NV = VK_PIPELINE_STAGE_MESH_SHADER_BIT_EXT,

    /// <inheritdoc cref="VK_PIPELINE_STAGE_NONE" />
    /// <remarks>Provided by VK_KHR_synchronization2</remarks>
    VK_PIPELINE_STAGE_NONE_KHR = VK_PIPELINE_STAGE_NONE,
}
