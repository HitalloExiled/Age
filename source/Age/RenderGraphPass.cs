using Age.Core;
using Age.Rendering.Resources;
using Age.Rendering.Vulkan;

namespace Age;

public abstract class RenderGraphPass(VulkanRenderer renderer, Window window) : Disposable
{
    public event Action? Recreated;

    protected VulkanRenderer Renderer { get; } = renderer;
    protected Window         Window   { get; } = window;

    public abstract RenderPass RenderPass { get; }

    public bool Disabled { get; set; }

    public abstract void Execute();
    public abstract void Recreate();

    public void NotifyRecreated() =>
        this.Recreated?.Invoke();
}
