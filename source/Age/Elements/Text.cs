using System.Runtime.CompilerServices;
using Age.Commands;
using Age.Core;
using Age.Core.Collections;
using Age.Core.Extensions;
using Age.Numerics;
using Age.Platforms.Display;
using Age.Resources;
using Age.Storage;
using Age.Styling;
using SkiaSharp;

using Timer = Age.Scenes.Timer;

using static Age.Shaders.CanvasShader;
using System.Diagnostics;

namespace Age.Elements;

public sealed class Text : Layoutable
{
    private readonly RectCommand    caretCommand;
    private readonly Timer          caretTimer;
    private readonly uint           caretWidth = 2;
    private readonly Timer          selectionTimer;
    private readonly List<TextLine> textLines = new(1);

    private bool           caretIsDirty;
    private bool           caretIsVisible;
    private SKFont?        font;
    private float          fontLeading;
    private Vector2<float> previouCursor;
    private bool           selectionIsDirty;
    private bool           textIsDirty;

    private  bool        CanSelect  => this.ComposedParentElement?.ComputedStyle.TextSelection != false;
    internal Rect<float> CursorRect => this.caretCommand.GetAffineRect();

    internal override bool IsParentDependent { get; }

    internal override StencilLayer? StencilLayer
    {
        get => base.StencilLayer;
        set
        {
            if (base.StencilLayer != value)
            {
                base.StencilLayer = value;

                foreach (var command in this.Commands)
                {
                    command.StencilLayer = value;
                }
            }
        }
    }

    public StringBuffer Buffer { get; } = new();

    public uint CursorPosition
    {
        get;
        set
        {
            if (field != value || !this.caretTimer.Running)
            {
                field = value < this.Buffer.Length && char.IsLowSurrogate(this.Buffer[(int)value])
                    ? value > field ? value + 1 : value - 1
                    : value;

                this.ShowCaret();
                this.AdjustScroll();
            }
        }
    }

    public override string NodeName => nameof(Text);

    public TextSelection? Selection
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;

                this.selectionIsDirty = true;
                this.RequestUpdate(false);
            }
        }
    }

    public string? Value
    {
        get => this.Buffer.ToString();
        set => this.Buffer.Set(value);
    }

    public Text()
    {
        this.caretTimer = new()
        {
            WaitTime = TimeSpan.FromMilliseconds(500),
        };

        this.selectionTimer = new()
        {
            WaitTime = TimeSpan.FromMilliseconds(16),
        };

        this.caretTimer.Timeout += this.BlinkCaret;

        this.AppendChild(this.caretTimer);
        this.AppendChild(this.selectionTimer);

        this.caretCommand              = CommandPool.RectCommand.Get();
        this.caretCommand.Color        = Color.White;
        this.caretCommand.Flags        = Flags.ColorAsBackground;
        this.caretCommand.Owner        = this;
        this.caretCommand.Size         = new(this.caretWidth, this.LineHeight);
        this.caretCommand.StencilLayer = this.StencilLayer;
        this.caretCommand.TextureMap   = TextureMap.Default;

        this.AddCommand(this.caretCommand);

        this.Buffer.Modified += this.OnTextChange;

        this.Seal();
    }

    public Text(string? value) : this() =>
        this.Buffer.Set(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ApplySelection(int index, in TextSelection selection, TextCommand selectionCommand)
    {
        if (selectionCommand.Surrogate == TextCommand.SurrogateKind.High)
        {
            return;
        }

        if (index >= selection.Start && index < selection.End)
        {
            selectionCommand.Color         = new(0, 0, 1, 0.5f);
            selectionCommand.CommandFilter = CommandFilter.Color | CommandFilter.Encode;
            selectionCommand.Flags         = Flags.ColorAsBackground;

            if (selectionCommand.Index != default)
            {
                ((TextCommand)this.Commands[selectionCommand.Index]).Color = Color.White;
            }

            this.MarkCommandsDirty();
        }
        else
        {
            this.ClearSelection(selectionCommand);
        }
    }

    private void BlinkCaret()
    {
        this.caretIsVisible = !this.caretIsVisible;

        this.caretCommand.CommandFilter = this.caretIsVisible ? CommandFilter.Color : default;

        this.RequestUpdate(false);

        this.MarkCommandsDirty();
    }

    private void ClearSelection(TextCommand selectionCommand)
    {
        if (selectionCommand.Surrogate == TextCommand.SurrogateKind.High)
        {
            return;
        }

        selectionCommand.Color         = default;
        selectionCommand.CommandFilter = CommandFilter.Encode;
        selectionCommand.Flags         = default;

        if (selectionCommand.Index != default)
        {
            ((TextCommand)this.Commands[selectionCommand.Index]).Color = this.GetInheritedStyleSource()?.ComputedStyle?.Color ?? default;
        }

        this.MarkCommandsDirty();
    }

    private void DrawCaret()
    {
        var textLength = this.Buffer.Length;

        if (textLength > 0)
        {
            Vector2<float> position;

            if (this.CursorPosition == textLength)
            {
                var command = (RectCommand)this.Commands[textLength - 1];

                if (this.Buffer[^1] == '\n')
                {
                    position.X = 0;
                    position.Y = command.LocalTransform.Position.Y - this.LineHeight;
                }
                else
                {
                    position = command.LocalTransform.Position;
                    position.X += command.Size.Width;
                }
            }
            else
            {
                position = this.Commands[(int)this.CursorPosition].LocalTransform.Position;
            }

            this.caretCommand.LocalTransform = Transform2D.CreateTranslated(position);
        }
        else
        {
            this.caretCommand.LocalTransform = Transform2D.Identity;
        }
    }

    private void DrawSelection()
    {
        var text = this.Buffer.AsSpan();

        if (this.Selection.HasValue)
        {
            var range = this.Selection.Value.Ordered();

            for (var i = 0; i < text.Length; i++)
            {
                this.ApplySelection(i, range, (TextCommand)this.Commands[i]);
            }
        }
        else
        {
            for (var i = 0; i < text.Length; i++)
            {
                this.ClearSelection((TextCommand)this.Commands[i]);
            }
        }
    }

    private void DrawText()
    {
        if (this.font == null)
        {
            throw new InvalidOperationException();
        }

        var textSpan = this.Buffer.AsSpan();

        var style = this.GetInheritedStyleSource()?.ComputedStyle;

        var glyphs = this.font.Typeface.GetGlyphs(textSpan);
        var atlas  = TextStorage.Singleton.GetAtlas(this.font.Typeface.FamilyName, (uint)this.font.Size);

        using var glyphsBoundsRef = new RefArray<SKRect>(glyphs.Length);
        using var glyphsWidths    = new RefArray<float>(glyphs.Length);

        var glyphsBounds = glyphsBoundsRef.AsSpan();

        this.font.GetGlyphWidths(glyphs, glyphsWidths, glyphsBounds);

        var baseLine  = -this.BaseLine;
        var cursor    = new Point<int>(0, baseLine);
        var boundings = new Size<uint>(0, this.LineHeight);

        textSpan.GetTextInfo(out var visibleCharacterCount, out var linesCount);

        this.textLines.Resize(linesCount, default);

        var lines = this.textLines.AsSpan();

        lines[0].Start = 0;

        this.RemoveCommandAt(^1);

        this.AllocateCommands(textSpan.Length + visibleCharacterCount, CommandPool.TextCommand);

        this.AddCommand(this.caretCommand);

        var elementIndex = this.Index + 1;
        var range = this.Selection?.Ordered();

        var textOffset = 0;
        var charOffset = 0;
        var lineIndex  = 0;
        var color      = style?.Color ?? new();
        var commands   = this.Commands;

        for (var i = 0; i < textSpan.Length; i++)
        {
            var charIndex        = i - charOffset;
            var selectionCommand = (TextCommand)commands[i];

            selectionCommand.Border       = default;
            selectionCommand.Color        = default;
            selectionCommand.Flags        = default;
            selectionCommand.Index        = default;
            selectionCommand.Surrogate    = default;
            selectionCommand.Line         = lineIndex;
            selectionCommand.Owner        = this;
            selectionCommand.StencilLayer = this.StencilLayer;
            selectionCommand.TextureMap   = TextureMap.Default;

            var character = textSpan[i];

            if (!char.IsLowSurrogate(character))
            {
                var width = (uint)float.Round(glyphsWidths[charIndex]);

                selectionCommand.LocalTransform = Transform2D.CreateTranslated(new(cursor.X, cursor.Y - baseLine));
                selectionCommand.Metadata       = CombineIds(elementIndex, i + 1);
                selectionCommand.Size           = new(width, this.LineHeight);

                if (!char.IsWhiteSpace(character))
                {
                    ref readonly var bounds = ref glyphsBounds[charIndex];

                    var len = 1;

                    if (char.IsHighSurrogate(character))
                    {
                        len++;
                        charOffset++;
                        selectionCommand.CommandFilter = CommandFilter.None;
                        selectionCommand.Surrogate     = TextCommand.SurrogateKind.High;
                    }

                    var chars = textSpan.Slice(i, len);

                    var glyph  = TextStorage.Singleton.DrawGlyph(this.font, atlas, chars, bounds);
                    var size   = new Size<uint>((uint)bounds.Width, (uint)bounds.Height);
                    var offset = new Vector2<float>(float.Round(cursor.X + bounds.Left), float.Round(cursor.Y - bounds.Top));

                    var characterIndex = textSpan.Length + textOffset;

                    selectionCommand.Index = characterIndex;

                    var characterCommand = (TextCommand)commands[characterIndex];

                    characterCommand.Border   = default;
                    characterCommand.Metadata = default;
                    characterCommand.UserData = default;

                    characterCommand.Color          = color;
                    characterCommand.CommandFilter  = CommandFilter.Color;
                    characterCommand.Flags          = Flags.GrayscaleTexture | Flags.MultiplyColor;
                    characterCommand.Index          = selectionCommand.Index;
                    characterCommand.Line           = selectionCommand.Line;
                    characterCommand.LocalTransform = Transform2D.CreateTranslated(offset);
                    characterCommand.Owner          = this;
                    characterCommand.Size           = size;
                    characterCommand.StencilLayer   = this.StencilLayer;
                    characterCommand.TextureMap     = glyph;

                    cursor.X += (int)width;

                    textOffset++;
                }
                else if (character == '\n')
                {
                    cursor.X = 0;
                    cursor.Y -= (int)(this.LineHeight + -this.fontLeading);

                    boundings.Height += this.LineHeight + (uint)this.fontLeading;

                    lines[lineIndex].Length = (uint)i - lines[lineIndex].Start + 1;

                    lineIndex++;

                    if (lineIndex < lines.Length)
                    {
                        lines[lineIndex].Start = (uint)i + 1;
                    }
                }
                else
                {
                    cursor.X += (int)width;
                }

                boundings.Width = uint.Max(boundings.Width, (uint)cursor.X);
            }
            else
            {
                var previous = (TextCommand)commands[i - 1];

                selectionCommand.Index          = previous.Index;
                selectionCommand.LocalTransform = previous.LocalTransform;
                selectionCommand.Metadata       = previous.Metadata;
                selectionCommand.Size           = previous.Size;
                selectionCommand.Surrogate      = TextCommand.SurrogateKind.Low;
            }

            if (range.HasValue)
            {
                this.ApplySelection(i, range.Value, selectionCommand);
            }
            else
            {
                this.ClearSelection(selectionCommand);
            }
        }

        lines[^1].Length = (uint)textSpan.Length - 1 - lines[^1].Start + 1;

        atlas.Update();

        this.Boundings = boundings;
    }

    private void GetCharacterOffset(ushort x, ushort y, ref uint character)
    {
        if (char.IsControl(this.Buffer[(int)character]))
        {
            return;
        }

        var command = (TextCommand)this.Commands[(int)character];

        var cursor = this.Transform.Matrix.Inverse() * new Vector2<float>(x, -y);

        if (cursor.X > command.LocalTransform.Position.X + (command.Size.Width / 2))
        {
            character += command.Surrogate == TextCommand.SurrogateKind.High ? 2u : 1;
        }
    }

    private Element? GetInheritedStyleSource() =>
        GetStyleSource(this.Parent);

    private void OnParentStyleChanged(StyleProperty property)
    {
        if (property.HasAnyFlag(StyleProperty.Color | StyleProperty.FontFamily | StyleProperty.FontSize | StyleProperty.FontWeight))
        {
            var inheritedStyleSource = this.GetInheritedStyleSource();

            var style = inheritedStyleSource?.ComputedStyle;

            var fontFamily = style?.FontFamily ?? DEFAULT_FONT_FAMILY;
            var fontWeight = (int)(style?.FontWeight ?? FontWeight.Normal);
            var fontSize   = style?.FontSize ?? DEFAULT_FONT_SIZE;

            if (this.font?.Size != fontSize || this.font.Typeface.FamilyName != fontFamily || this.font.Typeface.FontWeight != fontWeight)
            {
                this.font = TextStorage.Singleton.GetFont(fontFamily, fontSize, fontWeight, inheritedStyleSource?.StyleSheet?.FontFaces);
                this.font.GetFontMetrics(out var metrics);

                this.LineHeight  = (uint)float.Round(-metrics.Ascent + metrics.Descent);
                this.BaseLine    = (int)float.Round(-metrics.Ascent);
                this.fontLeading = metrics.Leading;

                this.caretCommand.Size = new(this.caretWidth, this.LineHeight);

                this.caretIsDirty = true;
            }

            this.textIsDirty = true;
            this.RequestUpdate(true);
        }
    }

    private void OnTextChange()
    {
        this.Selection = this.Selection?.WithEnd(uint.Min(this.Selection.Value.End, (uint)this.Buffer.Length));
        this.textIsDirty = true;

        this.RequestUpdate(true);
    }

    private protected override void OnAttachedInternal()
    {
        base.OnAttachedInternal();

        switch (this.Parent)
        {
            case Element parentElement:
                this.HandleAdopted(parentElement);
                break;

            case ShadowTree shadowTree:
                this.HandleAdopted(shadowTree.Host);
                break;
        }
    }

    private protected override void OnDisposedInternal()
    {
        foreach (var command in this.Commands[..^1])
        {
            CommandPool.TextCommand.Return((TextCommand)command);
        }

        CommandPool.RectCommand.Return(this.caretCommand);
        this.ClearCommands();
    }

    private protected override void OnIndexChanged()
    {
        this.UpdateDirtyLayout();

        var elementIndex = this.Index + 1;
        var commands     = this.Commands[..this.Buffer.Length];

        for (var i = 0; i < commands.Length; i++)
        {
            var command = (TextCommand)commands[i];

            if (command.Surrogate != TextCommand.SurrogateKind.Low)
            {
                commands[i].Metadata = CombineIds(elementIndex, i + 1);
            }
            else
            {
                command.Metadata = commands[i - 1].Metadata;
            }
        }
    }

    private protected override void OnDetachingInternal()
    {
        switch (this.Parent)
        {
            case Element parentElement:
                this.HandleRemoved(parentElement);
                break;

            case ShadowTree shadowTree:
                this.HandleRemoved(shadowTree.Host);
                break;
        }
    }

    internal void AdjustScroll() =>
        this.ComposedParentElement?.ScrollTo(this.GetCursorBoundings());

    internal void ClearCaret()
    {
        if (!this.CanSelect)
        {
            return;
        }

        this.HideCaret();
    }

    internal void HandleAdopted(Element parentElement)
    {
        parentElement.StyleChanged += this.OnParentStyleChanged;

        this.OnParentStyleChanged(StyleProperty.All);
    }

    internal void HandleMouseOut()
    {
        IsHoveringText = false;

        if (ActiveText != this || !this.CanSelect)
        {
            return;
        }

        if (IsSelectingText)
        {
            Debug.Assert(this.Scene?.Viewport?.Window != null);

            this.Scene.Viewport.Window.MouseMove += this.WindowOnMouseMove;
        }
    }

    internal void HandleMouseOver()
    {
        IsHoveringText = true;

        if (IsDraggingScrollBar || IsSelectingText || !this.CanSelect)
        {
            return;
        }

        this.SetCursor(Cursor.Text);
    }

    internal void HandleRemoved(Element parentElement) =>
        parentElement.StyleChanged -= this.OnParentStyleChanged;

    internal void HandleActivate()
    {
        if (!this.CanSelect)
        {
            return;
        }

        ActiveText = this;
    }

    private void WindowOnMouseMove(in WindowMouseEvent mouseEvent)
    {
        if (mouseEvent.IsHoldingPrimaryButton)
        {
            this.UpdateSelection(mouseEvent.X, mouseEvent.Y);
        }
        else
        {
            Debug.Assert(this.Scene?.Viewport?.Window != null);

            this.Scene.Viewport.Window.MouseMove -= this.WindowOnMouseMove;
            this.HandleDeactivate();
        }
    }

    internal void HandleDeactivate() => ActiveText = null;

    internal void PropagateSelection(uint characterPosition)
    {
        var text = this.Buffer.AsSpan();

        if (!this.CanSelect || text.Length == 0 || char.IsWhiteSpace(text[(int)characterPosition]))
        {
            return;
        }

        var start = (int)characterPosition - 1;
        var end   = (int)characterPosition;

        while (start > -1 && !char.IsWhiteSpace(text[start]))
        {
            start--;
        }

        start++;

        while (end < text.Length && !char.IsWhiteSpace(text[end]))
        {
            end++;
        }

        this.CursorPosition = (uint)end;

        this.Selection = new((uint)start, (uint)end);
    }

    internal void SetCaret(ushort x, ushort y)
    {
        if (this.Buffer.IsEmpty || !this.CanSelect)
        {
            return;
        }

        var cursor = this.TransformWithOffset.Matrix.Inverse() * new Vector2<float>(x, -y);

        if (cursor == this.previouCursor)
        {
            return;
        }

        this.previouCursor = cursor;

        this.ClearSelection();

        bool isOnCursorLine(in Rect<float> rect) => cursor.Y >= rect.Position.Y - rect.Size.Height && cursor.Y <= rect.Position.Y;

        var text = this.Buffer;

        var lineSpan = this.textLines.AsSpan();
        var position = (uint)text.Length;

        for (var i = 0; i < lineSpan.Length; i++)
        {
            var rect = this.Commands[(int)lineSpan[i].Start].GetAffineRect();

            if (isOnCursorLine(rect))
            {
                position = lineSpan[i].End + 1 == text.Length && text[^1] != '\n'
                    ? (uint)text.Length
                    : lineSpan[i].End;

                break;
            }
        }

        this.CursorPosition = position;
    }

    private void SetCaret(ushort x, ushort y, uint position)
    {
        if (!this.CanSelect)
        {
            return;
        }

        this.GetCharacterOffset(x, y, ref position);

        this.CursorPosition = position;
    }

    internal override void UpdateLayout()
    {
        if (this.Buffer.IsEmpty)
        {
            if (this.textIsDirty)
            {
                this.RemoveCommandAt(^1);

                this.ReleaseCommands(this.Commands.Length, CommandPool.TextCommand);

                this.AddCommand(this.caretCommand);

                this.Selection = default;
                this.Boundings = default;
            }
        }
        else
        {
            if (this.textIsDirty)
            {
                this.DrawText();
            }
            else if (this.selectionIsDirty)
            {
                this.DrawSelection();
            }
        }

        if (this.caretIsDirty)
        {
            this.DrawCaret();
        }

        this.caretIsDirty     = false;
        this.selectionIsDirty = false;
        this.textIsDirty      = false;
    }

    internal void UpdateSelection(ushort x, ushort y)
    {
        if (this.Buffer.IsEmpty || !this.CanSelect)
        {
            return;
        }

        var cursor = this.TransformWithOffset.Matrix.Inverse() * new Vector2<float>(x, -y);

        if (cursor == this.previouCursor)
        {
            return;
        }

        this.previouCursor = cursor;

        var selection  = this.Selection ?? new(this.CursorPosition, this.CursorPosition);
        var textLength = this.Buffer.Length;

        var startIndex = int.Min((int)selection.Start, textLength - 1);
        var endIndex   = int.Min((int)selection.End, textLength - 1);

        var lineSpan = this.textLines.AsSpan();
        var commands = this.Commands;

        var startAnchor = (TextCommand)commands[startIndex];
        var endAnchor   = (TextCommand)commands[endIndex];

        var startAnchorRect = startAnchor.GetAffineRect();
        var endAnchorRect   = endAnchor.GetAffineRect();

        var position = selection.End;

        if (isOnCursorLine(endAnchorRect))
        {
            resolveLine(lineSpan, endAnchor.Line, ref position);
        }
        else if (isCursorBelow(startAnchorRect))
        {
            if (isCursorBelowTop(endAnchorRect))
            {
                position = (uint)textLength;

                scanBelow(commands, endAnchor.Line, lineSpan.Length, lineSpan, ref position);
            }
            else
            {
                scanAbove(commands, endAnchor.Line, startAnchor.Line, lineSpan, ref position);
            }
        }
        else
        {
            if (isCursorAboveBottom(endAnchorRect))
            {
                position = 0;

                scanAbove(commands, endAnchor.Line, -1, lineSpan, ref position);
            }
            else
            {
                scanBelow(commands, endAnchor.Line, startAnchor.Line + 1, lineSpan, ref position);
            }
        }

        this.Selection      = selection.WithEnd(position);
        this.CursorPosition = position;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void resolveLine(scoped ReadOnlySpan<TextLine> lines, int index, ref uint position) =>
            position = isCursorBefore(endAnchorRect)
                ? lines[index].Start
                : lines[index].End + 1 == textLength ? (uint)textLength : lines[index].End;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool isCursorAfter(in Rect<float> rect) => cursor.X > rect.Position.X + (rect.Size.Width / 2);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool isCursorBefore(in Rect<float> rect) => !isCursorAfter(rect);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool isOnCursorLine(in Rect<float> rect) => isCursorBelowTop(rect) && isCursorAboveBottom(rect);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool isCursorBelow(in Rect<float> rect) => cursor.Y < rect.Position.Y - (rect.Size.Height / 2);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool isCursorBelowTop(in Rect<float> rect) => cursor.Y < rect.Position.Y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool isCursorAboveBottom(in Rect<float> rect) => cursor.Y > rect.Position.Y - rect.Size.Height;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void scanAbove(scoped ReadOnlySpan<Command> commands, int start, int end, scoped ReadOnlySpan<TextLine> lines, ref uint position)
        {
            for (var i = start; i > end; i--)
            {
                var rect = ((TextCommand)commands[(int)lines[i].Start]).GetAffineRect();

                if (isCursorBelowTop(rect))
                {
                    resolveLine(lines, i, ref position);

                    break;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void scanBelow(scoped ReadOnlySpan<Command> commands, int start, int end, scoped ReadOnlySpan<TextLine> lines, ref uint position)
        {
            for (var i = start; i < end; i++)
            {
                var rect = ((TextCommand)commands[(int)lines[i].Start]).GetAffineRect();

                if (isCursorAboveBottom(rect))
                {
                    resolveLine(lines, i, ref position);

                    break;
                }
            }
        }
    }

    internal void HandleVirtualChildMouseDown(in WindowMouseEvent mouseEvent, uint virtualChildIndex, bool focus)
    {
        if (ActiveText != this)
        {
            return;
        }

        if (focus)
        {
            this.SetCaret(mouseEvent.X, mouseEvent.Y, virtualChildIndex - 1);
        }
        else
        {
            this.UpdateSelection(mouseEvent.X, mouseEvent.Y, virtualChildIndex - 1);
        }
    }

    internal void HandleVirtualChildMouseMove(in WindowMouseEvent mouseEvent, uint virtualChildIndex)
    {
        if (ActiveText != this)
        {
            return;
        }

        if (mouseEvent.IsHoldingPrimaryButton)
        {
            Debug.Assert(this.Scene?.Viewport?.Window != null);

            this.Scene.Viewport.Window.MouseMove -= this.WindowOnMouseMove;
            this.UpdateSelection(mouseEvent.X, mouseEvent.Y, virtualChildIndex - 1);
        }
    }

    private void UpdateSelection(ushort x, ushort y, uint character)
    {
        if (ActiveText != this)
        {
            return;
        }

        this.GetCharacterOffset(x, y, ref character);

        this.Selection      = this.Selection?.WithEnd(character) ?? new(this.CursorPosition, character);
        this.CursorPosition = character;
    }

    public void ClearSelection() =>
        this.Selection = default;

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

        this.UpdateLayoutIndependentAncestor();

        var rect = this.Commands[(int)index].GetAffineRect();

        var transform = this.TransformWithOffset;

        var position = new Point<int>(
            (int)(transform.Position.X  + rect.Position.X),
            -(int)(transform.Position.Y + rect.Position.Y)
        );

        return new(rect.Size.Cast<int>(), position);
    }

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
            (int)(transform.Position.X  + rect.Position.X),
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

        this.UpdateLayoutIndependentAncestor();

        var slice = this.Commands.Slice((int)textSelection.Start + 1, textSelection.Length);

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

    public void HideCaret()
    {
        this.caretIsDirty   = true;
        this.caretIsVisible = false;

        this.caretCommand.CommandFilter = default;

        this.caretTimer.Stop();

        this.RequestUpdate(false);

        this.MarkCommandsDirty();
    }

    public void ShowCaret()
    {
        this.caretIsDirty   = true;
        this.caretIsVisible = true;

        this.caretCommand.CommandFilter = CommandFilter.Color;

        this.caretTimer.Start();

        this.RequestUpdate(false);

        this.MarkCommandsDirty();
    }

    public override string ToString() =>
        this.Buffer.ToString();
}
