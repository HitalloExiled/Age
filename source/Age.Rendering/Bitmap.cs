using Age.Numerics;

namespace Age.Rendering;

public class Bitmap
{
    public byte[]     Buffer    { get; }
    public ColorMode  ColorMode { get; }
    public Size<uint> Size      { get; }

    public ushort BytesPerPixel => (ushort)this.ColorMode;

    public Bitmap(Size<uint> size, ColorMode colorMode, byte[]? buffer = null)
    {
        var bufferSize = (int)colorMode * size.Height * size.Width;

        if (buffer != null && bufferSize != buffer.Length)
        {
            throw new ArgumentException($"{nameof(buffer)} must have the length equal to ${bufferSize}");
        }

        this.Buffer    = buffer ?? new byte[bufferSize];
        this.ColorMode = colorMode;
        this.Size      = size;
    }
}
