using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Age.Numerics;

[DebuggerDisplay("\\{ Size: {Size}, Position: {Position} \\}")]
public record struct Rect<T> where T : INumber<T>
{
    public Size<T>  Size;
    public Point<T> Position;

    public Rect(Size<T> size, Point<T> position)
    {
        this.Size     = size;
        this.Position = position;
    }

    public Rect(T width, T height, T x, T y) : this(new(width, height), new(x, y))
    { }

    public void Grow(in Rect<T> rect)
    {
        var current = this;

        this.Position.X = T.Min(rect.Position.X, current.Position.X);
        this.Position.Y = T.Min(rect.Position.Y, current.Position.Y);

        this.Size.Width  = T.Max(rect.Position.X + rect.Size.Width,  current.Position.X + current.Size.Width);
        this.Size.Height = T.Max(rect.Position.Y + rect.Size.Height, current.Position.Y + current.Size.Height);

        this.Size -= Unsafe.BitCast<Point<T>, Size<T>>(this.Position);
    }

    public readonly Rect<T> InvertedY() =>
        new(this.Size, new(this.Position.X, -this.Position.Y));

}
