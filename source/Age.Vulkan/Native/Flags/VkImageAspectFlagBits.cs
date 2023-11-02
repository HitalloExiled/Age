namespace Age.Vulkan.Native.Flags;

/// <summary>
/// Bitmask specifying which aspects of an image are included in a view
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
[Flags]
public enum VkImageAspectFlagBits
{
    /// <summary>
    /// VK_IMAGE_ASPECT_COLOR_BIT specifies the color aspect.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    VK_IMAGE_ASPECT_COLOR_BIT = 0x00000001,

    /// <summary>
    /// VK_IMAGE_ASPECT_DEPTH_BIT specifies the depth aspect.
    /// </summary>
    VK_IMAGE_ASPECT_DEPTH_BIT = 0x00000002,

    /// <summary>
    /// VK_IMAGE_ASPECT_STENCIL_BIT specifies the stencil aspect.
    /// </summary>
    VK_IMAGE_ASPECT_STENCIL_BIT = 0x00000004,

    /// <summary>
    /// VK_IMAGE_ASPECT_METADATA_BIT specifies the metadata aspect used for sparse resource operations.
    /// </summary>
    VK_IMAGE_ASPECT_METADATA_BIT = 0x00000008,

    /// <summary>
    /// VK_IMAGE_ASPECT_PLANE_0_BIT specifies plane 0 of a multi-planar image format.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_IMAGE_ASPECT_PLANE_0_BIT = 0x00000010,

    /// <summary>
    /// VK_IMAGE_ASPECT_PLANE_1_BIT specifies plane 1 of a multi-planar image format.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_IMAGE_ASPECT_PLANE_1_BIT = 0x00000020,

    /// <summary>
    /// VK_IMAGE_ASPECT_PLANE_2_BIT specifies plane 2 of a multi-planar image format.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_IMAGE_ASPECT_PLANE_2_BIT = 0x00000040,

    /// <summary>
    /// VK_IMAGE_ASPECT_NONE specifies no image aspect, or the image aspect is not applicable.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_IMAGE_ASPECT_NONE = 0,

    /// <summary>
    /// VK_IMAGE_ASPECT_MEMORY_PLANE_0_BIT_EXT specifies memory plane 0.
    /// </summary>
    /// <remarks>Provided by VK_EXT_image_drm_format_modifier</remarks>
    VK_IMAGE_ASPECT_MEMORY_PLANE_0_BIT_EXT = 0x00000080,

    /// <summary>
    /// VK_IMAGE_ASPECT_MEMORY_PLANE_1_BIT_EXT specifies memory plane 1.
    /// </summary>
    /// <remarks>Provided by VK_EXT_image_drm_format_modifier</remarks>
    VK_IMAGE_ASPECT_MEMORY_PLANE_1_BIT_EXT = 0x00000100,

    /// <summary>
    /// VK_IMAGE_ASPECT_MEMORY_PLANE_2_BIT_EXT specifies memory plane 2.
    /// </summary>
    /// <remarks>Provided by VK_EXT_image_drm_format_modifier</remarks>
    VK_IMAGE_ASPECT_MEMORY_PLANE_2_BIT_EXT = 0x00000200,

    /// <summary>
    /// VK_IMAGE_ASPECT_MEMORY_PLANE_3_BIT_EXT specifies memory plane 3.
    /// </summary>
    /// <remarks>Provided by VK_EXT_image_drm_format_modifier</remarks>
    VK_IMAGE_ASPECT_MEMORY_PLANE_3_BIT_EXT = 0x00000400,

    /// <inheritdoc cref="VK_IMAGE_ASPECT_PLANE_0_BIT" />
    /// <remarks>Provided by VK_KHR_sampler_ycbcr_conversion</remarks>
    VK_IMAGE_ASPECT_PLANE_0_BIT_KHR = VK_IMAGE_ASPECT_PLANE_0_BIT,

    /// <inheritdoc cref="VK_IMAGE_ASPECT_PLANE_1_BIT" />
    /// <remarks>Provided by VK_KHR_sampler_ycbcr_conversion</remarks>
    VK_IMAGE_ASPECT_PLANE_1_BIT_KHR = VK_IMAGE_ASPECT_PLANE_1_BIT,

    /// <inheritdoc cref="VK_IMAGE_ASPECT_PLANE_2_BIT" />
    /// <remarks>Provided by VK_KHR_sampler_ycbcr_conversion</remarks>
    VK_IMAGE_ASPECT_PLANE_2_BIT_KHR = VK_IMAGE_ASPECT_PLANE_2_BIT,

    /// <inheritdoc cref="VK_IMAGE_ASPECT_NONE" />
    /// <remarks>Provided by VK_KHR_maintenance4</remarks>
    VK_IMAGE_ASPECT_NONE_KHR = VK_IMAGE_ASPECT_NONE,
}
