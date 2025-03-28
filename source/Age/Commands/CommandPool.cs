using Age.Core;

namespace Age.Commands;

public static class CommandPool
{
    public static ObjectPool<MeshCommand> MeshCommand { get; } = new(static () => new());
    public static ObjectPool<RectCommand> RectCommand { get; } = new(static () => new());
    public static ObjectPool<TextCommand> TextCommand { get; } = new(static () => new());
}
