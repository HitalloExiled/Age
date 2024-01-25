namespace ThirdParty.Vulkan.Enums;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkStencilOp.html">VkStencilOp</see>
/// </summary>
public enum StencilOp
{
    Keep              = 0,
    Zero              = 1,
    Replace           = 2,
    IncrementAndClamp = 3,
    DecrementAndClamp = 4,
    Invert            = 5,
    IncrementAndWrap  = 6,
    DecrementAndWrap  = 7,
}
