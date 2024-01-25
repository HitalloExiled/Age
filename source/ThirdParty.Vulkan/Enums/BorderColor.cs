namespace ThirdParty.Vulkan.Enums;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkBorderColor.html">VkBorderColor</see>
/// </summary>
public enum BorderColor
{
    FloatTransparentBlack = 0,
    IntTransparentBlack   = 1,
    FloatOpaqueBlack      = 2,
    IntOpaqueBlack        = 3,
    FloatOpaqueWhite      = 4,
    IntOpaqueWhite        = 5,
    FloatCustomExt        = 1000287003,
    IntCustomExt          = 1000287004,
}
