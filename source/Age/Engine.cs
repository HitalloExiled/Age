using Age.Rendering.Drawing;
using Age.Rendering.Services;
using Age.Rendering.Vulkan;

namespace Age;

public class Engine : IDisposable
{
    private readonly Window           mainWindow;
    private readonly VulkanRenderer   renderer = new();
    private readonly RenderingService renderingService;
    private readonly TextService      textService;

    private bool disposed;

    public bool Running { get; private set; }

    public Engine()
    {
        Window.Register(this.renderer);

        this.mainWindow = new Window("Age", new(800, 600), new(800, 300));

        Singleton.RenderingService = this.renderingService = new(this.renderer);
        Singleton.TextService      = this.textService      = new(this.renderingService);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                Platforms.Display.Window.CloseAll();

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

        this.mainWindow.Content.Add(new Text("Hello\nWorld\n!!!", new() { FontSize = 100, Position = new(100, -100) }));
        this.mainWindow.Content.Add(new Text("Hello World!!!",    new() { FontSize = 50,  Position = new(50, -400) }));

        while (this.Running)
        {
            Window.DoEventsAll();

            this.renderer.BeginFrame();

            foreach (var window in Window.Windows)
            {
                if (!window.Closed && !window.Minimized)
                {
                    window.Content.Update();
                    this.renderingService.Render(window);
                }
            }

            this.renderer.EndFrame();

            this.Running = Window.Windows.Any(x => !x.Closed);
        }
    }
}
