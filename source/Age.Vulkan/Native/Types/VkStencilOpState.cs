using Age.Vulkan.Native.Enums;

namespace Age.Vulkan.Native.Types;

/// <summary>
/// Structure specifying stencil operation state.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public struct VkStencilOpState
{
    /// <summary>
    /// Is a <see cref="VkStencilOp"/> value specifying the action performed on samples that fail the stencil test.
    /// </summary>
    public VkStencilOp failOp;

    /// <summary>
    /// Is a <see cref="VkStencilOp"/> value specifying the action performed on samples that pass both the depth and stencil tests.
    /// </summary>
    public VkStencilOp passOp;

    /// <summary>
    /// Is a <see cref="VkStencilOp"/> value specifying the action performed on samples that pass the stencil test and fail the depth test.
    /// </summary>
    public VkStencilOp depthFailOp;

    /// <summary>
    /// Is a VkCompareOp value specifying the comparison operator used in the stencil test.
    /// </summary>
    public VkCompareOp compareOp;

    /// <summary>
    /// Selects the bits of the unsigned integer stencil values participating in the stencil test.
    /// </summary>
    public uint compareMask;

    /// <summary>
    /// Selects the bits of the unsigned integer stencil values updated by the stencil test in the stencil framebuffer attachment.
    /// </summary>
    public uint writeMask;

    /// <summary>
    /// Is an integer stencil reference value that is used in the unsigned stencil comparison.
    /// </summary>
    public uint reference;
}
