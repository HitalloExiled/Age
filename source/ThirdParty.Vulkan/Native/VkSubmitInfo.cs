namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkSubmitInfo.html">VkSubmitInfo</see>
/// </summary>
public unsafe struct VkSubmitInfo
{
    public readonly VkStructureType sType;

    public void*                 pNext;
    public uint                  waitSemaphoreCount;
    public VkSemaphore*          pWaitSemaphores;
    public VkPipelineStageFlags* pWaitDstStageMask;
    public uint                  commandBufferCount;
    public VkCommandBuffer*      pCommandBuffers;
    public uint                  signalSemaphoreCount;
    public VkSemaphore*          pSignalSemaphores;

    public VkSubmitInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_SUBMIT_INFO;
}
