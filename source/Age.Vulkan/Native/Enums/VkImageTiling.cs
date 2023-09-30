namespace Age.Vulkan.Native.Enums;

/// <summary>
/// Specifies the tiling arrangement of data in an image.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public enum VkImageTiling
{
    /// <summary>
    /// Specifies optimal tiling (texels are laid out in an implementation-dependent arrangement, for more efficient memory access).
    /// </summary>
    VK_IMAGE_TILING_OPTIMAL = 0,

    /// <summary>
    /// Specifies linear tiling (texels are laid out in memory in row-major order, possibly with some padding on each row).
    /// </summary>
    VK_IMAGE_TILING_LINEAR = 1,

    /// <summary>
    /// Indicates that the image’s tiling is defined by a Linux DRM format modifier. The modifier is specified at image creation with <see cref="VkImageDrmFormatModifierListCreateInfoEXT"/> or <see cref="VkImageDrmFormatModifierExplicitCreateInfoEXT"/>, and can be queried with <see cref="VkExtImageDrmFormatModifier.GetImageDrmFormatModifierProperties"/>.
    /// </summary>
    /// <remarks>Provided by VK_EXT_image_drm_format_modifier</remarks>
    VK_IMAGE_TILING_DRM_FORMAT_MODIFIER_EXT = 1000158000,
}
