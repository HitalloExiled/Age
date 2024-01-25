using ThirdParty.Vulkan.Enums;

namespace ThirdParty.Vulkan;

public unsafe partial record AllocationCallbacks
{
    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/PFN_vkAllocationFunction.html">PFN_vkAllocationFunction</see>
    /// </summary>
    public unsafe delegate nint AllocationFunction(ulong size, ulong alignment, SystemAllocationScope allocationScope);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/PFN_vkAllocationFunction.html">PFN_vkAllocationFunction</see>
    /// </summary>
    public unsafe delegate nint ReallocationFunction(nint pOriginal, ulong size, ulong alignment, SystemAllocationScope allocationScope);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/PFN_vkFreeFunction.html">PFN_vkFreeFunction</see>
    /// </summary>
    public unsafe delegate void FreeFunction(nint pMemory);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/PFN_vkInternalAllocationNotification.html">PFN_vkInternalAllocationNotification</see>
    /// </summary>
    public unsafe delegate void InternalAllocationNotification(ulong size, InternalAllocationType allocationType, SystemAllocationScope allocationScope);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/PFN_vkInternalFreeNotification.html">PFN_vkInternalFreeNotification</see>
    /// </summary>
    public unsafe delegate void InternalFreeNotification(ulong size, VkInternalAllocationType allocationType, VkSystemAllocationScope allocationScope);
}
