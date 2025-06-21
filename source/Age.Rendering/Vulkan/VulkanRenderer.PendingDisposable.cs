namespace Age.Rendering.Vulkan;

public unsafe partial class VulkanRenderer
{
    private struct PendingDisposable(IDisposable disposable, ushort framesRemaining)
    {
        public IDisposable Disposable = disposable;
        public ushort FramesRemaining = framesRemaining;
    }
}
