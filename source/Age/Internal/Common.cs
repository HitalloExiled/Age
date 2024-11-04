
using Age.Core.Extensions;
using Age.Rendering.Resources;
using SkiaSharp;
using ThirdParty.Vulkan.Flags;

namespace Age.Internal;

public static class Common
{
    private static readonly string debugDirectory = Path.Join(Directory.GetCurrentDirectory(), ".debug");

    public static void SaveImage(Image image, VkImageAspectFlags aspectMask, string filename)
    {
        var data = image.ReadBuffer(aspectMask);

        static SKColor convert(uint value) => new(value);

        var pixels = data.Select(convert).ToArray();

        var bitmap = new SKBitmap((int)image.Extent.Width, (int)image.Extent.Height)
        {
            Pixels = pixels
        };

        SaveImage(bitmap, filename);
    }

    public static void SaveImage(SKBitmap bitmap, string filename)
    {
        using var image = SKImage.FromBitmap(bitmap);

        try
        {
            Directory.CreateDirectory(debugDirectory);

            using var stream = File.OpenWrite(Path.IsPathFullyQualified(filename) ? filename : Path.Join(debugDirectory, filename));

            image.Encode(SKEncodedImageFormat.Png, 100).SaveTo(stream);
        }
        catch
        {

        }
    }

    public static void SaveImage(Bitmap bitmap, string filename)
    {
        using var skBitmap = new SKBitmap((int)bitmap.Size.Width, (int)bitmap.Size.Height);

        if (bitmap.ColorMode == ColorMode.Grayscale)
        {
            var pixels = new SKColor[skBitmap.Pixels.Length];

            var span = bitmap.Buffer.AsSpan().Cast<byte, short>();

            for (var i = 0; i < pixels.Length; i++)
            {
                pixels[i] = (uint)(span[i] << 16);
            }

            skBitmap.Pixels = pixels;
        }
        else
        {
            skBitmap.Pixels = bitmap.Buffer.AsSpan().Cast<byte, SKColor>().ToArray();
        }

        SaveImage(skBitmap, filename);
    }
}
