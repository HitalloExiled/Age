using System.Collections.Concurrent;

namespace Age.Core;

public class ObjectPool<T>(Func<T> generator) where T : class
{
    private readonly ConcurrentBag<T> entries = new();

    public T Get() => entries.TryTake(out T? item) ? item : generator.Invoke();

    public void Return(T item) => entries.Add(item);
}
