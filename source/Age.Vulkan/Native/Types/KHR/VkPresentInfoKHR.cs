using Age.Vulkan.Native.Enums;

namespace Age.Vulkan.Native.Types.KHR;

/// <summary>
/// <para>Structure describing parameters of a queue presentation.</para>
/// <para>Before an application can present an image, the image’s layout must be transitioned to the <see cref="VkImageLayout.VK_IMAGE_LAYOUT_PRESENT_SRC_KHR"/> layout, or for a shared presentable image the <see cref="VkImageLayout.VK_IMAGE_LAYOUT_SHARED_PRESENT_KHR"/> layout.</para>
/// <remarks>Note: When transitioning the image to <see cref="VkImageLayout.VK_IMAGE_LAYOUT_SHARED_PRESENT_KHR"/> or <see cref="VkImageLayout.VK_IMAGE_LAYOUT_PRESENT_SRC_KHR"/>, there is no need to delay subsequent processing, or perform any visibility operations (as <see cref="VkKhrSwapchain.QueuePresent"/> performs automatic visibility operations). To achieve this, the dstAccessMask member of the <see cref="VkImageMemoryBarrier"/> should be set to 0, and the dstStageMask parameter should be set to <see cref="VkPipelineStageFlagBits.VK_PIPELINE_STAGE_BOTTOM_OF_PIPE_BIT"/>.</remarks>
/// </summary>
/// <remarks>Provided by VK_KHR_swapchain</remarks>
public unsafe struct VkPresentInfoKHR
{
    /// <summary>
    /// A <see cref="VkStructureType"/> value identifying this structure.
    /// </summary>
    public readonly VkStructureType sType;

    /// <summary>
    /// Null or a pointer to a structure extending this structure.
    /// </summary>
    public void* pNext;

    /// <summary>
    /// The number of semaphores to wait for before issuing the present request. The number may be zero.
    /// </summary>
    public uint waitSemaphoreCount;

    /// <summary>
    /// Null or a pointer to an array of <see cref="VkSemaphore"/> objects with waitSemaphoreCount entries, and specifies the semaphores to wait for before issuing the present request.
    /// </summary>
    public VkSemaphore* pWaitSemaphores;

    /// <summary>
    /// The number of swapchains being presented to by this command.
    /// </summary>
    public uint swapchainCount;

    /// <summary>
    /// A pointer to an array of <see cref="VkSwapchainKHR"/> objects with swapchainCount entries.
    /// </summary>
    public VkSwapchainKHR* pSwapchains;

    /// <summary>
    /// A pointer to an array of indices into the array of each swapchain’s presentable images, with swapchainCount entries. Each entry in this array identifies the image to present on the corresponding entry in the pSwapchains array.
    /// </summary>
    public uint* pImageIndices;

    /// <summary>
    /// A pointer to an array of <see cref="VkResult"/> typed elements with swapchainCount entries. Applications that do not need per-swapchain results can use NULL for pResults. If non-NULL, each entry in pResults will be set to the <see cref="VkResult"/> for presenting the swapchain corresponding to the same index in pSwapchains.
    /// </summary>
    public VkResult* pResults;

    public VkPresentInfoKHR() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_PRESENT_INFO_KHR;
}
