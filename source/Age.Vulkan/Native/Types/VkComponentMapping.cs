using Age.Vulkan.Native.Enums;

namespace Age.Vulkan.Native.Types;

/// <summary>
/// Structure specifying a color component mapping.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public struct VkComponentMapping
{
    /// <summary>
    /// A <see cref="VkComponentSwizzle"/> specifying the component value placed in the R component of the output vector.
    /// </summary>
    public VkComponentSwizzle r;

    /// <summary>
    /// A <see cref="VkComponentSwizzle"/> specifying the component value placed in the G component of the output vector.
    /// </summary>
    public VkComponentSwizzle g;

    /// <summary>
    /// A <see cref="VkComponentSwizzle"/> specifying the component value placed in the B component of the output vector.
    /// </summary>
    public VkComponentSwizzle b;

    /// <summary>
    /// A <see cref="VkComponentSwizzle"/> specifying the component value placed in the A component of the output vector.
    /// </summary>
    public VkComponentSwizzle a;
}
