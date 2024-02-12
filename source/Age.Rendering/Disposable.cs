namespace Age.Rendering;

public abstract record Disposable : IDisposable
{
    private bool disposed;

    protected abstract void OnDispose();

    public void Dispose()
    {
        if (!this.disposed)
        {
            this.disposed = true;

            this.OnDispose();
        }

        GC.SuppressFinalize(this);
    }
}
