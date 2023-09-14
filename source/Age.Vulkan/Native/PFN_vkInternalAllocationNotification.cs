using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Age.Vulkan.Native;

/// <summary>
/// Application-defined memory allocation notification function
/// </summary>
public record struct PFN_vkInternalAllocationNotification(nint Value)
{
    /// <summary>
    /// This is a purely informational callback.
    /// </summary>
    /// <param name="pUserData">The value specified for <see cref="VkAllocationCallbacks.pUserData"/> in the allocator specified by the application.</param>
    /// <param name="size">The requested size of an allocation.</param>
    /// <param name="allocationType">A <see cref="VkInternalAllocationType"/> value specifying the requested type of an allocation.</param>
    /// <param name="allocationScope">A <see cref="VkSystemAllocationScope"/> value specifying the allocation scope of the lifetime of the allocation, as described here.</param>
    public unsafe delegate void* Function(void* pUserData, uint size, VkInternalAllocationType allocationType, VkSystemAllocationScope allocationScope);

    public PFN_vkInternalAllocationNotification(Function value) : this(Marshal.GetFunctionPointerForDelegate(value))
    { }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator nint(PFN_vkInternalAllocationNotification value) => value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static explicit operator Function(PFN_vkInternalAllocationNotification value) => Marshal.GetDelegateForFunctionPointer<Function>(value.Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static explicit operator PFN_vkInternalAllocationNotification(Function value) => new(value);
}
