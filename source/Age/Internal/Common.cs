
using Age.Rendering.Resources;
using SkiaSharp;
using ThirdParty.Vulkan.Flags;

namespace Age.Internal;

internal static class Common
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
}
