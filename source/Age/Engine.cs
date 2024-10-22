using System.Diagnostics;
using Age.Core;
using Age.Internal;
using Age.Numerics;
using Age.Rendering.Vulkan;
using Age.RenderPasses;
using Age.Scene;
using Age.Services;
using Age.Storage;
using ThirdParty.Vulkan.Flags;

namespace Age;

public class Engine : Disposable
{
    private const bool   FPS_LOCKED        = false;
    private const ushort TARGET_FPS        = 60;
    private const double TARGET_FRAME_TIME = 1000.0 / TARGET_FPS;

    private readonly VulkanRenderer   renderer  = new();
    private readonly RenderingService renderingService;

    public Window Window { get; }

    private readonly TextService    textService;
    private readonly TextureStorage textureStorage;
    private readonly ShaderStorage  shaderStorage;

    public bool Running { get; private set; }

    public Engine(string name, Size<uint> windowSize, Point<int> windowPosition)
    {
        Window.Register(this.renderer);

        this.Window           = new Window(name, windowSize, windowPosition);
        this.renderingService = new RenderingService(this.renderer);
        this.shaderStorage    = new ShaderStorage(this.renderer);
        this.textService      = new TextService(this.renderer);
        this.textureStorage   = new TextureStorage(this.renderer);

        var canvasIndexRenderGraphPass = new CanvasIndexRenderGraphPass(this.renderer, this.Window);

        this.Window.Resized += () =>
        {
            var canvasIndexImage = canvasIndexRenderGraphPass.ColorImage;
            Common.SaveImage(canvasIndexImage, VkImageAspectFlags.Color, "CanvasIndex.png");
        };

        var renderGraph = new RenderGraph
        {
            Name   = "Default",
            Passes =
            [
                canvasIndexRenderGraphPass,
                new SceneRenderGraphPass(this.renderer, this.Window),
                new CanvasRenderGraphPass(this.renderer, this.Window),
            ]
        };

        RenderGraph.Active = renderGraph;

        this.renderingService.RegisterRenderGraph(this.Window, renderGraph);

        this.Window.Resized += this.renderingService.RequestDraw;

        Input.ListenInputEvents(this.Window);
    }

    protected override void Disposed(bool disposing)
    {
        if (disposing)
        {
            Platforms.Display.Window.CloseAll();

            this.renderingService.Dispose();
            this.textService.Dispose();
            this.textureStorage.Dispose();
            this.shaderStorage.Dispose();
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
            window.Tree.Initialize();
        }

        var watch = Stopwatch.StartNew();

        while (this.Running)
        {
            Input.Update();

            frameTime += current - previous;

            if (!FPS_LOCKED || frameTime >= TARGET_FRAME_TIME)
            {
                Time.DeltaTime = frameTime / 1000 * Time.Scale;

                foreach (var window in Window.Windows)
                {
                    window.DoEvents();

                    if (!window.IsClosed)
                    {
                        window.Tree.ResetCache();
                        window.Tree.Update();

                        if (window.Tree.IsDirty)
                        {
                            this.renderingService.RequestDraw();
                            window.Tree.IsDirty = false;
                        }
                    }
                }

                this.renderingService.Render(Window.Windows);

                Time.Frames++;

                Node2D.CacheVersion++;
                Node3D.CacheVersion++;

                this.Running = Window.Windows.Any(x => !x.IsClosed);

                frameTime = 0;
            }

            previous = current;
            current  = watch.Elapsed.TotalMilliseconds;
        }

        watch.Stop();
    }
}
