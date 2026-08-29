using Age.Core;
using Age.Core.Interfaces;

namespace Age.Scenes;

public sealed class Empty : Node, IPoolable
{
    public static ObjectPool<Empty> Pool { get; } = new();

    public override string NodeName => nameof(Empty);

    public void Reset() { }
}
