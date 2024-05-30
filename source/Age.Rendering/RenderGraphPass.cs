using Age.Rendering.Interfaces;
using Age.Rendering.Vulkan;

namespace Age.Rendering;

public abstract class RenderGraphPass(VulkanRenderer renderer, IWindow window) : Disposable
{
    protected VulkanRenderer Renderer { get; } = renderer;
    protected IWindow        Window { get; }   = window;

    protected abstract void Create();
    protected abstract void Destroy();
    public abstract void Execute();

    public void Recreate()
    {
        this.Destroy();
        this.Create();
    }

    protected override void OnDispose() =>
        this.Destroy();
}
