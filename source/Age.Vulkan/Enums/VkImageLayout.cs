namespace Age.Vulkan.Enums;

/// <summary>
/// <para>Layout of image and image subresources.</para>
/// <para>The layout of each image subresource is not a state of the image subresource itself, but is rather a property of how the data in memory is organized, and thus for each mechanism of accessing an image in the API the application must specify a parameter or structure member that indicates which image layout the image subresource(s) are considered to be in when the image will be accessed. For transfer commands, this is a parameter to the command (see https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#clears and https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#copies). For use as a framebuffer attachment, this is a member in the substructures of the <see cref="VkRenderPassCreateInfo"/> (see Render Pass). For use in a descriptor set, this is a member in the <see cref="VkDescriptorImageInfo"/> structure (see https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-updates).</para>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public enum VkImageLayout
{
    /// <summary>
    /// Specifies that the layout is unknown. Image memory cannot be transitioned into this layout. This layout can be used as the initialLayout member of <see cref="VkImageCreateInfo"/>. This layout can be used in place of the current image layout in a layout transition, but doing so will cause the contents of the image’s memory to be undefined.
    /// </summary>
    VK_IMAGE_LAYOUT_UNDEFINED = 0,

    /// <summary>
    /// Supports all types of device access.
    /// </summary>
    VK_IMAGE_LAYOUT_GENERAL = 1,

    /// <summary>
    /// Must only be used as a color or resolve attachment in a <see cref="VkFramebuffer"/>. This layout is valid only for image subresources of images created with the <see cref="VkImageUsageFlagBits.VK_IMAGE_USAGE_COLOR_ATTACHMENT_BIT"/> usage bit enabled.
    /// </summary>
    VK_IMAGE_LAYOUT_COLOR_ATTACHMENT_OPTIMAL = 2,

    /// <summary>
    /// Specifies a layout for both the depth and stencil aspects of a depth/stencil format image allowing read and write access as a depth/stencil attachment. It is equivalent to <see cref="VK_IMAGE_LAYOUT_DEPTH_ATTACHMENT_OPTIMAL"/> and <see cref="VK_IMAGE_LAYOUT_STENCIL_ATTACHMENT_OPTIMAL"/>.
    /// </summary>
    VK_IMAGE_LAYOUT_DEPTH_STENCIL_ATTACHMENT_OPTIMAL = 3,

    /// <summary>
    /// Specifies a layout for both the depth and stencil aspects of a depth/stencil format image allowing read only access as a depth/stencil attachment or in shaders as a sampled image, combined image/sampler, or input attachment. It is equivalent to <see cref="VK_IMAGE_LAYOUT_DEPTH_READ_ONLY_OPTIMAL"/> and <see cref="VK_IMAGE_LAYOUT_STENCIL_READ_ONLY_OPTIMAL"/>.
    /// </summary>
    VK_IMAGE_LAYOUT_DEPTH_STENCIL_READ_ONLY_OPTIMAL = 4,

    /// <summary>
    /// Specifies a layout allowing read-only access in a shader as a sampled image, combined image/sampler, or input attachment. This layout is valid only for image subresources of images created with the <see cref="VkImageUsageFlagBits.VK_IMAGE_USAGE_SAMPLED_BIT"/> or <see cref="VkImageUsageFlagBits.VK_IMAGE_USAGE_INPUT_ATTACHMENT_BIT"/> usage bits enabled.
    /// </summary>
    VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL = 5,

    /// <summary>
    /// Must only be used as a source image of a transfer command (see the definition of <see cref="VkPipelineStageFlagBits.VK_PIPELINE_STAGE_TRANSFER_BIT"/>). This layout is valid only for image subresources of images created with the <see cref="VkImageUsageFlagBits.VK_IMAGE_USAGE_TRANSFER_SRC_BIT"/> usage bit enabled.
    /// </summary>
    VK_IMAGE_LAYOUT_TRANSFER_SRC_OPTIMAL = 6,

    /// <summary>
    /// Must only be used as a destination image of a transfer command. This layout is valid only for image subresources of images created with the <see cref="VkImageUsageFlagBits.VK_IMAGE_USAGE_TRANSFER_DST_BIT"/> usage bit enabled.
    /// </summary>
    VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL = 7,

    /// <summary>
    /// Specifies that an image’s memory is in a defined layout and can be populated by data, but that it has not yet been initialized by the driver. Image memory cannot be transitioned into this layout. This layout can be used as the initialLayout member of <see cref="VkImageCreateInfo"/>. This layout is intended to be used as the initial layout for an image whose contents are written by the host, and hence the data can be written to memory immediately, without first executing a layout transition. Currently, <see cref="VK_IMAGE_LAYOUT_PREINITIALIZED"/> is only useful with linear images because there is not a standard layout defined for <see cref="VK_IMAGE_TILING_OPTIMAL"/> images.
    /// </summary>
    VK_IMAGE_LAYOUT_PREINITIALIZED = 8,

    /// <summary>
    /// Specifies a layout for depth/stencil format images allowing read and write access to the stencil aspect as a stencil attachment, and read only access to the depth aspect as a depth attachment or in shaders as a sampled image, combined image/sampler, or input attachment. It is equivalent to <see cref="VK_IMAGE_LAYOUT_DEPTH_READ_ONLY_OPTIMAL"/> and <see cref="VK_IMAGE_LAYOUT_STENCIL_ATTACHMENT_OPTIMAL"/>.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_IMAGE_LAYOUT_DEPTH_READ_ONLY_STENCIL_ATTACHMENT_OPTIMAL = 1000117000,

    /// <summary>
    /// Specifies a layout for depth/stencil format images allowing read and write access to the depth aspect as a depth attachment, and read only access to the stencil aspect as a stencil attachment or in shaders as a sampled image, combined image/sampler, or input attachment. It is equivalent to <see cref="VK_IMAGE_LAYOUT_DEPTH_ATTACHMENT_OPTIMAL"/> and <see cref="VK_IMAGE_LAYOUT_STENCIL_READ_ONLY_OPTIMAL"/>.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_IMAGE_LAYOUT_DEPTH_ATTACHMENT_STENCIL_READ_ONLY_OPTIMAL = 1000117001,

    /// <summary>
    /// Specifies a layout for the depth aspect of a depth/stencil format image allowing read and write access as a depth attachment.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_IMAGE_LAYOUT_DEPTH_ATTACHMENT_OPTIMAL = 1000241000,

    /// <summary>
    /// Specifies a layout for the depth aspect of a depth/stencil format image allowing read-only access as a depth attachment or in shaders as a sampled image, combined image/sampler, or input attachment.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_IMAGE_LAYOUT_DEPTH_READ_ONLY_OPTIMAL = 1000241001,

    /// <summary>
    /// Specifies a layout for the stencil aspect of a depth/stencil format image allowing read and write access as a stencil attachment.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_IMAGE_LAYOUT_STENCIL_ATTACHMENT_OPTIMAL = 1000241002,

    /// <summary>
    /// Specifies a layout for the stencil aspect of a depth/stencil format image allowing read-only access as a stencil attachment or in shaders as a sampled image, combined image/sampler, or input attachment.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_IMAGE_LAYOUT_STENCIL_READ_ONLY_OPTIMAL = 1000241003,

    /// <summary>
    /// Specifies a layout allowing read only access as an attachment, or in shaders as a sampled image, combined image/sampler, or input attachment.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_IMAGE_LAYOUT_READ_ONLY_OPTIMAL = 1000314000,

    /// <summary>
    /// Specifies a layout that must only be used with attachment accesses in the graphics pipeline.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_IMAGE_LAYOUT_ATTACHMENT_OPTIMAL = 1000314001,

    /// <summary>
    /// Must only be used for presenting a presentable image for display.
    /// </summary>
    /// <remarks>Provided by VK_KHR_swapchain</remarks>
    VK_IMAGE_LAYOUT_PRESENT_SRC_KHR = 1000001002,

    /// <summary>
    /// Must only be used as a decode output picture in a video decode operation. This layout is valid only for image subresources of images created with the <see cref="VkImageUsageFlagBits.VK_IMAGE_USAGE_VIDEO_DECODE_DST_BIT_KHR"/> usage bit enabled.
    /// </summary>
    /// <remarks>Provided by VK_KHR_video_decode_queue</remarks>
    VK_IMAGE_LAYOUT_VIDEO_DECODE_DST_KHR = 1000024000,

    /// <summary>
    /// Is reserved for future use.
    /// </summary>
    /// <remarks>Provided by VK_KHR_video_decode_queue</remarks>
    VK_IMAGE_LAYOUT_VIDEO_DECODE_SRC_KHR = 1000024001,

    /// <summary>
    /// Must only be used as an output reconstructed picture or an input reference picture in a video decode operation. This layout is valid only for image subresources of images created with the <see cref="VkImageUsageFlagBits.VK_IMAGE_USAGE_VIDEO_DECODE_DPB_BIT_KHR"/> usage bit enabled.
    /// </summary>
    /// <remarks>Provided by VK_KHR_video_decode_queue</remarks>
    VK_IMAGE_LAYOUT_VIDEO_DECODE_DPB_KHR = 1000024002,

    /// <summary>
    /// Is valid only for shared presentable images, and must be used for any usage the image supports.
    /// </summary>
    /// <remarks>Provided by VK_KHR_shared_presentable_image</remarks>
    VK_IMAGE_LAYOUT_SHARED_PRESENT_KHR = 1000111000,

    /// <summary>
    /// Must only be used as a fragment density map attachment in a <see cref="VkRenderPass"/>. This layout is valid only for image subresources of images created with the <see cref="VkImageUsageFlagBits.VK_IMAGE_USAGE_FRAGMENT_DENSITY_MAP_BIT_EXT"/> usage bit enabled.
    /// </summary>
    /// <remarks>Provided by VK_EXT_fragment_density_map</remarks>
    VK_IMAGE_LAYOUT_FRAGMENT_DENSITY_MAP_OPTIMAL_EXT = 1000218000,

    /// <summary>
    /// Must only be used as a fragment shading rate attachment or shading rate image. This layout is valid only for image subresources of images created with the <see cref="VkImageUsageFlagBits.VK_IMAGE_USAGE_FRAGMENT_SHADING_RATE_ATTACHMENT_BIT_KHR"/> usage bit enabled.
    /// </summary>
    /// <remarks>Provided by VK_KHR_fragment_shading_rate</remarks>
    VK_IMAGE_LAYOUT_FRAGMENT_SHADING_RATE_ATTACHMENT_OPTIMAL_KHR = 1000164003,
#if VK_ENABLE_BETA_EXTENSIONS

    /// <summary>
    /// Is reserved for future use.
    /// </summary>
    /// <remarks>Provided by VK_KHR_video_encode_queue</remarks>
    VK_IMAGE_LAYOUT_VIDEO_ENCODE_DST_KHR = 1000299000,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <summary>
    /// Must only be used as an encode input picture in a video encode operation. This layout is valid only for image subresources of images created with the <see cref="VkImageUsageFlagBits.VK_IMAGE_USAGE_VIDEO_ENCODE_SRC_BIT_KHR"/> usage bit enabled.
    /// </summary>
    /// <remarks>Provided by VK_KHR_video_encode_queue</remarks>
    VK_IMAGE_LAYOUT_VIDEO_ENCODE_SRC_KHR = 1000299001,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <summary>
    /// Must only be used as an output reconstructed picture or an input reference picture in a video encode operation. This layout is valid only for image subresources of images created with the <see cref="VkImageUsageFlagBits.VK_IMAGE_USAGE_VIDEO_ENCODE_DPB_BIT_KHR"/> usage bit enabled.
    /// </summary>
    /// <remarks>Provided by VK_KHR_video_encode_queue</remarks>
    VK_IMAGE_LAYOUT_VIDEO_ENCODE_DPB_KHR = 1000299002,
#endif

    /// <summary>
    /// Must only be used as either a color attachment or depth/stencil attachment in a <see cref="VkFramebuffer"/> and/or read-only access in a shader as a sampled image, combined image/sampler, or input attachment. This layout is valid only for image subresources of images created with the <see cref="VkImageUsageFlagBits.VK_IMAGE_USAGE_ATTACHMENT_FEEDBACK_LOOP_BIT_EXT"/> usage bit enabled and either the <see cref="VkImageUsageFlagBits.VK_IMAGE_USAGE_COLOR_ATTACHMENT_BIT"/> or <see cref="VkImageUsageFlagBits.VK_IMAGE_USAGE_DEPTH_STENCIL_ATTACHMENT_BIT"/> and either the <see cref="VkImageUsageFlagBits.VK_IMAGE_USAGE_INPUT_ATTACHMENT_BIT"/> or <see cref="VkImageUsageFlagBits.VK_IMAGE_USAGE_SAMPLED_BIT"/> usage bits enabled.
    /// </summary>
    /// <remarks>Provided by VK_EXT_attachment_feedback_loop_layout</remarks>
    VK_IMAGE_LAYOUT_ATTACHMENT_FEEDBACK_LOOP_OPTIMAL_EXT = 1000339000,

    /// <inheritdoc cref="VK_IMAGE_LAYOUT_DEPTH_READ_ONLY_STENCIL_ATTACHMENT_OPTIMAL" />
    /// <remarks>Provided by VK_KHR_maintenance2</remarks>
    VK_IMAGE_LAYOUT_DEPTH_READ_ONLY_STENCIL_ATTACHMENT_OPTIMAL_KHR = VK_IMAGE_LAYOUT_DEPTH_READ_ONLY_STENCIL_ATTACHMENT_OPTIMAL,

    /// <inheritdoc cref="VK_IMAGE_LAYOUT_DEPTH_ATTACHMENT_STENCIL_READ_ONLY_OPTIMAL" />
    /// <remarks>Provided by VK_KHR_maintenance2</remarks>
    VK_IMAGE_LAYOUT_DEPTH_ATTACHMENT_STENCIL_READ_ONLY_OPTIMAL_KHR = VK_IMAGE_LAYOUT_DEPTH_ATTACHMENT_STENCIL_READ_ONLY_OPTIMAL,

    /// <inheritdoc cref="VK_IMAGE_LAYOUT_FRAGMENT_SHADING_RATE_ATTACHMENT_OPTIMAL_KHR" />
    /// <remarks>Provided by VK_NV_shading_rate_image</remarks>
    VK_IMAGE_LAYOUT_SHADING_RATE_OPTIMAL_NV = VK_IMAGE_LAYOUT_FRAGMENT_SHADING_RATE_ATTACHMENT_OPTIMAL_KHR,

    /// <inheritdoc cref="VK_IMAGE_LAYOUT_DEPTH_ATTACHMENT_OPTIMAL" />
    /// <remarks>Provided by VK_KHR_separate_depth_stencil_layouts</remarks>
    VK_IMAGE_LAYOUT_DEPTH_ATTACHMENT_OPTIMAL_KHR = VK_IMAGE_LAYOUT_DEPTH_ATTACHMENT_OPTIMAL,

    /// <inheritdoc cref="VK_IMAGE_LAYOUT_DEPTH_READ_ONLY_OPTIMAL" />
    /// <remarks>Provided by VK_KHR_separate_depth_stencil_layouts</remarks>
    VK_IMAGE_LAYOUT_DEPTH_READ_ONLY_OPTIMAL_KHR = VK_IMAGE_LAYOUT_DEPTH_READ_ONLY_OPTIMAL,

    /// <inheritdoc cref="VK_IMAGE_LAYOUT_STENCIL_ATTACHMENT_OPTIMAL" />
    /// <remarks>Provided by VK_KHR_separate_depth_stencil_layouts</remarks>
    VK_IMAGE_LAYOUT_STENCIL_ATTACHMENT_OPTIMAL_KHR = VK_IMAGE_LAYOUT_STENCIL_ATTACHMENT_OPTIMAL,

    /// <inheritdoc cref="VK_IMAGE_LAYOUT_STENCIL_READ_ONLY_OPTIMAL" />
    /// <remarks>Provided by VK_KHR_separate_depth_stencil_layouts</remarks>
    VK_IMAGE_LAYOUT_STENCIL_READ_ONLY_OPTIMAL_KHR = VK_IMAGE_LAYOUT_STENCIL_READ_ONLY_OPTIMAL,

    /// <inheritdoc cref="VK_IMAGE_LAYOUT_READ_ONLY_OPTIMAL" />
    /// <remarks>Provided by VK_KHR_synchronization2</remarks>
    VK_IMAGE_LAYOUT_READ_ONLY_OPTIMAL_KHR = VK_IMAGE_LAYOUT_READ_ONLY_OPTIMAL,

    /// <inheritdoc cref="VK_IMAGE_LAYOUT_ATTACHMENT_OPTIMAL" />
    /// <remarks>Provided by VK_KHR_synchronization2</remarks>
    VK_IMAGE_LAYOUT_ATTACHMENT_OPTIMAL_KHR = VK_IMAGE_LAYOUT_ATTACHMENT_OPTIMAL,
}
