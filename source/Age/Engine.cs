using System.Diagnostics;
using Age.Numerics;
using Age.Rendering;
using Age.Rendering.Services;
using Age.Rendering.Storage;
using Age.Rendering.Vulkan;

namespace Age;

public class Engine : IDisposable
{
    private const bool   FPS_UNLOCKED = true;
    private const ushort TARGET_FPS   = 60;

    private readonly Container      container;
    private readonly VulkanRenderer renderer = new();
    private readonly double         targetFrameTime = 1000.0 / TARGET_FPS;

    private bool disposed;

    public Window Window { get; }
    public bool Running { get; private set; }

    public Engine(string name, Size<uint> windowSize, Point<int> windowPosition)
    {
        Window.Register(this.renderer);

        this.Window = new Window(name, windowSize, windowPosition);

        var textureStorage   = new TextureStorage(this.renderer);
        var renderingService = new RenderingService(this.renderer, textureStorage);
        var textService      = new TextService(renderingService, textureStorage);

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

        var watch = new Stopwatch();

        while (this.Running)
        {
            watch.Restart();

            this.container.RenderingService.Render(Window.Windows);

            var frameTime = watch.Elapsed.TotalMilliseconds;

            if (!FPS_UNLOCKED && frameTime < this.targetFrameTime)
            {
                var remaining = this.targetFrameTime - frameTime;

                Thread.Sleep(new TimeSpan((long)(remaining * TimeSpan.TicksPerMillisecond) / 2));
            }

            var deltaTime = watch.Elapsed.TotalMilliseconds / 1000;

            foreach (var window in Window.Windows)
            {
                window.DoEvents();
                window.Tree.Update(deltaTime);
            }

            this.Running = Window.Windows.Any(x => !x.Closed);

        }
    }
}
