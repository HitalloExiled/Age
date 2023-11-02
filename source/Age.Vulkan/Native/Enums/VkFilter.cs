namespace Age.Vulkan.Native.Enums;

/// <summary>
/// Specify filters used for texture lookups.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public enum VkFilter
{
    /// <summary>
    /// Specifies nearest filtering.
    /// </summary>
    VK_FILTER_NEAREST = 0,

    /// <summary>
    /// Specifies linear filtering.
    /// </summary>
    VK_FILTER_LINEAR = 1,

    /// <summary>
    /// Specifies cubic filtering.
    /// </summary>
    /// <remarks>Provided by VK_EXT_filter_cubic</remarks>
    VK_FILTER_CUBIC_EXT = 1000015000,

    /// <inheritdoc cref="VK_FILTER_CUBIC_EXT" />
    /// <remarks>Provided by VK_IMG_filter_cubic</remarks>
    VK_FILTER_CUBIC_IMG = VK_FILTER_CUBIC_EXT,
}
