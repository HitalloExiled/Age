#define DUMP_IMAGES
using Age.Core.Extensions;
using Age.Numerics;
using Age.Rendering.Commands;
using Age.Rendering.Drawing.Elements;
using Age.Rendering.Resources;
using Age.Rendering.Storage;
using SkiaSharp;

namespace Age.Rendering.Services;

internal partial class TextService(RenderingService renderingService, TextureStorage textureStorage) : IDisposable
{
    private readonly Dictionary<int, TextureAtlas> atlases          = [];
    private readonly Dictionary<int, Glyph>        glyphs           = [];
    private readonly RenderingService              renderingService = renderingService;
    private readonly Sampler                       sampler          = renderingService.CreateSampler();

    private bool disposed;

#if DUMP_IMAGES
    private static void SaveToFile(TextureAtlas atlas)
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

        using var stream = File.OpenWrite(Path.Join(Directory.GetCurrentDirectory(), $"Atlas-{atlas.Size.Width}x{atlas.Size.Height}.png"));

        skimage.Encode(SKEncodedImageFormat.Png, 100).SaveTo(stream);
    }
#endif

    private Glyph DrawGlyph(TextureAtlas atlas, char character, ushort fontSize, in SKRect bounds, SKPaint paint)
    {
        const ushort PADDING = 2;

        var hashcode = character.GetHashCode() ^ fontSize.GetHashCode();

        if (!this.glyphs.TryGetValue(hashcode, out var glyph))
        {
            var charString = character.ToString();

            using var bitmap = new SKBitmap(
                (int)bounds.Width  + PADDING * 2,
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

    private TextureAtlas GetAtlas(string familyName, int fontSize)
    {
        var hashcode = familyName.GetHashCode() ^ fontSize;

        if (!this.atlases.TryGetValue(hashcode, out var atlas))
        {
            var axisSize = (uint)Math.Max(fontSize * 8, 256);
            var size     = new Size<uint>(axisSize, axisSize);

            var texture = textureStorage.CreateTexture(size, ColorMode.GrayScale, Enums.TextureType.N2D);

            this.atlases[hashcode] = atlas = new(size, ColorMode.GrayScale, texture);
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
                    textureStorage.FreeTexture(atlas.Texture);
                }

                this.renderingService.DestroySampler(this.sampler);
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
        var fontFamily = style.Font.Family;
        var fontSize   = style.Font.Size;
        var commands   = textNode.Commands;


        var typeface = SKTypeface.FromFamilyName(fontFamily, SKFontStyleWeight.Normal, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);

        var paint = new SKPaint
        {
            Color       = SKColors.Black,
            IsAntialias = true,
            TextAlign   = SKTextAlign.Left,
            TextSize    = fontSize,
            Typeface    = typeface,
            SubpixelText = true,
        };

        var atlas  = this.GetAtlas(fontFamily, fontSize);
        var glyphs = typeface.GetGlyphs(text);
        var font   = paint.ToFont();

        font.GetFontMetrics(out var metrics);

        var glyphsBounds = new SKRect[glyphs.Length];
        var glyphsWidths = new float[glyphs.Length];

        font.GetGlyphWidths(glyphs, glyphsWidths, glyphsBounds, paint);

        var lineHeight = float.Round(-metrics.Ascent + metrics.Descent);
        var baseLine   = float.Round(metrics.Ascent);
        var offset     = new Point<float>(0, baseLine);
        var maxSize    = new Size<float>(0, lineHeight);

        commands.Clear();

        for (var i = 0; i < text.Length; i++)
        {
            var character = text[i];

            if (!char.IsWhiteSpace(character))
            {
                ref readonly var bounds = ref glyphsBounds[i];

                var glyph    = this.DrawGlyph(atlas, character, fontSize, bounds, paint);
                var size     = new Size<float>(bounds.Width, bounds.Height);
                var position = new Point<float>(float.Round(offset.X + bounds.Left), float.Round(offset.Y - bounds.Top));
                var color    = style.Color == default ? new() : style.Color;

                var atlasSize = new Point<float>(atlas.Size.Width, atlas.Size.Height);

                var uv = new UVRect
                {
                    P1 = new Point<float>(glyph.Position.X, glyph.Position.Y) / atlasSize,
                    P2 = new Point<float>(glyph.Position.X + glyph.Size.Width, glyph.Position.Y) / atlasSize,
                    P3 = new Point<float>(glyph.Position.X + glyph.Size.Width, glyph.Position.Y + glyph.Size.Height) / atlasSize,
                    P4 = new Point<float>(glyph.Position.X, glyph.Position.Y + glyph.Size.Height) / atlasSize,
                };

                var command = new RectDrawCommand
                {
                    Rect           = new(size, position),
                    Color          = color,
                    SampledTexture = new(atlas.Texture, this.sampler, uv),
                };

                textNode.Commands.Add(command);

                maxSize.Width = float.Max(maxSize.Width, float.Round(position.X + bounds.Right));
                offset.X += float.Round(glyphsWidths[i]);

            }
            else if (character == '\n')
            {
                offset.X  = 0;
                offset.Y -= lineHeight + -metrics.Leading;

                maxSize.Height += lineHeight + metrics.Leading;
            }
            else
            {
                offset.X += glyphsWidths[i];
            }
        }

        textNode.BaseLine   = baseLine;
        textNode.LineHeight = lineHeight;
        textNode.Size       = maxSize;

        if (atlas.IsDirty)
        {
            this.renderingService.UpdateTexture(atlas.Texture, atlas.Bitmap.Buffer);

            atlas.IsDirty = false;
#if DUMP_IMAGES
            SaveToFile(atlas);
#endif
        }

        this.renderingService.RequestDraw();
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }
}
