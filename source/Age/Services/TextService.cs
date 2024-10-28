using Age.Commands;
using Age.Core.Extensions;
using Age.Core;
using Age.Elements;
using Age.Numerics;
using Age.Rendering.Vulkan;
using Age.Resources;
using SkiaSharp;

using static Age.Rendering.Shaders.Canvas.CanvasShader;

namespace Age.Services;


internal partial class TextService : IDisposable
{
    public struct TextDrawInfo
    {
        public Size<uint> Boundings;
        public uint       LineHeight;
        public int        Start;
        public int        End;
    }

    private static TextService? singleton;

    public static TextService Singleton => singleton ?? throw new NullReferenceException();

    private readonly Dictionary<int, TextureAtlas> atlases = [];
    private readonly Dictionary<int, Glyph>        glyphs = [];
    private readonly VulkanRenderer                renderer;
    private readonly ObjectPool<RectCommand>       rectCommandPool = new(static () => new());

    private bool disposed;

    public TextService(VulkanRenderer renderer)
    {
        if (singleton != null)
        {
            throw new InvalidOperationException($"Only one single instace of {nameof(TextService)} is allowed");
        }

        singleton = this;

        this.renderer = renderer;
    }

    private Glyph DrawGlyph(TextureAtlas atlas, char character, string fontFamily, ushort fontSize, in SKRect bounds, SKPaint paint)
    {
        const ushort PADDING = 2;

        var hashcode = character.GetHashCode() ^ fontFamily.GetHashCode() ^ fontSize.GetHashCode();

        if (!this.glyphs.TryGetValue(hashcode, out var glyph))
        {
            var charString = character.ToString();

            using var bitmap = new SKBitmap(
                (int)bounds.Width + PADDING * 2,
                (int)bounds.Height + PADDING * 2
            );

            using var canvas = new SKCanvas(bitmap);

            canvas.DrawText(charString, PADDING + -bounds.Location.X, PADDING + -bounds.Location.Y, paint);

            var position = atlas.Pack(bitmap.Pixels.AsSpan().Cast<SKColor, uint>(), new((uint)bitmap.Width, (uint)bitmap.Height));

            this.glyphs[hashcode] = glyph = new()
            {
                Atlas     = atlas,
                Character = character,
                Position  = position + PADDING,
                Size      = new((uint)bounds.Width, (uint)bounds.Height),
            };
        }

        return glyph;
    }

    private TextureAtlas GetAtlas(string familyName, uint fontSize)
    {
        var hashcode = familyName.GetHashCode() ^ fontSize.GetHashCode();

        if (!this.atlases.TryGetValue(hashcode, out var atlas))
        {
            var axisSize = uint.Max(fontSize * 8, 256);
            var size     = new Size<uint>(axisSize, axisSize);

            this.atlases[hashcode] = atlas = new(size, ColorMode.Grayscale);
        }

        return atlas;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                foreach (var atlas in this.atlases.Values)
                {
                    this.renderer.DeferredDispose(atlas);
                }
            }

            this.disposed = true;
        }
    }

    public TextDrawInfo DrawText(TextNode textNode, string text)
    {
        if (textNode.ParentElement == null)
        {
            return default;
        }

        var style      = textNode.ParentElement.Layout.State.Style;
        var fontFamily = string.Intern(style.FontFamily ?? "Segoi UI");
        var fontSize   = style.FontSize ?? 16;
        var commands   = textNode.Commands;

        var typeface = SKTypeface.FromFamilyName(fontFamily);

        var paint = new SKPaint
        {
            Color        = SKColors.Black,
            IsAntialias  = true,
            TextAlign    = SKTextAlign.Left,
            TextSize     = fontSize,
            Typeface     = typeface,
            SubpixelText = true,
        };

        var atlas  = this.GetAtlas(typeface.FamilyName, fontSize);
        var glyphs = typeface.GetGlyphs(text);
        var font   = paint.ToFont();

        font.GetFontMetrics(out var metrics);

        Span<SKRect> glyphsBounds = stackalloc SKRect[glyphs.Length];
        Span<float>  glyphsWidths = stackalloc float[glyphs.Length];

        font.GetGlyphWidths(glyphs, glyphsWidths, glyphsBounds, paint);

        var lineHeight = (uint)float.Round(-metrics.Ascent + metrics.Descent);
        var baseLine   = (int)float.Round(metrics.Ascent);
        var cursor     = new Point<int>(0, baseLine);
        var boundings  = new Size<uint>(0, lineHeight);

        this.ReleaseCommands(commands, text.Length);

        var textDrawInfo = new TextDrawInfo
        {
            Start = cursor.Y,
            LineHeight = lineHeight,
        };

        var elementIndex = textNode.Index + 1;

        var selectionStart = 0u;
        var selectionEnd   = 0u;
        var hasSelection   = false;

        if (textNode.Layout.Selection.HasValue)
        {
            hasSelection = true;

            (selectionStart, selectionEnd) = textNode.Layout.Selection.Value.Start < textNode.Layout.Selection.Value.End
                ? (textNode.Layout.Selection.Value.Start, textNode.Layout.Selection.Value.End)
                : (textNode.Layout.Selection.Value.End, textNode.Layout.Selection.Value.Start);
        }

        for (var i = 0; i < text.Length; i++)
        {
            var character = text[i];

            var selectionCommand = this.rectCommandPool.Get();

            selectionCommand.Rect          = new(new(glyphsWidths[i], lineHeight), new(cursor.X, cursor.Y - baseLine));
            selectionCommand.MappedTexture = MappedTexture.Default;
            selectionCommand.StencilLayer  = textNode.Layout.StencilLayer;
            selectionCommand.ObjectId      = (uint)(((i + 1) << 12) | elementIndex);

            if (hasSelection && i >= selectionStart && i < selectionEnd && (character != '\n' || i < selectionEnd - 1))
            {
                selectionCommand.Color   = Color.Blue;
                selectionCommand.Flags   = Flags.ColorAsBackground;
                selectionCommand.Variant = Variant.Index | Variant.Color;
            }
            else
            {
                selectionCommand.Color   = default;
                selectionCommand.Flags   = default;
                selectionCommand.Variant = Variant.Index;
            }

            commands[i] = selectionCommand;

            if (!char.IsWhiteSpace(character))
            {
                ref readonly var bounds = ref glyphsBounds[i];

                var glyph    = this.DrawGlyph(atlas, character, typeface.FamilyName, fontSize, bounds, paint);
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

                var characterCommand = this.rectCommandPool.Get();

                characterCommand.Rect          = new(size, position);
                characterCommand.Color         = color;
                characterCommand.Flags         = Flags.GrayscaleTexture | Flags.MultiplyColor;
                characterCommand.Variant       = Variant.Color;
                characterCommand.MappedTexture = new(atlas.Texture, uv);
                characterCommand.StencilLayer  = textNode.Layout.StencilLayer;

                commands.Add(characterCommand);

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
        }

        atlas.Update();

        textDrawInfo.End = cursor.Y;
        textDrawInfo.Boundings = boundings;

        return textDrawInfo;
    }

    private void ReleaseCommands(List<Command> commands, int length)
    {
        if (length < commands.Count)
        {
            foreach (var command in commands)
            {
                this.rectCommandPool.Return((RectCommand)command);
            }

            commands.RemoveRange(length, commands.Count - length);
        }
        else
        {
            for (var i = 0; i < length; i++)
            {
                if (i < commands.Count)
                {
                    this.rectCommandPool.Return((RectCommand)commands[i]);
                    commands[i] = default!;
                }
                else
                {
                    commands.Add(default!);
                }
            }
        }
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }
}
