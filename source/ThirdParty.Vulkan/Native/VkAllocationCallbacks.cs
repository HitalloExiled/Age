namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkAllocationCallbacks.html">VkAllocationCallbacks</see>
/// </summary>
public unsafe struct VkAllocationCallbacks
{
    public void*                                pUserData;
    public PFN_vkAllocationFunction             pfnAllocation;
    public PFN_vkReallocationFunction           pfnReallocation;
    public PFN_vkFreeFunction                   pfnFree;
    public PFN_vkInternalAllocationNotification pfnInternalAllocation;
    public PFN_vkInternalFreeNotification       pfnInternalFree;
}
