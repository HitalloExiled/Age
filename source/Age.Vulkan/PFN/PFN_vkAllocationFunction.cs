using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Age.Vulkan.Enums;

namespace Age.Vulkan.PFN;

/// <summary>
/// Application-defined memory allocation function
/// </summary>
[DebuggerDisplay("{Value}")]
public record struct PFN_vkAllocationFunction(nint Value)
{
    /// <summary>
    /// <para>If pfnAllocation is unable to allocate the requested memory, it must return NULL. If the allocation was successful, it must return a valid pointer to memory allocation containing at least size bytes, and with the pointer value being a multiple of alignment.</para>
    /// <para>If pfnAllocation returns NULL, and if the implementation is unable to continue correct processing of the current command without the requested allocation, it must treat this as a runtime error, and generate VK_ERROR_OUT_OF_HOST_MEMORY at the appropriate time for the command in which the condition was detected, as described in Return Codes.</para>
    /// <para>If the implementation is able to continue correct processing of the current command without the requested allocation, then it may do so, and must not generate VK_ERROR_OUT_OF_HOST_MEMORY as a result of this failed allocation.</para>
    /// </summary>
    /// <param name="pUserData">The value specified for <see cref="VkAllocationCallbacks.pUserData"/> in the allocator specified by the application.</param>
    /// <param name="size">The size in bytes of the requested allocation.</param>
    /// <param name="alignment">The requested alignment of the allocation in bytes and must be a power of two.</param>
    /// <param name="allocationScope">A <see cref="VkSystemAllocationScope"/> value specifying the allocation scope of the lifetime of the allocation, as described <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-host-allocation-scope">here</see>.</param>
    public unsafe delegate void* Function(void* pUserData, ulong size, ulong alignment, VkSystemAllocationScope allocationScope);

    public PFN_vkAllocationFunction(Function value) : this(Marshal.GetFunctionPointerForDelegate(value))
    { }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator nint(PFN_vkAllocationFunction value) => value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static explicit operator Function(PFN_vkAllocationFunction value) => Marshal.GetDelegateForFunctionPointer<Function>(value.Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static explicit operator PFN_vkAllocationFunction(Function value) => new(value);
}
