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
            this.Disposed(disposing);
            this.disposed = true;
        }
    }

    protected abstract void Disposed(bool disposing);

    protected void ThrowIfDisposed() =>
        ObjectDisposedException.ThrowIf(this.disposed, this);

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }
}
