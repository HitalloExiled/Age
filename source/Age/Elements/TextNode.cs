using Age.Commands;
using Age.Core.Extensions;
using Age.Elements.Layouts;
using Age.Extensions;
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

    internal void AdjustScroll()
    {
        var parent = this.ParentElement;

        if (parent == null)
        {
            return;
        }

        if (this.Value?.Length > 0)
        {
            var boxModel        = parent.GetBoxModel();
            var cursorBoundings = this.GetCursorBoundings();

            var leftBounds   = boxModel.Boundings.Left   + boxModel.Border.Left   + boxModel.Padding.Left;
            var rightBounds  = boxModel.Boundings.Right  - boxModel.Border.Right  - boxModel.Padding.Right;
            var topBounds    = boxModel.Boundings.Top    + boxModel.Border.Top    + boxModel.Padding.Top;
            var bottomBounds = boxModel.Boundings.Bottom - boxModel.Border.Bottom - boxModel.Padding.Bottom;

            var scroll = parent.Scroll;

            var position = this.CursorPosition == this.Value.Length ?
                this.CursorPosition.ClampSubtract(1)
                : this.CursorPosition;

            if (cursorBoundings.Left < leftBounds)
            {
                var characterBounds = this.GetCharacterBoundings(position);

                scroll.X = (uint)(characterBounds.Left + scroll.X - leftBounds);

            }
            else if (cursorBoundings.Right > rightBounds)
            {
                var characterBounds = this.GetCharacterBoundings(position.ClampSubtract(1));

                scroll.X = (uint)(characterBounds.Right + scroll.X + cursorBoundings.Size.Width - rightBounds);
            }

            if (cursorBoundings.Top < topBounds)
            {
                var characterBounds = this.GetCharacterBoundings(position);

                scroll.Y = (uint)(characterBounds.Top + scroll.Y - topBounds);
            }
            else if (cursorBoundings.Bottom > bottomBounds)
            {
                var characterBounds = this.GetCharacterBoundings(position.ClampSubtract(1));

                scroll.Y = (uint)(characterBounds.Bottom + scroll.Y + cursorBoundings.Size.Height - bottomBounds);
            }

            parent.Scroll = scroll;
        }
        else
        {
            parent.Scroll = default;
        }
    }

    internal void InvokeActivate() =>
        this.Layout.TargetActivated();

    internal void InvokeDeactivate() =>
        this.Layout.TargetDeactivated();

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

        this.AdjustScroll();
    }

    public Rect<int> GetCursorBoundings()
    {
        this.Layout.Update();

        var rect = this.Layout.CursorRect;

        var transform = this.TransformWithOffset;

        var position = new Point<int>(
            (int)(transform.Position.X + rect.Position.X),
            -(int)(transform.Position.Y + rect.Position.Y)
        );

        return new(rect.Size.Cast<int>(), position);
    }

    public Rect<int> GetCharacterBoundings(uint index)
    {
        if (index >= this.Value?.Length)
        {
            throw new IndexOutOfRangeException();
        }

        this.UpdateIndependentAncestorLayout();

        var rect = ((RectCommand)this.Commands[(int)index + 1]).Rect;

        var transform = this.TransformWithOffset;

        var position = new Point<int>(
            (int)(transform.Position.X + rect.Position.X),
            -(int)(transform.Position.Y + rect.Position.Y)
        );

        return new(rect.Size.Cast<int>(), position);
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

        var transform = this.TransformWithOffset;

        var position = new Point<float>(
            (float)(transform.Position.X + rect.Position.X),
            -(float)(transform.Position.Y + rect.Position.Y)
        );

        rect.Position = position;

        return rect.Cast<int>();
    }

    public void HideCaret() =>
        this.Layout.HideCaret();

    public void ShowCaret() =>
        this.Layout.ShowCaret();

    public override string ToString() =>
        this.Value ?? "";
}
