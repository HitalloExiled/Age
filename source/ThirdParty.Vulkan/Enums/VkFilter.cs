namespace ThirdParty.Vulkan.Enums;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkFilter.html">VkFilter</see>
/// </summary>
public enum VkFilter
{
    Nearest  = 0,
    Linear   = 1,
    CubicEXT = 1000015000,
    CubicImg = CubicEXT,
}
