using Age.Rendering.Interfaces;

namespace Age.Rendering.Drawing;

public sealed class NodeTree(IWindow window) : Node
{
    internal List<Node> Nodes { get; } = [];

    public override string NodeName { get; } = nameof(NodeTree);

    public IWindow Window => window;
}
