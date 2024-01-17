using Age.Rendering.Display;
using Age.Rendering.Vulkan;

namespace Age.Platforms;

public abstract class Platform : IDisposable
{
    public abstract bool           CanDraw  { get; }
    public abstract VulkanRenderer Renderer { get; }

    public abstract Window CreateWindow(string title, uint width, uint height, int x, int y, Window? parent = null);
    public abstract void DestroyWindow(Window window);
    public abstract void Dispose();
    public abstract void DoEvents();
}
