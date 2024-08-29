namespace Age.Platforms;

public partial class Library : IDisposable
{
    private bool disposed;

    public bool IsLoaded => this.PlatformIsLoaded();

    ~Library() =>
        this.Dispose(false);

    protected virtual void Dispose(bool disposing) =>
        this.PlatformDispose(disposing);

    public nint GetProcAddress(string proc) =>
        this.PlatformGetProcAddress(proc);

    public T? GetProcAddress<T>(string proc) where T : Delegate =>
        this.PlatformGetProcAddress<T>(proc);

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }
}
