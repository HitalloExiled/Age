using System.Globalization;
using System.Numerics;

namespace Age.Numerics;

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

    public override readonly string ToString() =>
        string.Create(CultureInfo.InvariantCulture, $"{{ Width: {this.Width}, Height: {this.Height} }}");

    public readonly Vector2<float> ToVector() =>
        new(float.CreateChecked(this.Width), float.CreateChecked(this.Height));

    public readonly Vector2<U> ToVector<U>() where U : IFloatingPoint<U>, IFloatingPointIeee754<U>, IRootFunctions<U>, ITrigonometricFunctions<U>  =>
        new(U.CreateChecked(this.Width), U.CreateChecked(this.Height));

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

    public static Size<T> operator *(Size<T> left, T value) =>
        new(left.Width * value, left.Height * value);
}
