namespace Age.Vulkan.Flags.KHR;

/// <summary>
/// Sransforms supported on a device.
/// </summary>
/// <remarks>Provided by VK_KHR_surface</remarks>
[Flags]
public enum VkSurfaceTransformFlagBitsKHR
{
    /// <summary>
    /// Specifies that image content is presented without being transformed.
    /// </summary>
    VK_SURFACE_TRANSFORM_IDENTITY_BIT_KHR = 0x00000001,

    /// <summary>
    /// Specifies that image content is rotated 90 degrees clockwise.
    /// </summary>
    VK_SURFACE_TRANSFORM_ROTATE_90_BIT_KHR = 0x00000002,

    /// <summary>
    /// Specifies that image content is rotated 180 degrees clockwise.
    /// </summary>
    VK_SURFACE_TRANSFORM_ROTATE_180_BIT_KHR = 0x00000004,

    /// <summary>
    /// Specifies that image content is rotated 270 degrees clockwise.
    /// </summary>
    VK_SURFACE_TRANSFORM_ROTATE_270_BIT_KHR = 0x00000008,

    /// <summary>
    /// Specifies that image content is mirrored horizontally.
    /// </summary>
    VK_SURFACE_TRANSFORM_HORIZONTAL_MIRROR_BIT_KHR = 0x00000010,

    /// <summary>
    /// Specifies that image content is mirrored horizontally, then rotated 90 degrees clockwise.
    /// </summary>
    VK_SURFACE_TRANSFORM_HORIZONTAL_MIRROR_ROTATE_90_BIT_KHR = 0x00000020,

    /// <summary>
    /// Specifies that image content is mirrored horizontally, then rotated 180 degrees clockwise.
    /// </summary>
    VK_SURFACE_TRANSFORM_HORIZONTAL_MIRROR_ROTATE_180_BIT_KHR = 0x00000040,

    /// <summary>
    /// Specifies that image content is mirrored horizontally, then rotated 270 degrees clockwise.
    /// </summary>
    VK_SURFACE_TRANSFORM_HORIZONTAL_MIRROR_ROTATE_270_BIT_KHR = 0x00000080,

    /// <summary>
    /// Specifies that the presentation transform is not specified, and is instead determined by platform-specific considerations and mechanisms outside Vulkan.
    /// </summary>
    VK_SURFACE_TRANSFORM_INHERIT_BIT_KHR = 0x00000100,
}
