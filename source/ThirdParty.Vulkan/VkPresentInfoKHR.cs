using ThirdParty.Vulkan.Enums;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPresentInfoKHR.html">VkPresentInfoKHR</see>
/// </summary>
public unsafe struct VkPresentInfoKHR
{
    public readonly VkStructureType SType;

    public void*                     PNext;
    public uint                      WaitSemaphoreCount;
    public VkHandle<VkSemaphore>*    PWaitSemaphores;
    public uint                      SwapchainCount;
    public VkHandle<VkSwapchainKHR>* PSwapchains;
    public uint*                     PImageIndices;
    public VkResult*                 PResults;

    public VkPresentInfoKHR() =>
        this.SType = VkStructureType.PresentInfoKHR;
}
