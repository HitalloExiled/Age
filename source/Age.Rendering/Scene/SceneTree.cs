using Age.Numerics;
using Age.Rendering.Commands;
using Age.Rendering.Interfaces;

namespace Age.Rendering.Scene;

public sealed class SceneTree(IWindow window) : Node
{
    private readonly List<Command2DEntry> command2DEntriesCache = [];
    private readonly List<Command3DEntry> command3DEntriesCache = [];

    public record struct Command2DEntry(Matrix3x2<float> Transform, Command Command);
    public record struct Command3DEntry(Matrix4x4<float> Transform, Command Command);

    internal List<Node>    Nodes    { get; } = [];
    internal List<Scene3D> Scenes3D { get; } = [];

    public override string NodeName { get; } = nameof(SceneTree);

    public bool IsDirty { get; internal set; }

    public IWindow Window => window;

    internal IEnumerable<Command2DEntry> Enumerate2DCommands()
    {
        if (this.command2DEntriesCache.Count > 0)
        {
            for (var i = 0; i < this.command2DEntriesCache.Count; i++)
            {
                yield return this.command2DEntriesCache[i];
            }
        }
        else
        {
            foreach (var node in this.Traverse(true))
            {
                if (node is Node2D node2D)
                {
                    var transform = (Matrix3x2<float>)node2D.TransformCache;

                    foreach (var command in node2D.Commands)
                    {
                        var entry = new Command2DEntry(transform, command);

                        this.command2DEntriesCache.Add(entry);

                        yield return entry;
                    }
                }
            }
        }
    }

    internal IEnumerable<Command3DEntry> Enumerate3DCommands()
    {
        if (this.command3DEntriesCache.Count > 0)
        {
            for (var i = 0; i < this.command3DEntriesCache.Count; i++)
            {
                yield return this.command3DEntriesCache[i];
            }
        }
        else
        {
            foreach (var node in this.Traverse(true))
            {
                if (node is Node3D node3D)
                {
                    var transform = (Matrix4x4<float>)node3D.TransformCache;

                    foreach (var command in node3D.Commands)
                    {
                        var entry = new Command3DEntry(transform, command);

                        this.command3DEntriesCache.Add(entry);

                        yield return entry;
                    }
                }
            }
        }
    }

    internal void ResetCache()
    {
        this.command2DEntriesCache.Clear();
        this.command3DEntriesCache.Clear();
    }

}
