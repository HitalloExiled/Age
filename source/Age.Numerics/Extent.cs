using System.Numerics;

namespace Age.Numerics;

public record struct Extent<T> where T : INumber<T>
{
    public T Width;
    public T Height;
    public T Depth;

    public readonly T Area => this.Width * this.Height * this.Depth;

    public Extent(T width, T height, T depth)
    {
        this.Width  = width;
        this.Height = height;
        this.Depth  = depth;
    }

    public Extent(T value) : this(value, value, value)
    { }

    public static Extent<T> operator +(Extent<T> size, T value) =>
        new(size.Width + value, size.Height + value, size.Depth + value);

    public static Extent<T> operator +(Extent<T> left, Extent<T> right) =>
        new(left.Width + right.Width, left.Height + right.Height, left.Depth + right.Depth);

    public static Extent<T> operator -(Extent<T> left, Extent<T> right) =>
        new(left.Width - right.Width, left.Height - right.Height, left.Depth - right.Depth);

    public static Extent<T> operator -(Extent<T> size, T value) =>
        new(size.Width - value, size.Height - value, size.Depth - value);

    public static Extent<T> operator /(Extent<T> left, Extent<T> right) =>
        new(left.Width / right.Width, left.Height / right.Height, left.Depth / right.Depth);

    public static Extent<T> operator /(Extent<T> size, T value) =>
        new(size.Width / value, size.Height / value, size.Depth / value);

    public static Extent<T> operator *(Extent<T> left, Extent<T> right) =>
        new(left.Width * right.Width, left.Height * right.Height, left.Depth * right.Depth);

    public static Extent<T> operator *(Extent<T> left, T value) =>
        new(left.Width * value, left.Height * value, left.Depth * value);
}
