namespace ThirdParty.Vulkan.KHR;

public partial class Swapchain : DisposableNativeHandle
{
    protected override void OnDispose() => throw new NotImplementedException();

    public uint AcquireNextImage(ulong maxValue, Semaphore? semaphore, Fence? fence) => throw new NotImplementedException();
    public Image[] GetImages() => throw new NotImplementedException();
}
