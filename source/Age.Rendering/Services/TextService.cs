#define DUMP_IMAGES
using Age.Core.Extensions;
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

    private Glyph DrawGlyph(TextureAtlas atlas, char character, ushort fontSize, SKRect bounds, SKPaint paint)
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

            var texture = this.renderingService.CreateTexture(size, ColorMode.GrayScale, Enums.TextureType.N2D);

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
        var offset     = new Point<float>(0, lineHeight);

        element.Commands.Clear();

        RectDrawCommand? backgroundDrawCommand = null;

        if (style.Border != null)
        {
            backgroundDrawCommand = new()
            {
                Border  = style.Border,
                Sampler = this.sampler,
                Texture = atlas.Texture,
            };
            element.Commands.Add(backgroundDrawCommand);
        }

        var elementBounds = new Rect<float>(new(), offset).InvertedY();

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

                var atlasSize = new Point<float>(atlas.Size.Width, atlas.Size.Height);

                var uv = new Point<float>[4]
                {
                    new Point<float>(glyph.Position.X, glyph.Position.Y) / atlasSize,
                    new Point<float>(glyph.Position.X + glyph.Size.Width, glyph.Position.Y) / atlasSize,
                    new Point<float>(glyph.Position.X + glyph.Size.Width, glyph.Position.Y + glyph.Size.Height) / atlasSize,
                    new Point<float>(glyph.Position.X, glyph.Position.Y + glyph.Size.Height) / atlasSize,
                };

                var command = new RectDrawCommand
                {
                    Rect     = new(size, position),
                    UV       = uv,
                    Color    = color,
                    Texture  = atlas.Texture,
                    Sampler  = this.sampler
                };

                element.Commands.Add(command);

                elementBounds.Grow(command.Rect.InvertedY());
            }
            else if (character == '\n' && i < text.Length - 1)
            {
                offset.X  = -glyphsPosition[i + 1].X;
                offset.Y += lineHeight + -4;
            }
        }

        element.Transform = new(elementBounds.Size, new(elementBounds.Position.X, elementBounds.Position.Y), 0);

        if (backgroundDrawCommand != null)
        {
            backgroundDrawCommand.Rect = elementBounds.InvertedY();
        }

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
