using System.Globalization;
using System.Numerics;

namespace Age.Numerics;

public record struct Point<T> where T : INumber<T>
{
    public T X;
    public T Y;

    public readonly Point<T> Inverted  => new(-this.X, -this.Y);
    public readonly Point<T> InvertedX => new(-this.X, this.Y);
    public readonly Point<T> InvertedY => new(this.X, -this.Y);

    public Point(T value) : this(value, value)
    { }

    public Point(T x, T y)
    {
        this.X = x;
        this.Y = y;
    }

    public readonly Point<U> Cast<U>() where U : INumber<U> =>
        new(U.CreateChecked(this.X), U.CreateChecked(this.Y));

    public readonly Vector2<U> ToVector2<U>() where U : IFloatingPoint<U>, IFloatingPointIeee754<U>, IRootFunctions<U>, ITrigonometricFunctions<U> =>
        new(U.CreateChecked(this.X), U.CreateChecked(this.Y));

    public override readonly string ToString() =>
        string.Create(CultureInfo.InvariantCulture, $"{{ X: {this.X}, Y: {this.Y} }}");

    public static Point<T> operator +(in Point<T> left, in Point<T> right) =>
        new(left.X + right.X, left.Y + right.Y);

    public static Point<T> operator +(in Point<T> point, T value) =>
        new(point.X + value, point.Y + value);

    public static Point<T> operator -(in Point<T> point, T value) =>
        new(point.X - value, point.Y - value);

    public static Point<T> operator -(T value, in Point<T> point) =>
        new(value - point.X, value - point.Y);

    public static Point<T> operator -(in Point<T> left, in Point<T> right) =>
        new(left.X - right.X, left.Y - right.Y);

    public static Point<T> operator /(in Point<T> left, in Point<T> right) =>
        new(left.X / right.X, left.Y / right.Y);

    public static Point<T> operator /(in Point<T> point, T value) =>
        new(point.X / value, point.Y / value);

    public static Point<T> operator *(in Point<T> left, in Point<T> right) =>
        new(left.X * right.X, left.Y * right.Y);

    public static Point<T> operator *(in Point<T> point, T value) =>
        new(point.X * value, point.Y * value);

    public static implicit operator Point<T>(in Size<T> size) =>
        new(size.Width, size.Height);

    public static implicit operator Point<T>(in Vector2<float> vector) =>
        vector.ToPoint<T>();

    public static implicit operator Vector2<float>(in Point<T> point) =>
        point.ToVector2<float>();

    public static implicit operator Point<T>(in Vector2<double> vector) =>
        vector.ToPoint<T>();

    public static implicit operator Vector2<double>(in Point<T> point) =>
        point.ToVector2<double>();
}
