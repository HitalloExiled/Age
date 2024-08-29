namespace ThirdParty.Vulkan.Enums;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkSamplerAddressMode.html">VkSamplerAddressMode</see>
/// </summary>
public enum VkSamplerAddressMode
{
    Repeat               = 0,
    MirroredRepeat       = 1,
    ClampToEdge          = 2,
    ClampToBorder        = 3,
    MirrorClampToEdge    = 4,
    MirrorClampToEdgeKHR = MirrorClampToEdge,
}
