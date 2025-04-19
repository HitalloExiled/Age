using Age.Core;

namespace Age.Commands;

public static partial class CommandPool
{
    public sealed class TextCommandPool : ObjectPool<TextCommand>
    {
        protected override TextCommand Create() => new();
    }
}
