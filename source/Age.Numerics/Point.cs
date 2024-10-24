using System.Numerics;

namespace Age.Numerics;

public record struct Point<T> where T : INumber<T>
{
    public T X;
    public T Y;

    public Point(T value) : this(value, value)
    { }

    public Point(T x, T y)
    {
        this.X = x;
        this.Y = y;
    }

    public readonly Point<U> Cast<U>() where U : INumber<U> =>
        new(U.CreateChecked(this.X), U.CreateChecked(this.Y));

    public override readonly string ToString() =>
        $"{{ X: {this.X}, Y: {this.Y} }}";

    public static Point<T> operator +(Point<T> point, T value) =>
        new(point.X + value, point.Y + value);

    public static Point<T> operator +(Point<T> left, Point<T> right) =>
        new(left.X + right.X, left.Y + right.Y);

    public static Point<T> operator -(T value, Point<T> point) =>
        new(value - point.X, value - point.Y);

    public static Point<T> operator -(Point<T> point, T value) =>
        new(point.X - value, point.Y - value);

    public static Point<T> operator -(Point<T> left, Point<T> right) =>
        new(left.X - right.X, left.Y - right.Y);

    public static Point<T> operator /(Point<T> point, T value) =>
        new(point.X / value, point.Y / value);

    public static Point<T> operator /(Point<T> left, Point<T> right) =>
        new(left.X / right.X, left.Y / right.Y);

    public static Point<T> operator *(Point<T> point, T value) =>
        new(point.X * value, point.Y * value);

    public static Point<T> operator *(Point<T> left, Point<T> right) =>
        new(left.X * right.X, left.Y * right.Y);

    public static implicit operator Point<T>(Size<T> size) =>
        new(size.Height, size.Width);

    public static implicit operator Point<T>(Vector2<float> vector) =>
        new(T.CreateChecked(vector.X), T.CreateChecked(vector.Y));

    public static implicit operator Vector2<float>(Point<T> point) =>
        new(float.CreateChecked(point.X), float.CreateChecked(point.Y));

    public static implicit operator Point<T>(Vector2<double> vector) =>
        new(T.CreateChecked(vector.X), T.CreateChecked(vector.Y));

    public static implicit operator Vector2<double>(Point<T> point) =>
        new(double.CreateChecked(point.X), double.CreateChecked(point.Y));
}
