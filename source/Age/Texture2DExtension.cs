using Age.Numerics;
using Age.Rendering.Resources;
using Age.Storage;
using SkiaSharp;

namespace Age;

public static class Texture2DExtension
{
    private static readonly Texture2D @default = CreateAndStore("Default", Color.Margenta);
    private static readonly Texture2D empty    = CreateAndStore("Empty", default);

    private static Texture2D CreateAndStore(string name, in Color color)
    {
        var texture = new Texture2D(new Texture2D.CreateInfo { Size = new(1) }, color);

        TextureStorage.Singleton.Add(name, texture);

        return texture;
    }

    extension(Texture2D)
    {
        public static Texture2D Default => @default;
        public static Texture2D Empty   => empty;

        public static Texture2D Load(string path)
        {
            using var stream = File.OpenRead(path);

            var bitmap = SKBitmap.Decode(stream);

            var buffer = bitmap.GetPixelSpan();

            return new(new Texture2D.CreateInfo { Size = new((uint)bitmap.Width, (uint)bitmap.Height) }, buffer);
        }
    }
}
