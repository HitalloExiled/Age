namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkAllocationCallbacks.html">VkAllocationCallbacks</see>
/// </summary>
public unsafe struct VkAllocationCallbacks
{
    public void*                                PUserData;
    public PFN_vkAllocationFunction             PfnAllocation;
    public PFN_vkReallocationFunction           PfnReallocation;
    public PFN_vkFreeFunction                   PfnFree;
    public PFN_vkInternalAllocationNotification PfnInternalAllocation;
    public PFN_vkInternalFreeNotification       PfnInternalFree;
}
