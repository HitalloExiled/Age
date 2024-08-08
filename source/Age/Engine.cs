using System.Diagnostics;
using Age.Numerics;
using Age.Rendering.Resources;
using Age.Rendering.Vulkan;
using Age.RenderPasses;
using Age.Scene;
using Age.Services;
using Age.Storage;
using SkiaSharp;
using ThirdParty.Vulkan.Flags;

namespace Age;

public class Engine : IDisposable
{
    private const bool   FPS_LOCKED        = false;
    private const ushort TARGET_FPS        = 60;
    private const double TARGET_FRAME_TIME = 1000.0 / TARGET_FPS;

    private readonly VulkanRenderer   renderer  = new();
    private readonly RenderingService renderingService;

    private bool disposed;

    public Window Window { get; }

    private readonly TextService textService;
    private readonly TextureStorage textureStorage;
    private readonly ShaderStorage shaderStorage;

    public bool Running { get; private set; }

    public Engine(string name, Size<uint> windowSize, Point<int> windowPosition)
    {
        Window.Register(this.renderer);

        this.renderingService = new RenderingService(this.renderer);

        this.Window           = new Window(name, windowSize, windowPosition);
        this.textService      = new TextService(this.renderer);
        this.textureStorage   = new TextureStorage(this.renderer);
        this.shaderStorage    = new ShaderStorage(this.renderer);

        var canvasIndexRenderGraphPass = new CanvasIndexRenderGraphPass(this.renderer, this.Window);

        this.Window.SizeChanged += () =>
        {
            var canvasIndexImage = canvasIndexRenderGraphPass.ColorImage;
            SaveImage(canvasIndexImage, VkImageAspectFlags.Color, "./.debug/CanvasIndex.png");
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

        this.Window.SizeChanged  += this.renderingService.RequestDraw;
        this.Window.WindowClosed += this.Window.Tree.Destroy;

        Input.ListenInputEvents(this.Window);
    }

    private static void SaveImage(Image image, VkImageAspectFlags aspectMask, string filename)
    {
        var data = image.ReadBuffer(aspectMask);

        static SKColor convert(uint value) => new(value);

        var pixels = data.Select(convert).ToArray();

        var bitmap = new SKBitmap((int)image.Extent.Width, (int)image.Extent.Height)
        {
            Pixels = pixels
        };

        var skimage = SKImage.FromBitmap(bitmap);

        try
        {
            using var stream = File.OpenWrite(Path.Join(Directory.GetCurrentDirectory(), filename));

            skimage.Encode(SKEncodedImageFormat.Png, 100).SaveTo(stream);
        }
        catch
        {

        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
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
            Input.Update();

            frameTime += current - previous;

            if (!FPS_LOCKED || frameTime >= TARGET_FRAME_TIME)
            {
                var deltaTime = frameTime / 1000;

                foreach (var window in Window.Windows)
                {
                    window.DoEvents();
                    window.Tree.ResetCache();
                    window.Tree.Update(deltaTime);

                    if (window.Tree.IsDirty)
                    {
                        this.renderingService.RequestDraw();
                        window.Tree.IsDirty = false;
                    }
                }

                this.renderingService.Render(Window.Windows);

                Node2D.CacheVersion++;
                Node3D.CacheVersion++;

                this.Running = Window.Windows.Any(x => !x.Closed);

                frameTime = 0;
            }

            previous = current;
            current  = watch.Elapsed.TotalMilliseconds;
        }

        watch.Stop();
    }
}
