using Age.Core.Interfaces;

namespace Age.Core;

public sealed class GenericPool<T>(Func<T> generator) : ObjectPool<T> where T : class, IPoolable
{
    protected override T Create() => generator.Invoke();
}
