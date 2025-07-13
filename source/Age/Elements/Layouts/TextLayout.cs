using Age.Commands;
using Age.Core.Extensions;
using Age.Numerics;
using Age.Platforms.Display;
using Age.Resources;
using Age.Storage;
using Age.Styling;
using SkiaSharp;
using System.Runtime.CompilerServices;

using Timer = Age.Scene.Timer;

using static Age.Shaders.CanvasShader;
using Age.Core.Collections;

namespace Age.Elements.Layouts;

internal sealed class TextLayout : Layout
{
    private readonly RectCommand    caretCommand;
    private readonly Timer          caretTimer;
    private readonly int            caretWidth = 2;
    private readonly Timer          selectionTimer;
    private readonly List<TextLine> textLines = new(1);

    private bool                      caretIsDirty;
    private bool                      caretIsVisible;
    private SKFont?                   font;
    private float                     fontLeading;
    private bool                      isMouseOverText;
    private Vector2<float>            previouCursor;
    private bool                      selectionIsDirty;
    private bool                      textIsDirty;

    private bool CanSelect => this.Parent?.ComputedStyle.TextSelection != false;

    public uint CaretPosition
    {
        get;
        set
        {
            if (field != value || !this.caretTimer.Running)
            {
                field = value < this.Target.Buffer.Length && char.IsLowSurrogate(this.Target.Buffer[(int)value])
                    ? value > field ? value + 1 : value - 1
                    : value;

                this.ShowCaret();
                this.AdjustScroll();
            }
        }
    }

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

    public override StencilLayer? StencilLayer
    {
        get => base.StencilLayer;
        set
        {
            if (base.StencilLayer != value)
            {
                base.StencilLayer = value;

                foreach (var command in this.Target.Commands)
                {
                    command.StencilLayer = value;
                }
            }
        }
    }

    public Rect<float> CursorRect => this.caretCommand.GetAffineRect();

    public override bool IsParentDependent { get; }

    public override Text Target { get; }

    public TextLayout(Text target)
    {
        this.Target = target;

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

        target.AppendChild(this.caretTimer);
        target.AppendChild(this.selectionTimer);

        this.caretCommand              = CommandPool.RectCommand.Get();
        this.caretCommand.Color        = Color.White;
        this.caretCommand.Flags        = Flags.ColorAsBackground;
        this.caretCommand.TextureMap   = TextureMap.Default;
        this.caretCommand.Size         = new(this.caretWidth, this.LineHeight);
        this.caretCommand.StencilLayer = this.StencilLayer;

        target.Commands.Add(this.caretCommand);

        this.Target.Buffer.Modified += this.OnTextChange;
    }

    private static void AllocateCommands(List<Command> commands, int count)
    {
        commands.EnsureCapacity(count);

        if (count < commands.Count)
        {
            ReleaseCommands(commands, commands.Count - count);
        }
        else
        {
            var previousCount = commands.Count;

            commands.SetCount(count);

            var span = commands.AsSpan();

            for (var i = previousCount; i < span.Length; i++)
            {
                span[i] = CommandPool.TextCommand.Get();
            }
        }
    }

    private static ulong CombineIds(int characterIndex, int elementIndex) =>
        ((ulong)characterIndex << 24) | ((uint)elementIndex);

    private static void ReleaseCommands(List<Command> commands, int count)
    {
        if (count > 0)
        {
            var span  = commands.AsSpan();
            var start = span.Length - count;

            for (var i = start; i < span.Length; i++)
            {
                CommandPool.TextCommand.Return((TextCommand)span[i]);

                span[i] = default!;
            }

            commands.SetCount(start);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ApplySelection(int index, in TextSelection selection, TextCommand selectionCommand)
    {
        if (selectionCommand.Surrogate == TextCommand.SurrogateKind.High)
        {
            return;
        }

        if (index >= selection.Start && index < selection.End)
        {
            selectionCommand.Color           = new(0, 0, 1, 0.5f);
            selectionCommand.Flags           = Flags.ColorAsBackground;
            selectionCommand.PipelineVariant = PipelineVariant.Index | PipelineVariant.Color;

            if (selectionCommand.Index != default)
            {
                ((TextCommand)this.Target.Commands[selectionCommand.Index]).Color = Color.White;
            }
        }
        else
        {
            this.ClearSelection(selectionCommand);
        }
    }

    private void BlinkCaret()
    {
        this.caretIsVisible = !this.caretIsVisible;

        this.caretCommand.PipelineVariant = this.caretIsVisible ? PipelineVariant.Color : default;

        this.RequestUpdate(false);
    }

    private void ClearSelection(TextCommand selectionCommand)
    {
        if (selectionCommand.Surrogate == TextCommand.SurrogateKind.High)
        {
            return;
        }

        selectionCommand.Color           = default;
        selectionCommand.Flags           = default;
        selectionCommand.PipelineVariant = PipelineVariant.Index;

        if (selectionCommand.Index != default)
        {
            ((TextCommand)this.Target.Commands[selectionCommand.Index]).Color = this.GetInheritedStyleSource()?.ComputedStyle?.Color ?? default;
        }
    }

    private void DrawCaret()
    {
        var textLength = this.Target.Buffer.Length;

        if (textLength > 0)
        {
            Vector2<float> position;

            if (this.CaretPosition == textLength)
            {
                var command = (RectCommand)this.Target.Commands[textLength - 1];

                if (this.Target.Buffer[^1] == '\n')
                {
                    position.X = 0;
                    position.Y = command.Transform.Position.Y - this.LineHeight;
                }
                else
                {
                    position = command.Transform.Position;
                    position.X += command.Size.Width;
                }
            }
            else
            {
                position = ((RectCommand)this.Target.Commands[(int)this.CaretPosition]).Transform.Position;
            }

            this.caretCommand.Transform = Transform2D.CreateTranslated(position);
        }
        else
        {
            this.caretCommand.Transform = Transform2D.Identity;
        }
    }

    private void DrawSelection()
    {
        var text = this.Target.Buffer.AsSpan();

        if (this.Selection.HasValue)
        {
            var range = this.Selection.Value.Ordered();

            for (var i = 0; i < text.Length; i++)
            {
                this.ApplySelection(i, range, (TextCommand)this.Target.Commands[i]);
            }
        }
        else
        {
            for (var i = 0; i < text.Length; i++)
            {
                this.ClearSelection((TextCommand)this.Target.Commands[i]);
            }
        }
    }

    private void DrawText()
    {
        if (this.font == null)
        {
            throw new InvalidOperationException();
        }

        var textSpan = this.Target.Buffer.AsSpan();

        var style = this.GetInheritedStyleSource()?.ComputedStyle;

        var glyphs = this.font.Typeface.GetGlyphs(textSpan);
        var atlas  = TextStorage.Singleton.GetAtlas(this.font.Typeface.FamilyName, (uint)this.font.Size);

        using var glyphsBoundsRef = new RefArray<SKRect>(glyphs.Length);
        using var glyphsWidths    = new RefArray<float>(glyphs.Length);

        var glyphsBounds = glyphsBoundsRef.AsSpan();

        this.font.GetGlyphWidths(glyphs, glyphsWidths, glyphsBounds);

        var baseLine   = -this.BaseLine;
        var cursor     = new Point<int>(0, baseLine);
        var boundings  = new Size<uint>(0, this.LineHeight);

        textSpan.GetTextInfo(out var visibleCharacterCount, out var linesCount);

        this.textLines.Resize(linesCount, default);

        var lines = this.textLines.AsSpan();

        lines[0].Start = 0;

        this.Target.Commands.RemoveAt(this.Target.Commands.Count - 1);

        AllocateCommands(this.Target.Commands, textSpan.Length + visibleCharacterCount);

        this.Target.Commands.Add(this.caretCommand);

        var elementIndex = this.Target.Index + 1;
        var range        = this.Selection?.Ordered();

        var textOffset = 0;
        var charOffset = 0;
        var lineIndex  = 0;
        var color      = style?.Color ?? new();

        for (var i = 0; i < textSpan.Length; i++)
        {
            var charIndex        = i - charOffset;
            var selectionCommand = (TextCommand)this.Target.Commands[i];

            selectionCommand.Border       = default;
            selectionCommand.Color        = default;
            selectionCommand.Flags        = default;
            selectionCommand.Index        = default;
            selectionCommand.Surrogate    = default;
            selectionCommand.Line         = lineIndex;
            selectionCommand.TextureMap   = TextureMap.Default;
            selectionCommand.StencilLayer = this.StencilLayer;

            var character = textSpan[i];

            if (!char.IsLowSurrogate(character))
            {
                selectionCommand.ObjectId  = CombineIds(i + 1, elementIndex);
                selectionCommand.Size      = new(glyphsWidths[charIndex], this.LineHeight);
                selectionCommand.Transform = Transform2D.CreateTranslated(new(cursor.X, cursor.Y - baseLine));

                if (!char.IsWhiteSpace(character))
                {
                    ref readonly var bounds = ref glyphsBounds[charIndex];

                    var len = 1;

                    if (char.IsHighSurrogate(character))
                    {
                        len++;
                        charOffset++;
                        selectionCommand.Surrogate       = TextCommand.SurrogateKind.High;
                        selectionCommand.PipelineVariant = PipelineVariant.None;
                    }

                    var chars = textSpan.Slice(i, len);

                    var glyph  = TextStorage.Singleton.DrawGlyph(this.font, atlas, chars, bounds);
                    var size   = new Size<float>(bounds.Width, bounds.Height);
                    var offset = new Vector2<float>(float.Round(cursor.X + bounds.Left), float.Round(cursor.Y - bounds.Top));

                    var characterIndex = textSpan.Length + textOffset;

                    selectionCommand.Index = characterIndex;

                    var characterCommand = (TextCommand)this.Target.Commands[characterIndex];

                    characterCommand.Border          = default;
                    characterCommand.Color           = color;
                    characterCommand.Flags           = Flags.GrayscaleTexture | Flags.MultiplyColor;
                    characterCommand.Index           = selectionCommand.Index;
                    characterCommand.Line            = selectionCommand.Line;
                    characterCommand.TextureMap      = glyph;
                    characterCommand.ObjectId        = default;
                    characterCommand.PipelineVariant = PipelineVariant.Color;
                    characterCommand.Size            = size;
                    characterCommand.Transform       = Transform2D.CreateTranslated(offset);
                    characterCommand.StencilLayer    = this.StencilLayer;

                    cursor.X += (int)float.Round(glyphsWidths[charIndex]);

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
                    cursor.X += (int)float.Round(glyphsWidths[charIndex]);
                }

                boundings.Width = uint.Max(boundings.Width, (uint)cursor.X);
            }
            else
            {
                var previous = (TextCommand)this.Target.Commands[i - 1];

                selectionCommand.Index     = previous.Index;
                selectionCommand.ObjectId  = previous.ObjectId;
                selectionCommand.Size      = previous.Size;
                selectionCommand.Transform = previous.Transform;
                selectionCommand.Surrogate = TextCommand.SurrogateKind.Low;
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
        if (char.IsControl(this.Target.Buffer[(int)character]))
        {
            return;
        }

        var command = (TextCommand)this.Target.Commands[(int)character];

        var cursor = this.Target.Transform.Matrix.Inverse() * new Vector2<float>(x, -y);

        if (cursor.X > command.Transform.Position.X + (command.Size.Width / 2))
        {
            character += command.Surrogate == TextCommand.SurrogateKind.High ? 2u : 1;
        }
    }

    private StyledLayout? GetInheritedStyleSource() =>
        GetStyleSource(this.Target.Parent);

    private void OnParentStyleChanged(StyleProperty property)
    {
        if (property.HasAnyFlag(StyleProperty.FontFamily | StyleProperty.FontSize | StyleProperty.FontWeight))
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
        this.Selection   = this.Selection?.WithEnd(uint.Min(this.Selection.Value.End, (uint)this.Target.Buffer.Length));
        this.textIsDirty = true;

        this.RequestUpdate(true);
    }

    protected override void OnDisposed()
    {
        foreach (var command in this.Target.Commands[..^1])
        {
            CommandPool.TextCommand.Return((TextCommand)command);
        }

        CommandPool.RectCommand.Return(this.caretCommand);
        this.Target.Commands.Clear();
    }

    public void AdjustScroll() =>
        this.Target.ComposedParentElement?.ScrollTo(this.Target.GetCursorBoundings());

    public void ClearCaret()
    {
        if (!this.CanSelect)
        {
            return;
        }

        this.HideCaret();
    }

    public void ClearSelection() =>
        this.Selection = default;

    public TextLine GetCharacterLine(uint index) =>
        this.textLines[this.GetCharacterLineIndex(index)];

    public int GetCharacterLineIndex(uint index) =>
        ((TextCommand)this.Target.Commands[(int)index]).Line;

    public TextLine? GetCharacterNextLine(uint index) =>
        !this.Target.Buffer.IsEmpty && this.GetCharacterLineIndex(index) + 1 is var lineIndex && lineIndex < this.textLines.Count
            ? this.textLines[lineIndex]
            : (TextLine?)default;

    public TextLine? GetCharacterPreviousLine(uint index) =>
        !this.Target.Buffer.IsEmpty && this.GetCharacterLineIndex(index) - 1 is var lineIndex && lineIndex > -1
            ? this.textLines[lineIndex]
            : (TextLine?)default;

    public void HandleTargetActivated()
    {
        if (!this.CanSelect)
        {
            return;
        }

        this.selectionTimer.Start();

        IsSelectingText = true;
    }

    public void HandleTargetAdopted(Element parentElement)
    {
        parentElement.Layout.StyleChanged += this.OnParentStyleChanged;

        this.OnParentStyleChanged(StyleProperty.All);
    }

    public void HandleTargetDeactivated()
    {
        this.selectionTimer.Stop();

        IsSelectingText = false;
    }

    public void HandleTargetIndexed()
    {
        this.UpdateDirtyLayout();

        var elementIndex = this.Target.Index + 1;
        var commands     = this.Target.Commands.AsSpan(0, this.Target.Buffer.Length);

        for (var i = 0; i < commands.Length; i++)
        {
            var command = (TextCommand)commands[i];

            if (command.Surrogate != TextCommand.SurrogateKind.Low)
            {
                commands[i].ObjectId = CombineIds(i + 1, elementIndex);
            }
            else
            {
                command.ObjectId = commands[i - 1].ObjectId;
            }
        }
    }

    public void HandleTargetMouseOut()
    {
        if (!this.CanSelect)
        {
            return;
        }

        this.isMouseOverText = IsHoveringText = false;
    }

    public void HandleTargetMouseOver()
    {
        if (!this.CanSelect)
        {
            return;
        }

        this.isMouseOverText = IsHoveringText = true;

        this.SetCursor(Cursor.Text);
    }

    public void HandleTargetRemoved(Element parentElement) =>
        parentElement.Layout.StyleChanged -= this.OnParentStyleChanged;

    public void HideCaret()
    {
        this.caretIsDirty   = true;
        this.caretIsVisible = false;

        this.caretCommand.PipelineVariant = default;

        this.caretTimer.Stop();

        this.RequestUpdate(false);
    }

    public void PropagateSelection(uint characterPosition)
    {
        var text = this.Target.Buffer.AsSpan();

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

        this.CaretPosition = (uint)end;

        this.Selection = new((uint)start, (uint)end);
    }

    public void SetCaret(ushort x, ushort y)
    {
        if (this.Target.Buffer.IsEmpty || !this.CanSelect)
        {
            return;
        }

        var cursor = this.Target.TransformWithOffset.Matrix.Inverse() * new Vector2<float>(x, -y);

        if (cursor == this.previouCursor)
        {
            return;
        }

        this.previouCursor = cursor;

        this.ClearSelection();

        bool isOnCursorLine(in Rect<float> rect) => cursor.Y >= rect.Position.Y - rect.Size.Height && cursor.Y <= rect.Position.Y;

        var text = this.Target.Buffer;

        var lineSpan = this.textLines.AsSpan();
        var position = (uint)text.Length;

        for (var i = 0; i < lineSpan.Length; i++)
        {
            var rect = ((TextCommand)this.Target.Commands[(int)lineSpan[i].Start]).GetAffineRect();

            if (isOnCursorLine(rect))
            {
                position = lineSpan[i].End + 1 == text.Length && text[^1] != '\n'
                    ? (uint)text.Length
                    : lineSpan[i].End;

                break;
            }
        }

        this.CaretPosition = position;
    }

    public void SetCaret(ushort x, ushort y, uint position)
    {
        if (!this.CanSelect)
        {
            return;
        }

        this.GetCharacterOffset(x, y, ref position);

        this.CaretPosition = position;
    }

    public void ShowCaret()
    {
        this.caretIsDirty   = true;
        this.caretIsVisible = true;

        this.caretCommand.PipelineVariant = PipelineVariant.Color;

        this.caretTimer.Start();

        this.RequestUpdate(false);
    }

    private void UpdateSelection()
    {
        if (this.isMouseOverText)
        {
            return;
        }

        var position = Input.GetMousePosition();

        this.UpdateSelection(position.X, position.Y);
    }

    public void UpdateSelection(ushort x, ushort y)
    {
        if (this.Target.Buffer.IsEmpty || !this.CanSelect)
        {
            return;
        }

        var cursor = this.Target.TransformWithOffset.Matrix.Inverse() * new Vector2<float>(x, -y);

        if (cursor == this.previouCursor)
        {
            return;
        }

        this.previouCursor = cursor;

        var selection = this.Selection ?? new(this.CaretPosition, this.CaretPosition);

        var textLength = this.Target.Buffer.Length;

        var startIndex = int.Min((int)selection.Start, textLength - 1);
        var endIndex   = int.Min((int)selection.End, textLength - 1);

        var lineSpan     = this.textLines.AsSpan();
        var commandsSpan = this.Target.Commands.AsSpan();

        var startAnchor = (TextCommand)commandsSpan[startIndex];
        var endAnchor   = (TextCommand)commandsSpan[endIndex];

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

                scanBelow(commandsSpan, endAnchor.Line, lineSpan.Length, lineSpan, ref position);
            }
            else
            {
                scanAbove(commandsSpan, endAnchor.Line, startAnchor.Line, lineSpan, ref position);
            }
        }
        else
        {
            if (isCursorAboveBottom(endAnchorRect))
            {
                position = 0;

                scanAbove(commandsSpan, endAnchor.Line, -1, lineSpan, ref position);
            }
            else
            {
                scanBelow(commandsSpan, endAnchor.Line, startAnchor.Line + 1, lineSpan, ref position);
            }
        }

        this.Selection     = selection.WithEnd(position);
        this.CaretPosition = position;

        #region local methods
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
        #endregion
    }

    public void UpdateSelection(ushort x, ushort y, uint character)
    {
        if (!this.CanSelect)
        {
            return;
        }

        this.GetCharacterOffset(x, y, ref character);

        this.Selection     = this.Selection?.WithEnd(character) ?? new(this.CaretPosition, character);
        this.CaretPosition = character;
    }

    public override void Update()
    {
        if (this.Target.Buffer.IsEmpty)
        {
            if (this.textIsDirty)
            {
                this.Target.Commands.RemoveAt(this.Target.Commands.Count - 1);

                ReleaseCommands(this.Target.Commands, this.Target.Commands.Count);

                this.Target.Commands.Add(this.caretCommand);

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
}
