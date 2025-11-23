
using Age.Core.Extensions;
using Age.Numerics.Converters;
using Age.Rendering.Resources;
using SkiaSharp;
using ThirdParty.Vulkan.Flags;

namespace Age.Internal;

public static class Common
{
    private static readonly string debugDirectory = Path.Join(Directory.GetCurrentDirectory(), ".debug");

    public static void SaveImage(Texture2D texture, VkImageAspectFlags aspectMask, string filename)
    {
        SKColor[] pixels;

        if (texture.Image.BytesPerPixel == 8)
        {
            var data = texture.Image.ReadBuffer64bits(aspectMask);

            static SKColor convert(ulong value) => new(ColorFormatConverter.RGBAtoBGRA(ColorFormatConverter.RGBA64toRGBA32(value)));

            pixels = [.. data.Select(convert)];
        }
        else
        {
            var data = texture.Image.ReadBuffer(aspectMask);

            static SKColor convert(uint value) => new(value);

            pixels = [.. data.Select(convert)];
        }

        var bitmap = new SKBitmap((int)texture.Extent.Width, (int)texture.Extent.Height)
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
        using var skBitmap = new SKBitmap((int)bitmap.Size.Width, (int)bitmap.Size.Height)
        {
            Pixels = bitmap.GetPixels().AsSpan().Cast<uint, SKColor>().ToArray()
        };

        SaveImage(skBitmap, filename);
    }
}
