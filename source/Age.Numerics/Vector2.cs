using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Age.Numerics;

[DebuggerDisplay("X: {X}, Y: {Y}")]
public record struct Vector2<T> where T : IFloatingPoint<T>, IRootFunctions<T>, ITrigonometricFunctions<T>
{
    public static Vector2<T> Down  => new(T.Zero, -T.One);
    public static Vector2<T> Left  => new(-T.One, T.Zero);
    public static Vector2<T> One   => new(T.One);
    public static Vector2<T> Right => new(T.One, T.Zero);
    public static Vector2<T> Up    => new(T.Zero, T.One);
    public static Vector2<T> Zero  => default;

    public T X;
    public T Y;

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

    public readonly T Length        => T.Sqrt(this.LengthSquared);
    public readonly T LengthSquared => this.Dot(this);

    public readonly T CrossProduct(in Vector2<T> other) =>
        Vector2.CrossProduct(this, other);

    public readonly T Dot(in Vector2<T> other) =>
        Vector2.Dot(this, other);

    public readonly bool IsApprox(Vector2<T> other) =>
        MathX.IsApprox(this.X, other.X) && MathX.IsApprox(this.Y, other.Y);

    public readonly Vector2<T> Normalized() =>
        this.Length is T length && length > T.Zero ? this / length : default;

    public readonly Vector2<T> Rotated(T radians)
    {
        var cos = T.Cos(radians);
        var sin = T.Sin(radians);

        return new(this.X * cos - this.Y * sin, this.X * sin + this.Y * cos);
    }

    public readonly Point<U> ToPoint<U>() where U : INumber<U> =>
        new(U.CreateChecked(this.X), U.CreateChecked(this.Y));

    public override readonly string ToString() =>
        $"{{ X = {this.X}, Y = {this.Y} }}";

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

public static class Vector2
{
    public static T Angle<T>(in Vector2<T> v1, in Vector2<T> v2) where T : IFloatingPoint<T>, IRootFunctions<T>, ITrigonometricFunctions<T>
    {
        var angle = T.Acos(Dot(v1, v2) / (v1.Length * v2.Length));

        return angle > T.Zero && v1.CrossProduct(v2) < T.Zero
            ? T.CreateChecked(2 * Math.PI) - angle
            : angle;
    }

    public static T CrossProduct<T>(in Vector2<T> v1, in Vector2<T> v2) where T : IFloatingPoint<T>, IRootFunctions<T>, ITrigonometricFunctions<T> =>
        v1.X * v2.Y - v1.Y * v2.X;

    public static T Dot<T>(in Vector2<T> v1, in Vector2<T> v2) where T : IFloatingPoint<T>, IRootFunctions<T>, ITrigonometricFunctions<T> =>
        v1.X * v2.X + v1.Y * v2.Y;
}
