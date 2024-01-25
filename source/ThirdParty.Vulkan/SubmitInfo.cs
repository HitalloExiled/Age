using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

/// <summary>
/// <para>Structure specifying a queue submit operation.</para>
/// <para>The order that command buffers appear in <see cref="CommandBuffers"/> is used to determine <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-submission-order">submission order</see>, and thus all the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-implicit">implicit ordering guarantees</see> that respect it. Other than these implicit ordering guarantees and any <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization">explicit synchronization primitives</see>, these command buffers may overlap or otherwise execute out of order.</para>
/// </summary>
public unsafe record SubmitInfo : NativeReference<VkSubmitInfo>
{
    private CommandBuffer[]      commandBuffers   = [];
    private Semaphore[]          signalSemaphores = [];
    private PipelineStageFlags[] waitDstStageMask = [];
    private Semaphore[]          waitSemaphores   = [];

    /// <summary>
    /// Structure extending this structure.
    /// </summary>
    public nint Next
    {
        get => (nint)this.PNative->pNext;
        init => this.PNative->pNext = value.ToPointer();
    }

    /// <summary>
    /// An array of handles upon which to wait before the command buffers for this batch begin execution. If semaphores to wait on are provided, they define a <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-semaphores-waiting">semaphore wait operation</see>.
    /// </summary>
    public Semaphore[] WaitSemaphores
    {
        get => this.waitSemaphores;
        init => Init(ref this.waitSemaphores, ref this.PNative->pWaitSemaphores, ref this.PNative->waitSemaphoreCount, value);
    }

    /// <summary>
    /// Array of pipeline stages at which each corresponding semaphore wait will occur.
    /// </summary>
    public PipelineStageFlags[] WaitDstStageMask
    {
        get => this.waitDstStageMask;
        init => Init(ref this.waitDstStageMask, ref this.PNative->pWaitDstStageMask, value);
    }

    /// <summary>
    /// An array of <see cref="CommandBuffer"/> handles to execute in the batch.
    /// </summary>
    public CommandBuffer[] CommandBuffers
    {
        get => this.commandBuffers;
        init => Init(ref this.commandBuffers, ref this.PNative->pCommandBuffers, ref this.PNative->commandBufferCount, value);
    }

    /// <summary>
    /// An array of <see cref="Semaphore"/> which will be signaled when the command buffers for this batch have completed execution. If semaphores to be signaled are provided, they define a semaphore signal operation.
    /// </summary>
    public Semaphore[] SignalSemaphores
    {
        get => this.signalSemaphores;
        init => Init(ref this.signalSemaphores, ref this.PNative->pSignalSemaphores, ref this.PNative->signalSemaphoreCount, value);
    }

    protected override void OnFinalize()
    {
        Free(ref this.PNative->pWaitSemaphores);
        Free(ref this.PNative->pWaitDstStageMask);
        Free(ref this.PNative->pCommandBuffers);
        Free(ref this.PNative->pSignalSemaphores);
    }
}
