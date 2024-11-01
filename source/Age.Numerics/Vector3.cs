using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Age.Numerics;

public record struct Vector3<T> where T : IFloatingPoint<T>, IRootFunctions<T>, ITrigonometricFunctions<T>
{
    public static Vector3<T> Back  => new(T.Zero, T.Zero, -T.One);
    public static Vector3<T> Down  => new(T.Zero, -T.One, T.Zero);
    public static Vector3<T> Front => new(T.Zero, T.Zero, T.One);
    public static Vector3<T> Left  => new(-T.One, T.Zero, T.Zero);
    public static Vector3<T> One   => new(T.One);
    public static Vector3<T> Right => new(T.One, T.Zero, T.Zero);
    public static Vector3<T> Up    => new(T.Zero, T.One, T.Zero);
    public static Vector3<T> Zero  => default;

    public T X;
    public T Y;
    public T Z;

    public Vector3(T value)
    {
        this.X = value;
        this.Y = value;
        this.Z = value;
    }

    public Vector3(T x, T y, T z)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
    }

    public Vector3(in Vector2<T> vector2, T z)
    {
        this.X = vector2.X;
        this.Y = vector2.Y;
        this.Z = z;
    }

    public T this[int index]
    {
        get
        {
            Common.ThrowIfOutOfRange(0, 3, index);

            return Unsafe.Add(ref this.X, index);
        }
        set
        {
            Common.ThrowIfOutOfRange(0, 3, index);

            Unsafe.Add(ref this.X, index) = value;
        }
    }

    public readonly T Length        => T.Sqrt(this.LengthSquared);
    public readonly T LengthSquared => this.Dot(this);

    public static Vector3<T> Cross(in Vector3<T> v1, in Vector3<T> v2) =>
        new(v1.Y * v2.Z - v1.Z * v2.Y, v1.Z * v2.X - v1.X * v2.Z, v1.X * v2.Y - v1.Y * v2.X);

    public static T Dot(in Vector3<T> v1, in Vector3<T> v2) =>
        v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;

    public static bool IsAprox(in Vector3<T> left, in Vector3<T> right) =>
        Math<T>.IsApprox(left.X, right.X) && Math<T>.IsApprox(left.Y, right.Y) && Math<T>.IsApprox(left.Z, right.Z);

    public static Vector3<T> Normalized(in Vector3<T> vector) =>
        vector.Length is T length && length > T.Zero ? vector / length : default;

    public readonly Vector3<T> Cross(in Vector3<T> other) =>
        Cross(this, other);

    public readonly T Dot(in Vector3<T> other) =>
        Dot(this, other);

    public readonly bool IsAprox(in Vector3<T> other) =>
        IsAprox(this, other);

    public readonly Vector3<T> Normalized() =>
        Normalized(this);

    public override readonly string ToString() =>
        string.Create(CultureInfo.InvariantCulture, $"{{ X: {this.X}, Y: {this.Y}, Z: {this.Z} }}");

    public readonly Vector2<T> ToVector2() =>
        new(this.X, this.Y);

    public static Vector3<T> operator +(Vector3<T> vector, T value) =>
        new(vector.X + value, vector.Y + value, vector.Z + value);

    public static Vector3<T> operator +(Vector3<T> left, Vector3<T> right) =>
        new(left.X + right.X, left.Y + right.Y, left.Z + right.Z);

    public static Vector3<T> operator -(Vector3<T> left, Vector3<T> right) =>
        new(left.X - right.X, left.Y - right.Y, left.Z - right.Z);

    public static Vector3<T> operator -(Vector3<T> vector) =>
        new(-vector.X, -vector.Y, -vector.Z);

    public static Vector3<T> operator *(Vector3<T> vector, T scalar) =>
        new(vector.X * scalar, vector.Y * scalar, vector.Z * scalar);

    public static Vector3<T> operator *(T scalar, Vector3<T> vector) =>
        vector * scalar;

    public static Vector3<T> operator *(Vector3<T> left, Vector3<T> right) =>
        new(left.X * right.X, left.Y * right.Y, left.Z * right.Z);

    public static Vector3<T> operator /(Vector3<T> vector, T scalar) =>
        new(vector.X / scalar, vector.Y / scalar, vector.Z / scalar);

    public static Vector3<T> operator /(Vector3<T> left, Vector3<T> right) =>
        new(left.X / right.X, left.Y / right.Y, left.Z / right.Z);

    public static implicit operator Vector2<T>(Vector3<T> vector) =>
        new(vector.X, vector.Y);

    public static implicit operator Vector4<T>(Vector3<T> vector) =>
        new(vector.X, vector.Y, vector.Z, T.Zero);
}
