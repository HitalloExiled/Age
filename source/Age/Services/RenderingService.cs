using Age.Core;
using Age.Rendering.Vulkan;

namespace Age.Services;

internal class RenderingService : Disposable
{
    private static RenderingService? singleton;

    private readonly VulkanRenderer                  renderer;
    private readonly Dictionary<Window, RenderGraph> renderGraphs = [];

    private int changes;

    public static RenderingService Singleton => singleton ?? throw new NullReferenceException();

    public RenderingService(Window window, VulkanRenderer renderer)
    {
        if (singleton != null)
        {
            throw new InvalidOperationException($"Only one single instace of {nameof(RenderingService)} is allowed");
        }

        singleton = this;

        this.renderer = renderer;
        window.Surface.SwapchainRecreated += this.OnSwapchainRecreated;
    }

    private void OnSwapchainRecreated()
    {
        this.RequestDrawIncremental();

        foreach (var renderGraph in this.renderGraphs.Values)
        {
            renderGraph.Recreate();
        }
    }

    private void RequestDrawIncremental() =>
        this.changes++;

    protected override void OnDisposed(bool disposing)
    {
        if (disposing)
        {
            this.renderer.DeferredDispose(this.renderGraphs.Values);
        }
    }

    public void RegisterRenderGraph(Window window, RenderGraph renderGraph) =>
        this.renderGraphs[window] = renderGraph;

    public void Render(IEnumerable<Window> windows)
    {
        if (this.changes > 0)
        {
            this.renderer.BeginFrame();

            foreach (var window in windows)
            {
                if (window.Surface.Visible)
                {
                    this.renderGraphs[window].Execute();
                }
            }

            this.renderer.EndFrame();

            Time.Redraws++;

            this.changes--;
        }
    }

    public void RequestDraw()
    {
        if (this.changes == 0)
        {
            this.changes++;
        }
    }
}
