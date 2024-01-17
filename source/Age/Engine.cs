using Age.Platforms;
using Age.Rendering.Display;
using Age.Rendering.Drawing;
using Age.Rendering.Services;

namespace Age;

public class Engine : IDisposable
{
    private readonly Platform         platform;
    private readonly RenderingService renderingService;
    private readonly TextService      textService;
    private readonly Window           window;

    private bool disposed;

    public bool Running { get; private set; }

    public Engine(Platform platform)
    {
        this.platform         = platform;
        this.window           = this.platform.CreateWindow("Age", 600, 400, 800, 300);
        this.renderingService = new(platform.Renderer);
        this.textService      = new(this.renderingService);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                this.textService.Dispose();
                this.renderingService.Dispose();
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

        this.window.Content.Add(new Label("Hello World!!!", new() { FontSize = 80, Position = new(0, 0) }));
        this.window.Content.Add(new Label("Hello World!!!", new() { FontSize = 80, Position = new(0, 100) }));

        while (this.Running)
        {
            this.platform.DoEvents();
            this.window.Content.Update();

            if (this.platform.CanDraw)
            {
                this.renderingService.Render(this.window);
            }

            this.Running = !this.window.Closed;
        }
    }
}
