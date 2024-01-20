using System.Runtime.InteropServices;

namespace Age.Vulkan.Types;

/// <summary>
/// <para>Structure specifying a clear value.</para>
/// <para>This union is used where part of the API requires either color or depth/stencil clear values, depending on the attachment, and defines the initial clear values in the VkRenderPassBeginInfo structure.</para>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
[StructLayout(LayoutKind.Explicit)]
public struct VkClearValue
{
    /// <summary>
    /// Specifies the color image clear values to use when clearing a color image or attachment.
    /// </summary>
    [FieldOffset(0)]
    public VkClearColorValue color;

    /// <summary>
    /// Specifies the depth and stencil clear values to use when clearing a depth/stencil image or attachment.
    /// </summary>
    [FieldOffset(0)]
    public VkClearDepthStencilValue depthStencil;
}
