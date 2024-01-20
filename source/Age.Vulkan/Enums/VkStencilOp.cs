namespace Age.Vulkan.Enums;

/// <summary>
/// Stencil comparison function.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public enum VkStencilOp
{
    /// <summary>
    /// Keeps the current value.
    /// </summary>
    VK_STENCIL_OP_KEEP = 0,

    /// <summary>
    /// Sets the value to 0.
    /// </summary>
    VK_STENCIL_OP_ZERO = 1,

    /// <summary>
    /// Sets the value to reference.
    /// </summary>
    VK_STENCIL_OP_REPLACE = 2,

    /// <summary>
    /// Increments the current value and clamps to the maximum representable unsigned value.
    /// </summary>
    VK_STENCIL_OP_INCREMENT_AND_CLAMP = 3,

    /// <summary>
    /// Decrements the current value and clamps to 0.
    /// </summary>
    VK_STENCIL_OP_DECREMENT_AND_CLAMP = 4,

    /// <summary>
    /// BitwiseInverts the current value.
    /// </summary>
    VK_STENCIL_OP_INVERT = 5,

    /// <summary>
    /// Increments the current value and wraps to 0 when the maximum value would have been exceeded.
    /// </summary>
    VK_STENCIL_OP_INCREMENT_AND_WRAP = 6,

    /// <summary>
    /// Decrements the current value and wraps to the maximum possible value when the value would go below 0.
    /// </summary>
    VK_STENCIL_OP_DECREMENT_AND_WRAP = 7,
}
