using Age.Commands;
using Age.Core.Extensions;
using Age.Core;
using Age.Elements.Layouts;
using Age.Numerics;
using Age.Scene;

namespace Age.Elements;

public sealed class Text : Layoutable
{
    internal override TextLayout Layout { get; }

    public StringBuffer Buffer { get; } = new();

    public override string NodeName => nameof(Text);

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
        get => this.Buffer.ToString();
        set => this.Buffer.Set(value);
    }

    public Text()
    {
        this.Layout = new(this);
        this.Flags  = NodeFlags.Immutable;
    }

    public Text(string? value) : this() =>
        this.Buffer.Set(value);

    protected override void OnAdopted(Node parent)
    {
        switch (parent)
        {
            case Element parentElement:
                this.Layout.HandleTargetAdopted(parentElement);
                break;

            case ShadowTree shadowTree:
                this.Layout.HandleTargetAdopted(shadowTree.Host);
                break;
        }
    }

    protected override void OnRemoved(Node parent)
    {
        switch (parent)
        {
            case Element parentElement:
                this.Layout.HandleTargetRemoved(parentElement);
                break;

            case ShadowTree shadowTree:
                this.Layout.HandleTargetRemoved(shadowTree.Host);
                break;
        }
    }

    protected override void OnConnected(RenderTree renderTree) =>
        this.Layout.HandleTargetConnected();

    protected override void OnDisconnected(RenderTree renderTree) =>
        this.Layout.HandleTargetDisconnected();

    protected override void OnIndexed() =>
        this.Layout.HandleTargetIndexed();

    internal void InvokeActivate() =>
        this.Layout.HandleTargetActivated();

    internal void InvokeDeactivate() =>
        this.Layout.HandleTargetDeactivated();

    public void ClearSelection() =>
        this.Layout.ClearSelection();

    public string? Copy(TextSelection selection)
    {
        if (this.Buffer.Length == 0)
        {
            return null;
        }

        var range = selection.Ordered();

        return new(this.Buffer.Substring((int)range.Start, (int)(range.End - range.Start)));
    }

    public string? CopySelected() =>
        !this.Selection.HasValue ? null : this.Copy(this.Selection.Value);

    public string? Cut(TextSelection selection)
    {
        var content = this.Copy(selection);

        this.Delete(selection);

        return content;
    }

    public string? CutSelected() =>
        !this.Selection.HasValue ? null : this.Cut(this.Selection.Value);

    public void Delete(TextSelection selection)
    {
        if (!this.Buffer.IsEmpty)
        {
            var range = selection.Ordered();

            this.Buffer.Remove((int)range.Start, range.Length);

            this.Layout.ClearSelection();
            this.CursorPosition = range.Start;

            this.Layout.AdjustScroll();
        }
    }

    public void DeleteSelected()
    {
        if (this.Layout.Selection.HasValue)
        {
            this.Delete(this.Layout.Selection.Value);
        }
    }

    public Rect<int> GetCharacterBoundings(uint index)
    {
        if (index >= this.Buffer?.Length)
        {
            throw new IndexOutOfRangeException();
        }

        this.UpdateIndependentAncestorLayout();

        var rect = ((RectCommand)this.Commands[(int)index]).GetAffineRect();

        var transform = this.TransformWithOffset;

        var position = new Point<int>(
            (int)(transform.Position.X + rect.Position.X),
            -(int)(transform.Position.Y + rect.Position.Y)
        );

        return new(rect.Size.Cast<int>(), position);
    }

    public TextLine GetCharacterLine(uint index) =>
        this.Layout.GetCharacterLine(index);

    public TextLine? GetCharacterNextLine(uint index) =>
        this.Layout.GetCharacterNextLine(index);

    public TextLine? GetCharacterPreviousLine(uint index) =>
        this.Layout.GetCharacterPreviousLine(index);

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

    public Rect<int> GetTextSelectionBounds(TextSelection textSelection)
    {
        textSelection = textSelection.Ordered();

        if (this.Buffer == null || textSelection.Start > this.Buffer.Length || textSelection.End > this.Buffer.Length)
        {
            throw new IndexOutOfRangeException();
        }

        this.UpdateIndependentAncestorLayout();

        var slice = this.Commands.AsSpan((int)textSelection.Start + 1, (int)textSelection.End + 1);

        var rect = new Rect<float>();

        for (var i = 0; i < slice.Length; i++)
        {
            var command = (RectCommand)slice[i];

            rect.Grow(command.GetAffineRect());
        }

        var transform = this.TransformWithOffset;

        rect.Position = new Point<float>(
            (float)(transform.Position.X + rect.Position.X),
            -(float)(transform.Position.Y + rect.Position.Y)
        );

        return rect.Cast<int>();
    }

    public void HideCaret() =>
        this.Layout.HideCaret();

    public void ShowCaret() =>
        this.Layout.ShowCaret();

    public override string ToString() =>
        this.Buffer.ToString();
}
