using System.Runtime.CompilerServices;
using Age.Core;
using Age.Core.Extensions;
using Age.Internal;
using Age.Numerics;
using Age.Rendering.Resources;
using Age.Rendering.Vulkan;
using ThirdParty.Vulkan.Enums;

namespace Age;

public sealed class TextureAtlas : Disposable
{
    private Point<uint> cursor;
    private bool        isDirty;
    private uint        maxHeight;

    public Bitmap  Bitmap  { get; }
    public Texture Texture { get; }

    public Size<uint> Size => this.Bitmap.Size;

    public TextureAtlas(Size<uint> size, ColorMode colorMode)
    {
        var textureCreateInfo = new TextureCreateInfo
        {
            Format     = colorMode == ColorMode.Grayscale ? VkFormat.R8G8Unorm : VkFormat.B8G8R8A8Unorm,
            ImageType  = VkImageType.N2D,
            Width      = size.Width,
            Height     = size.Height,
            Depth      = 1,
        };

        this.Bitmap  = new(size, colorMode);
        this.Texture = new(textureCreateInfo);
    }

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

        // TODO - implements Buffer.BlockCopy

        var bitmapSpan = this.Bitmap.AsSpan();

        while (sourceCursor.Y < size.Height)
        {
            if (this.Bitmap.ColorMode == ColorMode.Grayscale)
            {
                while (sourceCursor.X < size.Width)
                {
                    var sourceIndex      = (int)(sourceCursor.X + size.Width * sourceCursor.Y);
                    var destinationIndex = (int)(sourceCursor.X + this.cursor.X + this.Size.Width * (sourceCursor.Y + this.cursor.Y)) * (int)ColorMode.Grayscale;

                    bitmapSpan[destinationIndex..].Cast<byte, uint>()[0] = pixels[sourceIndex] >> 16;

                    sourceCursor.X++;
                }
            }
            else
            {
                // TODO: Need tests
                var sourceIndex      = (int)(sourceCursor.X + size.Width * sourceCursor.Y);
                var destinationIndex = (int)(sourceCursor.X + this.cursor.X + this.Size.Width * (sourceCursor.Y + this.cursor.Y)) * (int)ColorMode.RGBA;

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

        if (this.Bitmap.ColorMode == ColorMode.Grayscale)
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

    protected override void Disposed(bool disposing)
    {
        if (disposing)
        {
            this.Texture.Dispose();
            this.Bitmap.Dispose();
        }
    }
}
