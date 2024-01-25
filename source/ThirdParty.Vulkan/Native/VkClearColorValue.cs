using System.Runtime.InteropServices;

namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkClearColorValue.html">VkClearColorValue</see>
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public unsafe struct VkClearColorValue
{
    [FieldOffset(0)]
    public fixed float float32[4];

    [FieldOffset(0)]
    public fixed int int32[4];

    [FieldOffset(0)]
    public fixed uint uint32[4];
}
