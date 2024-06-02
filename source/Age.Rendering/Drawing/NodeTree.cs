using Age.Numerics;
using Age.Rendering.Commands;
using Age.Rendering.Interfaces;

namespace Age.Rendering.Drawing;

public sealed class NodeTree(IWindow window) : Node
{
    private readonly List<CommandEntry> commandEntriesCache = [];

    public record struct CommandEntry(Matrix3x2<float> Transform, DrawCommand Command);

    internal List<Node> Nodes { get; } = [];

    public override string NodeName { get; } = nameof(NodeTree);

    public IWindow Window => window;

    internal IEnumerable<CommandEntry> EnumerateCommands()
    {
        if (this.commandEntriesCache.Count > 0)
        {
            for (var i = 0; i < this.commandEntriesCache.Count; i++)
            {
                yield return this.commandEntriesCache[i];
            }
        }
        else
        {
            foreach (var node in this.Window.Tree.Traverse<Node2D>(true))
            {
                var transform = (Matrix3x2<float>)node.TransformCache;

                foreach (var command in node.Commands)
                {
                    var entry = new CommandEntry(transform, command);

                    this.commandEntriesCache.Add(entry);

                    yield return entry;
                }
            }
        }
    }

    internal void ResetCache() =>
        this.commandEntriesCache.Clear();
}
