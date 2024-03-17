using System.Diagnostics;
using System.Numerics;

namespace Age.Numerics;

[DebuggerDisplay("\\{ Width: {Width}, Height: {Height} \\}")]
public record struct Size<T> where T : INumber<T>
{
    public T Width;
    public T Height;

    public Size(T width, T height)
    {
        this.Width  = width;
        this.Height = height;
    }

    public static Size<T> operator +(Size<T> size, T value) =>
        new(size.Width + value, size.Height + value);

    public static Size<T> operator +(Size<T> left, Size<T> right) =>
        new(left.Width + right.Width, left.Height + right.Height);

    public static Size<T> operator -(Size<T> size, T value) =>
        new(size.Width - value, size.Height - value);

    public static Size<T> operator -(Size<T> left, Size<T> right) =>
        new(left.Width - right.Width, left.Height - right.Height);

    public static Size<T> operator /(Size<T> left, Size<T> right) =>
        new(left.Width / right.Width, left.Height / right.Height);
}
