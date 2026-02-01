using Age.Core;
using Age.Rendering.Vulkan;

namespace Age.Services;

internal class RenderingService : Disposable
{
    private static RenderingService? singleton;

    private readonly VulkanRenderer renderer;
    private readonly List<Window>   windows = [];

    private int changes;

    public static RenderingService Singleton => singleton ?? throw new NullReferenceException();

    public RenderingService(VulkanRenderer renderer)
    {
        if (singleton != null)
        {
            throw new InvalidOperationException($"Only one single instace of {nameof(RenderingService)} is allowed");
        }

        singleton = this;

        this.renderer = renderer;
    }

    public void RegisterWindow(Window window)
    {
        this.windows.Add(window);

        window.Surface.SwapchainRecreated += this.RequestDrawIncremental;
        window.Resized                    += this.RequestDraw;
    }

    public void UnregisterWindow(Window window)
    {
        this.windows.Remove(window);

        window.Surface.SwapchainRecreated -= this.RequestDrawIncremental;
        window.Resized                    -= this.RequestDraw;
    }

    private void RequestDrawIncremental() =>
        this.changes++;

    protected override void OnDisposed(bool disposing)
    {
        foreach (var window in this.windows)
        {
            window.Surface.SwapchainRecreated -= this.RequestDrawIncremental;
            window.Resized                    -= this.RequestDraw;
        }
    }

    public void Render()
    {
        if (this.changes > 0)
        {
            this.renderer.BeginFrame();

            foreach (var window in Window.Windows)
            {
                var viewports = window.RenderTree.Viewports;

                for (var i = viewports.Length - 1; i > -1; i--)
                {
                    viewports[i].RenderGraph.Execute();
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
