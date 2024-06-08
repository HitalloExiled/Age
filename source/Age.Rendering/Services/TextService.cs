// #define DUMP_IMAGES
using Age.Core;
using Age.Core.Extensions;
using Age.Numerics;
using Age.Rendering.Commands;
using Age.Rendering.Drawing.Elements;
using Age.Rendering.Interfaces;
using Age.Rendering.Resources;
using Age.Rendering.Vulkan;
using SkiaSharp;
using static Age.Rendering.Shaders.Canvas.CanvasShader;

namespace Age.Rendering.Services;

internal partial class TextService(VulkanRenderer renderer, ITextureStorage textureStorage) : ITextService
{
    private readonly Dictionary<int, TextureAtlas> atlases = [];
    private readonly Dictionary<int, Glyph> glyphs = [];
    private readonly Sampler sampler = renderer.CreateSampler();
    private readonly ObjectPool<RectDrawCommand> rectDrawCommandPool = new(static () => new RectDrawCommand());

    private bool disposed;

#if DUMP_IMAGES
    private static void SaveToFile(string fontFamily, TextureAtlas atlas)
    {
        var pixels = atlas.GetPixels().AsSpan().Cast<uint, SKColor>().ToArray();

        var bitmap = new SKBitmap(
            (int)atlas.Bitmap.Size.Width,
            (int)atlas.Bitmap.Size.Height
        )
        {
            Pixels = pixels
        };

        var skimage = SKImage.FromBitmap(bitmap);

        using var stream = File.OpenWrite(Path.Join(Directory.GetCurrentDirectory(), $"Atlas-{fontFamily}-{atlas.Size.Width}x{atlas.Size.Height}.png"));

        skimage.Encode(SKEncodedImageFormat.Png, 100).SaveTo(stream);
    }
#endif

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

            var texture = textureStorage.CreateTexture(size, ColorMode.Grayscale, Enums.TextureType.N2D);

            this.atlases[hashcode] = atlas = new(size, ColorMode.Grayscale, texture);
        }

        return atlas;
    }

    private void ReleaseCommands(List<DrawCommand> commands)
    {
        foreach (var command in commands)
        {
            this.rectDrawCommandPool.Return((RectDrawCommand)command);
        }

        commands.Clear();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                foreach (var atlas in this.atlases.Values)
                {
                    textureStorage.FreeTexture(atlas.Texture);
                }

                renderer.DeferredDispose(this.sampler);
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            this.disposed = true;
        }
    }

    public void DrawText(TextNode textNode, string text)
    {
        if (textNode.ParentElement == null)
        {
            return;
        }

        var style      = textNode.ParentElement.Style;
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
        var offset     = new Point<int>(0, baseLine);
        var maxSize    = new Size<uint>(0, lineHeight);

        this.ReleaseCommands(commands);

        for (var i = 0; i < text.Length; i++)
        {
            var character = text[i];

            if (!char.IsWhiteSpace(character))
            {
                ref readonly var bounds = ref glyphsBounds[i];

                var glyph    = this.DrawGlyph(atlas, character, typeface.FamilyName, fontSize, bounds, paint);
                var size     = new Size<float>(bounds.Width, bounds.Height);
                var position = new Point<float>(float.Round(offset.X + bounds.Left), float.Round(offset.Y - bounds.Top));
                var color    = style.Color ?? new();

                var atlasSize = new Point<float>(atlas.Size.Width, atlas.Size.Height);

                var uv = new UVRect
                {
                    P1 = new Point<float>(glyph.Position.X, glyph.Position.Y) / atlasSize,
                    P2 = new Point<float>(glyph.Position.X + glyph.Size.Width, glyph.Position.Y) / atlasSize,
                    P3 = new Point<float>(glyph.Position.X + glyph.Size.Width, glyph.Position.Y + glyph.Size.Height) / atlasSize,
                    P4 = new Point<float>(glyph.Position.X, glyph.Position.Y + glyph.Size.Height) / atlasSize,
                };

                var command = this.rectDrawCommandPool.Get();

                command.ObjectId       = (uint)(textNode.ObjectId | (i + 1u) << 16);
                command.Rect           = new(size, position);
                command.Color          = color;
                command.Flags          = Flags.GrayscaleTexture | Flags.MultiplyColor;
                command.SampledTexture = new(atlas.Texture, this.sampler, uv);

                textNode.Commands.Add(command);

                maxSize.Width = uint.Max(maxSize.Width, (uint)float.Round(position.X + bounds.Right));
                offset.X += (int)float.Round(glyphsWidths[i]);

            }
            else if (character == '\n')
            {
                offset.X = 0;
                offset.Y -= (int)(lineHeight + -metrics.Leading);

                maxSize.Height += lineHeight + (uint)metrics.Leading;
            }
            else
            {
                offset.X += (int)float.Round(glyphsWidths[i]);
            }
        }

        textNode.Baseline = 1 - -offset.Y / (float)maxSize.Height;
        textNode.LineHeight = lineHeight;
        textNode.Size = maxSize;

        if (atlas.IsDirty)
        {
            atlas.Texture.Update(atlas.Bitmap.Buffer);

            atlas.IsDirty = false;
#if DUMP_IMAGES
            SaveToFile(fontFamily, atlas);
#endif
        }
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }
}
