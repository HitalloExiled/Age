using Age.Commands;
using Age.Core.Extensions;
using Age.Core;
using Age.Elements.Layouts;
using Age.Numerics;
using Age.Scene;
using Age.Resources;
using static Age.Shaders.CanvasShader;

namespace Age.Elements;

public sealed partial class Text : Layoutable
{
    public StringBuffer Buffer { get; } = new();

    public override string NodeName => nameof(Text);

    // public uint CursorPosition
    // {
    //     get => this.Layout.CaretPosition;
    //     set => this.Layout.CaretPosition = value;
    // }

    // public TextSelection? Selection
    // {
    //     get => this.Layout.Selection;
    //     set => this.Layout.Selection = value;
    // }

    public string? Value
    {
        get => this.Buffer.ToString();
        set => this.Buffer.Set(value);
    }

    public Text()
    {
        this.NodeFlags  = NodeFlags.Immutable;

        this.caretTimer = new()
        {
            WaitTime = TimeSpan.FromMilliseconds(500),
        };

        this.selectionTimer = new()
        {
            WaitTime = TimeSpan.FromMilliseconds(16),
        };

        this.caretTimer.Timeout += this.BlinkCaret;
        this.selectionTimer.Timeout += this.UpdateSelection;

        this.AppendChild(this.caretTimer);
        this.AppendChild(this.selectionTimer);

        this.caretCommand              = CommandPool.RectCommand.Get();
        this.caretCommand.Color        = Color.White;
        this.caretCommand.Flags        = Flags.ColorAsBackground;
        this.caretCommand.TextureMap   = TextureMap.Default;
        this.caretCommand.Size         = new(this.caretWidth, this.LineHeight);
        this.caretCommand.StencilLayer = this.StencilLayer;

        this.Commands.Add(this.caretCommand);

        this.Buffer.Modified += this.OnTextChange;
    }

    public Text(string? value) : this() =>
        this.Buffer.Set(value);

    protected override void OnAdopted(Node parent)
    {
        switch (parent)
        {
            case Element parentElement:
                this.HandleAdopted(parentElement);
                break;

            case ShadowTree shadowTree:
                this.HandleAdopted(shadowTree.Host);
                break;
        }
    }

    protected override void OnRemoved(Node parent)
    {
        switch (parent)
        {
            case Element parentElement:
                this.HandleRemoved(parentElement);
                break;

            case ShadowTree shadowTree:
                this.HandleRemoved(shadowTree.Host);
                break;
        }
    }

    protected override void OnIndexed() =>
        this.HandleIndexed();

    internal void InvokeActivate() =>
        this.HandleActivated();

    internal void InvokeDeactivate() =>
        this.HandleDeactivated();

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

            this.ClearSelection();
            this.CursorPosition = range.Start;

            this.AdjustScroll();
        }
    }

    public void DeleteSelected()
    {
        if (this.Selection.HasValue)
        {
            this.Delete(this.Selection.Value);
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

    public void ClearSelection() =>
        this.Selection = default;

    public TextLine GetCharacterLine(uint index) =>
        this.textLines[this.GetCharacterLineIndex(index)];

    public int GetCharacterLineIndex(uint index) =>
        ((TextCommand)this.Commands[(int)index]).Line;

    public TextLine? GetCharacterNextLine(uint index) =>
        !this.Buffer.IsEmpty && this.GetCharacterLineIndex(index) + 1 is var lineIndex && lineIndex < this.textLines.Count
            ? this.textLines[lineIndex]
            : (TextLine?)default;

    public TextLine? GetCharacterPreviousLine(uint index) =>
        !this.Buffer.IsEmpty && this.GetCharacterLineIndex(index) - 1 is var lineIndex && lineIndex > -1
            ? this.textLines[lineIndex]
            : (TextLine?)default;

    public Rect<int> GetCursorBoundings()
    {
        this.UpdateLayout();

        var rect = this.CursorRect;

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

    // public void HideCaret() =>
    //     this.Layout.HideCaret();

    // public void ShowCaret() =>
    //     this.Layout.ShowCaret();

    public override string ToString() =>
        this.Buffer.ToString();
}
