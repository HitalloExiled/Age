using System.Diagnostics;
using Age.Numerics;
using Age.Rendering;
using Age.Rendering.RenderPasses;
using Age.Rendering.Resources;
using Age.Rendering.Scene;
using Age.Rendering.Services;
using Age.Rendering.Storage;
using Age.Rendering.Vulkan;
using Age.Services;
using SkiaSharp;
using ThirdParty.Vulkan.Flags;

namespace Age;

public class Engine : IDisposable
{
    private const bool   FPS_LOCKED        = false;
    private const ushort TARGET_FPS        = 60;
    private const double TARGET_FRAME_TIME = 1000.0 / TARGET_FPS;

    private readonly Container        container;
    private readonly VulkanRenderer   renderer  = new();
    private readonly RenderingService renderingService;

    private bool    disposed;
    private Texture viewportTexture;

    public Window Window { get; }
    public bool Running { get; private set; }

    public Engine(string name, Size<uint> windowSize, Point<int> windowPosition)
    {
        Window.Register(this.renderer);

        this.renderingService = new RenderingService(this.renderer);
        this.Window           = new Window(name, windowSize, windowPosition);

        var textService    = new TextService(this.renderer);
        var textureStorage = new TextureStorage(this.renderer);
        var shaderStorage  = new ShaderStorage(this.renderer);

        var canvasIndexRenderGraphPass = new CanvasIndexRenderGraphPass(this.renderer, this.Window);
        var geometryRenderGraphPass    = new GeometryRenderGraphPass(this.renderer, this.Window);

        this.Window.SizeChanged += () =>
        {
            var canvasIndexImage   = canvasIndexRenderGraphPass.ColorImage;
            // var geometryColorImage = geometryRenderGraphPass.ColorImage;
            // var geometryDepthImage = geometryRenderGraphPass.DepthImage;

            // SaveImage(geometryColorImage, VkImageAspectFlags.Color, "./.debug/Geometry.Color.png");
            // SaveImage(geometryDepthImage, VkImageAspectFlags.Depth, "./.debug/Geometry.Depth.png");
            SaveImage(canvasIndexImage,   VkImageAspectFlags.Color, "./.debug/CanvasIndex.png");
        };

        var renderGraph = new RenderGraph
        {
            Name   = "Default",
            Passes =
            [
                geometryRenderGraphPass,
                canvasIndexRenderGraphPass,
                new SceneRenderGraphPass(this.renderer, this.Window),
                new CanvasRenderGraphPass(this.renderer, this.Window),
            ]
        };

        RenderGraph.Active = renderGraph;

        this.renderingService.RegisterRenderGraph(this.Window, renderGraph);

        this.container = new()
        {
            TextService    = textService,
            TextureStorage = textureStorage,
            ShaderStorage  = shaderStorage,
        };

        this.Window.SizeChanged  += this.renderingService.RequestDraw;
        this.Window.WindowClosed += this.Window.Tree.Destroy;

        var viewport = new ViewportOld
        {
            Texture = this.viewportTexture = this.renderer.CreateTexture(geometryRenderGraphPass.ColorImage, false),
        };

        geometryRenderGraphPass.Recreated += () =>
        {
            this.viewportTexture.Dispose();

            viewport.Texture = this.viewportTexture = this.renderer.CreateTexture(geometryRenderGraphPass.ColorImage, false);
        };

        this.Window.Tree.AppendChild(viewport);
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

                this.viewportTexture.Dispose();
                this.renderingService.Dispose();
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
                    window.Tree.IsDirty = false;

                    window.DoEvents();
                    window.Tree.ResetCache();
                    window.Tree.Update(deltaTime);

                    if (window.Tree.IsDirty)
                    {
                        this.renderingService.RequestDraw();
                    }
                }

                Node2D.CacheVersion++;
                Node3D.CacheVersion++;

                this.renderingService.Render(Window.Windows);

                this.Running = Window.Windows.Any(x => !x.Closed);

                frameTime = 0;
            }

            previous = current;
            current  = watch.Elapsed.TotalMilliseconds;
        }

        watch.Stop();
    }
}
