using Age.Core.Interop;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Native.KHR;

namespace ThirdParty.Vulkan.KHR;

/// <inheritdoc cref="VkPresentInfoKHR" />
public unsafe record PresentInfo : NativeReference<VkPresentInfoKHR>
{
    private uint[]      imageIndices   = [];
    private Swapchain[] swapchains     = [];
    private Semaphore[] waitSemaphores = [];

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

    public Swapchain[] Swapchains
    {
        get => this.swapchains;
        init => Init(ref this.swapchains, ref this.PNative->pSwapchains, ref this.PNative->swapchainCount, value);
    }

    public uint[] ImageIndices
    {
        get => this.imageIndices;
        init => Init(ref this.imageIndices, ref this.PNative->pImageIndices, value);
    }

    public Result[] Results
    {
        get => PointerHelper.ToArray(this.PNative->pResults, this.PNative->swapchainCount);
        init => Init(ref this.PNative->pResults, value);
    }

    protected override void OnFinalize()
    {
        Free(ref this.PNative->pWaitSemaphores);
        Free(ref this.PNative->pSwapchains);
        Free(ref this.PNative->pImageIndices);
        Free(ref this.PNative->pResults);
    }
}
