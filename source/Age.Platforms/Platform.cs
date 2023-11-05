using Age.Rendering.Vulkan;

namespace Age.Platforms;

public abstract class Platform : IDisposable
{
    public abstract bool           CanDraw  { get; }
    public abstract VulkanRenderer Renderer { get; }

    public bool QuitRequested { get; protected set; }

    public abstract void Dispose();
    public abstract void DoEvents();
}
