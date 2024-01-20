namespace Age.Platforms;

public partial class Library
{
    private bool disposed;

    public bool IsLoaded => PlatformIsLoaded();

    ~Library() =>
        this.Dispose(false);

    protected virtual void Dispose(bool disposing) =>
        PlatformDispose(disposing);

    public nint GetProcAddress(string proc) =>
        PlatformGetProcAddress(proc);

    public T? GetProcAddress<T>(string proc) where T : Delegate =>
        PlatformGetProcAddress<T>(proc);

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }
}
