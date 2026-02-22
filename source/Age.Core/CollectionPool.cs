namespace Age.Core;

public abstract class CollectionPool<T, U> where T : ICollection<U>
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
            item.Clear();

            this.entries.Push(item);
        }
    }
}
