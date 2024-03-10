using System.Diagnostics;
using System.Numerics;

namespace Age.Numerics;

[DebuggerDisplay("\\{ X: {X}, Y: {Y} \\}")]
public record struct Point<T> where T : INumber<T>
{
    public T X;
    public T Y;

    public Point(T x, T y)
    {
        this.X = x;
        this.Y = y;
    }

    public readonly Point<U> Cast<U>() where U : INumber<U> =>
        new(U.CreateChecked(this.X), U.CreateChecked(this.Y));

    public static Point<T> operator /(Point<T> left, Point<T> right) =>
        new(left.X / right.X, left.Y / right.Y);

    public static Point<T> operator /(Point<T> left, Size<T> right) =>
        new(left.X / right.Width, left.Y / right.Height);

    public static Point<T> operator +(Point<T> point, T value) =>
        new(point.X + value, point.Y + value);

    public static Point<T> operator -(Point<T> point, T value) =>
        new(point.X - value, point.Y - value);
}
