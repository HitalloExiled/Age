namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkAllocationCallbacks.html">VkAllocationCallbacks</see>
/// </summary>
public unsafe struct VkAllocationCallbacks
{
    public void*                                       PUserData;
    public VkPfn<PFN_vkAllocationFunction>             PfnAllocation;
    public VkPfn<PFN_vkReallocationFunction>           PfnReallocation;
    public VkPfn<PFN_vkFreeFunction>                   PfnFree;
    public VkPfn<PFN_vkInternalAllocationNotification> PfnInternalAllocation;
    public VkPfn<PFN_vkInternalFreeNotification>       PfnInternalFree;
}
