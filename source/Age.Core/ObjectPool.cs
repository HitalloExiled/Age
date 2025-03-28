namespace Age.Core;

public interface IPoolable
{
    public void Reset();
}

public sealed class ObjectPool<T>(Func<T> generator) where T : class, IPoolable
{
    private readonly Lock @lock = new();
    private readonly Stack<T> entries = [];

    public T Get()
    {
        lock (this.@lock)
        {
            return this.entries.Count == 0 ? generator.Invoke() : this.entries.Pop();
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
