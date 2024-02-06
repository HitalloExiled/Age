using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ThirdParty.Vulkan.Enums;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/PFN_vkInternalFreeNotification.html">PFN_vkInternalFreeNotification</see>
/// </summary>
[DebuggerDisplay("{Value}")]
public readonly record struct PFN_vkInternalFreeNotification
{
    public unsafe delegate void* Function(void* pUserData, ulong size, VkInternalAllocationType allocationType, VkSystemAllocationScope allocationScope);

    private readonly nint handle;

    public PFN_vkInternalFreeNotification(Function value) =>
        Marshal.GetFunctionPointerForDelegate(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator nint(PFN_vkInternalFreeNotification value) => value.handle;
}
