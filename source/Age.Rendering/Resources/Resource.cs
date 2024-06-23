using Age.Core.Extensions;

namespace Age.Rendering.Resources;

public abstract class Resource : IDisposable
{
    private bool disposed;

    public List<Resource> Dependencies { get; init; } = [];

    protected abstract void OnDispose();

    public void Dispose()
    {
        if (!this.disposed)
        {
            this.disposed = true;

            foreach (var dependency in this.Dependencies.AsSpan())
            {
                dependency.Dispose();
            }

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
