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

namespace Age.Elements.Layouts;

internal sealed partial class TextLayout : Layout
{
    private static readonly ObjectPool<RectCommand> rectCommandPool = new(static () => new());

    #region 8-bytes
    private readonly Timer    caretTimer;
    private readonly TextNode target;

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

    private float fontLeading;

    public uint CaretPosition
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                this.ShowCaret();
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
    private bool        caretIsDirty;
    private bool        caretIsVisible;
    private bool        selectionIsDirty;
    private bool        textIsDirty;
    private SKTypeface? typeface;
    private SKPaint?    paint;

    public override bool IsParentDependent { get; }
    #endregion

    private RectCommand CaretCommand => (RectCommand)this.target.Commands[0];

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
        this.caretTimer  = new()
        {
            WaitTime = TimeSpan.FromMilliseconds(500),
        };

        this.caretTimer.Timeout += this.BlinkCaret;

        target.AppendChild(this.caretTimer);

        var caretCommand = rectCommandPool.Get();

        caretCommand.Color           = Color.White;
        caretCommand.Flags           = Flags.ColorAsBackground;
        caretCommand.MappedTexture   = MappedTexture.Default;
        caretCommand.PipelineVariant = PipelineVariant.Color;
        caretCommand.StencilLayer    = this.StencilLayer;
        caretCommand.Metadata        = default;
        caretCommand.Rect            = default;

        target.Commands.Add(caretCommand);
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
            var index = commands.Count - count;

            for (var i = index; i < commands.Count; i++)
            {
                rectCommandPool.Return((RectCommand)commands[i]);
            }

            commands.RemoveRange(index, count);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ApplySelection(int index, in TextSelection selection, RectCommand selectionCommand)
    {
        if (index >= selection.Start && index < selection.End && (this.Text![index] != '\n' || index < selection.End - 1))
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
        var caretCommand = (RectCommand)this.target.Commands[0];

        if (this.Text?.Length > 0)
        {
            Point<float> position;

            if (this.CaretPosition == this.Text.Length)
            {
                var rect = ((RectCommand)this.target.Commands[this.Text.Length]).Rect;

                position = rect.Position;
                position.X += rect.Size.Width;
            }
            else
            {
                position = ((RectCommand)this.target.Commands[(int)this.CaretPosition + 1]).Rect.Position;
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
                this.ApplySelection(i, range, (RectCommand)this.target.Commands[i + 1]);
            }
        }
        else
        {
            for (var i = 1; i <= textLength; i++)
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


        Span<SKRect> glyphsBounds = stackalloc SKRect[glyphs.Length];
        Span<float>  glyphsWidths = stackalloc float[glyphs.Length];

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

            var selectionCommand = (RectCommand)this.target.Commands[i + 1];

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

                var characterIndex = text.Length + textOffset + 1;

                selectionCommand.Metadata = (uint)characterIndex;

                var characterCommand = (RectCommand)this.target.Commands[characterIndex];

                characterCommand.Rect            = new(size, position);
                characterCommand.Color           = color;
                characterCommand.Flags           = Flags.GrayscaleTexture | Flags.MultiplyColor;
                characterCommand.PipelineVariant = PipelineVariant.Color;
                characterCommand.MappedTexture   = new(atlas.Texture, uv);
                characterCommand.StencilLayer    = this.StencilLayer;

                boundings.Width = uint.Max(boundings.Width, (uint)float.Round(cursor.X + glyphsWidths[i]));
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

            if (range.HasValue)
            {
                this.ApplySelection(i, range.Value, selectionCommand);
            }
            else
            {
                this.ClearSelection(selectionCommand);
            }
        }

        atlas.Update();

        this.Boundings = boundings;
    }

    private void GetCharacterOffset(ushort x, ushort y, ref uint character)
    {
        var command = (RectCommand)this.target.Commands[(int)character + 1];

        var localPosition = this.Target.Transform.Matrix.Inverse() * new Vector2<float>(x, -y);

        if (localPosition.X > command.Rect.Position.X + command.Rect.Size.Width / 2)
        {
            character++;
        }
    }

    private void TargetParentStyleChanged(StyleProperty property)
    {
        if (property != StyleProperty.Color)
        {
            var style = this.target.ParentElement!.Layout.State.Style;

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
        }

        this.textIsDirty = true;
        this.RequestUpdate(true);
    }

    protected override void Disposed()
    {
        this.paint?.Dispose();
        this.typeface?.Dispose();
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
        if (this.Parent?.State.Style.TextSelection == false || this.Text == null || char.IsWhiteSpace(this.Text[(int)characterPosition]))
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

    public void SetCaret(ushort x, ushort y, uint position)
    {
        if (this.Parent?.State.Style.TextSelection == false)
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
        if (this.Parent?.State.Style.TextSelection == false)
        {
            return;
        }

        this.HideCaret();
    }

    public void TargetAdopted(Element parentElement)
    {
        parentElement.Layout.State.Changed += this.TargetParentStyleChanged;

        this.TargetParentStyleChanged(StyleProperty.All);
    }

    public void TargetIndexed()
    {
        var parentIndex = this.Target.Index + 1;
        var i           = 1;

        foreach (var command in this.Target.Commands.AsSpan())
        {
            if (command.PipelineVariant.HasFlag(PipelineVariant.Index))
            {
                command.ObjectId = (uint)((i << 12) | parentIndex);
                i++;
            }
        }
    }

    public void TargetMouseOut()
    {
        if (this.Parent?.State.Style.TextSelection == false)
        {
            return;
        }

        if (this.target.Tree is RenderTree renderTree)
        {
            this.Parent!.IsHoveringText = false;
            renderTree.Window.Cursor = this.Parent?.State.Style.Cursor ?? default;
        }
    }

    public void TargetMouseOver()
    {
        if (this.Parent?.State.Style.TextSelection == false)
        {
            return;
        }

        if (this.target.Tree is RenderTree renderTree)
        {
            this.Parent!.IsHoveringText = true;
            renderTree.Window.Cursor = CursorKind.Text;
        }
    }

    public void TargetRemoved(Element parentElement) =>
        parentElement.Layout.State.Changed -= this.TargetParentStyleChanged;

    public void UpdateSelection(ushort x, ushort y, uint character)
    {
        if (this.Parent?.State.Style.TextSelection == false)
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
