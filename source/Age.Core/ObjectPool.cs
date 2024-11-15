using System.Collections.Concurrent;

namespace Age.Core;

public sealed class ObjectPool<T>(Func<T> generator) where T : class
{
    private readonly ConcurrentBag<T> entries = [];

    public T Get() => this.entries.TryTake(out var item) ? item : generator.Invoke();

    public void Return(T item) => this.entries.Add(item);
}
