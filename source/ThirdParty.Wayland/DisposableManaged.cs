namespace ThirdParty.Wayland;

public abstract class DisposableManaged<T> : Managed<T>, IDisposable
where T : unmanaged
{
    private bool disposed;

    internal DisposableManaged(Handle<T> handle) : base(handle)
    { }

    ~DisposableManaged() =>
        this.Dispose(disposing: false);

    private void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            this.OnDisposed(disposing);

            this.disposed = true;
        }
    }

    protected abstract void OnDisposed(bool disposing);

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }
}
