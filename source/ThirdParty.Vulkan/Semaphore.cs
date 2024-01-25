namespace ThirdParty.Vulkan;

public partial class Semaphore : DisposableNativeHandle
{
    protected override void OnDispose() => throw new NotImplementedException();
}
