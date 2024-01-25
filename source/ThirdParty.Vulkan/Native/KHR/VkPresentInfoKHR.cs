namespace ThirdParty.Vulkan.Native.KHR;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPresentInfoKHR.html">VkPresentInfoKHR</see>
/// </summary>
public unsafe struct VkPresentInfoKHR
{
    public readonly VkStructureType sType;

    public void*           pNext;
    public uint            waitSemaphoreCount;
    public VkSemaphore*    pWaitSemaphores;
    public uint            swapchainCount;
    public VkSwapchainKHR* pSwapchains;
    public uint*           pImageIndices;
    public VkResult*       pResults;

    public VkPresentInfoKHR() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_PRESENT_INFO_KHR;
}
