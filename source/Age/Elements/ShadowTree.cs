using Age.Scene;

namespace Age.Elements;

public sealed class ShadowTree(Element host) : Node
{
    public override string NodeName => nameof(ShadowTree);

    public Element Host { get; } = host;

    protected override void OnChildAppended(Node child)
    {
        if (child is Layoutable layoutable)
        {
            this.Host.Layout.HandleLayoutableAppended(layoutable);
        }
    }

    protected override void OnChildRemoved(Node child)
    {
        if (child is Layoutable layoutable)
        {
            this.Host.Layout.HandleLayoutableRemoved(layoutable);
        }
    }
}
