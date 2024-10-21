namespace Age.Rendering.Resources;

public abstract class Resource : IDisposable
{
    internal List<ResourceCache.Entry> Dependencies { get; } = [];

    private bool disposed;


    protected abstract void Disposed();

    public void Dispose()
    {
        if (!this.disposed)
        {
            this.disposed = true;

            this.Disposed();

            foreach (var dependecy in this.Dependencies)
            {
                dependecy.Dispose();
            }

            this.Dependencies.Clear();
        }

        GC.SuppressFinalize(this);
    }
}

public abstract class Resource<T> : Resource
{
    public abstract T Instance { get; }

    public static implicit operator T(Resource<T> value) => value.Instance;
}
