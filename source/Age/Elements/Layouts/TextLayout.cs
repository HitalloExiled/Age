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

using static Age.Rendering.Shaders.Canvas.CanvasShader;
using Age.Styling;

namespace Age.Elements.Layouts;

internal sealed partial class TextLayout : Layout
{
    private static readonly ObjectPool<RectCommand> rectCommandPool = new(static () => new());

    #region 8-bytes
    private readonly TextNode target;
    private readonly Timer timer;

    private string? text;
    #endregion

    #region 4-bytes
    private readonly int caretWidth = 2;
    private int caretPosition = -1;
    private Range? selection;
    #endregion

    #region 1-byte
    private bool caretIsDirty;
    private bool caretIsVisible;
    private bool selectionIsDirty;
    private bool textIsDirty;
    #endregion

    public int CaretPosition
    {
        get => this.caretPosition;
        private set
        {
            if (this.caretPosition != value)
            {
                this.caretPosition = value;
                this.caretIsDirty = true;

                this.RequestUpdate();
            }
        }
    }

    public Range? Selection
    {
        get => this.selection;
        private set
        {
            if (this.selection != value)
            {
                this.selection        = value;
                this.selectionIsDirty = true;
                this.RequestUpdate();
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

    public string? Text
    {
        get => this.text;
        internal set
        {
            if (value != this.text)
            {
                this.text = value;

                this.Selection   = this.Selection?.WithEnd(uint.Min(this.Selection.Value.End, (uint)(this.text?.Length ?? 0)));
                this.textIsDirty = true;

                this.RequestUpdate();
            }
        }
    }

    public override BoxLayout? Parent => this.target.ParentElement?.Layout;

    public string? SelectedText => this.text != null && this.selection?.Ordered() is Range range
        ? this.text.Substring((int)range.Start, (int)(range.End - range.Start))
        : null;

    public override TextNode Target => this.target;

    public TextLayout(TextNode target)
    {
        this.target = target;
        this.timer  = new()
        {
            WaitTime = TimeSpan.FromMilliseconds(500),
        };

        this.timer.Timeout += this.BlinkCaret;

        target.AppendChild(this.timer);
    }

    private static void ReleaseCommands(List<Command> commands, int length)
    {
        if (length < commands.Count)
        {
            foreach (var command in commands.AsSpan())
            {
                rectCommandPool.Return((RectCommand)command);
            }

            commands.RemoveRange(length, commands.Count - length);
        }
        else
        {
            for (var i = 0; i < length; i++)
            {
                if (i < commands.Count)
                {
                    rectCommandPool.Return((RectCommand)commands[i]);
                    commands[i] = default!;
                }
                else
                {
                    commands.Add(default!);
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ApplySelection(int index, in Range range, RectCommand selectionCommand)
    {
        if (index >= range.Start && index < range.End && (this.text![index] != '\n' || index < range.End - 1))
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

        if (this.text != null && this.caretPosition > -1)
        {
            var caretCommand = (RectCommand)this.target.Commands[this.text.Length];

            caretCommand.PipelineVariant = this.caretIsVisible ? PipelineVariant.Color : default;

            this.RequestUpdate();
        }
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

    private void DrawCaret(int textLength)
    {
        var caretCommand = (RectCommand)this.target.Commands[textLength];

        if (this.caretPosition > -1)
        {
            Point<float> position;

            if (this.CaretPosition == textLength)
            {
                var rect = ((RectCommand)this.target.Commands[textLength - 1]).Rect;

                position = rect.Position;
                position.X += rect.Size.Width;
            }
            else
            {
                position = ((RectCommand)this.target.Commands[this.caretPosition]).Rect.Position;
            }

            caretCommand.Rect            = new(new(this.caretWidth, this.LineHeight), position);
            caretCommand.Flags           = Flags.ColorAsBackground;
            caretCommand.PipelineVariant = PipelineVariant.Color;
            caretCommand.Color           = Color.White;
        }
        else
        {
            caretCommand.Rect            = default;
            caretCommand.Flags           = default;
            caretCommand.PipelineVariant = default;
            caretCommand.Color           = default;
        }

        this.caretIsVisible = true;
    }

    private void DrawSelection(int textLength)
    {
        if (this.selection.HasValue)
        {
            var range = this.selection.Value.Ordered();

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
        if (this.target.ParentElement == null)
        {
            return;
        }

        var style      = this.target.ParentElement.Layout.State.Style;
        var fontFamily = string.Intern(style.FontFamily ?? "Segoi UI");
        var fontSize   = style.FontSize ?? 16;
        var fontWeight = style.FontWeight ?? FontWeight.Normal;

        var typeface = SKTypeface.FromFamilyName(fontFamily, (SKFontStyleWeight)(int)fontWeight, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);

        var paint = new SKPaint
        {
            Color        = SKColors.Black,
            IsAntialias  = true,
            TextAlign    = SKTextAlign.Left,
            TextSize     = fontSize,
            Typeface     = typeface,
            SubpixelText = false,
        };

        var atlas  = TextStorage.Singleton.GetAtlas(typeface.FamilyName, fontSize);
        var glyphs = typeface.GetGlyphs(this.text);
        var font   = paint.ToFont();

        font.GetFontMetrics(out var metrics);

        Span<SKRect> glyphsBounds = stackalloc SKRect[glyphs.Length];
        Span<float>  glyphsWidths = stackalloc float[glyphs.Length];

        font.GetGlyphWidths(glyphs, glyphsWidths, glyphsBounds, paint);

        var lineHeight = (uint)float.Round(-metrics.Ascent + metrics.Descent);
        var baseLine   = (int)float.Round(metrics.Ascent);
        var cursor     = new Point<int>(0, baseLine);
        var boundings  = new Size<uint>(0, lineHeight);

        ReleaseCommands(this.target.Commands, text.Length + 1);

        var elementIndex = this.target.Index + 1;

        var range = this.selection?.Ordered();

        var caretCommand = rectCommandPool.Get();

        caretCommand.MappedTexture   = MappedTexture.Default;
        caretCommand.StencilLayer    = this.StencilLayer;
        caretCommand.Color           = default;
        caretCommand.Flags           = default;
        caretCommand.Metadata        = default;
        caretCommand.PipelineVariant = default;
        caretCommand.Rect            = default;

        this.target.Commands[text.Length] = caretCommand;

        for (var i = 0; i < text.Length; i++)
        {
            var character = text[i];

            var selectionCommand = rectCommandPool.Get();

            selectionCommand.MappedTexture = MappedTexture.Default;
            selectionCommand.ObjectId      = (uint)(((i + 1) << 12) | elementIndex);
            selectionCommand.Rect          = new(new(glyphsWidths[i], lineHeight), new(cursor.X, cursor.Y - baseLine));
            selectionCommand.StencilLayer  = this.StencilLayer;
            selectionCommand.Metadata      = default;

            this.target.Commands[i] = selectionCommand;

            if (!char.IsWhiteSpace(character))
            {
                ref readonly var bounds = ref glyphsBounds[i];

                var glyph    = TextStorage.Singleton.DrawGlyph(atlas, character, typeface.FamilyName, fontSize, bounds, paint);
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

                var characterCommand = rectCommandPool.Get();

                characterCommand.Rect            = new(size, position);
                characterCommand.Color           = color;
                characterCommand.Flags           = Flags.GrayscaleTexture | Flags.MultiplyColor;
                characterCommand.PipelineVariant = PipelineVariant.Color;
                characterCommand.MappedTexture   = new(atlas.Texture, uv);
                characterCommand.StencilLayer    = this.StencilLayer;

                selectionCommand.Metadata = (uint)this.target.Commands.Count;

                this.target.Commands.Add(characterCommand);

                boundings.Width = uint.Max(boundings.Width, (uint)float.Round(cursor.X + glyphsWidths[i]));
                cursor.X += (int)float.Round(glyphsWidths[i]);

            }
            else if (character == '\n')
            {
                cursor.X = 0;
                cursor.Y -= (int)(lineHeight + -metrics.Leading);

                boundings.Height += lineHeight + (uint)metrics.Leading;
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

        this.BaseLine   = -baseLine;
        this.LineHeight = lineHeight;
        this.Size       = boundings;
    }

    private void GetCharacterOffset(ushort x, ushort y, ref uint character)
    {
        var command = (RectCommand)this.Target.Commands[(int)character];

        var localPosition = this.Target.Transform.Matrix.Inverse() * new Vector2<float>(x, -y);

        if (localPosition.X > command.Rect.Position.X + command.Rect.Size.Width / 2)
        {
            character++;
        }
    }

    protected override void Disposed()
    { }

    public void ClearSelection()
    {
        if (this.Parent?.State.Style.TextSelection == false)
        {
            return;
        }

        this.ClearCaret();

        this.Selection = default;
    }

    public void PropagateSelection(uint characterPosition)
    {
        if (this.Parent?.State.Style.TextSelection == false || this.text == null || char.IsWhiteSpace(this.text[(int)characterPosition]))
        {
            return;
        }

        var start = (int)characterPosition - 1;
        var end   = (int)characterPosition;

        while (start > -1 && !char.IsWhiteSpace(this.text[start]))
        {
            start--;
        }

        start++;

        while (end < this.text.Length && !char.IsWhiteSpace(this.text[end]))
        {
            end++;
        }

        this.CaretPosition = end;

        this.Selection = new((uint)start, (uint)end);
    }

    public void SetCaret(ushort x, ushort y, uint position)
    {
        if (this.Parent?.State.Style.TextSelection == false)
        {
            return;
        }

        this.GetCharacterOffset(x, y, ref position);

        this.CaretPosition = (int)position;

        this.timer.Start();
    }

    public void ClearCaret()
    {
        if (this.Parent?.State.Style.TextSelection == false)
        {
            return;
        }

        this.timer.Stop();
        this.CaretPosition  = -1;
        this.caretIsVisible = false;
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

    public void UpdateSelection(ushort x, ushort y, uint character)
    {
        if (this.Parent?.State.Style.TextSelection == false)
        {
            return;
        }

        this.GetCharacterOffset(x, y, ref character);

        this.Selection     = this.Selection?.WithEnd(character) ?? new((uint)this.caretPosition, character);
        this.CaretPosition = (int)character;
    }

    public override void Update()
    {
        if (this.HasPendingUpdate)
        {
            if (string.IsNullOrEmpty(this.text))
            {
                this.target.Commands.Clear();

                this.BaseLine      = -1;
                this.CaretPosition = 0;
                this.LineHeight    = 0;
                this.selection     = default;
                this.Size          = default;
            }
            else
            {
                if (this.textIsDirty)
                {
                    this.DrawText(this.text);
                }
                else if (this.selectionIsDirty)
                {
                    this.DrawSelection(this.text.Length);
                }

                if (this.caretIsDirty)
                {
                    this.DrawCaret(this.text.Length);
                }
            }

            this.caretIsDirty     = false;
            this.selectionIsDirty = false;
            this.textIsDirty      = false;
            this.HasPendingUpdate = false;
        }
    }
}
