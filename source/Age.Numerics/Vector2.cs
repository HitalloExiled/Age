using System.Globalization;
using System.Numerics;

namespace Age.Numerics;

public record struct Vector2<T> where T : IFloatingPoint<T>, IFloatingPointIeee754<T>, IRootFunctions<T>, ITrigonometricFunctions<T>
{
    public static Vector2<T> Down  => new(T.Zero, -T.One);
    public static Vector2<T> Left  => new(-T.One, T.Zero);
    public static Vector2<T> One   => new(T.One);
    public static Vector2<T> Right => new(T.One, T.Zero);
    public static Vector2<T> Up    => new(T.Zero, T.One);
    public static Vector2<T> Zero  => default;

    public T X;
    public T Y;

    public readonly Vector2<T> Inverted  => new(-this.X, -this.Y);
    public readonly Vector2<T> InvertedX => new(-this.X, this.Y);
    public readonly Vector2<T> InvertedY => new(this.X, -this.Y);

    public Vector2(T value)
    {
        this.X = value;
        this.Y = value;
    }

    public Vector2(T x, T y)
    {
        this.X = x;
        this.Y = y;
    }

    public T this[int index]
    {
        readonly get => index switch
        {
            0 => this.X,
            1 => this.Y,
            _ => throw new IndexOutOfRangeException(),
        };
        set
        {
            switch (index)
            {
                case 0: this.X = value; break;
                case 1: this.Y = value; break;
                default: throw new IndexOutOfRangeException();
            }
        }
    }

    public readonly T Length        => T.Sqrt(this.LengthSquared);
    public readonly T LengthSquared => Dot(this, this);

    public static T Angle(in Vector2<T> v1, in Vector2<T> v2) =>
        T.Atan2(CrossProduct(v1, v2), Dot(v1, v2));

    public static T CrossProduct(in Vector2<T> v1, in Vector2<T> v2) =>
        (v1.X * v2.Y) - (v1.Y * v2.X);

    public static T Dot(in Vector2<T> v1, in Vector2<T> v2) =>
        (v1.X * v2.X) + (v1.Y * v2.Y);

    public static bool IsApprox(in Vector2<T> left, in Vector2<T> right) =>
        Math<T>.IsApprox(left.X, right.X) && Math<T>.IsApprox(left.Y, right.Y);

    public static Vector2<T> Normalized(in Vector2<T> vector) =>
        vector.Length is T length && length > T.Zero ? vector / length : default;

    public readonly T CrossProduct(in Vector2<T> other) =>
        CrossProduct(this, other);

    public readonly T Dot(in Vector2<T> other) =>
        Dot(this, other);

    public readonly bool IsApprox(in Vector2<T> other) =>
        IsApprox(this, other);

    public readonly Vector2<T> Normalized() =>
        Normalized(this);

    public readonly Vector2<T> Rotated(T radians)
    {
        var cos = T.Cos(radians);
        var sin = T.Sin(radians);

        return new((this.X * cos) - (this.Y * sin), (this.X * sin) + (this.Y * cos));
    }

    public readonly Point<T> ToPoint() =>
        new(this.X, this.Y);

    public readonly Point<U> ToPoint<U>() where U : INumber<U> =>
        new(U.CreateChecked(this.X), U.CreateChecked(this.Y));

    public override readonly string ToString() =>
        string.Create(CultureInfo.InvariantCulture, $"{{ X: {this.X}, Y: {this.Y} }}");

    public static Vector2<T> operator +(in Vector2<T> vector, T value) =>
        new(vector.X + value, vector.Y + value);

    public static Vector2<T> operator +(in Vector2<T> left, in Vector2<T> right) =>
        new(left.X + right.X, left.Y + right.Y);

    public static Vector2<T> operator -(in Vector2<T> vector) =>
        new(-vector.X, -vector.Y);

    public static Vector2<T> operator -(in Vector2<T> vector, T value) =>
        new(vector.X - value, vector.Y - value);

    public static Vector2<T> operator -(in Vector2<T> left, in Vector2<T> right) =>
        new(left.X - right.X, left.Y - right.Y);

    public static Vector2<T> operator *(in Vector2<T> left, in Vector2<T> right) =>
        new(left.X * right.X, left.Y * right.Y);

    public static Vector2<T> operator *(in Vector2<T> vector, T scalar) =>
        new(vector.X * scalar, vector.Y * scalar);

    public static Vector2<T> operator *(T scalar, in Vector2<T> vector) =>
        new(vector.X * scalar, vector.Y * scalar);

    public static Vector2<T> operator /(in Vector2<T> left, in Vector2<T> right) =>
        new(left.X / right.X, left.Y / right.Y);

    public static Vector2<T> operator /(in Vector2<T> vector, T scalar) =>
        new(vector.X / scalar, vector.Y / scalar);
}
