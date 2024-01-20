namespace Age.Vulkan.Flags;

/// <summary>
/// Bitmask specifying allowed usage of a buffer.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
[Flags]
public enum VkBufferUsageFlagBits
{
    /// <summary>
    /// Specifies that the buffer can be used as the source of a transfer command (see the definition of <see cref="VkPipelineStageFlagBits.VK_PIPELINE_STAGE_TRANSFER_BIT"/>).
    /// </summary>
    VK_BUFFER_USAGE_TRANSFER_SRC_BIT = 0x00000001,

    /// <summary>
    /// Specifies that the buffer can be used as the destination of a transfer command.
    /// </summary>
    VK_BUFFER_USAGE_TRANSFER_DST_BIT = 0x00000002,

    /// <summary>
    /// Specifies that the buffer can be used to create a <see cref="VkBufferView"/> suitable for occupying a <see cref="VkDescriptorSet"/> slot of type <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_UNIFORM_TEXEL_BUFFER"/>.
    /// </summary>
    VK_BUFFER_USAGE_UNIFORM_TEXEL_BUFFER_BIT = 0x00000004,

    /// <summary>
    /// Specifies that the buffer can be used to create a <see cref="VkBufferView"/> suitable for occupying a <see cref="VkDescriptorSet"/> slot of type <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_STORAGE_TEXEL_BUFFER"/>.
    /// </summary>
    VK_BUFFER_USAGE_STORAGE_TEXEL_BUFFER_BIT = 0x00000008,

    /// <summary>
    /// Specifies that the buffer can be used in a <see cref="VkDescriptorBufferInfo"/> suitable for occupying a <see cref="VkDescriptorSet"/> slot either of type <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER"/> or <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER_DYNAMIC"/>.
    /// </summary>
    VK_BUFFER_USAGE_UNIFORM_BUFFER_BIT = 0x00000010,

    /// <summary>
    /// Specifies that the buffer can be used in a <see cref="VkDescriptorBufferInfo"/> suitable for occupying a <see cref="VkDescriptorSet"/> slot either of type <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_STORAGE_BUFFER"/> or <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_STORAGE_BUFFER_DYNAMIC"/>.
    /// </summary>
    VK_BUFFER_USAGE_STORAGE_BUFFER_BIT = 0x00000020,

    /// <summary>
    /// Specifies that the buffer is suitable for passing as the buffer parameter to <see cref="VkKhrMaintenance5.CmdBindIndexBuffer2"/> and <see cref="Vk.CmdBindIndexBuffer"/>.
    /// </summary>
    VK_BUFFER_USAGE_INDEX_BUFFER_BIT = 0x00000040,

    /// <summary>
    /// Specifies that the buffer is suitable for passing as an element of the pBuffers array to <see cref="Vk.CmdBindVertexBuffers"/>.
    /// </summary>
    VK_BUFFER_USAGE_VERTEX_BUFFER_BIT = 0x00000080,

    /// <summary>
    /// Specifies that the buffer is suitable for passing as the buffer parameter to <see cref="Vk.CmdDrawIndirect"/>, <see cref="Vk.CmdDrawIndexedIndirect"/>, <see cref="VkNvMeshShader.CmdDrawMeshTasksIndirect"/>, <see cref="VkNvMeshShader.CmdDrawMeshTasksIndirectCount"/>, <see cref="VkExtMeshShader.CmdDrawMeshTasksIndirect"/>, <see cref="VkExtMeshShader.CmdDrawMeshTasksIndirectCount"/>, <see cref="VkHuaweiClusterCullingShader.CmdDrawClusterIndirect"/>, or <see cref="Vk.CmdDispatchIndirect"/>. It is also suitable for passing as the buffer member of <see cref="VkIndirectCommandsStreamNV"/>, or sequencesCountBuffer or sequencesIndexBuffer or preprocessedBuffer member of <see cref="VkGeneratedCommandsInfoNV"/>
    /// </summary>
    VK_BUFFER_USAGE_INDIRECT_BUFFER_BIT = 0x00000100,

    /// <summary>
    /// Specifies that the buffer can be used to retrieve a buffer device address via <see cref="Vk.GetBufferDeviceAddress"/> and use that address to access the buffer’s memory from a shader.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_BUFFER_USAGE_SHADER_DEVICE_ADDRESS_BIT = 0x00020000,

    /// <summary>
    /// Specifies that the buffer can be used as the source video bitstream buffer in a video decode operation.
    /// </summary>
    /// <remarks>Provided by VK_KHR_video_decode_queue</remarks>
    VK_BUFFER_USAGE_VIDEO_DECODE_SRC_BIT_KHR = 0x00002000,

    /// <summary>
    /// Is reserved for future use.
    /// </summary>
    /// <remarks>Provided by VK_KHR_video_decode_queue</remarks>
    VK_BUFFER_USAGE_VIDEO_DECODE_DST_BIT_KHR = 0x00004000,

    /// <summary>
    /// Specifies that the buffer is suitable for using for binding as a transform feedback buffer with <see cref="VkExtTransformFeedback.CmdBindTransformFeedbackBuffers"/>.
    /// </summary>
    /// <remarks>Provided by VK_EXT_transform_feedback</remarks>
    VK_BUFFER_USAGE_TRANSFORM_FEEDBACK_BUFFER_BIT_EXT = 0x00000800,

    /// <summary>
    /// Specifies that the buffer is suitable for using as a counter buffer with <see cref="VkExtTransformFeedback.CmdBeginTransformFeedback"/> and <see cref="VkExtTransformFeedback.CmdEndTransformFeedback"/>.
    /// </summary>
    /// <remarks>Provided by VK_EXT_transform_feedback</remarks>
    VK_BUFFER_USAGE_TRANSFORM_FEEDBACK_COUNTER_BUFFER_BIT_EXT = 0x00001000,

    /// <summary>
    /// Specifies that the buffer is suitable for passing as the buffer parameter to <see cref="VkExtConditionalRendering.CmdBeginConditionalRendering"/>.
    /// </summary>
    /// <remarks>Provided by VK_EXT_conditional_rendering</remarks>
    VK_BUFFER_USAGE_CONDITIONAL_RENDERING_BIT_EXT = 0x00000200,
#if VK_ENABLE_BETA_EXTENSIONS

    /// <summary>
    /// Specifies that the buffer can be used for as scratch memory for execution graph dispatch.
    /// </summary>
    /// <remarks>Provided by VK_AMDX_shader_enqueue</remarks>
    VK_BUFFER_USAGE_EXECUTION_GRAPH_SCRATCH_BIT_AMDX = 0x02000000,
#endif

    /// <summary>
    /// Specifies that the buffer is suitable for use as a read-only input to an acceleration structure build.
    /// </summary>
    /// <remarks>Provided by VK_KHR_acceleration_structure</remarks>
    VK_BUFFER_USAGE_ACCELERATION_STRUCTURE_BUILD_INPUT_READ_ONLY_BIT_KHR = 0x00080000,

    /// <summary>
    /// Specifies that the buffer is suitable for storage space for a <see cref="VkAccelerationStructureKHR"/>.
    /// </summary>
    /// <remarks>Provided by VK_KHR_acceleration_structure</remarks>
    VK_BUFFER_USAGE_ACCELERATION_STRUCTURE_STORAGE_BIT_KHR = 0x00100000,

    /// <summary>
    /// Specifies that the buffer is suitable for use as a Shader Binding Table.
    /// </summary>
    /// <remarks>Provided by VK_KHR_ray_tracing_pipeline</remarks>
    VK_BUFFER_USAGE_SHADER_BINDING_TABLE_BIT_KHR = 0x00000400,
#if VK_ENABLE_BETA_EXTENSIONS

    /// <summary>
    /// Specifies that the buffer can be used as the destination video bitstream buffer in a video encode operation.
    /// </summary>
    /// <remarks>Provided by VK_KHR_video_encode_queue</remarks>
    VK_BUFFER_USAGE_VIDEO_ENCODE_DST_BIT_KHR = 0x00008000,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <summary>
    /// Is reserved for future use.
    /// </summary>
    /// <remarks>Provided by VK_KHR_video_encode_queue</remarks>
    VK_BUFFER_USAGE_VIDEO_ENCODE_SRC_BIT_KHR = 0x00010000,
#endif

    /// <summary>
    /// Specifies that the buffer is suitable to contain sampler and combined image sampler descriptors when bound as a descriptor buffer. Buffers containing combined image sampler descriptors must also specify <see cref="VK_BUFFER_USAGE_RESOURCE_DESCRIPTOR_BUFFER_BIT_EXT"/>.
    /// </summary>
    /// <remarks>Provided by VK_EXT_descriptor_buffer</remarks>
    VK_BUFFER_USAGE_SAMPLER_DESCRIPTOR_BUFFER_BIT_EXT = 0x00200000,

    /// <summary>
    /// Specifies that the buffer is suitable to contain resource descriptors when bound as a descriptor buffer.
    /// </summary>
    /// <remarks>Provided by VK_EXT_descriptor_buffer</remarks>
    VK_BUFFER_USAGE_RESOURCE_DESCRIPTOR_BUFFER_BIT_EXT = 0x00400000,

    /// <summary>
    /// Specifies that the buffer, when bound, can be used by the implementation to support push descriptors when using descriptor buffers.    ///
    /// </summary>
    /// <remarks>Provided by VK_EXT_descriptor_buffer</remarks>
    VK_BUFFER_USAGE_PUSH_DESCRIPTORS_DESCRIPTOR_BUFFER_BIT_EXT = 0x04000000,

    /// <remarks>Provided by VK_EXT_opacity_micromap</remarks>
    VK_BUFFER_USAGE_MICROMAP_BUILD_INPUT_READ_ONLY_BIT_EXT = 0x00800000,

    /// <remarks>Provided by VK_EXT_opacity_micromap</remarks>
    VK_BUFFER_USAGE_MICROMAP_STORAGE_BIT_EXT = 0x01000000,

    /// <summary>
    /// Specifies that the buffer is suitable for use in <see cref="VkNvRayTracing.CmdTraceRays"/>.
    /// </summary>
    /// <remarks>Provided by VK_NV_ray_tracing</remarks>
    VK_BUFFER_USAGE_RAY_TRACING_BIT_NV = VK_BUFFER_USAGE_SHADER_BINDING_TABLE_BIT_KHR,

    /// <inheritdoc cref="VK_BUFFER_USAGE_SHADER_DEVICE_ADDRESS_BIT" />
    /// <remarks>Provided by VK_EXT_buffer_device_address</remarks>
    VK_BUFFER_USAGE_SHADER_DEVICE_ADDRESS_BIT_EXT = VK_BUFFER_USAGE_SHADER_DEVICE_ADDRESS_BIT,

    /// <inheritdoc cref="VK_BUFFER_USAGE_SHADER_DEVICE_ADDRESS_BIT" />
    /// <remarks>Provided by VK_KHR_buffer_device_address</remarks>
    VK_BUFFER_USAGE_SHADER_DEVICE_ADDRESS_BIT_KHR = VK_BUFFER_USAGE_SHADER_DEVICE_ADDRESS_BIT,
}
