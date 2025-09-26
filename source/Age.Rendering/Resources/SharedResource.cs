namespace Age.Rendering.Resources;

public sealed class SharedResource<T>(T resource) : IDisposable where T : IDisposable
{
    private bool disposed;

    public T Resource => resource;

    public int Users { get; private set; }

    public SharedResource<T> Share()
    {
        this.Users++;

        return this;
    }

    ~SharedResource() =>
        this.Dispose(false);

    private void Dispose(bool disposing)
    {
        if (!this.disposed && (!disposing || --this.Users == 0))
        {
            this.Resource.Dispose();
            this.disposed = true;
        }
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }
}
