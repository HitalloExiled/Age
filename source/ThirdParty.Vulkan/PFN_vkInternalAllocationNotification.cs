using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ThirdParty.Vulkan.Enums;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/PFN_vkInternalAllocationNotification.html">PFN_vkInternalAllocationNotification</see>
/// </summary>
[DebuggerDisplay("{Value}")]
public readonly record struct PFN_vkInternalAllocationNotification
{
    public unsafe delegate void* Function(void* pUserData, size_t size, VkInternalAllocationType allocationType, VkSystemAllocationScope allocationScope);

    private readonly nint handle;

    public PFN_vkInternalAllocationNotification(Function value) =>
        Marshal.GetFunctionPointerForDelegate(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator nint(PFN_vkInternalAllocationNotification value) => value.handle;
}
