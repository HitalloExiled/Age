using Age.Scene;

namespace Age.Elements;

public sealed class ShadowTree(Element host) : Node
{
    public override string NodeName { get; } = nameof(ShadowTree);

    public Element Host { get; } = host;

    protected override void ChildAppended(Node child)
    {
        if (child is Layoutable layoutable)
        {
            this.Host.Layout.LayoutableAppended(layoutable);
        }
    }

    protected override void ChildRemoved(Node child)
    {
        if (child is Layoutable layoutable)
        {
            this.Host.Layout.LayoutableRemoved(layoutable);
        }
    }
}
