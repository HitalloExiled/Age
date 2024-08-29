using System.Runtime.InteropServices;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkClearValue.html">VkClearValue</see>
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public struct VkClearValue
{
    [FieldOffset(0)]
    public VkClearColorValue Color;

    [FieldOffset(0)]
    public VkClearDepthStencilValue DepthStencil;
}
