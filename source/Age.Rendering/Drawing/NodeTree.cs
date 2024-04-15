using Age.Rendering.Interfaces;

namespace Age.Rendering.Drawing;

public sealed class NodeTree(IWindow window) : Node
{
    public override string NodeName { get; } = nameof(NodeTree);
    public IWindow Window => window;

    public void UpdatedTree() => throw new NotImplementedException();
}
