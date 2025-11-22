using Age.Core;
using Age.Numerics;
using Age.Rendering.Vulkan;
using Age.RenderPasses;
using Age.Services;
using Age.Storage;
using System.Diagnostics;

namespace Age;

public sealed class Engine : Disposable
{
    private const bool   FPS_LOCKED        = false;
    private const ushort TARGET_FPS        = 60;
    private const double TARGET_FRAME_TIME = 1000.0 / TARGET_FPS;

    private readonly VulkanRenderer   renderer  = new();
    private readonly RenderingService renderingService;

    public Window Window { get; }

    private readonly ShaderStorage  shaderStorage;
    private readonly TextStorage    textStorage;
    private readonly TextureStorage textureStorage;

    public bool Running { get; private set; }

    public Engine(string name, Size<uint> windowSize, Point<int> windowPosition)
    {
        this.Window           = new Window(name, windowSize, windowPosition);
        this.renderingService = new RenderingService(this.Window, this.renderer);
        this.shaderStorage    = new ShaderStorage(this.renderer);
        this.textStorage      = new TextStorage(this.renderer);
        this.textureStorage   = new TextureStorage(this.renderer);

        var renderGraph = new RenderGraph
        {
            Name   = "Default",
            Passes =
            [
                new CanvasEncodeRenderGraphPass(this.renderer, this.Window),
                new SceneRenderGraphPass(this.renderer, this.Window),
                new CanvasColorRenderGraphPass(this.renderer, this.Window),
            ]
        };

        RenderGraph.Active = renderGraph;

        this.renderingService.RegisterRenderGraph(this.Window, renderGraph);

        this.Window.Resized += this.renderingService.RequestDraw;

        Input.ListenInputEvents(this.Window);
    }

    protected override void OnDisposed(bool disposing)
    {
        if (disposing)
        {
            Platforms.Display.Window.CloseAll();

            this.renderingService.Dispose();
            this.shaderStorage.Dispose();
            this.textStorage.Dispose();
            this.textureStorage.Dispose();

            GC.Collect();

            this.renderer.Dispose();
        }
    }

    public void Run()
    {
        this.Running = true;

        Time.Start = DateTime.Now;

        var previous  = 0D;
        var frameTime = 0D;
        var current   = TARGET_FRAME_TIME;

        foreach (var window in Window.Windows)
        {
            window.RenderTree.Initialize();
        }

        var watch = Stopwatch.StartNew();

        while (this.Running)
        {
            Input.Update();

            frameTime += current - previous;

            if (!FPS_LOCKED || frameTime >= TARGET_FRAME_TIME)
            {
                Time.DeltaTime = frameTime / 1000 * Time.Scale;

                foreach (var window in Window.Windows.ToArray())
                {
                    window.DoEvents();

                    if (!window.IsClosed)
                    {
                        window.RenderTree.Update();

                        if (window.RenderTree.IsDirty)
                        {
                            this.renderingService.RequestDraw();

                            window.RenderTree.MakePristine();
                        }
                    }
                }

                this.renderingService.Render(Window.Windows);

                Time.Frames++;

                CacheTracker.Invalidate();

                this.Running = false;

                foreach (var window in Window.Windows)
                {
                    if (!window.IsClosed)
                    {
                        this.Running = true;

                        break;
                    }
                }

                frameTime = 0;
            }

            previous = current;
            current  = watch.Elapsed.TotalMilliseconds;
        }

        watch.Stop();
    }
}
