using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace Age.Numerics;

public record struct Rect<T> where T : INumber<T>
{
    public Size<T>  Size;
    public Point<T> Position;

    public readonly T Area => this.Size.Area;

    public readonly T Bottom => this.Top + this.Size.Height;
    public readonly T Left   => this.Position.X;
    public readonly T Right  => this.Left + this.Size.Width;
    public readonly T Top    => this.Position.Y;

    public Rect(in Size<T> size, in Point<T> position)
    {
        this.Size     = size;
        this.Position = position;
    }

    public Rect(T width, T height, T x, T y) : this(new(width, height), new(x, y))
    { }

    public readonly Rect<U> Cast<U>() where U : INumber<U> =>
        new(this.Size.Cast<U>(), this.Position.Cast<U>());

    public void Grow(in Size<T> size, Point<T> posistion) =>
        this.Grow(new(size, posistion));

    public void Grow(in Rect<T> rect)
    {
        if (this == default)
        {
            this = rect;
        }
        else
        {
            var current = this;

            this.Position.X = T.Min(rect.Position.X, current.Position.X);
            this.Position.Y = T.Min(rect.Position.Y, current.Position.Y);

            this.Size.Width  = T.Max(rect.Position.X + rect.Size.Width,  current.Position.X + current.Size.Width);
            this.Size.Height = T.Max(rect.Position.Y + rect.Size.Height, current.Position.Y + current.Size.Height);

            this.Size -= Unsafe.BitCast<Point<T>, Size<T>>(this.Position);
        }
    }

    public readonly bool Intersects(in Point<T> point) =>
        Rect.Intersects(this, point);

    public readonly bool Intersects(in Rect<T> other) =>
        Rect.Intersects(this, other);

    public readonly Rect<T> Intersection(in Rect<T> other) =>
        Rect.Intersection(this, other);

    public readonly Rect<T> InvertedY() =>
        new(this.Size, new(this.Position.X, -this.Position.Y));

    public override readonly string ToString() =>
        string.Create(CultureInfo.InvariantCulture, $"{{ Size: {this.Size}, Position: {this.Position} }}");
}

public static class Rect
{
    public static bool Intersects<T>(in Rect<T> rect, in Point<T> point) where T : INumber<T> =>
        Math<T>.IsInRange(point.X, rect.Left, rect.Right) && Math<T>.IsInRange(point.Y, rect.Top, rect.Bottom);

    public static bool Intersects<T>(in Rect<T> left, in Rect<T> right) where T : INumber<T> =>
        Intersection(left, right) != default;

    public static Rect<T> Intersection<T>(in Rect<T> left, in Rect<T> right) where T : INumber<T>
    {
        var x = T.Max(left.Position.X, right.Position.X);
        var y = T.Max(left.Position.Y, right.Position.Y);

        var width  = T.Min(left.Size.Width + left.Position.X, right.Size.Width + right.Position.X) - x;
        var height = T.Min(left.Size.Height + left.Position.Y, right.Size.Height + right.Position.Y) - y;

        return new(width, height, x, y);
    }
}
