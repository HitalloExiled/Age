using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ThirdParty.Vulkan.Enums;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/PFN_vkReallocationFunction.html">PFN_vkReallocationFunction</see>
/// </summary>
[DebuggerDisplay("{Value}")]
public readonly record struct PFN_vkReallocationFunction
{
    public unsafe delegate void* Function(void* pUserData, void* pOriginal,size_t size, size_t alignment, VkSystemAllocationScope allocationScope);

    private readonly nint handle;

    public PFN_vkReallocationFunction(Function value) =>
        Marshal.GetFunctionPointerForDelegate(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator nint(PFN_vkReallocationFunction value) => value.handle;
}
