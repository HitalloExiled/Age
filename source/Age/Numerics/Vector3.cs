using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Age.Numerics;

[DebuggerDisplay("X: {X}, Y: {Y}, Z: {Z}")]
public record struct Vector3<T> where T :  IFloatingPoint<T>, IRootFunctions<T>, ITrigonometricFunctions<T>
{
    public T X;
    public T Y;
    public T Z;

    public Vector3(T x, T y, T z)
    {
        this.X = x;
        this.Y = y;
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

    public readonly Vector3<T> Normalized
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => this.Length is T length && length > T.Zero ? this / length : default;
    }

    public readonly Vector3<T> Cross(Vector3<T> other) =>
        new(this.Y * other.Z - this.Z * other.Y, this.Z * other.X - this.X * other.Z, this.X * other.Y - this.Y * other.X);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly T Dot(Vector3<T> other) =>
        this.X * other.X + this.Y * other.Y + this.Z * other.Z;

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
}
