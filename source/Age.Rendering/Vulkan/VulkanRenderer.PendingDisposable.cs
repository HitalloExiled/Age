namespace Age.Rendering.Vulkan;

public partial class VulkanRenderer
{
    private struct PendingDisposable(IDisposable disposable, ushort framesRemaining)
    {
        public IDisposable Disposable = disposable;
        public ushort FramesRemaining = framesRemaining;
    }
}
