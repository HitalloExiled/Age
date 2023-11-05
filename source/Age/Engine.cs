using Age.Platforms;
using Age.Rendering.Services;

namespace Age;

public class Engine : IDisposable
{
    private readonly Platform         platform;
    private readonly RenderingService renderingService;
    private readonly TextService      textService;

    private bool disposed;

    public bool Running { get; private set; }

    public Engine(Platform platform)
    {
        this.platform         = platform;
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

        this.textService.DrawText("Hello World!!!", 80, new(0, 0));
        this.textService.DrawText("Hello World!!!", 80, new(0, 100));
        this.textService.DrawText("aAg21!Â", 80, new(0, 200));

        while (this.Running)
        {
            this.platform.DoEvents();

            if (this.platform.CanDraw)
            {
                // this.textService.DrawText("World", 50, new(50, 50));
                // this.textService.DrawText(DateTime.Now.ToLongTimeString(), new(0, 0));
                this.renderingService.Draw();
            }

            this.Running = !this.platform.QuitRequested;
        }
    }
}
