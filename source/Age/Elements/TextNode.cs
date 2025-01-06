using System.ComponentModel.Design;
using Age.Commands;
using Age.Core.Extensions;
using Age.Elements.Layouts;
using Age.Numerics;
using Age.Scene;

namespace Age.Elements;

public sealed class TextNode : ContainerNode
{
    internal override TextLayout Layout { get; }

    public override string NodeName { get; } = nameof(TextNode);

    public string? SelectedText => this.Layout.Text != null && this.Selection?.Ordered() is TextSelection selection
        ? this.Layout.Text.Substring((int)selection.Start, (int)(selection.End - selection.Start))
        : null;

    public uint CursorPosition
    {
        get => this.Layout.CaretPosition;
        set => this.Layout.CaretPosition = value;
    }

    public TextSelection? Selection
    {
        get => this.Layout.Selection;
        set => this.Layout.Selection = value;
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

    public void ClearSelection() =>
        this.Layout.ClearSelection();

    public void DeleteSelected()
    {
        var range = this.Layout.Selection?.Ordered();

        if (range.HasValue && this.Layout.Text != null)
        {
            var start = this.Layout.Text.AsSpan(..(int)range.Value.Start);
            var end   = this.Layout.Text.AsSpan((int)range.Value.End..);

            this.Layout.Text = string.Concat(start, end);

            this.Layout.ClearSelection();
            this.CursorPosition = range.Value.Start;
        }
    }

    public Rect<int> GetCursorBoundings()
    {
        this.Layout.Update();

        var cursorRect = this.Layout.CursorRect.Cast<int>();

        return new(cursorRect.Size, this.Transform.Position.ToPoint<int>() + cursorRect.Position);
    }

    public Rect<int> GetCharacterBoundings(uint index)
    {
        if (index > this.Value?.Length)
        {
            throw new IndexOutOfRangeException();
        }

        this.UpdateIndependentAncestorLayout();

        var rect = ((RectCommand)this.Commands[(int)index + 1]).Rect;

        rect.Position += this.Transform.Position.ToPoint<float>();

        return rect.Cast<int>();
    }

    public Rect<int> GetTextSelectionBounds(TextSelection textSelection)
    {
        textSelection = textSelection.Ordered();

        if (this.Value == null || textSelection.Start > this.Value.Length || textSelection.End > this.Value.Length)
        {
            throw new IndexOutOfRangeException();
        }

        this.UpdateIndependentAncestorLayout();

        var slice = this.Commands.AsSpan((int)textSelection.Start + 1, (int)textSelection.End + 1);

        var rect = new Rect<float>();

        for (var i = 0; i < slice.Length; i++)
        {
            var command = (RectCommand)slice[i];

            rect.Grow(command.Rect);
        }

        rect.Position += this.Transform.Position.ToPoint<float>();

        return rect.Cast<int>();
    }

    public void HideCaret() =>
        this.Layout.HideCaret();

    public void ShowCaret() =>
        this.Layout.ShowCaret();

    public override string ToString() =>
        this.Value ?? "";
}
