using Age.Numerics;
using Age.Rendering.Commands;
using SkiaSharp;

namespace Age.Rendering.Services;

public class TextService(RenderingService renderingService) : IDisposable
{
    private readonly RenderingService               renderingService = renderingService;
    private readonly Sampler                        sampler          = renderingService.CreateSampler();
    private readonly Dictionary<string, Texture>    textures         = [];

    private bool disposed;

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

    public void DrawText(string text, int fontSize, Point<int> position)
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

        var canvasItem = new CanvasItem();

        canvasItem.AddCommand(new RectDrawCommand { Rect = rect, Texture = texture });

        this.renderingService.AddCanvasItem(canvasItem);
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }
}
