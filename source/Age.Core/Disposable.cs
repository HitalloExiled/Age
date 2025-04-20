namespace Age.Core;

public abstract class Disposable : IDisposable
{
    private bool disposed;

    ~Disposable() =>
        this.Dispose(false);

    protected void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            this.OnDisposed(disposing);
            this.disposed = true;
        }
    }

    protected abstract void OnDisposed(bool disposing);

    protected void ThrowIfDisposed() =>
        ObjectDisposedException.ThrowIf(this.disposed, this);

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }
}
