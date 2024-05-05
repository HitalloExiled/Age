using Age.Rendering.Interfaces;

namespace Age.Rendering;

internal class Container : IDisposable
{
    private bool disposed;

    private static Container? singleton;

    public static Container Singleton => singleton ?? throw new NullReferenceException($"{nameof(Container)} not initialized");

    public required IRenderingService RenderingService { get; init; }
    public required ITextService      TextService      { get; init; }
    public required ITextureStorage   TextureStorage   { get; init; }

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
            this.TextService.Dispose();
            this.TextureStorage.Dispose();
            this.RenderingService.Dispose();

            this.disposed = true;
        }

        GC.SuppressFinalize(this);
    }
}
