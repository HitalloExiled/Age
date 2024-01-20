namespace Age.Vulkan.Flags;

/// <summary>
/// Bitmask specifying intended usage of an image.
/// </summary>
[Flags]
public enum VkImageUsageFlagBits
{
    /// <summary>
    /// Specifies that the image can be used as the source of a transfer command.
    /// </summary>
    VK_IMAGE_USAGE_TRANSFER_SRC_BIT = 0x00000001,

    /// <summary>
    /// Specifies that the image can be used as the destination of a transfer command.
    /// </summary>
    VK_IMAGE_USAGE_TRANSFER_DST_BIT = 0x00000002,

    /// <summary>
    /// Specifies that the image can be used to create a VkImageView suitable for occupying a VkDescriptorSet slot either of type VK_DESCRIPTOR_TYPE_SAMPLED_IMAGE or VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER, and be sampled by a shader.
    /// </summary>
    VK_IMAGE_USAGE_SAMPLED_BIT = 0x00000004,

    /// <summary>
    /// Specifies that the image can be used to create a VkImageView suitable for occupying a VkDescriptorSet slot of type VK_DESCRIPTOR_TYPE_STORAGE_IMAGE.
    /// </summary>
    VK_IMAGE_USAGE_STORAGE_BIT = 0x00000008,

    /// <summary>
    /// Specifies that the image can be used to create a VkImageView suitable for use as a color or resolve attachment in a VkFramebuffer.
    /// </summary>
    VK_IMAGE_USAGE_COLOR_ATTACHMENT_BIT = 0x00000010,

    /// <summary>
    /// Specifies that the image can be used to create a VkImageView suitable for use as a depth/stencil or depth/stencil resolve attachment in a VkFramebuffer.
    /// </summary>
    VK_IMAGE_USAGE_DEPTH_STENCIL_ATTACHMENT_BIT = 0x00000020,

    /// <summary>
    /// Specifies that implementations may support using memory allocations with the VK_MEMORY_PROPERTY_LAZILY_ALLOCATED_BIT to back an image with this usage. This bit can be set for any image that can be used to create a VkImageView suitable for use as a color, resolve, depth/stencil, or input attachment.
    /// </summary>
    VK_IMAGE_USAGE_TRANSIENT_ATTACHMENT_BIT = 0x00000040,

    /// <summary>
    /// Specifies that the image can be used to create a VkImageView suitable for occupying VkDescriptorSet slot of type VK_DESCRIPTOR_TYPE_INPUT_ATTACHMENT; be read from a shader as an input attachment; and be used as an input attachment in a framebuffer.
    /// </summary>
    VK_IMAGE_USAGE_INPUT_ATTACHMENT_BIT = 0x00000080,

    /// <summary>
    /// Specifies that the image can be used as a decode output picture in a video decode operation.
    /// </summary>
    /// <remarks>Provided by VK_KHR_video_decode_queue</remarks>
    VK_IMAGE_USAGE_VIDEO_DECODE_DST_BIT_KHR = 0x00000400,

    /// <summary>
    /// Is reserved for future use.
    /// </summary>
    /// <remarks>Provided by VK_KHR_video_decode_queue</remarks>
    VK_IMAGE_USAGE_VIDEO_DECODE_SRC_BIT_KHR = 0x00000800,

    /// <summary>
    /// Specifies that the image can be used as an output reconstructed picture or an input reference picture in a video decode operation.
    /// </summary>
    /// <remarks>Provided by VK_KHR_video_decode_queue</remarks>
    VK_IMAGE_USAGE_VIDEO_DECODE_DPB_BIT_KHR = 0x00001000,

    /// <summary>
    /// Specifies that the image can be used to create a VkImageView suitable for use as a fragment density map image.
    /// </summary>
    /// <remarks>Provided by VK_EXT_fragment_density_map</remarks>
    VK_IMAGE_USAGE_FRAGMENT_DENSITY_MAP_BIT_EXT = 0x00000200,

    /// <summary>
    /// Specifies that the image can be used to create a VkImageView suitable for use as a fragment shading rate attachment or shading rate image
    /// </summary>
    /// <remarks>Provided by VK_KHR_fragment_shading_rate</remarks>
    VK_IMAGE_USAGE_FRAGMENT_SHADING_RATE_ATTACHMENT_BIT_KHR = 0x00000100,

    /// <summary>
    /// Specifies that the image can be used with host copy commands and host layout transitions.
    /// </summary>
    /// <remarks>Provided by VK_EXT_host_image_copy</remarks>
    VK_IMAGE_USAGE_HOST_TRANSFER_BIT_EXT = 0x00400000,
#if VK_ENABLE_BETA_EXTENSIONS

    /// <summary>
    /// Is reserved for future use.
    /// </summary>
    /// <remarks>Provided by VK_KHR_video_encode_queue</remarks>
    VK_IMAGE_USAGE_VIDEO_ENCODE_DST_BIT_KHR = 0x00002000,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <summary>
    /// Specifies that the image can be used as an encode input picture in a video encode operation.
    /// </summary>
    /// <remarks>Provided by VK_KHR_video_encode_queue</remarks>
    VK_IMAGE_USAGE_VIDEO_ENCODE_SRC_BIT_KHR = 0x00004000,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <summary>
    /// Specifies that the image can be used as an output reconstructed picture or an input reference picture in a video encode operation.
    /// </summary>
    /// <remarks>Provided by VK_KHR_video_encode_queue</remarks>
    VK_IMAGE_USAGE_VIDEO_ENCODE_DPB_BIT_KHR = 0x00008000,
#endif

    /// <summary>
    /// Specifies that the image can be transitioned to the VK_IMAGE_LAYOUT_ATTACHMENT_FEEDBACK_LOOP_OPTIMAL_EXT layout to be used as a color or depth/stencil attachment in a VkFramebuffer and/or as a read-only input resource in a shader (sampled image, combined image sampler or input attachment) in the same render pass.
    /// </summary>
    /// <remarks>Provided by VK_EXT_attachment_feedback_loop_layout</remarks>
    VK_IMAGE_USAGE_ATTACHMENT_FEEDBACK_LOOP_BIT_EXT = 0x00080000,

    /// <remarks>Provided by VK_HUAWEI_invocation_mask</remarks>
    VK_IMAGE_USAGE_INVOCATION_MASK_BIT_HUAWEI = 0x00040000,

    /// <remarks>Provided by VK_QCOM_image_processing</remarks>
    VK_IMAGE_USAGE_SAMPLE_WEIGHT_BIT_QCOM = 0x00100000,

    /// <remarks>Provided by VK_QCOM_image_processing</remarks>
    VK_IMAGE_USAGE_SAMPLE_BLOCK_MATCH_BIT_QCOM = 0x00200000,

    /// <inheritdoc cref="VK_IMAGE_USAGE_FRAGMENT_SHADING_RATE_ATTACHMENT_BIT_KHR" />
    /// <remarks>Provided by VK_NV_shading_rate_image</remarks>
    VK_IMAGE_USAGE_SHADING_RATE_IMAGE_BIT_NV = VK_IMAGE_USAGE_FRAGMENT_SHADING_RATE_ATTACHMENT_BIT_KHR,
}
