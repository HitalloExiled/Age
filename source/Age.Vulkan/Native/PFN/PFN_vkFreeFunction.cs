using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Age.Vulkan.Native.PFN;

/// <summary>
/// Application-defined memory free function
/// </summary>
[DebuggerDisplay("{Value}")]
public record struct PFN_vkFreeFunction(nint Value)
{
    /// <summary>
    /// pMemory may be NULL, which the callback must handle safely. If pMemory is non-NULL, it must be a pointer previously allocated by pfnAllocation or pfnReallocation. The application should free this memory.
    /// </summary>
    /// <param name="pUserData">The value specified for VkAllocationCallbacks::pUserData in the allocator specified by the application.</param>
    /// <param name="pMemory">The allocation to be freed.</param>
    public unsafe delegate void* Function(void* pUserData, void* pMemory);

    public PFN_vkFreeFunction(Function value) : this(Marshal.GetFunctionPointerForDelegate(value))
    { }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator nint(PFN_vkFreeFunction value) => value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static explicit operator Function(PFN_vkFreeFunction value) => Marshal.GetDelegateForFunctionPointer<Function>(value.Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static explicit operator PFN_vkFreeFunction(Function value) => new(value);
}
