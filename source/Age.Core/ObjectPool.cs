using Age.Core.Interfaces;

namespace Age.Core;

public class ObjectPool<T> where T : class, IPoolable, new()
{
    private readonly Lock @lock = new();
    private readonly Stack<T> entries = [];

    public T Get()
    {
        lock (this.@lock)
        {
            return this.entries.Count == 0 ? new T() : this.entries.Pop();
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
