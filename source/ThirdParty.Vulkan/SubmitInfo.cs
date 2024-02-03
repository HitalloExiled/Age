using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

/// <inheritdoc cref="VkSubmitInfo" />
public unsafe record SubmitInfo : NativeReference<VkSubmitInfo>
{
    private CommandBuffer[]      commandBuffers   = [];
    private Semaphore[]          signalSemaphores = [];
    private PipelineStageFlags[] waitDstStageMask = [];
    private Semaphore[]          waitSemaphores   = [];

    public nint Next
    {
        get => (nint)this.PNative->pNext;
        init => this.PNative->pNext = value.ToPointer();
    }

    public Semaphore[] WaitSemaphores
    {
        get => this.waitSemaphores;
        init => Init(ref this.waitSemaphores, ref this.PNative->pWaitSemaphores, ref this.PNative->waitSemaphoreCount, value);
    }

    public PipelineStageFlags[] WaitDstStageMask
    {
        get => this.waitDstStageMask;
        init => Init(ref this.waitDstStageMask, ref this.PNative->pWaitDstStageMask, value);
    }

    public CommandBuffer[] CommandBuffers
    {
        get => this.commandBuffers;
        init => Init(ref this.commandBuffers, ref this.PNative->pCommandBuffers, ref this.PNative->commandBufferCount, value);
    }

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
