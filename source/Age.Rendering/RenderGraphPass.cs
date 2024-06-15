using Age.Rendering.Interfaces;
using Age.Rendering.Resources;
using Age.Rendering.Vulkan;

namespace Age.Rendering;

public abstract class RenderGraphPass(VulkanRenderer renderer, IWindow window) : Disposable
{
    public event Action? Recreated;

    protected VulkanRenderer Renderer { get; } = renderer;
    protected IWindow        Window   { get; } = window;

    public abstract RenderPass RenderPass { get; }

    public bool Disabled { get; set; }

    public abstract void Execute();
    public abstract void Recreate();

    public void NotifyRecreated() =>
        this.Recreated?.Invoke();
}
