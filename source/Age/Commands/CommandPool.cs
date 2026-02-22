using Age.Numerics;
using Age.Scenes;

namespace Age.Commands;

public abstract class CommandPool<TCommand, TNode>
where TCommand : Command<TNode>, new()
where TNode    : Node
{
    private readonly Lock @lock = new();
    private readonly Stack<TCommand> entries = [];

    public TCommand Get(TNode owner, CommandFilter commandFilter = default)
    {
        lock (this.@lock)
        {
            var command = this.entries.Count == 0 ? new() : this.entries.Pop();

            command.Owner         = owner;
            command.CommandFilter = commandFilter;

            return command;
        }
    }

    public void Return(TCommand item)
    {
        lock (this.@lock)
        {
            item.Reset();
            this.entries.Push(item);
        }
    }
}

public static class CommandPool
{
    public sealed class MeshCommandPool : CommandPool<MeshCommand, Spatial<Command3D, Matrix4x4<float>>>;
    public sealed class RectCommandPool : CommandPool<RectCommand, Spatial<Command2D, Matrix3x2<float>>>;
    public sealed class TextCommandPool : CommandPool<TextCommand, Spatial<Command2D, Matrix3x2<float>>>;

    public static MeshCommandPool MeshCommand { get; } = new();
    public static RectCommandPool RectCommand { get; } = new();
    public static TextCommandPool TextCommand { get; } = new();
}
