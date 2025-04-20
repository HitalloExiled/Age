using Age.Core;

namespace Age.Rendering.Resources;

public abstract class Resource : Disposable
{
    internal List<ResourceCache.Entry> Dependencies { get; } = [];

    protected override void OnDisposed(bool disposing)
    {
        if (disposing)
        {
            this.Disposed();

            foreach (var dependecy in this.Dependencies)
            {
                dependecy.Dispose();
            }

            this.Dependencies.Clear();
        }
    }

    protected abstract void Disposed();
}

public abstract class Resource<T> : Resource
{
    public abstract T Instance { get; }

    public static implicit operator T(Resource<T> value) => value.Instance;
}
