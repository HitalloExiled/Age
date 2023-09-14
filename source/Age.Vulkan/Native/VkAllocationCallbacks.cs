namespace Age.Vulkan.Native;

/// <summary>
/// Structure containing callback function pointers for memory allocation
/// </summary>
public unsafe struct VkAllocationCallbacks
{
    /// <summary>
    /// A value to be interpreted by the implementation of the callbacks. When any of the callbacks in <see cref="VkAllocationCallbacks"/> are called, the Vulkan implementation will pass this value as the first parameter to the callback. This value can vary each time an allocator is passed into a command, even when the same object takes an allocator in multiple commands.
    /// </summary>
    public void* pUserData;

    /// <summary>
    /// A <see cref="PFN_vkAllocationFunction"/> pointer to an application-defined memory allocation function.
    /// </summary>
    public PFN_vkAllocationFunction pfnAllocation;

    /// <summary>
    /// A <see cref="PFN_vkReallocationFunction"/> pointer to an application-defined memory reallocation function.
    /// </summary>
    public PFN_vkReallocationFunction pfnReallocation;

    /// <summary>
    /// A <see cref="PFN_vkFreeFunction"/> pointer to an application-defined memory free function.
    /// </summary>
    public PFN_vkFreeFunction pfnFree;

    /// <summary>
    /// A <see cref="PFN_vkInternalAllocationNotification"/> pointer to an application-defined function that is called by the implementation when the implementation makes internal allocations.
    /// </summary>
    public PFN_vkInternalAllocationNotification pfnInternalAllocation;

    /// <summary>
    /// A <see cref="PFN_vkInternalFreeNotification"/> pointer to an application-defined function that is called by the implementation when the implementation frees internal allocations.
    /// </summary>
    public PFN_vkInternalFreeNotification pfnInternalFree;
}
