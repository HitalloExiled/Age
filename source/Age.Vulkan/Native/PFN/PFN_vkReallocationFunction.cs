using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Age.Vulkan.Native.PFN;

/// <summary>
/// Application-defined memory reallocation function
/// </summary>
[DebuggerDisplay("{Value}")]
public record struct PFN_vkReallocationFunction(nint Value)
{
    /// <summary>
    /// <para>If the reallocation was successful, pfnReallocation must return an allocation with enough space for size bytes, and the contents of the original allocation from bytes zero to min(original size, new size) - 1 must be preserved in the returned allocation. If size is larger than the old size, the contents of the additional space are undefined. If satisfying these requirements involves creating a new allocation, then the old allocation should be freed.</para>
    /// <para>If pOriginal is NULL, then pfnReallocation must behave equivalently to a call to <see cref="PFN_vkAllocationFunction"/> with the same parameter values (without pOriginal).</para>
    /// <para>If size is zero, then pfnReallocation must behave equivalently to a call to PFN_vkFreeFunction with the same pUserData parameter value, and pMemory equal to pOriginal.</para>
    /// <para>If pOriginal is non-NULL, the implementation must ensure that alignment is equal to the alignment used to originally allocate pOriginal.</para>
    /// <para>If this function fails and pOriginal is non-NULL the application must not free the old allocation.</para>
    /// <para>pfnReallocation must follow the same rules for return values as <see cref="PFN_vkAllocationFunction"/>.</para>
    /// </summary>
    /// <param name="pUserData">The value specified for <see cref="VkAllocationCallbacks.pUserData"/> in the allocator specified by the application.</param>
    /// <param name="pOriginal">Must be either NULL or a pointer previously returned by pfnReallocation or pfnAllocation of a compatible allocator.</param>
    /// <param name="size">The size in bytes of the requested allocation.</param>
    /// <param name="alignment">Is the requested alignment of the allocation in bytes and must be a power of two.</param>
    /// <param name="allocationScope">A <see cref="VkSystemAllocationScope"/> value specifying the allocation scope of the lifetime of the allocation, as described here.</param>
    public unsafe delegate void* Function(void* pUserData, void* pOriginal, uint size, uint alignment, VkSystemAllocationScope allocationScope);

    public PFN_vkReallocationFunction(Function value) : this(Marshal.GetFunctionPointerForDelegate(value))
    { }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator nint(PFN_vkReallocationFunction value) => value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static explicit operator Function(PFN_vkReallocationFunction value) => Marshal.GetDelegateForFunctionPointer<Function>(value.Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static explicit operator PFN_vkReallocationFunction(Function value) => new(value);
}
