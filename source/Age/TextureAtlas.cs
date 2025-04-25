using System.Runtime.CompilerServices;
using Age.Core;
using Age.Core.Extensions;
using Age.Extensions;
using Age.Internal;
using Age.Numerics;
using Age.Resources;

namespace Age;

public sealed class TextureAtlas(Size<uint> size, TextureFormat format) : Disposable
{
    private Point<uint> cursor;
    private bool        isDirty;
    private uint        maxHeight;

    public Bitmap    Bitmap  { get; } = new(size, format.BytesPerPixel());
    public Texture2D Texture { get; } = new(size, format);

    public Size<uint> Size => this.Bitmap.Size;

    public Point<uint> Pack(scoped ReadOnlySpan<uint> pixels, Size<uint> size)
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

        var bitmapSpan = this.Bitmap.AsSpan();

        while (sourceCursor.Y < size.Height)
        {
            if (this.Texture.Format == TextureFormat.R8G8Unorm)
            {
                while (sourceCursor.X < size.Width)
                {
                    var sourceIndex      = (int)(sourceCursor.X + size.Width * sourceCursor.Y);
                    var destinationIndex = (int)(sourceCursor.X + this.cursor.X + this.Size.Width * (sourceCursor.Y + this.cursor.Y)) * this.Bitmap.BytesPerPixel;

                    bitmapSpan[destinationIndex..].Cast<byte, uint>()[0] = pixels[sourceIndex] >> 16;

                    sourceCursor.X++;
                }
            }
            else
            {
                // TODO: Need tests
                var sourceIndex      = (int)(sourceCursor.X + size.Width * sourceCursor.Y);
                var destinationIndex = (int)(sourceCursor.X + this.cursor.X + this.Size.Width * (sourceCursor.Y + this.cursor.Y)) * this.Bitmap.BytesPerPixel;

                pixels[sourceIndex..(int)(sourceIndex + size.Width)].CopyTo(bitmapSpan[destinationIndex..].Cast<byte, uint>());
            }

            sourceCursor.X = 0;
            sourceCursor.Y++;
        }

        var position = this.cursor;

        this.cursor.X += size.Width;

        this.isDirty = true;

        return position;
    }

    public uint[] GetPixels()
    {
        var buffer     = new uint[this.Size.Width * this.Size.Height];
        var bitmapSpan = this.Bitmap.AsSpan();

        if (this.Texture.Format == TextureFormat.R8G8Unorm)
        {
            for (var i = 0; i < buffer.Length; i++)
            {
                var value = Unsafe.As<byte, uint>(ref bitmapSpan[i * this.Bitmap.BytesPerPixel]);

                buffer[i] = value << 16;
            }
        }
        else
        {
            bitmapSpan.Cast<byte, uint>().CopyTo(buffer);
        }

        return buffer;
    }

    public void Update()
    {
        if (this.isDirty)
        {
            this.Texture.Update(this.Bitmap);

            #if DEBUG
            Common.SaveImage(this.Bitmap, $"TextureAtlas_{this.Size.Width}x{this.Size.Height}.png");
            #endif

            this.isDirty = false;
        }
    }

    protected override void OnDisposed(bool disposing)
    {
        if (disposing)
        {
            this.Texture.Dispose();
            this.Bitmap.Dispose();
        }
    }
}
