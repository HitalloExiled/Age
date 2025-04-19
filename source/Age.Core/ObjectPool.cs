using Age.Core.Interfaces;

namespace Age.Core;

public abstract class ObjectPool<T> where T : class, IPoolable
{
    private readonly Lock @lock = new();
    private readonly Stack<T> entries = [];

    protected abstract T Create();

    public T Get()
    {
        lock (this.@lock)
        {
            return this.entries.Count == 0 ? this.Create() : this.entries.Pop();
        }
    }

    public void Return(T item)
    {
        lock (this.@lock)
        {
            item.Reset();
            this.entries.Push(item);
        }
    }
}
