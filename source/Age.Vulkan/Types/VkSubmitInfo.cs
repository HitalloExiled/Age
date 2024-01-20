using Age.Vulkan.Enums;
using Age.Vulkan.Flags;

namespace Age.Vulkan.Types;

/// <summary>
/// <para>Structure specifying a queue submit operation.</para>
/// <para>The order that command buffers appear in pCommandBuffers is used to determine <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-submission-order">submission order</see>, and thus all the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-implicit">implicit ordering guarantees</see> that respect it. Other than these implicit ordering guarantees and any <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization">explicit synchronization primitives</see>, these command buffers may overlap or otherwise execute out of order.</para>
/// </summary>
public unsafe struct VkSubmitInfo
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
    /// The number of semaphores upon which to wait before executing the command buffers for the batch.
    /// </summary>
    public uint waitSemaphoreCount;

    /// <summary>
    /// A pointer to an array of <see cref="VkSemaphore"/> handles upon which to wait before the command buffers for this batch begin execution. If semaphores to wait on are provided, they define a <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-semaphores-waiting">semaphore wait operation</see>.
    /// </summary>
    public VkSemaphore* pWaitSemaphores;

    /// <summary>
    /// A pointer to an array of pipeline stages at which each corresponding semaphore wait will occur.
    /// </summary>
    public VkPipelineStageFlags* pWaitDstStageMask;

    /// <summary>
    /// The number of command buffers to execute in the batch.
    /// </summary>
    public uint commandBufferCount;

    /// <summary>
    /// A pointer to an array of VkCommandBuffer handles to execute in the batch.
    /// </summary>
    public VkCommandBuffer* pCommandBuffers;

    /// <summary>
    /// The number of semaphores to be signaled once the commands specified in pCommandBuffers have completed execution.
    /// </summary>
    public uint signalSemaphoreCount;

    /// <summary>
    /// A pointer to an array of VkSemaphore handles which will be signaled when the command buffers for this batch have completed execution. If semaphores to be signaled are provided, they define a semaphore signal operation.
    /// </summary>
    public VkSemaphore* pSignalSemaphores;

    public VkSubmitInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_SUBMIT_INFO;
}
