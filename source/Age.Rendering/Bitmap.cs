using Age.Numerics;

namespace Age.Rendering;

public class Bitmap
{
    public uint[]     Pixels { get; }
    public Size<uint> Size   { get; }

    public Bitmap(Size<uint> size, uint[]? pixels = null)
    {
        var pixelLength = size.Height * size.Width;

        if (pixels != null && pixelLength != pixels.Length)
        {
            throw new ArgumentException($"{nameof(pixels)} must have the length equal to ${pixelLength}");
        }

        this.Pixels = pixels ?? new uint[size.Width * size.Height];
        this.Size   = size;
    }
}
