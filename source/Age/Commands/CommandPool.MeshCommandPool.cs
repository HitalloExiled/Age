using Age.Core;

namespace Age.Commands;

public static partial class CommandPool
{
    public sealed class MeshCommandPool : ObjectPool<MeshCommand>
    {
        protected override MeshCommand Create() => new();
    }
}
