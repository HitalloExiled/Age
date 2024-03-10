#define DUMP_IMAGES
using System.Runtime.InteropServices;
using Age.Numerics;
using Age.Rendering.Commands;
using Age.Rendering.Drawing;
using Age.Rendering.Resources;
using SkiaSharp;

namespace Age.Rendering.Services;

public partial class TextService(RenderingService renderingService) : IDisposable
{
    private readonly Dictionary<int, TextureAtlas> atlases          = [];
    private readonly Dictionary<int, Glyph>        glyphs           = [];
    private readonly RenderingService              renderingService = renderingService;
    private readonly Sampler                       sampler          = renderingService.CreateSampler();

    private bool disposed;

#if DUMP_IMAGES
    private static void SaveToFile(TextureAtlas atlas)
    {
        using var stream = File.OpenWrite(Path.Join(Directory.GetCurrentDirectory(), $"Atlas-{atlas.Size.Width}x{atlas.Size.Height}.png"));

        var pixels = MemoryMarshal.Cast<uint, SKColor>(atlas.Bitmap.Pixels.AsSpan()).ToArray();

        var bitmap = new SKBitmap(
            (int)atlas.Bitmap.Size.Width,
            (int)atlas.Bitmap.Size.Height,
            SKColorType.Rgba8888,
            SKAlphaType.Premul
        )
        {
            Pixels = pixels
        };

        var skimage = SKImage.FromBitmap(bitmap);

        var data = skimage.Encode(SKEncodedImageFormat.Png, 100);

        data.SaveTo(stream);
    }
#endif

    private Glyph DrawGlyph(TextureAtlas atlas, char character, ushort fontSize, SKRect bounds, SKPaint paint)
    {
        const ushort PADDING = 2;

        var hashcode = character.GetHashCode() ^ fontSize.GetHashCode();

        if (!this.glyphs.TryGetValue(hashcode, out var glyph))
        {
            var charString = character.ToString();

            using var bitmap = new SKBitmap(
                (int)bounds.Width  + PADDING * 2,
                (int)bounds.Height + PADDING * 2,
                SKColorType.Rgba8888,
                SKAlphaType.Premul
            );

            using var canvas = new SKCanvas(bitmap);

            canvas.DrawText(charString, PADDING + -bounds.Location.X, PADDING + -bounds.Location.Y, paint);

            var position = atlas.Add(MemoryMarshal.Cast<SKColor, uint>(bitmap.Pixels.AsSpan()), new((uint)bitmap.Width, (uint)bitmap.Height));

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
            var size = (uint)Math.Max(fontSize * 8, 256);

            var texture = this.renderingService.CreateTexture(new(size, size), ColorMode.RGBA, Enums.TextureType.N2D);

            this.atlases[hashcode] = atlas = new(new(size, size), texture);
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
                    this.renderingService.FreeTexture(atlas.Texture);
                }

                this.renderingService.DestroySampler(this.sampler);
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            this.disposed = true;
        }
    }

    public void DrawText(Element element, string text)
    {
        var style    = element.Style;
        var typeface = SKTypeface.FromFamilyName(style.FontFamily, SKFontStyleWeight.Normal, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);

        var paint = new SKPaint
        {
            Color       = SKColors.Black,
            IsAntialias = true,
            TextAlign   = SKTextAlign.Left,
            TextSize    = style.FontSize,
            Typeface    = typeface,
        };

        var atlas = this.GetAtlas(style.FontFamily, style.FontSize);

        var font   = paint.ToFont();
        var glyphs = typeface.GetGlyphs(text);

        var glyphsPosition = new SKPoint[glyphs.Length];
        var glyphsBounds   = new SKRect[glyphs.Length];

        font.GetGlyphPositions(glyphs, glyphsPosition);
        font.GetGlyphWidths(glyphs, null, glyphsBounds, paint);

        var lineHeight = -glyphsBounds.Max(x => x.Height);
        var offset     = new Point<float>(style.Position.X, style.Position.Y + lineHeight);

        element.Commands.Clear();

        for (var i = 0; i < text.Length; i++)
        {
            var character = text[i];

            if (!char.IsWhiteSpace(character))
            {
                var bounds    = glyphsBounds[i];
                var glyph     = this.DrawGlyph(atlas, character, style.FontSize, glyphsBounds[i], paint);
                var size      = new Size<float>(bounds.Width, bounds.Height);
                var position  = new Point<float>(offset.X + glyphsPosition[i].X, offset.Y - glyphsBounds[i].Top);
                var color     = style.Color == default ? new() : style.Color;
                var atlasSize = new Size<float>(atlas.Size.Width, atlas.Size.Height);

                var uv = new Point<float>[4]
                {
                    new Point<float>(glyph.Position.X, glyph.Position.Y) / atlasSize,
                    new Point<float>(glyph.Position.X + glyph.Size.Width, glyph.Position.Y) / atlasSize,
                    new Point<float>(glyph.Position.X + glyph.Size.Width, glyph.Position.Y + glyph.Size.Height) / atlasSize,
                    new Point<float>(glyph.Position.X, glyph.Position.Y + glyph.Size.Height) / atlasSize,
                };

                var command = new RectDrawCommand(new(size, position), atlas.Texture, uv, color, this.sampler);

                element.Commands.Add(command);
            }
            else if (character == '\n' && i < text.Length - 1)
            {
                offset.X  = style.Position.X + -glyphsPosition[i + 1].X;
                offset.Y += lineHeight + -4;
            }
        }

        if (atlas.IsDirty)
        {
            this.renderingService.UpdateTexture(atlas.Texture, atlas.Bitmap.Pixels);

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
