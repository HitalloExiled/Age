using Age.Rendering.Interfaces;
using Age.Rendering.Vulkan;

namespace Age.Rendering.Services;

internal partial class RenderingService : IRenderingService
{
    private readonly VulkanRenderer                   renderer;
    private readonly Dictionary<IWindow, RenderGraph> renderGraphs = [];

    private int  changes;
    private bool disposed;

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

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                this.renderer.DeferredDispose(this.renderGraphs.Values);
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            this.disposed = true;
        }
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void RegisterRenderGraph(IWindow window, RenderGraph renderGraph) =>
        this.renderGraphs[window] = renderGraph;

    public void RequestDraw()
    {
        if (this.changes == 0)
        {
            this.changes++;
        }
    }

    public void Render(IEnumerable<IWindow> windows)
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
