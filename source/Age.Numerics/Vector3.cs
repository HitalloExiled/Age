using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Age.Numerics;

[DebuggerDisplay("X: {X}, Y: {Y}, Z: {Z}")]
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

    public readonly T Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => T.Sqrt(this.LengthSquared);
    }

    public readonly T LengthSquared
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => this.Dot(this);
    }

    public Vector2<T> AsVector2() =>
        Unsafe.As<Vector3<T>, Vector2<T>>(ref this);

    public readonly Vector3<T> Cross(Vector3<T> other) =>
        new(this.Y * other.Z - this.Z * other.Y, this.Z * other.X - this.X * other.Z, this.X * other.Y - this.Y * other.X);

    internal readonly bool IsAprox(Vector3<T> other) =>
        MathX.IsApprox(this.X, other.X) && MathX.IsApprox(this.Y, other.Y) && MathX.IsApprox(this.Z, other.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly T Dot(Vector3<T> other) =>
        this.X * other.X + this.Y * other.Y + this.Z * other.Z;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly Vector3<T> Normalized() =>
        this.Length is T length && length > T.Zero ? this / length : default;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector3<T> operator +(Vector3<T> vector, T value) =>
        new(vector.X + value, vector.Y + value, vector.Z + value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector3<T> operator +(Vector3<T> left, Vector3<T> right) =>
        new(left.X + right.X, left.Y + right.Y, left.Z + right.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector3<T> operator -(Vector3<T> left, Vector3<T> right) =>
        new(left.X - right.X, left.Y - right.Y, left.Z - right.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector3<T> operator -(Vector3<T> vector) =>
        new(-vector.X, -vector.Y, -vector.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector3<T> operator *(Vector3<T> vector, T scalar) =>
        new(vector.X * scalar, vector.Y * scalar, vector.Z * scalar);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector3<T> operator *(T scalar, Vector3<T> vector) =>
        vector * scalar;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector3<T> operator *(Vector3<T> left, Vector3<T> right) =>
        new(left.X * right.X, left.Y * right.Y, left.Z * right.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector3<T> operator /(Vector3<T> vector, T scalar) =>
        new(vector.X / scalar, vector.Y / scalar, vector.Z / scalar);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector3<T> operator /(Vector3<T> left, Vector3<T> right) =>
        new(left.X / right.X, left.Y / right.Y, left.Z / right.Z);

    public static implicit operator Vector2<T>(Vector3<T> vector) =>
        new(vector.X, vector.Y);

    public static implicit operator Vector4<T>(Vector3<T> vector) =>
        new(vector.X, vector.Y, vector.Z, T.Zero);

    public override readonly string ToString()
        => $"{{ X = {this.X}, Y = {this.Y}, Z = {this.Z} }}";
}
