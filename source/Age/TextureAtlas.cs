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
    #region 8-bytes
    public Bitmap  Bitmap  { get; }
    public Texture Texture { get; }
    #endregion

    #region 4-bytes
    private Point<uint> cursor;
    private uint        maxHeight;
    #endregion

    #region 2-bytes
    private bool isDirty;
    #endregion

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

        this.isDirty = true;

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

    public void Update()
    {
        if (this.isDirty)
        {
            this.Texture.Update(this.Bitmap.Buffer);

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
        }
    }
}
