using Age.Rendering.Services;
using Age.Rendering.Storage;

namespace Age.Rendering;

public class Container : IDisposable
{
    private bool disposed;

    private static Container? singleton;

    public static Container Singleton => singleton ?? throw new NullReferenceException($"{nameof(Container)} not initialized");

    public required RenderingService RenderingService { get; init; }
    public required TextService      TextService      { get; init; }
    public required TextureStorage   TextureStorage   { get; init; }

    public Container()
    {
        if (singleton != null)
        {
            throw new InvalidOperationException($"{nameof(Container)} already initialized");
        }

        singleton = this;
    }

    public void Dispose()
    {
        if (!this.disposed)
        {
            this.RenderingService.Dispose();
            this.TextService.Dispose();
            this.TextureStorage.Dispose();

            this.disposed = true;
        }

        GC.SuppressFinalize(this);
    }
}
