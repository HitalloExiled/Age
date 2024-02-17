using Age.Numerics;
using Age.Rendering.Commands;
using Age.Rendering.Drawing;
using Age.Rendering.Enums;
using Age.Rendering.Resources;
using SkiaSharp;

namespace Age.Rendering.Services;

public partial class TextService(RenderingService renderingService) : IDisposable
{
    private readonly Dictionary<int, Glyph> glyphs          = [];
    private readonly RenderingService      renderingService = renderingService;
    private readonly Sampler               sampler          = renderingService.CreateSampler();

    private bool disposed;

#if DUMP_IMAGES
    private static void SaveToFile(string text, SKPaint paint)
    {
        var drawBounds = new SKRect();

        paint.MeasureText(text, ref drawBounds);
        using var bitmap = new SKBitmap((int)drawBounds.Width, (int)drawBounds.Height);
        using var canvas = new SKCanvas(bitmap);

        canvas.DrawText(text, -drawBounds.Location.X, -drawBounds.Location.Y, paint);

        var skimage = SKImage.FromBitmap(bitmap);

        var data = skimage.Encode(SKEncodedImageFormat.Png, 100);

        #pragma warning disable SYSLIB1045
        using var stream = File.OpenWrite(Path.Join(Directory.GetCurrentDirectory(), $"{Regex.Replace(text, "[\n:\\\\]", "_")}.png"));
        #pragma warning restore SYSLIB1045
        data.SaveTo(stream);
    }
#endif

    private Glyph DrawGlyph(char character, ushort fontSize, SKRect bounds, SKPaint paint)
    {
        var hashcode = character.GetHashCode() ^ fontSize.GetHashCode();

        if (!this.glyphs.TryGetValue(hashcode, out var glyph))
        {
            var charString = character.ToString();

            using var bitmap = new SKBitmap((int)bounds.Width, (int)bounds.Height);
            using var canvas = new SKCanvas(bitmap);

            canvas.DrawText(charString, -bounds.Location.X, -bounds.Location.Y, paint);

            var pixels = bitmap.Pixels.Select(x => (uint)x).ToArray();

            var image = new Image
            {
                Width  = (uint)bitmap.Width,
                Height = (uint)bitmap.Height,
                Pixels = pixels,
            };

            var texture = this.renderingService.CreateTexture(image, TextureType.N2D);

            this.glyphs[hashcode] = glyph = new()
            {
                Character = character,
                Texture   = texture,
            };
        }

        return glyph;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                foreach (var glyph in this.glyphs.Values)
                {
                    this.renderingService.FreeTexture(glyph.Texture);
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
        var style = element.Style;
        var typeface = SKTypeface.FromFamilyName("Segoe UI", SKFontStyleWeight.Normal, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);

        var paint = new SKPaint
        {
            Color       = SKColors.Red,
            IsAntialias = true,
            TextAlign   = SKTextAlign.Left,
            TextSize    = style.FontSize,
            Typeface    = typeface,
        };

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
                var bounds   = glyphsBounds[i];
                var glyph    = this.DrawGlyph(character, style.FontSize, glyphsBounds[i], paint);
                var size     = new Size<float>(bounds.Width, bounds.Height);
                var position = new Point<float>(offset.X + glyphsPosition[i].X, offset.Y - glyphsBounds[i].Top);
                var command  = new RectDrawCommand(new(size, position), glyph.Texture, this.sampler);

                element.Commands.Add(command);
            }
            else if (character == '\n' && i < text.Length - 1)
            {
                offset.X  = style.Position.X + -glyphsPosition[i + 1].X;
                offset.Y += lineHeight + -4;
            }
        }

        this.renderingService.RequestDraw();

#if DUMP_IMAGES
        SaveToFile(text, paint);
#endif
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }
}
