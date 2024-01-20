namespace Age.Vulkan.Enums;

/// <summary>
/// <para>Framebuffer blending operations.</para>
/// <para>Once the source and destination blend factors have been selected, they along with the source and destination components are passed to the blending operations. RGB and alpha components can use different operations</para>
/// <para>See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkBlendOp.html">Basic Blend Operations</see></para>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public enum VkBlendOp
{
    VK_BLEND_OP_ADD = 0,
    VK_BLEND_OP_SUBTRACT = 1,
    VK_BLEND_OP_REVERSE_SUBTRACT = 2,
    VK_BLEND_OP_MIN = 3,
    VK_BLEND_OP_MAX = 4,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_BLEND_OP_ZERO_EXT = 1000148000,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_BLEND_OP_SRC_EXT = 1000148001,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_BLEND_OP_DST_EXT = 1000148002,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_BLEND_OP_SRC_OVER_EXT = 1000148003,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_BLEND_OP_DST_OVER_EXT = 1000148004,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_BLEND_OP_SRC_IN_EXT = 1000148005,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_BLEND_OP_DST_IN_EXT = 1000148006,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_BLEND_OP_SRC_OUT_EXT = 1000148007,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_BLEND_OP_DST_OUT_EXT = 1000148008,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_BLEND_OP_SRC_ATOP_EXT = 1000148009,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_BLEND_OP_DST_ATOP_EXT = 1000148010,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_BLEND_OP_XOR_EXT = 1000148011,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_BLEND_OP_MULTIPLY_EXT = 1000148012,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_BLEND_OP_SCREEN_EXT = 1000148013,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_BLEND_OP_OVERLAY_EXT = 1000148014,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_BLEND_OP_DARKEN_EXT = 1000148015,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_BLEND_OP_LIGHTEN_EXT = 1000148016,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_BLEND_OP_COLORDODGE_EXT = 1000148017,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_BLEND_OP_COLORBURN_EXT = 1000148018,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_BLEND_OP_HARDLIGHT_EXT = 1000148019,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_BLEND_OP_SOFTLIGHT_EXT = 1000148020,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_BLEND_OP_DIFFERENCE_EXT = 1000148021,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_BLEND_OP_EXCLUSION_EXT = 1000148022,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_BLEND_OP_INVERT_EXT = 1000148023,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_BLEND_OP_INVERT_RGB_EXT = 1000148024,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_BLEND_OP_LINEARDODGE_EXT = 1000148025,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_BLEND_OP_LINEARBURN_EXT = 1000148026,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_BLEND_OP_VIVIDLIGHT_EXT = 1000148027,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_BLEND_OP_LINEARLIGHT_EXT = 1000148028,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_BLEND_OP_PINLIGHT_EXT = 1000148029,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_BLEND_OP_HARDMIX_EXT = 1000148030,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_BLEND_OP_HSL_HUE_EXT = 1000148031,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_BLEND_OP_HSL_SATURATION_EXT = 1000148032,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_BLEND_OP_HSL_COLOR_EXT = 1000148033,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_BLEND_OP_HSL_LUMINOSITY_EXT = 1000148034,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_BLEND_OP_PLUS_EXT = 1000148035,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_BLEND_OP_PLUS_CLAMPED_EXT = 1000148036,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_BLEND_OP_PLUS_CLAMPED_ALPHA_EXT = 1000148037,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_BLEND_OP_PLUS_DARKER_EXT = 1000148038,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_BLEND_OP_MINUS_EXT = 1000148039,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_BLEND_OP_MINUS_CLAMPED_EXT = 1000148040,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_BLEND_OP_CONTRAST_EXT = 1000148041,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_BLEND_OP_INVERT_OVG_EXT = 1000148042,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_BLEND_OP_RED_EXT = 1000148043,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_BLEND_OP_GREEN_EXT = 1000148044,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_BLEND_OP_BLUE_EXT = 1000148045,
}
