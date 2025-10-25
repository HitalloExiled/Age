using Age.Core.Interfaces;

namespace Age.Core;

public record Ref<T> : IPoolable
{
    public T? Value { get; set; }

    public void Reset() =>
        this.Value = default;
}
