using Age.Core;
using Age.Rendering.Vulkan;

namespace Age.Services;

internal partial class RenderingService : Disposable
{
    private readonly VulkanRenderer                  renderer;
    private readonly Dictionary<Window, RenderGraph> renderGraphs = [];

    private int changes;

    public RenderingService(VulkanRenderer renderer)
    {
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
                if (window.Visible && !window.Minimized && !window.Closed)
                {
                    this.renderGraphs[window].Execute();
                }
            }

            this.renderer.EndFrame();

            this.changes--;
        }
    }
}
