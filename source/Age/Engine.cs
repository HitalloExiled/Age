using System.Diagnostics;
using Age.Numerics;
using Age.Rendering;
using Age.Rendering.Drawing;
using Age.Rendering.RenderPasses;
using Age.Rendering.Services;
using Age.Rendering.Storage;
using Age.Rendering.Vulkan;
using SkiaSharp;

namespace Age;

public class Engine : IDisposable
{
    private const bool   FPS_LOCKED        = false;
    private const ushort TARGET_FPS        = 60;
    private const double TARGET_FRAME_TIME = 1000.0 / TARGET_FPS;

    private readonly Container      container;
    private readonly VulkanRenderer renderer  = new();

    private bool disposed;

    public Window Window { get; }
    public bool Running { get; private set; }

    public Engine(string name, Size<uint> windowSize, Point<int> windowPosition)
    {
        Window.Register(this.renderer);

        this.Window = new Window(name, windowSize, windowPosition);

        var textureStorage   = new TextureStorage(this.renderer);
        var renderingService = new RenderingService(this.renderer);
        var textService      = new TextService(this.renderer, textureStorage);

        var canvasIdRenderGraphPass = new CanvasIndexRenderGraphPass(this.renderer, this.Window);

        this.Window.SizeChanged += () =>
        {
            var image = canvasIdRenderGraphPass.Image;
            var data  = canvasIdRenderGraphPass.Image.ReadBuffer();

            static SKColor convert(uint value) => new(value);

            var pixels = data.Select(convert).ToArray();

            var bitmap = new SKBitmap((int)image.Extent.Width, (int)image.Extent.Height)
            {
                Pixels = pixels
            };

            var skimage = SKImage.FromBitmap(bitmap);

            try
            {
                using var stream = File.OpenWrite(Path.Join(Directory.GetCurrentDirectory(), "CanvasIndex.png"));

                skimage.Encode(SKEncodedImageFormat.Png, 100).SaveTo(stream);
            }
            catch
            {

            }
        };

        var renderGraph = new RenderGraph
        {
            Name   = "Canvas",
            Passes =
            [
                canvasIdRenderGraphPass,
                new CanvasRenderGraphPass(this.renderer, this.Window, textureStorage),
            ]
        };

        renderingService.RegisterRenderGraph(this.Window, renderGraph);

        this.container = new()
        {
            RenderingService = renderingService,
            TextService      = textService,
            TextureStorage   = textureStorage,
        };

        this.Window.SizeChanged += this.container.RenderingService.RequestDraw;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                Platforms.Display.Window.CloseAll();

                this.container.Dispose();
                this.renderer.Dispose();
            }

            this.disposed = true;
        }
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void Run()
    {
        this.Running = true;

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
            frameTime += current - previous;

            if (!FPS_LOCKED || frameTime >= TARGET_FRAME_TIME)
            {
                var deltaTime = frameTime / 1000;

                foreach (var window in Window.Windows)
                {
                    window.DoEvents();
                    window.Tree.ResetCache();
                    window.Tree.Update(deltaTime);
                }

                Node2D.CacheVersion++;

                this.container.RenderingService.Render(Window.Windows);

                this.Running = Window.Windows.Any(x => !x.Closed);

                frameTime = 0;
            }

            previous = current;
            current  = watch.Elapsed.TotalMilliseconds;
        }

        watch.Stop();
    }
}
