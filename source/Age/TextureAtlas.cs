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

    public Point<uint> Pack(scoped ReadOnlySpan<uint> pixels, in Size<uint> size)
    {
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

        switch (this.Texture.Format)
        {
            case TextureFormat.R8Unorm:
                copy<byte>(pixels, size);
                break;

            case TextureFormat.R8G8Unorm:
                copy<ushort>(pixels, size);
                break;
            default:
                copy<uint>(pixels, size);
                break;
        }

        var position = this.cursor;

        this.cursor.X += size.Width;

        this.isDirty = true;

        return position;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        unsafe void copy<T>(scoped ReadOnlySpan<uint> pixels, in Size<uint> size) where T : unmanaged
        {
            var atlasWidth = this.Size.Width;
            var bitmapSpan = this.Bitmap.AsSpan();
            var startX     = this.cursor.X;
            var startY     = this.cursor.Y;

            for (var y = 0; y < size.Height; y++)
            {
                var sourceIndex      = (int)(size.Width * y);
                var destinationIndex = (int)(startX + atlasWidth * (y + startY)) * sizeof(T);

                pixels.Slice(sourceIndex, (int)size.Width).CopyTo(bitmapSpan[destinationIndex..].Cast<byte, T>(), true);
            }
        }
    }

    public Point<uint> Pack(scoped ReadOnlySpan<byte> pixels, in Size<uint> size) =>
        this.Pack(pixels.Cast<byte, uint>(), size);

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
