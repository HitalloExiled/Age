using Age.Numerics;
using Age.Rendering.Commands;
using Age.Rendering.Drawing;
using SkiaSharp;

namespace Age.Rendering.Services;

public partial class TextService(RenderingService renderingService) : IDisposable
{
    private readonly Dictionary<char, Glyph> glyphs           = [];
    private readonly RenderingService        renderingService = renderingService;
    private readonly Sampler                 sampler          = renderingService.CreateSampler();

    private bool disposed;

    private Glyph DrawGlyph(char character, SKPaint paint)
    {
        if (!this.glyphs.TryGetValue(character, out var glyph))
        {
            var charString = character.ToString();

            var bounds = new SKRect();

            paint.MeasureText(character.ToString(), ref bounds);

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

            var texture = this.renderingService.Create2DTexture(image, this.sampler);

            this.glyphs[character] = glyph = new()
            {
                Character = character,
                Bounds    = bounds,
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

                this.renderingService.FreeSampler(this.sampler);
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            this.disposed = true;
        }
    }

    public void DrawText(Element element, string text)
    {
        var style    = element.Style;
        var typeface = SKTypeface.FromFamilyName("Comic Sans", SKFontStyleWeight.Normal, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);

        var paint = new SKPaint
        {
            Color       = SKColors.Red,
            IsAntialias = true,
            TextAlign   = SKTextAlign.Left,
            TextSize    = style.FontSize,
            Typeface    = typeface,
        };

        paint.GetFontMetrics(out var fontMetrics);

        var cursor     = (float)style.Position.X;
        var lineHeight = style.Position.Y + fontMetrics.Ascent + fontMetrics.Descent;

        element.Commands.Clear();

        for (var i = 0; i < text.Length; i++)
        {
            var character = text[i];

            if (!char.IsWhiteSpace(character))
            {
                var glyph    = this.DrawGlyph(character, paint);
                var size     = new Size<float>(glyph.Bounds.Width, glyph.Bounds.Height);
                var position = new Point<float>(cursor + glyph.Bounds.Left, lineHeight - glyph.Bounds.Top);
                var command  = new RectDrawCommand(new(size, position), glyph.Texture);

                element.Commands.Add(command);

                cursor = position.X + glyph.Bounds.Right;
            }
            else
            {
                cursor += paint.MeasureText(character.ToString());
            }
        }

        var bounds = new SKRect();

        paint.MeasureText(text, ref bounds);

        using var bitmap = new SKBitmap((int)bounds.Width, (int)bounds.Height);
        using var canvas = new SKCanvas(bitmap);

        canvas.DrawText(text, -bounds.Location.X, -bounds.Location.Y, paint);

        var skimage = SKImage.FromBitmap(bitmap);

        var data = skimage.Encode(SKEncodedImageFormat.Png, 100);

        using var stream = File.OpenWrite(Path.Join(Directory.GetCurrentDirectory(), $"{text}.png"));

        data.SaveTo(stream);
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }
}
