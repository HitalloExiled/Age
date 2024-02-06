using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ThirdParty.Vulkan.Enums;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/PFN_vkAllocationFunction.html">PFN_vkAllocationFunction</see>
/// </summary>
[DebuggerDisplay("{Value}")]
public readonly record struct PFN_vkAllocationFunction
{
    public unsafe delegate void* Function(void* pUserData, size_t size, size_t alignment, VkSystemAllocationScope allocationScope);

    private readonly nint handle;

    public PFN_vkAllocationFunction(Function value) =>
        Marshal.GetFunctionPointerForDelegate(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator nint(PFN_vkAllocationFunction value) => value.handle;
}
