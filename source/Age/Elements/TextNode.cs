using Age.Elements.Layouts;

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
        this.Layout = new(this) { IsInline = true };

    public override string ToString() =>
        this.Value ?? "";
}
