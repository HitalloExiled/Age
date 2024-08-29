namespace Age.Core;

public abstract class Disposable : IDisposable
{
    private bool disposed;

    protected abstract void Disposed();

    public void Dispose()
    {
        if (!this.disposed)
        {
            this.disposed = true;

            this.Disposed();
        }

        GC.SuppressFinalize(this);
    }
}
