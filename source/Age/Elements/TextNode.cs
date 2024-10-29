using Age.Elements.Layouts;
using Age.Scene;

namespace Age.Elements;

public class TextNode : ContainerNode
{
    internal override TextLayout Layout { get; }

    public override string NodeName { get; } = nameof(TextNode);

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

    internal void ClearCaret() =>
        this.Layout.ClearCaret();

    internal void ClearSelection() =>
        this.Layout.ClearSelection();

    internal void PropagateSelection(uint characterPosition) =>
        this.Layout.PropagateSelection(characterPosition);

    internal void SetCaret(ushort x, ushort y, uint position) =>
        this.Layout.SetCaret(x, y, position);

    public override string ToString() =>
        this.Value ?? "";

    internal void UpdateSelection(ushort x, ushort y, uint character) =>
        this.Layout.UpdateSelection(x, y, character);
}
