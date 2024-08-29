using Age.Core;
using Age.Rendering.Vulkan;

namespace Age.Services;

internal partial class RenderingService : Disposable
{
    private static RenderingService? singleton;

    public static RenderingService Singleton => singleton ?? throw new NullReferenceException();

    private readonly VulkanRenderer                  renderer;
    private readonly Dictionary<Window, RenderGraph> renderGraphs = [];

    private int changes;

    public RenderingService(VulkanRenderer renderer)
    {
        if (singleton != null)
        {
            throw new InvalidOperationException($"Only one single instace of {nameof(RenderingService)} is allowed");
        }

        singleton = this;

        this.renderer = renderer;
        this.renderer.SwapchainRecreated += this.OnSwapchainRecreated;
    }

    private void RequestDrawIncremental() =>
        this.changes++;

    private void OnSwapchainRecreated()
    {
        this.RequestDrawIncremental();

        foreach (var renderGraph in this.renderGraphs.Values)
        {
            renderGraph.Recreate();
        }
    }

    protected override void Disposed() =>
        this.renderer.DeferredDispose(this.renderGraphs.Values);

    public void RegisterRenderGraph(Window window, RenderGraph renderGraph) =>
        this.renderGraphs[window] = renderGraph;

    public void RequestDraw()
    {
        if (this.changes == 0)
        {
            this.changes++;
        }
    }

    public void Render(IEnumerable<Window> windows)
    {
        if (this.changes > 0)
        {
            this.renderer.BeginFrame();

            foreach (var window in windows)
            {
                if (window.IsVisible && !window.IsMinimized && !window.IsClosed)
                {
                    this.renderGraphs[window].Execute();
                }
            }

            this.renderer.EndFrame();

            this.changes--;
        }
    }
}
