namespace Age.Vulkan.Native.Enums;

/// <summary>
/// <para>Specify border color used for texture lookups</para>
/// <para>These colors are described in detail in <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#textures-texel-replacement">Texel Replacement</see>.</para>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public enum VkBorderColor
{
    /// <summary>
    /// Specifies a transparent, floating-point format, black color.
    /// </summary>
    VK_BORDER_COLOR_FLOAT_TRANSPARENT_BLACK = 0,

    /// <summary>
    /// Specifies a transparent, integer format, black color.
    /// </summary>
    VK_BORDER_COLOR_INT_TRANSPARENT_BLACK = 1,

    /// <summary>
    /// Specifies an opaque, floating-point format, black color.
    /// </summary>
    VK_BORDER_COLOR_FLOAT_OPAQUE_BLACK = 2,

    /// <summary>
    /// Specifies an opaque, integer format, black color.
    /// </summary>
    VK_BORDER_COLOR_INT_OPAQUE_BLACK = 3,

    /// <summary>
    /// Specifies an opaque, floating-point format, white color.
    /// </summary>
    VK_BORDER_COLOR_FLOAT_OPAQUE_WHITE = 4,

    /// <summary>
    /// Specifies an opaque, integer format, white color.
    /// </summary>
    VK_BORDER_COLOR_INT_OPAQUE_WHITE = 5,

    /// <summary>
    /// Indicates that a <see cref="VkSamplerCustomBorderColorCreateInfoEXT"/> structure is included in the <see cref="VkSamplerCreateInfo::pNext"/> chain containing the color data in floating-point format.
    /// </summary>
    /// <remarks>Provided by VK_EXT_custom_border_color</remarks>
    VK_BORDER_COLOR_FLOAT_CUSTOM_EXT = 1000287003,

    /// <summary>
    /// Indicates that a <see cref="VkSamplerCustomBorderColorCreateInfoEXT"/> structure is included in the <see cref="VkSamplerCreateInfo::pNext"/> chain containing the color data in integer format.
    /// </summary>
    /// <remarks>Provided by VK_EXT_custom_border_color</remarks>
    VK_BORDER_COLOR_INT_CUSTOM_EXT = 1000287004,
}
