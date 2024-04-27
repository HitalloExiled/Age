using System.Runtime.CompilerServices;
using Age.Core.Extensions;
using Age.Numerics;
using Age.Rendering.Resources;

namespace Age.Rendering;

public class TextureAtlas(Size<uint> size, ColorMode colorMode, Texture texture)
{
    private Point<uint> cursor;
    private uint        maxHeight;

    public Size<uint> Size => this.Bitmap.Size;

    public Bitmap  Bitmap  { get; } = new(size, colorMode);
    public Texture Texture { get; } = texture;

    public bool IsDirty { get; set; }

    public Point<uint> Pack(Span<uint> pixels, Size<uint> size)
    {
        var sourceCursor = new Point<uint>();

        this.maxHeight = Math.Max(this.maxHeight, size.Height);

        if (this.cursor.X + size.Width > this.Size.Width)
        {
            if (this.cursor.Y + size.Height > this.Size.Height)
            {
                throw new InvalidOperationException($"Bitmap with dimensions {size.Width}x{size.Height} excceeded capacity of atlas");
            }

            this.cursor.X = 0;
            this.cursor.Y += this.maxHeight;
        }

        while (sourceCursor.Y < size.Height)
        {
            while (sourceCursor.X < size.Width)
            {
                var sourceIndex      = (int)(sourceCursor.X + size.Width * sourceCursor.Y);
                var destinationIndex = (int)(sourceCursor.X + this.cursor.X + this.Size.Width * (sourceCursor.Y + this.cursor.Y)) * this.Bitmap.BytesPerPixel;

                var span = this.Bitmap.Buffer.AsSpan(destinationIndex).Cast<byte, uint>();

                span[0] = this.Bitmap.ColorMode == ColorMode.Grayscale ? pixels[sourceIndex] >> 16 : pixels[sourceIndex];

                sourceCursor.X++;
            }

            sourceCursor.X = 0;
            sourceCursor.Y++;
        }

        var position = this.cursor;

        this.cursor.X += size.Width;

        this.IsDirty = true;

        return position;
    }

    public uint[] GetPixels()
    {
        var buffer = new uint[this.Size.Width * this.Size.Height];

        for (var i = 0; i < buffer.Length; i++)
        {
            var value = Unsafe.As<byte, uint>(ref this.Bitmap.Buffer[i * this.Bitmap.BytesPerPixel]);

            buffer[i] = this.Bitmap.ColorMode == ColorMode.Grayscale ? value << 16 : value;
        }

        return buffer;
    }
}
