using Age.Numerics;
using Age.Rendering.Resources;

namespace Age.Rendering;

public class TextureAtlas
{
    private Point<uint> cursor;
    private uint        maxHeight;

    public Size<uint> Size => this.Bitmap.Size;

    public Bitmap  Bitmap  { get; }
    public Texture Texture { get; }

    public bool IsDirty { get; set; }

    public TextureAtlas(Size<uint> size, Texture texture)
    {
        this.Bitmap  = new(size);
        this.Texture = texture;
    }

    public Point<uint> Add(Span<uint> pixels, Size<uint> size)
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
                var sourceIndex      = sourceCursor.X + (size.Width * sourceCursor.Y);
                var destinationIndex = sourceCursor.X + this.cursor.X + (this.Size.Width * (sourceCursor.Y + this.cursor.Y));

                this.Bitmap.Pixels[destinationIndex] = pixels[(int)sourceIndex];

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
}
