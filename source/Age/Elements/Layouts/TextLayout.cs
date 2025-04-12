using Age.Commands;
using Age.Core.Extensions;
using Age.Core.Interop;
using Age.Numerics;
using Age.Platforms.Display;
using Age.Resources;
using Age.Services;
using Age.Styling;
using SkiaSharp;
using System.Runtime.CompilerServices;

using static Age.Shaders.CanvasShader;

using Timer = Age.Scene.Timer;

namespace Age.Elements.Layouts;

internal sealed partial class TextLayout : Layout
{
    private readonly RectCommand    caretCommand;
    private readonly Timer          caretTimer;
    private readonly int            caretWidth = 2;
    private readonly Timer          selectionTimer;
    private readonly Text           target;
    private readonly List<TextLine> textLines = new(1);

    private bool           caretIsDirty;
    private bool           caretIsVisible;
    private SKFont?        font;
    private float          fontLeading;
    private bool           isMouseOverText;
    private Vector2<float> previouCursor;
    private bool           selectionIsDirty;
    private bool           textIsDirty;

    private bool CanSelect => this.Parent?.State.Style.TextSelection != false;

    public uint CaretPosition
    {
        get;
        set
        {
            if (field != value || !this.caretTimer.Running)
            {
                field = value;

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

                foreach (var command in this.target.Commands)
                {
                    command.StencilLayer = value;
                }
            }
        }
    }

    public Rect<float> CursorRect => this.caretCommand.Rect;

    public override bool IsParentDependent { get; }

    public override Text Target => this.target;

    public TextLayout(Text target)
    {
        this.target = target;

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

        this.caretCommand                 = CommandPool.RectCommand.Get();
        this.caretCommand.Color           = Color.White;
        this.caretCommand.Flags           = Flags.ColorAsBackground;
        this.caretCommand.MappedTexture   = MappedTexture.Default;
        this.caretCommand.Rect            = new(new(this.caretWidth, this.LineHeight), default);
        this.caretCommand.StencilLayer    = this.StencilLayer;

        target.Commands.Add(this.caretCommand);

        this.target.Buffer.Modified += this.OnTextChange;
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
        if (index >= selection.Start && index < selection.End)
        {
            selectionCommand.Color           = new(0, 0, 1, 0.5f);
            selectionCommand.Flags           = Flags.ColorAsBackground;
            selectionCommand.PipelineVariant = PipelineVariant.Index | PipelineVariant.Color;

            if (selectionCommand.Index != default)
            {
                ((TextCommand)this.target.Commands[selectionCommand.Index]).Color = Color.White;
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
        selectionCommand.Color           = default;
        selectionCommand.Flags           = default;
        selectionCommand.PipelineVariant = PipelineVariant.Index;

        if (selectionCommand.Index != default)
        {
            ((TextCommand)this.target.Commands[selectionCommand.Index]).Color = this.Parent?.State.Style.Color ?? default;
        }
    }

    private void DrawCaret()
    {
        if (this.target.Buffer.Length > 0)
        {
            Point<float> position;

            if (this.CaretPosition == this.target.Buffer.Length)
            {
                var rect = ((RectCommand)this.target.Commands[this.target.Buffer.Length - 1]).Rect;

                if (this.target.Buffer[^1] == '\n')
                {
                    position.X = 0;
                    position.Y = rect.Position.Y - this.LineHeight;
                }
                else
                {
                    position = rect.Position;
                    position.X += rect.Size.Width;
                }
            }
            else
            {
                position = ((RectCommand)this.target.Commands[(int)this.CaretPosition]).Rect.Position;
            }

            this.caretCommand.Rect = this.caretCommand.Rect with
            {
                Position = position,
            };
        }
        else
        {
            this.caretCommand.Rect = new()
            {
                Position = default,
                Size     = new(this.caretWidth, this.LineHeight),
            };
        }
    }

    private void DrawSelection()
    {
        var text = this.target.Buffer.AsSpan();

        if (this.Selection.HasValue)
        {
            var range = this.Selection.Value.Ordered();

            for (var i = 0; i < text.Length; i++)
            {
                this.ApplySelection(i, range, (TextCommand)this.target.Commands[i]);
            }
        }
        else
        {
            for (var i = 0; i < text.Length; i++)
            {
                this.ClearSelection((TextCommand)this.target.Commands[i]);
            }
        }
    }

    private void DrawText()
    {
        if (this.font == null)
        {
            throw new InvalidOperationException();
        }

        var text = this.target.Buffer.AsSpan();

        var style = this.target.ComposedParentElement!.Layout.State.Style;

        var glyphs = this.font.Typeface.GetGlyphs(text);
        var atlas  = TextStorage.Singleton.GetAtlas(this.font.Typeface.FamilyName, (uint)this.font.Size);

        using var glyphsBoundsRef = new RefArray<SKRect>(glyphs.Length);
        using var glyphsWidths    = new RefArray<float>(glyphs.Length);

        var glyphsBounds = glyphsBoundsRef.AsSpan();

        this.font.GetGlyphWidths(glyphs, glyphsWidths, glyphsBounds);

        var baseLine   = -this.BaseLine;
        var cursor     = new Point<int>(0, baseLine);
        var boundings  = new Size<uint>(0, this.LineHeight);

        text.GetTextInfo(out var nonWhitespaceCount, out var linesCount);

        this.textLines.Resize(linesCount, default);

        var lines = this.textLines.AsSpan();

        this.target.Commands.RemoveAt(this.target.Commands.Count - 1);

        AllocateCommands(this.target.Commands, text.Length + nonWhitespaceCount);

        this.target.Commands.Add(this.caretCommand);

        var elementIndex = this.target.Index + 1;
        var range        = this.Selection?.Ordered();

        var textOffset = 0;
        var lineIndex  = 0;

        lines[0].Start = 0;

        for (var i = 0; i < text.Length; i++)
        {
            var character = text[i];

            var selectionCommand = (TextCommand)this.target.Commands[i];

            selectionCommand.Border        = default;
            selectionCommand.Color         = default;
            selectionCommand.Flags         = default;
            selectionCommand.Index         = default;
            selectionCommand.Line          = lineIndex;
            selectionCommand.MappedTexture = MappedTexture.Default;
            selectionCommand.ObjectId      = CombineIds(i + 1, elementIndex);
            selectionCommand.Rect          = new(new(glyphsWidths[i], this.LineHeight), new(cursor.X, cursor.Y - baseLine));
            selectionCommand.StencilLayer  = this.StencilLayer;

            if (!char.IsWhiteSpace(character))
            {
                ref readonly var bounds = ref glyphsBounds[i];

                var glyph    = TextStorage.Singleton.DrawGlyph(this.font, atlas, character, bounds);
                var size     = new Size<float>(bounds.Width, bounds.Height);
                var position = new Point<float>(float.Round(cursor.X + bounds.Left), float.Round(cursor.Y - bounds.Top));
                var color    = style.Color ?? new();

                var atlasSize = new Point<float>(atlas.Size.Width, atlas.Size.Height);

                var uv = new UVRect
                {
                    P1 = new Point<float>(glyph.Position.X, glyph.Position.Y) / atlasSize,
                    P2 = new Point<float>(glyph.Position.X + glyph.Size.Width, glyph.Position.Y) / atlasSize,
                    P3 = new Point<float>(glyph.Position.X + glyph.Size.Width, glyph.Position.Y + glyph.Size.Height) / atlasSize,
                    P4 = new Point<float>(glyph.Position.X, glyph.Position.Y + glyph.Size.Height) / atlasSize,
                };

                var characterIndex = text.Length + textOffset;

                selectionCommand.Index = characterIndex;

                var characterCommand = (TextCommand)this.target.Commands[characterIndex];

                characterCommand.Border          = default;
                characterCommand.Color           = color;
                characterCommand.Flags           = Flags.GrayscaleTexture | Flags.MultiplyColor;
                characterCommand.Index           = selectionCommand.Index;
                characterCommand.Line            = selectionCommand.Line;
                characterCommand.MappedTexture   = new(atlas.Texture, uv);
                characterCommand.ObjectId        = default;
                characterCommand.PipelineVariant = PipelineVariant.Color;
                characterCommand.Rect            = new(size, position);
                characterCommand.StencilLayer    = this.StencilLayer;

                cursor.X += (int)float.Round(glyphsWidths[i]);

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
                cursor.X += (int)float.Round(glyphsWidths[i]);
            }

            boundings.Width = uint.Max(boundings.Width, (uint)cursor.X);

            if (range.HasValue)
            {
                this.ApplySelection(i, range.Value, selectionCommand);
            }
            else
            {
                this.ClearSelection(selectionCommand);
            }
        }

        lines[^1].Length = (uint)text.Length - 1 - lines[^1].Start + 1;

        atlas.Update();

        this.Boundings = boundings;
    }

    private void GetCharacterOffset(ushort x, ushort y, ref uint character)
    {
        if (char.IsControl(this.target.Buffer[(int)character]))
        {
            return;
        }

        var command = (RectCommand)this.target.Commands[(int)character];

        var cursor = this.target.Transform.Matrix.Inverse() * new Vector2<float>(x, -y);

        if (cursor.X > command.Rect.Position.X + command.Rect.Size.Width / 2)
        {
            character++;
        }
    }

    private void OnTextChange()
    {
        this.Selection   = this.Selection?.WithEnd(uint.Min(this.Selection.Value.End, (uint)this.target.Buffer.Length));
        this.textIsDirty = true;

        this.RequestUpdate(true);
    }

    private void TargetParentStyleChanged(StyleProperty property)
    {
        if (property.HasFlags(StyleProperty.FontFamily | StyleProperty.FontSize | StyleProperty.FontWeight))
        {
            var style = this.Parent!.State.Style;

            var fontFamily = string.Intern(style.FontFamily ?? "Segoi UI");
            var fontWeight = (int)(style.FontWeight ?? FontWeight.Normal);
            var fontSize   = this.Parent.FontSize;

            if (this.font?.Size != fontSize || this.font.Typeface.FamilyName != fontFamily || this.font.Typeface.FontWeight != fontWeight)
            {
                this.font = TextStorage.Singleton.GetFont(fontFamily, fontSize, fontWeight);
                this.font.GetFontMetrics(out var metrics);

                this.LineHeight  = (uint)float.Round(-metrics.Ascent + metrics.Descent);
                this.BaseLine    = (int)float.Round(-metrics.Ascent);
                this.fontLeading = metrics.Leading;

                this.caretCommand.Rect = this.caretCommand.Rect with
                {
                    Size = new(this.caretWidth, this.LineHeight),
                };

                this.caretIsDirty = true;
            }
        }

        this.textIsDirty = true;
        this.RequestUpdate(true);
    }

    protected override void Disposed()
    {
        foreach (var command in this.Target.Commands)
        {
            CommandPool.RectCommand.Return(this.caretCommand);
            CommandPool.TextCommand.Return((TextCommand)command);
            this.Target.Commands.Clear();
        }
    }

    public void AdjustScroll() =>
        this.target.ComposedParentElement?.ScrollTo(this.target.GetCursorBoundings());

    public void ClearSelection() =>
        this.Selection = default;

    public TextLine GetCharacterLine(uint index) =>
        this.textLines[this.GetCharacterLineIndex(index)];

    public int GetCharacterLineIndex(uint index) =>
        ((TextCommand)this.target.Commands[(int)index]).Line;

    public TextLine? GetCharacterNextLine(uint index)
    {
        if (!this.target.Buffer.IsEmpty && this.GetCharacterLineIndex(index) + 1 is var lineIndex && lineIndex < this.textLines.Count)
        {
            return this.textLines[lineIndex];
        }

        return default;
    }

    public TextLine? GetCharacterPreviousLine(uint index)
    {
        if (!this.target.Buffer.IsEmpty && this.GetCharacterLineIndex(index) - 1 is var lineIndex && lineIndex > -1)
        {
            return this.textLines[lineIndex];
        }

        return default;
    }

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
        var text = this.target.Buffer.AsSpan();

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
        if (this.target.Buffer.IsEmpty || !this.CanSelect)
        {
            return;
        }

        var cursor = this.target.TransformWithOffset.Matrix.Inverse() * new Vector2<float>(x, -y);

        if (cursor == this.previouCursor)
        {
            return;
        }

        this.previouCursor = cursor;

        this.ClearSelection();

        bool isOnCursorLine(in Rect<float> rect) => cursor.Y >= rect.Position.Y - rect.Size.Height && cursor.Y <= rect.Position.Y;

        var text = this.target.Buffer;

        var lineSpan = this.textLines.AsSpan();
        var position = (uint)text.Length;

        for (var i = 0; i < lineSpan.Length; i++)
        {
            var rect = ((TextCommand)this.target.Commands[(int)lineSpan[i].Start]).Rect;

            if (isOnCursorLine(rect))
            {
                position = lineSpan[i].End + 1 == text.Length && text[^1] != '\n'
                    ? (uint)text.Length
                    : lineSpan[i].End;

                break;
            }
        }

        this.CaretPosition = (uint)position;
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

    public void ClearCaret()
    {
        if (!this.CanSelect)
        {
            return;
        }

        this.HideCaret();
    }

    public void TargetActivated()
    {
        if (!this.CanSelect)
        {
            return;
        }

        this.selectionTimer.Start();

        IsSelectingText = true;
    }

    public void TargetAdopted(Element parentElement)
    {
        parentElement.Layout.State.Changed += this.TargetParentStyleChanged;

        this.TargetParentStyleChanged(StyleProperty.All);
    }

    public void TargetDeactivated()
    {
        this.selectionTimer.Stop();

        IsSelectingText = false;
    }

    public void TargetIndexed()
    {
        var elementIndex = this.target.Index + 1;
        var commands     = this.target.Commands.AsSpan(0, this.target.Buffer.Length);

        for (var i = 0; i < commands.Length; i++)
        {
            commands[i].ObjectId = CombineIds(i + 1, elementIndex);
        }
    }

    public void TargetMouseOut()
    {
        if (!this.CanSelect)
        {
            return;
        }

        this.isMouseOverText = IsHoveringText = false;
    }

    public void TargetMouseOver()
    {
        if (!this.CanSelect)
        {
            return;
        }

        this.isMouseOverText = IsHoveringText = true;

        this.SetCursor(CursorKind.Text);
    }

    public void TargetRemoved(Element parentElement) =>
        parentElement.Layout.State.Changed -= this.TargetParentStyleChanged;

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
        if (this.target.Buffer.IsEmpty || !this.CanSelect)
        {
            return;
        }

        var cursor = this.target.TransformWithOffset.Matrix.Inverse() * new Vector2<float>(x, -y);

        if (cursor == this.previouCursor)
        {
            return;
        }

        this.previouCursor = cursor;

        var selection = this.Selection ?? new(this.CaretPosition, this.CaretPosition);

        var startIndex = int.Min((int)selection.Start, this.target.Buffer.Length - 1);
        var endIndex   = int.Min((int)selection.End,   this.target.Buffer.Length - 1);

        var lineSpan     = this.textLines.AsSpan();
        var commandsSpan = this.target.Commands.AsSpan();

        var startAnchor = (TextCommand)commandsSpan[startIndex];
        var endAnchor   = (TextCommand)commandsSpan[endIndex];

        var position = selection.End;

        if (isOnCursorLine(endAnchor.Rect))
        {
            resolveLine(lineSpan, endAnchor.Line, ref position);
        }
        else if (isCursorBelow(startAnchor.Rect))
        {
            if (isCursorBelowTop(endAnchor.Rect))
            {
                position = (uint)this.target.Buffer.Length;

                scanBelow(commandsSpan, endAnchor.Line, lineSpan.Length, lineSpan, ref position);
            }
            else
            {
                scanAbove(commandsSpan, endAnchor.Line, startAnchor.Line, lineSpan, ref position);
            }
        }
        else
        {
            if (isCursorAboveBottom(endAnchor.Rect))
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
            position = isCursorBefore(endAnchor.Rect)
                ? lines[index].Start
                : lines[index].End + 1 == this.target.Buffer.Length
                    ? (uint)this.target.Buffer.Length
                    : lines[index].End;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool isCursorAfter(in Rect<float> rect) => cursor.X > rect.Position.X + rect.Size.Width / 2;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool isCursorBefore(in Rect<float> rect) => !isCursorAfter(rect);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool isOnCursorLine(in Rect<float> rect) => isCursorBelowTop(rect) && isCursorAboveBottom(rect);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool isCursorBelow(in Rect<float> rect) => cursor.Y < rect.Position.Y - rect.Size.Height / 2;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool isCursorBelowTop(in Rect<float> rect) => cursor.Y < rect.Position.Y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool isCursorAboveBottom(in Rect<float> rect) => cursor.Y > rect.Position.Y - rect.Size.Height;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void scanAbove(scoped ReadOnlySpan<Command> commands, int start, int end, scoped ReadOnlySpan<TextLine> lines, ref uint position)
        {
            for (var i = start; i > end; i--)
            {
                var command = (TextCommand)commands[(int)lines[i].Start];

                if (isCursorBelowTop(command.Rect))
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
                var command = (TextCommand)commands[(int)lines[i].Start];

                if (isCursorAboveBottom(command.Rect))
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
        if (this.target.Buffer.IsEmpty)
        {
            if (this.textIsDirty)
            {
                this.target.Commands.RemoveAt(this.target.Commands.Count - 1);

                ReleaseCommands(this.target.Commands, this.target.Commands.Count);

                this.target.Commands.Add(this.caretCommand);

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
