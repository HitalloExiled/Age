using System.Diagnostics;
using System.Numerics;

namespace Age.Numerics;

[DebuggerDisplay("\\{ Width: {Width}, Height: {Height} \\}")]
public record struct Size<T> where T : INumber<T>
{
    public T Width;
    public T Height;

    public readonly T Area => this.Width * this.Height;

    public Size(T width, T height)
    {
        this.Width  = width;
        this.Height = height;
    }

    public Size(T value) : this(value, value)
    { }

    public readonly Size<U> Cast<U>() where U : INumber<U> =>
        new(U.CreateChecked(this.Width), U.CreateChecked(this.Height));

    public readonly Size<T> Max(Size<T> other) =>
        new(T.Max(this.Width, other.Width), T.Max(this.Height, other.Height));

    public readonly Size<T> Min(Size<T> other) =>
        new(T.Min(this.Width, other.Width), T.Min(this.Height, other.Height));

    public readonly Size<T> Range(Size<T> min, Size<T> max) =>
        new(T.Max(T.Min(this.Width, min.Width), max.Width), T.Max(T.Min(this.Height, min.Height), max.Height));

    public static Size<T> operator +(Size<T> size, T value) =>
        new(size.Width + value, size.Height + value);

    public static Size<T> operator +(Size<T> left, Size<T> right) =>
        new(left.Width + right.Width, left.Height + right.Height);

    public static Size<T> operator -(Size<T> left, Size<T> right) =>
        new(left.Width - right.Width, left.Height - right.Height);

    public static Size<T> operator -(Size<T> size, T value) =>
        new(size.Width - value, size.Height - value);

    public static Size<T> operator /(Size<T> left, Size<T> right) =>
        new(left.Width / right.Width, left.Height / right.Height);

    public static Size<T> operator /(Size<T> size, T value) =>
        new(size.Width / value, size.Height / value);

    public static Size<T> operator *(Size<T> left, Size<T> right) =>
        new(left.Width * right.Width, left.Height * right.Height);

    public static implicit operator Size<T>(Point<T> point) =>
        new(point.X, point.Y);

    public static implicit operator Size<T>(Vector2<float> vector) =>
        new(T.CreateChecked(vector.X), T.CreateChecked(vector.Y));

    public static implicit operator Vector2<float>(Size<T> point) =>
        new(float.CreateChecked(point.Width), float.CreateChecked(point.Height));

    public static implicit operator Size<T>(Vector2<double> vector) =>
        new(T.CreateChecked(vector.X), T.CreateChecked(vector.Y));

    public static implicit operator Vector2<double>(Size<T> point) =>
        new(double.CreateChecked(point.Width), double.CreateChecked(point.Height));
}
