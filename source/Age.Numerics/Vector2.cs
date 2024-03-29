using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Age.Numerics;

[DebuggerDisplay("X: {X}, Y: {Y}")]
public record struct Vector2<T> where T : IFloatingPoint<T>
{
    public T X;
    public T Y;

    public Vector2(T x, T y)
    {
        this.X = x;
        this.Y = y;
    }

    public T this[int index]
    {
        get
        {
            Common.ThrowIfOutOfRange(0, 2, index);

            return Unsafe.Add(ref this.X, index);
        }
        set
        {
            Common.ThrowIfOutOfRange(0, 2, index);

            Unsafe.Add(ref this.X, index) = value;
        }
    }

    public readonly Point<U> ToPoint<U>() where U : INumber<U> =>
        new(U.CreateChecked(this.X), U.CreateChecked(this.Y));

    public static Vector2<T> operator +(Vector2<T> vector, T value) =>
        new(vector.X + value, vector.Y + value);

    public static Vector2<T> operator +(Vector2<T> left, Vector2<T> right) =>
        new(left.X + right.X, left.Y + right.Y);

    public static Vector2<T> operator -(Vector2<T> vector, T value) =>
        new(vector.X - value, vector.Y - value);

    public static Vector2<T> operator -(Vector2<T> left, Vector2<T> right) =>
        new(left.X - right.X, left.Y - right.Y);

    public static Vector2<T> operator /(Vector2<T> left, Vector2<T> right) =>
        new(left.X / right.X, left.Y / right.Y);
}
