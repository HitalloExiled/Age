namespace Age.Vulkan.Types;

/// <summary>
/// Structure specifying a clear depth stencil value.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public struct VkClearDepthStencilValue
{
    /// <summary>
    /// The clear value for the depth aspect of the depth/stencil attachment. It is a floating-point value which is automatically converted to the attachment’s format.
    /// </summary>
    public float depth;

    /// <summary>
    /// The clear value for the stencil aspect of the depth/stencil attachment. It is a 32-bit integer value which is converted to the attachment’s format by taking the appropriate number of LSBs.
    /// </summary>
    public uint stencil;
}
