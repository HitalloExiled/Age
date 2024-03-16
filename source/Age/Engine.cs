using System.Diagnostics;
using Age.Rendering;
using Age.Rendering.Drawing;
using Age.Rendering.Services;
using Age.Rendering.Storage;
using Age.Rendering.Vulkan;

namespace Age;

public class Engine : IDisposable
{
    private const bool   FPS_UNLOCKED = true;
    private const ushort TARGET_FPS   = 60;

    private readonly Container      container;
    private readonly Window         mainWindow;
    private readonly VulkanRenderer renderer = new();
    private readonly double         targetFrameTime = 1000.0 / TARGET_FPS;

    private bool disposed;

    public bool Running { get; private set; }

    public Engine()
    {
        Window.Register(this.renderer);

        this.mainWindow = new Window("Age", new(800, 600), new(800, 300));

        var renderingService = new RenderingService(this.renderer);
        var textService      = new TextService(renderingService);
        var textureStorage   = new TextureStorage(this.renderer);


        this.container = new()
        {
            RenderingService = renderingService,
            TextService      = textService,
            TextureStorage   = textureStorage,
        };

        this.mainWindow.SizeChanged += this.container.RenderingService.RequestDraw;
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

        var clockText = new Text("", new() { FontSize = 24, Color = new(1, 0, 0), Position = new(4, -4) });

        this.mainWindow.Content.Add(clockText);
        // this.mainWindow.Content.Add(new Text("H", new() { FontSize = 200, Color = new(0, 1, 0), Position = new(10, -10) }));
        this.mainWindow.Content.Add(new Text("Hello\nWorld\n!!!", new() { FontSize = 200, Color = new(0, 1, 0), Position = new(100, -200) }));
        this.mainWindow.Content.Add(new Text("Hello World!!!",    new() { FontSize = 50,  Color = new(0, 0, 1), Position = new(50, -500) }));

        var frames       = 0ul;
        var minFps       = double.MaxValue;
        var maxFps       = 0.0;
        var totalFps     = 0.0;
        var maxFrameTime = 0.0;
        var minFrameTime = double.MaxValue;

        var watch = new Stopwatch();

        while (this.Running)
        {
            watch.Restart();

            Platforms.Display.Window.DoEventsAll();

            this.container.RenderingService.Render(Window.Windows);

            var totalMilliseconds = watch.Elapsed.TotalMilliseconds;

            if (!FPS_UNLOCKED && totalMilliseconds < this.targetFrameTime)
            {
                var remaining = this.targetFrameTime - totalMilliseconds;

                Thread.Sleep(new TimeSpan((long)(remaining * TimeSpan.TicksPerMillisecond) / 2));
            }

            var fps       = Math.Round(1000.0 / watch.Elapsed.TotalMilliseconds, 2);
            var frameTime = Math.Round(watch.Elapsed.TotalMilliseconds, 2);
            var avgFps    = Math.Round(totalFps / frames, 2);

            totalFps += fps;

            maxFps = Math.Max(maxFps, fps);
            minFps = Math.Min(minFps, fps);

            maxFrameTime = Math.Max(maxFrameTime, frameTime);
            minFrameTime = Math.Min(minFrameTime, frameTime);

            clockText.Value =
                $"""
                Frames: {frames}
                FPS: {fps}
                    Avg: {avgFps}
                    Min: {minFps}
                    Max: {maxFps}

                FrameTime: {frameTime}ms
                    Min: {minFrameTime}ms
                    Max: {maxFrameTime}ms
                """;

            frames++;

            this.Running = Window.Windows.Any(x => !x.Closed);
        }
    }
}
