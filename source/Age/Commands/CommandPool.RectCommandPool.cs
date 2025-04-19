using Age.Core;

namespace Age.Commands;

public static partial class CommandPool
{
    public sealed class RectCommandPool : ObjectPool<RectCommand>
    {
        protected override RectCommand Create() => new();
    }
}
