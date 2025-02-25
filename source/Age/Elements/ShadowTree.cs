using Age.Scene;

namespace Age.Elements;

public sealed class ShadowTree(Element host) : Node
{
    public override string NodeName { get; } = nameof(ShadowTree);

    public Element Host { get; } = host;
}
