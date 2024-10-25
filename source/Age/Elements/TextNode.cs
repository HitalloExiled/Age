using Age.Elements.Layouts;
using Age.Scene;

namespace Age.Elements;

public class TextNode : ContainerNode
{
    internal override TextLayout Layout { get; }

    public override string NodeName { get; } = nameof(TextNode);

    public Element? ParentElement => this.Parent as Element;

    public string? Value
    {
        get => this.Layout.Text;
        set => this.Layout.Text = value;
    }

    public TextNode() =>
        this.Layout = new(this);

    protected override void Connected(RenderTree renderTree) =>
        this.Layout.TargetConnected();

    protected override void Disconnected(RenderTree renderTree) =>
        this.Layout.TargetDisconnected();

    protected override void Indexed() =>
        this.Layout.TargetIndexed();

    public override string ToString() =>
        this.Value ?? "";
}
