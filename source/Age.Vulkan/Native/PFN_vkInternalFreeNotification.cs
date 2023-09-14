using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Age.Vulkan.Native;

/// <summary>
/// Application-defined memory free notification function
/// </summary>
public record struct PFN_vkInternalFreeNotification(nint Value)
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="pUserData">The value specified for <see cref="VkAllocationCallbacks.pUserData"/> in the allocator specified by the application.</param>
    /// <param name="size">The requested size of an allocation.</param>
    /// <param name="allocationType">a <see cref="VkInternalAllocationType"/>Value specifying the requested type of an allocation.</param>
    /// <param name="allocationScope"><see cref="VkSystemAllocationScope"/>Value specifying the allocation scope of the lifetime of the allocation</param>
    public unsafe delegate void* Function(void* pUserData, uint size, VkInternalAllocationType allocationType, VkSystemAllocationScope allocationScope);

    public PFN_vkInternalFreeNotification(Function value) : this(Marshal.GetFunctionPointerForDelegate(value))
    { }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator nint(PFN_vkInternalFreeNotification value) => value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static explicit operator Function(PFN_vkInternalFreeNotification value) => Marshal.GetDelegateForFunctionPointer<Function>(value.Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static explicit operator PFN_vkInternalFreeNotification(Function value) => new(value);
}
