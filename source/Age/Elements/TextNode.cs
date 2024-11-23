using Age.Elements.Layouts;
using Age.Scene;

namespace Age.Elements;

public sealed class TextNode : ContainerNode
{
    internal override TextLayout Layout { get; }

    public override string NodeName { get; } = nameof(TextNode);

    public string? SelectedText => this.Layout.SelectedText;

    public int CursorPosition
    {
        get => this.Layout.CaretPosition;
        set => this.Layout.CaretPosition = value;
    }

    public string? Value
    {
        get => this.Layout.Text;
        set => this.Layout.Text = value;
    }

    public TextNode() =>
        this.Layout = new(this);

    public TextNode(string value) : this() =>
        this.Value = value;

    protected override void Adopted(Node parent)
    {
        if (parent is Element parentElement)
        {
            this.Layout.TargetAdopted(parentElement);
        }
    }

    protected override void Removed(Node parent)
    {
        if (parent is Element parentElement)
        {
            this.Layout.TargetRemoved(parentElement);
        }
    }

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

    internal void MouseOver() =>
        this.Layout.TargetMouseOver();

    internal void MouseOut() =>
        this.Layout.TargetMouseOut();

    internal void PropagateSelection(uint characterPosition) =>
        this.Layout.PropagateSelection(characterPosition);

    internal void SetCaret(ushort x, ushort y, uint position) =>
        this.Layout.SetCaret(x, y, position);

    internal void UpdateSelection(ushort x, ushort y, uint character) =>
        this.Layout.UpdateSelection(x, y, character);

    public void DeleteSelected()
    {
        var range = this.Layout.Selection?.Ordered();

        if (range.HasValue && this.Layout.Text != null)
        {
            Span<char> buffer = stackalloc char[this.Layout.Text.Length - range.Value.Offset];

            this.Layout.Text.AsSpan()[..(int)range.Value.Start].CopyTo(buffer);
            this.Layout.Text.AsSpan()[(int)range.Value.End..].CopyTo(buffer[(int)range.Value.Start..]);

            this.Layout.Text = new(buffer);

            this.ClearSelection();
            this.CursorPosition = (int)range.Value.Start;
        }
    }

    public override string ToString() =>
        this.Value ?? "";

}
