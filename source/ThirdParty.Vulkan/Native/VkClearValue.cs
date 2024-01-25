using System.Runtime.InteropServices;

namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkClearValue.html">VkClearValue</see>
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public struct VkClearValue
{
    [FieldOffset(0)]
    public VkClearColorValue color;

    [FieldOffset(0)]
    public VkClearDepthStencilValue depthStencil;
}
