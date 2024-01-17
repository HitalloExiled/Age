using Age.Numerics;
using Age.Rendering.Commands;
using Age.Rendering.Drawing;
using SkiaSharp;

namespace Age.Rendering.Services;

public class TextService : IDisposable
{
    public static TextService Singleton { get; private set; } = null!;

    private readonly RenderingService            renderingService;
    private readonly Sampler                     sampler;
    private readonly Dictionary<string, Texture> textures         = [];

    private bool disposed;

    public TextService(RenderingService renderingService)
    {
        Singleton = this;

        this.renderingService = renderingService;
        this.sampler = renderingService.CreateSampler();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                foreach (var texture in this.textures.Values)
                {
                    this.renderingService.FreeTexture(texture);
                }

                this.renderingService.FreeSampler(this.sampler);
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            this.disposed = true;
        }
    }

    public Element DrawText(string text, int fontSize, Point<int> position)
    {
        if (!this.textures.TryGetValue(text, out var texture))
        {
            var typeface = SKTypeface.FromFamilyName("Comic Sans", SKFontStyleWeight.Normal, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);

            var paint = new SKPaint
            {
                Color       = SKColors.Black,
                IsAntialias = true,
                TextAlign   = SKTextAlign.Left,
                TextSize    = fontSize,
                Typeface    = typeface,
            };

            var bounds = new SKRect();

            paint.MeasureText(text, ref bounds);

            using var bitmap = new SKBitmap((int)bounds.Width, (int)bounds.Height);
            using var canvas = new SKCanvas(bitmap);

            canvas.DrawText(text, -bounds.Location.X, -bounds.Location.Y, paint);

            var skimage = SKImage.FromBitmap(bitmap);

            var data = skimage.Encode(SKEncodedImageFormat.Png, 100);

            using var stream = File.OpenWrite($"C:\\Users\\rafael.franca\\Projects\\Age\\source\\{text}.png");

            data.SaveTo(stream);

            var pixels = bitmap.Pixels.Select(x => (uint)x).ToArray();

            var image = new Image
            {
                Width  = (uint)bitmap.Width,
                Height = (uint)bitmap.Height,
                Pixels = pixels,
            };

            texture = this.renderingService.Create2DTexture(image, this.sampler);

            this.textures[text] = texture;
        }

        var rect = new Rect<float>(new(texture.Image.Width, texture.Image.Height), position.Cast<float>());

        var element = new Element();

        element.AddCommand(new RectDrawCommand { Rect = rect, Texture = texture });

        return element;
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }
}
