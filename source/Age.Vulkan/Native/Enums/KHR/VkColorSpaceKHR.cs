namespace Age.Vulkan.Native.Enums.KHR;

/// <summary>
/// <para>Supported color space of the presentation engine.</para>
/// <remarks>
/// <para>In the initial release of the VK_KHR_surface and VK_KHR_swapchain extensions, the token <see cref="VK_COLORSPACE_SRGB_NONLINEAR_KHR"/> was used. Starting in the 2016-05-13 updates to the extension branches, matching release 1.0.13 of the core API specification, <see cref="VK_COLOR_SPACE_SRGB_NONLINEAR_KHR"/> is used instead for consistency with Vulkan naming rules. The older enum is still available for backwards compatibility.</para>
/// <para>In older versions of this extension <see cref="VK_COLOR_SPACE_DISPLAY_P3_LINEAR_EXT"/> was misnamed <see cref="VK_COLOR_SPACE_DCI_P3_LINEAR_EXT"/>. This has been updated to indicate that it uses RGB color encoding, not XYZ. The old name is deprecated but is maintained for backwards compatibility.</para>
/// <para>For a traditional “Linear” or non-gamma transfer function color space use <see cref="VK_COLOR_SPACE_PASS_THROUGH_EXT"/>.</para>
/// </remarks>
/// </summary>
public enum VkColorSpaceKHR
{
    /// <summary>
    /// Specifies support for the sRGB color space.
    /// </summary>
    VK_COLOR_SPACE_SRGB_NONLINEAR_KHR = 0,

    /// <summary>
    /// Specifies support for the Display-P3 color space to be displayed using an sRGB-like EOTF (defined below).
    /// </summary>
    /// <remarks>Provided by VK_EXT_swapchain_colorspace</remarks>
    VK_COLOR_SPACE_DISPLAY_P3_NONLINEAR_EXT = 1000104001,

    /// <summary>
    /// Specifies support for the extended sRGB color space to be displayed using a linear EOTF.
    /// </summary>
    /// <remarks>Provided by VK_EXT_swapchain_colorspace</remarks>
    VK_COLOR_SPACE_EXTENDED_SRGB_LINEAR_EXT = 1000104002,

    /// <summary>
    /// Specifies support for the Display-P3 color space to be displayed using a linear EOTF.
    /// </summary>
    /// <remarks>Provided by VK_EXT_swapchain_colorspace</remarks>
    VK_COLOR_SPACE_DISPLAY_P3_LINEAR_EXT = 1000104003,

    /// <summary>
    /// Specifies support for the DCI-P3 color space to be displayed using the DCI-P3 EOTF. Note that values in such an image are interpreted as XYZ encoded color data by the presentation engine.
    /// </summary>
    /// <remarks>Provided by VK_EXT_swapchain_colorspace</remarks>
    VK_COLOR_SPACE_DCI_P3_NONLINEAR_EXT = 1000104004,

    /// <summary>
    /// Specifies support for the BT709 color space to be displayed using a linear EOTF.
    /// </summary>
    /// <remarks>Provided by VK_EXT_swapchain_colorspace</remarks>
    VK_COLOR_SPACE_BT709_LINEAR_EXT = 1000104005,

    /// <summary>
    /// Specifies support for the BT709 color space to be displayed using the SMPTE 170M EOTF.
    /// </summary>
    /// <remarks>Provided by VK_EXT_swapchain_colorspace</remarks>
    VK_COLOR_SPACE_BT709_NONLINEAR_EXT = 1000104006,

    /// <summary>
    /// Specifies support for the BT2020 color space to be displayed using a linear EOTF.
    /// </summary>
    /// <remarks>Provided by VK_EXT_swapchain_colorspace</remarks>
    VK_COLOR_SPACE_BT2020_LINEAR_EXT = 1000104007,

    /// <summary>
    /// Specifies support for the HDR10 (BT2020 color) space to be displayed using the SMPTE ST2084 Perceptual Quantizer (PQ) EOTF.
    /// </summary>
    /// <remarks>Provided by VK_EXT_swapchain_colorspace</remarks>
    VK_COLOR_SPACE_HDR10_ST2084_EXT = 1000104008,

    /// <summary>
    /// Specifies support for the Dolby Vision (BT2020 color space), proprietary encoding, to be displayed using the SMPTE ST2084 EOTF.
    /// </summary>
    /// <remarks>Provided by VK_EXT_swapchain_colorspace</remarks>
    VK_COLOR_SPACE_DOLBYVISION_EXT = 1000104009,

    /// <summary>
    /// Specifies support for the HDR10 (BT2020 color space) to be displayed using the Hybrid Log Gamma (HLG) EOTF.
    /// </summary>
    /// <remarks>Provided by VK_EXT_swapchain_colorspace</remarks>
    VK_COLOR_SPACE_HDR10_HLG_EXT = 1000104010,

    /// <summary>
    /// Specifies support for the AdobeRGB color space to be displayed using a linear EOTF.
    /// </summary>
    /// <remarks>Provided by VK_EXT_swapchain_colorspace</remarks>
    VK_COLOR_SPACE_ADOBERGB_LINEAR_EXT = 1000104011,

    /// <summary>
    /// Specifies support for the AdobeRGB color space to be displayed using the Gamma 2.2 EOTF.
    /// </summary>
    /// <remarks>Provided by VK_EXT_swapchain_colorspace</remarks>
    VK_COLOR_SPACE_ADOBERGB_NONLINEAR_EXT = 1000104012,

    /// <summary>
    /// Specifies that color components are used “as is”. This is intended to allow applications to supply data for color spaces not described here.
    /// </summary>
    /// <remarks>Provided by VK_EXT_swapchain_colorspace</remarks>
    VK_COLOR_SPACE_PASS_THROUGH_EXT = 1000104013,

    /// <summary>
    /// Specifies support for the extended sRGB color space to be displayed using an sRGB EOTF.
    /// </summary>
    /// <remarks>Provided by VK_EXT_swapchain_colorspace</remarks>
    VK_COLOR_SPACE_EXTENDED_SRGB_NONLINEAR_EXT = 1000104014,

    /// <summary>
    /// Specifies support for the display’s native color space. This matches the color space expectations of AMD’s FreeSync2 standard, for displays supporting it.
    /// </summary>
    /// <remarks>Provided by VK_AMD_display_native_hdr</remarks>
    VK_COLOR_SPACE_DISPLAY_NATIVE_AMD = 1000213000,

    /// <inheritdoc cref="VK_COLOR_SPACE_SRGB_NONLINEAR_KHR" />
    VK_COLORSPACE_SRGB_NONLINEAR_KHR = VK_COLOR_SPACE_SRGB_NONLINEAR_KHR,

    /// <inheritdoc cref="VK_COLOR_SPACE_DISPLAY_P3_LINEAR_EXT" />
    /// <remarks>Provided by VK_EXT_swapchain_colorspace</remarks>
    VK_COLOR_SPACE_DCI_P3_LINEAR_EXT = VK_COLOR_SPACE_DISPLAY_P3_LINEAR_EXT,
}
