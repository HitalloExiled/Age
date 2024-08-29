namespace Age.Rendering.Resources;

public abstract class Resource : IDisposable
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

public abstract class Resource<T> : Resource
{
    public T Value { get; }

    internal Resource(T value) =>
        this.Value = value;

    public static implicit operator T(Resource<T> value) => value.Value;
}
