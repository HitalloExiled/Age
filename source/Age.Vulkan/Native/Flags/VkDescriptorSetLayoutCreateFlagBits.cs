namespace Age.Vulkan.Native.Flags;

/// <summary>
/// Bitmask specifying descriptor set layout properties.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
[Flags]
public enum VkDescriptorSetLayoutCreateFlagBits
{
    /// VK_DESCRIPTOR_SET_LAYOUT_CREATE_UPDATE_AFTER_BIND_POOL_BIT specifies that descriptor sets using this layout must be allocated from a descriptor pool created with the <see cref="VkDescriptorPoolCreateFlagBits.VK_DESCRIPTOR_POOL_CREATE_UPDATE_AFTER_BIND_BIT"/> bit set. Descriptor set layouts created with this bit set have alternate limits for the maximum number of descriptors per-stage and per-pipeline layout. The non-UpdateAfterBind limits only count descriptors in sets created without this flag. The UpdateAfterBind limits count all descriptors, but the limits may be higher than the non-UpdateAfterBind limits.
    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_DESCRIPTOR_SET_LAYOUT_CREATE_UPDATE_AFTER_BIND_POOL_BIT = 0x00000002,

    /// VK_DESCRIPTOR_SET_LAYOUT_CREATE_PUSH_DESCRIPTOR_BIT_KHR specifies that descriptor sets must not be allocated using this layout, and descriptors are instead pushed by <see cref="VkKhrPushDescriptor.CmdPushDescriptorSet"/>.
    /// <remarks>Provided by VK_KHR_push_descriptor</remarks>
    VK_DESCRIPTOR_SET_LAYOUT_CREATE_PUSH_DESCRIPTOR_BIT_KHR = 0x00000001,

    /// VK_DESCRIPTOR_SET_LAYOUT_CREATE_DESCRIPTOR_BUFFER_BIT_EXT specifies that this layout must only be used with descriptor buffers.
    /// <remarks>Provided by VK_EXT_descriptor_buffer</remarks>
    VK_DESCRIPTOR_SET_LAYOUT_CREATE_DESCRIPTOR_BUFFER_BIT_EXT = 0x00000010,

    /// VK_DESCRIPTOR_SET_LAYOUT_CREATE_EMBEDDED_IMMUTABLE_SAMPLERS_BIT_EXT specifies that this is a layout only containing immutable samplers that can be bound by <see cref="VkExtDescriptorBuffer.CmdBindDescriptorBufferEmbeddedSamplers"/>. Unlike normal immutable samplers, embedded immutable samplers do not require the application to provide them in a descriptor buffer.
    /// <remarks>Provided by VK_EXT_descriptor_buffer</remarks>
    VK_DESCRIPTOR_SET_LAYOUT_CREATE_EMBEDDED_IMMUTABLE_SAMPLERS_BIT_EXT = 0x00000020,

    /// VK_DESCRIPTOR_SET_LAYOUT_CREATE_INDIRECT_BINDABLE_BIT_NV specifies that descriptor sets using this layout allows them to be bound with compute pipelines that are created with <see cref="VkPipelineCreateFlagBits.VK_PIPELINE_CREATE_INDIRECT_BINDABLE_BIT_NV"/> flag set to be used in Device-Generated Commands.
    /// <remarks>Provided by VK_NV_device_generated_commands_compute</remarks>
    VK_DESCRIPTOR_SET_LAYOUT_CREATE_INDIRECT_BINDABLE_BIT_NV = 0x00000080,

    /// VK_DESCRIPTOR_SET_LAYOUT_CREATE_HOST_ONLY_POOL_BIT_EXT specifies that descriptor sets using this layout must be allocated from a descriptor pool created with the <see cref="VkDescriptorPoolCreateFlagBits.VK_DESCRIPTOR_POOL_CREATE_HOST_ONLY_BIT_EXT"/> bit set. Descriptor set layouts created with this bit have no expressible limit for maximum number of descriptors per-stage. Host descriptor sets are limited only by available host memory, but may be limited for implementation specific reasons. Implementations may limit the number of supported descriptors to UpdateAfterBind limits or non-UpdateAfterBind limits, whichever is larger.
    /// <remarks>Provided by VK_EXT_mutable_descriptor_type</remarks>
    VK_DESCRIPTOR_SET_LAYOUT_CREATE_HOST_ONLY_POOL_BIT_EXT = 0x00000004,

    /// <inheritdoc cref="VK_DESCRIPTOR_SET_LAYOUT_CREATE_UPDATE_AFTER_BIND_POOL_BIT" />
    /// <remarks>Provided by VK_EXT_descriptor_indexing</remarks>
    VK_DESCRIPTOR_SET_LAYOUT_CREATE_UPDATE_AFTER_BIND_POOL_BIT_EXT = VK_DESCRIPTOR_SET_LAYOUT_CREATE_UPDATE_AFTER_BIND_POOL_BIT,

    /// <inheritdoc cref="VK_DESCRIPTOR_SET_LAYOUT_CREATE_HOST_ONLY_POOL_BIT_EXT" />
    /// <remarks>Provided by VK_VALVE_mutable_descriptor_type</remarks>
    VK_DESCRIPTOR_SET_LAYOUT_CREATE_HOST_ONLY_POOL_BIT_VALVE = VK_DESCRIPTOR_SET_LAYOUT_CREATE_HOST_ONLY_POOL_BIT_EXT,
}
