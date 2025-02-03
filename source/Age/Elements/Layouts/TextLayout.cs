using Age.Commands;
using Age.Core.Extensions;
using Age.Core;
using Age.Numerics;
using Age.Platforms.Display;
using Age.Resources;
using Age.Scene;
using Age.Services;
using SkiaSharp;
using System.Runtime.CompilerServices;

using Timer = Age.Scene.Timer;

using static Age.Shaders.CanvasShader;
using Age.Styling;
using Age.Extensions;
using static Age.Styling.StyledStateManager;

namespace Age.Elements.Layouts;

internal sealed partial class TextLayout : Layout
{
    private static readonly ObjectPool<RectCommand> rectCommandPool = new(static () => new());

    #region 8-bytes
    private readonly Timer    caretTimer;
    private readonly Timer    selectionTimer;
    private readonly TextNode target;

    private SKTypeface? typeface;
    private SKPaint?    paint;

    public string? Text
    {
        get;
        internal set
        {
            if (value != field)
            {
                field = value;

                this.Selection   = this.Selection?.WithEnd(uint.Min(this.Selection.Value.End, (uint)(field?.Length ?? 0)));
                this.textIsDirty = true;

                this.RequestUpdate(true);
            }
        }
    }
    #endregion

    #region 4-bytes
    private readonly int caretWidth = 2;

    private float          fontLeading;
    private Vector2<float> previouCursor;

    public uint CaretPosition
    {
        get;
        set
        {
            if (field != value || !this.caretTimer.Running)
            {
                field = value;
                this.ShowCaret();

                if (this.Parent!.State.Style.Overflow is OverflowKind.Scroll or OverflowKind.ScrollX or OverflowKind.ScrollY)
                {
                    this.AdjustScroll();
                }
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
    #endregion

    #region 1-byte
    private bool caretIsDirty;
    private bool caretIsVisible;
    private bool selectionIsDirty;
    private bool textIsDirty;
    private bool isMouseOverText;
    private bool canScrollX;
    private bool canScrollY;

    private RectCommand CaretCommand => (RectCommand)this.Target.Commands[^1];
    private bool        CanSelect    => this.Parent?.State.Style.TextSelection != false;

    public override bool IsParentDependent { get; }
    #endregion


    public Rect<float> CursorRect => this.CaretCommand.Rect;

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

    public override BoxLayout? Parent => this.target.ParentElement?.Layout;

    public override TextNode Target => this.target;

    public TextLayout(TextNode target)
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

        target.Commands.Add(rectCommandPool.Get());
    }

    private static void AllocateCommands(List<Command> commands, int length)
    {
        if (length < commands.Count)
        {
            ReleaseCommands(commands, commands.Count - length);
        }
        else
        {
            for (var i = commands.Count; i < length; i++)
            {
                commands.Add(rectCommandPool.Get());
            }
        }
    }

    private static void ReleaseCommands(List<Command> commands, int count)
    {
        if (count > 0)
        {
            for (var i = 0; i < commands.Count; i++)
            {
                rectCommandPool.Return((RectCommand)commands[i]);
            }

            commands.RemoveRange(0, count);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ApplySelection(int index, in TextSelection selection, RectCommand selectionCommand)
    {
        if (index >= selection.Start && index < selection.End)
        {
            selectionCommand.Color           = new(0, 0, 1, 0.5f);
            selectionCommand.Flags           = Flags.ColorAsBackground;
            selectionCommand.PipelineVariant = PipelineVariant.Index | PipelineVariant.Color;

            if (selectionCommand.Metadata != default)
            {
                ((RectCommand)this.target.Commands[(int)selectionCommand.Metadata]).Color = Color.White;
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

        this.CaretCommand.PipelineVariant = this.caretIsVisible ? PipelineVariant.Color : default;

        this.RequestUpdate(false);
    }

    private void ClearSelection(RectCommand selectionCommand)
    {
        selectionCommand.Color           = default;
        selectionCommand.Flags           = default;
        selectionCommand.PipelineVariant = PipelineVariant.Index;

        if (selectionCommand.Metadata != default)
        {
            ((RectCommand)this.target.Commands[(int)selectionCommand.Metadata]).Color = this.Parent?.State.Style.Color ?? default;
        }
    }

    private void DrawCaret()
    {
        var caretCommand = this.CaretCommand;

        if (this.Text?.Length > 0)
        {
            Point<float> position;

            if (this.CaretPosition == this.Text.Length)
            {
                var rect = ((RectCommand)this.target.Commands[this.Text.Length - 1]).Rect;

                if (this.Text[^1] == '\n')
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

            caretCommand.Rect = new(new(this.caretWidth, this.LineHeight), position);
        }
        else
        {
            caretCommand.Rect = new()
            {
                Position = default,
                Size     = new(this.caretWidth, this.LineHeight),
            };
        }
    }

    private void DrawSelection(int textLength)
    {
        if (this.Selection.HasValue)
        {
            var range = this.Selection.Value.Ordered();

            for (var i = 0; i < textLength; i++)
            {
                this.ApplySelection(i, range, (RectCommand)this.target.Commands[i]);
            }
        }
        else
        {
            for (var i = 0; i < textLength; i++)
            {
                this.ClearSelection((RectCommand)this.target.Commands[i]);
            }
        }
    }

    private void DrawText(string text)
    {
        if (this.typeface == null || this.paint == null)
        {
            throw new InvalidOperationException();
        }

        var style = this.target.ParentElement!.Layout.State.Style;

        var glyphs = this.typeface.GetGlyphs(this.Text);
        var font   = this.paint.ToFont();
        var atlas  = TextStorage.Singleton.GetAtlas(this.typeface!.FamilyName, (uint)this.paint.TextSize);

        var glyphsBounds = new SKRect[glyphs.Length];
        var glyphsWidths = new float[glyphs.Length];

        font.GetGlyphWidths(glyphs, glyphsWidths, glyphsBounds, this.paint);

        var baseLine   = -this.BaseLine;
        var cursor     = new Point<int>(0, baseLine);
        var boundings  = new Size<uint>(0, this.LineHeight);

        AllocateCommands(this.target.Commands, text.Length + text.AsSpan().CountNonWhitespaceCharacters() + 1);

        var elementIndex = this.target.Index + 1;

        var range = this.Selection?.Ordered();

        var textOffset = 0;

        for (var i = 0; i < text.Length; i++)
        {
            var character = text[i];

            var selectionCommand = (RectCommand)this.target.Commands[i];

            selectionCommand.MappedTexture = MappedTexture.Default;
            selectionCommand.ObjectId      = (uint)(((i + 1) << 12) | elementIndex);
            selectionCommand.Rect          = new(new(glyphsWidths[i], this.LineHeight), new(cursor.X, cursor.Y - baseLine));
            selectionCommand.StencilLayer  = this.StencilLayer;
            selectionCommand.Metadata      = default;

            if (!char.IsWhiteSpace(character))
            {
                ref readonly var bounds = ref glyphsBounds[i];

                var glyph    = TextStorage.Singleton.DrawGlyph(atlas, character, this.typeface.FamilyName, (ushort)this.paint.TextSize, bounds, this.paint);
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

                selectionCommand.Metadata = (uint)characterIndex;

                var characterCommand = (RectCommand)this.target.Commands[characterIndex];

                characterCommand.Rect            = new(size, position);
                characterCommand.Color           = color;
                characterCommand.Flags           = Flags.GrayscaleTexture | Flags.MultiplyColor;
                characterCommand.PipelineVariant = PipelineVariant.Color;
                characterCommand.MappedTexture   = new(atlas.Texture, uv);
                characterCommand.StencilLayer    = this.StencilLayer;

                cursor.X += (int)float.Round(glyphsWidths[i]);

                textOffset++;

            }
            else if (character == '\n')
            {
                cursor.X = 0;
                cursor.Y -= (int)(this.LineHeight + -this.fontLeading);

                boundings.Height += this.LineHeight + (uint)this.fontLeading;
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

        var caretCommand = (RectCommand)this.target.Commands[^1];

        caretCommand.Color           = Color.White;
        caretCommand.Flags           = Flags.ColorAsBackground;
        caretCommand.MappedTexture   = MappedTexture.Default;
        caretCommand.PipelineVariant = PipelineVariant.Color;
        caretCommand.StencilLayer    = this.StencilLayer;
        caretCommand.Metadata        = default;
        caretCommand.Rect            = default;

        atlas.Update();

        this.Boundings = boundings;
    }

    private void GetCharacterOffset(ushort x, ushort y, ref uint character)
    {
        if (char.IsControl(this.Text![(int)character]))
        {
            return;
        }

        var command = (RectCommand)this.target.Commands[(int)character];

        var cursor = this.Target.Transform.Matrix.Inverse() * new Vector2<float>(x, -y);

        if (cursor.X > command.Rect.Position.X + command.Rect.Size.Width / 2)
        {
            character++;
        }
    }

    private void TargetParentStyleChanged(StyleProperty property)
    {
        if (property != StyleProperty.Color)
        {
            var style = this.Parent!.State.Style;

            var fontFamily = string.Intern(style.FontFamily ?? "Segoi UI");
            var fontWeight = (int)(style.FontWeight ?? FontWeight.Normal);

            if (this.paint?.TextSize != this.Parent!.FontSize || this.typeface?.FamilyName != fontFamily || this.typeface?.FontWeight != fontWeight)
            {
                this.typeface?.Dispose();
                this.paint?.Dispose();

                this.typeface = SKTypeface.FromFamilyName(fontFamily, (SKFontStyleWeight)fontWeight, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);
                this.paint    = new SKPaint
                {
                    Color        = SKColors.Black,
                    IsAntialias  = true,
                    TextAlign    = SKTextAlign.Left,
                    TextSize     = this.Parent!.FontSize,
                    Typeface     = this.typeface,
                    SubpixelText = false,
                };

                this.paint.GetFontMetrics(out var metrics);

                this.LineHeight  = (uint)float.Round(-metrics.Ascent + metrics.Descent);
                this.BaseLine    = (int)float.Round(-metrics.Ascent);
                this.fontLeading = metrics.Leading;
            }

            this.canScrollX = style.Overflow is OverflowKind.Scroll or OverflowKind.ScrollX;
            this.canScrollY = style.Overflow is OverflowKind.Scroll or OverflowKind.ScrollY;
        }

        this.textIsDirty = true;
        this.RequestUpdate(true);
    }

    protected override void Disposed()
    {
        this.paint?.Dispose();
        this.typeface?.Dispose();
    }

    public void AdjustScroll()
    {
        var parent = this.Target.ParentElement;

        if (!this.canScrollX || !this.canScrollY || parent == null)
        {
            return;
        }

        if (this.Text?.Length > 0)
        {
            var boxModel        = parent.GetBoxModel();
            var cursorBoundings = this.Target.GetCursorBoundings();

            var leftBounds   = boxModel.Boundings.Left   + boxModel.Border.Left   + boxModel.Padding.Left;
            var rightBounds  = boxModel.Boundings.Right  - boxModel.Border.Right  - boxModel.Padding.Right;
            var topBounds    = boxModel.Boundings.Top    + boxModel.Border.Top    + boxModel.Padding.Top;
            var bottomBounds = boxModel.Boundings.Bottom - boxModel.Border.Bottom - boxModel.Padding.Bottom;

            var scroll = parent.Scroll;

            var position = this.CaretPosition == this.Text.Length ?
                this.CaretPosition.ClampSubtract(1)
                : this.CaretPosition;

            if (this.canScrollX)
            {
                if (cursorBoundings.Left < leftBounds)
                {
                    var characterBounds = this.Target.GetCharacterBoundings(position);

                    scroll.X = (uint)(characterBounds.Left + scroll.X - leftBounds);

                }
                else if (cursorBoundings.Right > rightBounds)
                {
                    var characterBounds = this.Target.GetCharacterBoundings(position.ClampSubtract(1));

                    scroll.X = (uint)(characterBounds.Right + scroll.X + cursorBoundings.Size.Width - rightBounds);
                }
            }

            if (this.canScrollY)
            {
                if (cursorBoundings.Top < topBounds)
                {
                    var characterBounds = this.Target.GetCharacterBoundings(position);

                    scroll.Y = (uint)(characterBounds.Top + scroll.Y - topBounds);
                }
                else if (cursorBoundings.Bottom > bottomBounds)
                {
                    var characterBounds = this.Target.GetCharacterBoundings(position.ClampSubtract(1));

                    scroll.Y = (uint)(characterBounds.Bottom + scroll.Y + cursorBoundings.Size.Height - bottomBounds);
                }
            }

            parent.Scroll = scroll;
        }
        else
        {
            parent.Scroll = default;
        }
    }

    public void ClearSelection() =>
        this.Selection = default;

    public void HideCaret()
    {
        this.caretIsDirty   = true;
        this.caretIsVisible = false;

        this.CaretCommand.PipelineVariant = default;

        this.caretTimer.Stop();

        this.RequestUpdate(false);
    }

    public void PropagateSelection(uint characterPosition)
    {
        if (!this.CanSelect || this.Text == null || char.IsWhiteSpace(this.Text[(int)characterPosition]))
        {
            return;
        }

        var start = (int)characterPosition - 1;
        var end   = (int)characterPosition;

        while (start > -1 && !char.IsWhiteSpace(this.Text[start]))
        {
            start--;
        }

        start++;

        while (end < this.Text.Length && !char.IsWhiteSpace(this.Text[end]))
        {
            end++;
        }

        this.CaretPosition = (uint)end;

        this.Selection = new((uint)start, (uint)end);
    }

    public void SetCaret(ushort x, ushort y)
    {
        if (this.Text == null || !this.CanSelect)
        {
            return;
        }

        this.ClearSelection();

        var length   = this.Text.Length + 1;
        var position = (uint)length;

        var cursor = this.Target.TransformWithOffset.Matrix.Inverse() * new Vector2<float>(x, -y);

        bool isCursorWithinRect(in Rect<float> rect) => cursor.Y >= rect.Position.Y - rect.Size.Height && cursor.Y <= rect.Position.Y;

        for (var i = 0; i < length; i++)
        {
            var rect = ((RectCommand)this.target.Commands[i]).Rect;

            if (isCursorWithinRect(rect))
            {
                position = (uint)i;

                for (var j = i + 1; j < length; j++)
                {
                    rect = ((RectCommand)this.target.Commands[j]).Rect;

                    if (!isCursorWithinRect(rect))
                    {
                        break;
                    }

                    position = (uint)j;
                }

                if (position == this.Text.Length)
                {
                    position += 1;
                }

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

        this.CaretCommand.PipelineVariant = PipelineVariant.Color;

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
    }

    public void TargetAdopted(Element parentElement)
    {
        parentElement.Layout.State.Changed += this.TargetParentStyleChanged;

        this.TargetParentStyleChanged(StyleProperty.All);
    }

    public void TargetDeactivated() =>
        this.selectionTimer.Stop();

    public void TargetIndexed()
    {
        var parentIndex = this.Target.Index + 1;
        var commands    = this.Target.Commands.AsSpan(0, this.Text?.Length ?? 0);

        for (var i = 0; i < commands.Length; i++)
        {
            commands[i].ObjectId = (uint)(((i + 1) << 12) | parentIndex);
        }
    }

    public void TargetMouseOut()
    {
        if (!this.CanSelect)
        {
            return;
        }

        if (this.target.Tree is RenderTree renderTree)
        {
            this.Parent!.IsHoveringText = false;
            renderTree.Window.Cursor = this.Parent?.State.Style.Cursor ?? default;
        }

        this.isMouseOverText = false;
    }

    public void TargetMouseOver()
    {
        if (!this.CanSelect)
        {
            return;
        }

        if (this.target.Tree is RenderTree renderTree)
        {
            this.Parent!.IsHoveringText = true;
            renderTree.Window.Cursor = CursorKind.Text;
        }

        this.isMouseOverText = true;
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
        if (string.IsNullOrEmpty(this.Text) || !this.CanSelect)
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

        var start = uint.Min(selection.Start, (uint)(this.Text.Length - 1));
        var end   = uint.Min(selection.End,   (uint)(this.Text.Length - 1));

        var startRect = ((RectCommand)this.target.Commands[(int)start]).Rect;
        var endRect   = ((RectCommand)this.target.Commands[(int)end]).Rect;

        var position = selection.End;

        if (isOnCursorLine(endRect))
        {
            locateOnCursorLine(ref position);
        }
        else if (isCursorBelow(startRect))
        {
            locateBelowStart(ref position);
        }
        else
        {
            locateAboveStart(ref position);
        }

        this.Selection     = selection.WithEnd(position);
        this.CaretPosition = position;

        #region local methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool isCursorAfter(in Rect<float> rect) => cursor.X > rect.Position.X + rect.Size.Width / 2;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool isOnCursorLine(in Rect<float> rect) => isCursorBelowTop(rect) && isCursorAboveBottom(rect);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool isCursorBelow(in Rect<float> rect) => cursor.Y < rect.Position.Y - rect.Size.Height / 2;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool isCursorAboveTop(in Rect<float> rect) => cursor.Y > rect.Position.Y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool isCursorBelowTop(in Rect<float> rect) => cursor.Y < rect.Position.Y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool isCursorAboveBottom(in Rect<float> rect) => cursor.Y > rect.Position.Y - rect.Size.Height;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool isCursorBelowBottom(in Rect<float> rect) => cursor.Y < rect.Position.Y - rect.Size.Height;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void locateOnCursorLine(ref uint position)
        {
            if (isCursorAfter(endRect))
            {
                locateCursorEndLine(ref position);
            }
            else
            {
                locateCursorStartLine(ref position);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void locateCursorEndLine(ref uint position)
        {
            position = (uint)this.Text.Length;

            for (var i = (int)end; i < this.Text.Length; i++)
            {
                var command = (RectCommand)this.target.Commands[i];

                if (!isOnCursorLine(command.Rect))
                {
                    position = (uint)i - 1;

                    break;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void locateCursorStartLine(ref uint position)
        {
            position = 0;

            for (var i = (int)end; i > -1; i--)
            {
                var command = (RectCommand)this.target.Commands[i];

                if (!isOnCursorLine(command.Rect))
                {
                    position = (uint)i + 1;

                    break;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void locateForwardDownCursorStartLine(ref uint position)
        {
            for (var i = (int)end + 1; i < this.Text.Length; i++)
            {
                var command = (RectCommand)this.target.Commands[i];

                if (isCursorAboveBottom(command.Rect))
                {
                    position = (uint)i;

                    break;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void locateForwardDownCursorEndLine(ref uint position)
        {
            for (var i = (int)end + 1; i < this.Text.Length; i++)
            {
                var command = (RectCommand)this.target.Commands[i];

                if (isCursorAboveTop(command.Rect))
                {
                    position = (uint)i - 1;

                    break;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void locateBackwardDownCursorEndLine(ref uint position)
        {
            for (var i = (int)end; i > start - 1; i--)
            {
                var command = (RectCommand)this.target.Commands[i];

                if (isCursorBelowTop(command.Rect))
                {
                    position = (uint)i;

                    break;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void locateBackwardDownCursorStartLine(ref uint position)
        {
            for (var i = (int)end; i > start - 1; i--)
            {
                var command = (RectCommand)this.target.Commands[i];

                if (isCursorBelowBottom(command.Rect))
                {
                    position = (uint)i + 1;

                    break;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void locateBackwardUpCursorEndLine(ref uint position)
        {
            for (var i = (int)end - 1; i > -1; i--)
            {
                var command = (RectCommand)this.target.Commands[i];

                if (isCursorBelowTop(command.Rect))
                {
                    position = (uint)i;

                    break;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void locateBackwardUpCursorStartLine(ref uint position)
        {
            for (var i = (int)end - 1; i > -1; i--)
            {
                var command = (RectCommand)this.target.Commands[i];

                if (isCursorBelowBottom(command.Rect))
                {
                    position = (uint)i + 1;

                    break;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void locateForwardUpCursorEndLine(ref uint position)
        {
            for (var i = (int)end; i < start - 1; i++)
            {
                var command = (RectCommand)this.target.Commands[i];

                if (isCursorAboveTop(command.Rect))
                {
                    position = (uint)i - 1;

                    break;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void locateForwardUpCursorStartLine(ref uint position)
        {
            for (var i = (int)end; i < start - 1; i++)
            {
                var command = (RectCommand)this.target.Commands[i];

                if (isCursorAboveBottom(command.Rect))
                {
                    position = (uint)i;

                    break;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void locateBelowStart(ref uint position)
        {
            if (isCursorBelowTop(endRect))
            {
                if (isCursorAfter(endRect))
                {
                    position = (uint)this.Text.Length;

                    locateForwardDownCursorEndLine(ref position);
                }
                else
                {
                    locateForwardDownCursorStartLine(ref position);
                }
            }
            else if (isCursorAfter(endRect))
            {
                locateBackwardDownCursorEndLine(ref position);
            }
            else
            {
                locateBackwardDownCursorStartLine(ref position);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        uint locateAboveStart(ref uint position)
        {
            if (isCursorAboveBottom(endRect))
            {
                position = 0;

                if (isCursorAfter(endRect))
                {
                    locateBackwardUpCursorEndLine(ref position);
                }
                else
                {
                    locateBackwardUpCursorStartLine(ref position);
                }
            }
            else if (isCursorAfter(endRect))
            {
                locateForwardUpCursorEndLine(ref position);
            }
            else
            {
                locateForwardUpCursorStartLine(ref position);
            }

            return position;
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
        if (this.IsDirty)
        {
            if (string.IsNullOrEmpty(this.Text))
            {
                ReleaseCommands(this.target.Commands, this.target.Commands.Count - 1);

                this.Selection = default;
                this.Boundings = default;
            }
            else
            {
                if (this.textIsDirty)
                {
                    this.DrawText(this.Text);
                }
                else if (this.selectionIsDirty)
                {
                    this.DrawSelection(this.Text.Length);
                }
            }

            if (this.caretIsDirty)
            {
                this.DrawCaret();
            }

            this.caretIsDirty     = false;
            this.selectionIsDirty = false;
            this.textIsDirty      = false;

            this.MakePristine();
        }
    }
}
