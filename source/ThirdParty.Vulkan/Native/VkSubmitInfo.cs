using ThirdParty.Vulkan.Flags;

namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkSubmitInfo.html">VkSubmitInfo</see>
/// </summary>
public unsafe struct VkSubmitInfo
{
    public readonly VkStructureType SType;

    public void*                      PNext;
    public uint                       WaitSemaphoreCount;
    public VkHandle<VkSemaphore>*     PWaitSemaphores;
    public VkPipelineStageFlags*      PWaitDstStageMask;
    public uint                       CommandBufferCount;
    public VkHandle<VkCommandBuffer>* PCommandBuffers;
    public uint                       SignalSemaphoreCount;
    public VkHandle<VkSemaphore>*     PSignalSemaphores;

    public VkSubmitInfo() =>
        this.SType = VkStructureType.SubmitInfo;
}
