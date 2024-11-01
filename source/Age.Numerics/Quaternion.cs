using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Age.Numerics;

public record struct Quaternion<T> where T : IFloatingPoint<T>, IFloatingPointIeee754<T>, IRootFunctions<T>, ITrigonometricFunctions<T>
{
    public static Quaternion<T> Identity => new(T.Zero, T.Zero, T.Zero, T.One);
    public static Quaternion<T> Zero     => default;

    public T X;
    public T Y;
    public T Z;
    public T W;

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

    public readonly Vector3<T> Axis
    {
        get
        {
            var sinHalfAngle = T.Sqrt(T.One - this.W * this.W);

            return sinHalfAngle < T.CreateChecked(0.001)
                ? new(T.One, T.Zero, T.Zero)
                : new(this.X / sinHalfAngle, this.Y / sinHalfAngle, this.Z / sinHalfAngle);
        }
    }

    public readonly T             Length        => T.Sqrt(this.LengthSquared);
    public readonly T             LengthSquared => Quaternion<T>.Dot(this, this);
    public readonly Quaternion<T> Normalized    => this / this.Length;

    public Quaternion(T x, T y, T z, T w)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
        this.W = w;
    }

    public Quaternion(in Vector3<T> axis, T radians)
    {
        var halfAngle    = radians * T.CreateChecked(0.5);
		var sinHalfAngle = T.Sin(halfAngle);

		this.X = axis.X * sinHalfAngle;
		this.Y = axis.Y * sinHalfAngle;
		this.Z = axis.Z * sinHalfAngle;
		this.W = T.Cos(halfAngle);
    }

    public static Quaternion<T> Conjugate(in Quaternion<T> value) =>
        value * new Vector4<T>(-T.One, -T.One, -T.One, T.One);

    public static T Dot(in Quaternion<T> left, in Quaternion<T> right) =>
        left.X * right.X + left.Y * right.Y + left.Z * right.Z + left.W * right.W;

    public readonly T Dot(in Quaternion<T> other) =>
        Quaternion<T>.Dot(this, other);

    public override readonly int GetHashCode() =>
        this.X.GetHashCode() ^ (this.Y.GetHashCode() << 2) ^ (this.Z.GetHashCode() >> 2) ^ (this.W.GetHashCode() >> 1);

    public readonly Quaternion<T> Inversed() =>
        Conjugate(this) / this.LengthSquared;

    public override readonly string ToString() =>
        string.Create(CultureInfo.InvariantCulture, $"{{ X: {this.X}, Y: {this.Y}, Z: {this.Z}, W: {this.W} }}");

    public static Quaternion<T> operator +(in Quaternion<T> left, in Quaternion<T> right) =>
        new(left.X + right.X, left.Y + right.Y, left.Z + right.Z, left.W + right.W);

    public static Quaternion<T> operator -(in Quaternion<T> value) =>
        new(-value.X, -value.Y, -value.Z, -value.W);

    public static Quaternion<T> operator *(in Quaternion<T> left, in Quaternion<T> right)
    {
        Unsafe.SkipInit<Quaternion<T>>(out var result);

        result.X = left.W * right.X + left.X * right.W + left.Y * right.Z - left.Z * right.Y;
        result.Y = left.W * right.Y - left.X * right.Z + left.Y * right.W + left.Z * right.X;
        result.Z = left.W * right.Z + left.X * right.Y - left.Y * right.X + left.Z * right.W;
        result.W = left.W * right.W - left.X * right.X - left.Y * right.Y - left.Z * right.Z;

        return result;
    }

    public static Quaternion<T> operator *(in Quaternion<T> left, in Vector4<T> right) =>
        new(left.X * right.X, left.Y * right.Y, left.Z * right.Z, left.W * right.W);

    public static Quaternion<T> operator *(in Quaternion<T> left, T multiplier) =>
        new(left.X * multiplier, left.Y * multiplier, left.Z * multiplier, left.W * multiplier);

    public static Quaternion<T> operator /(in Quaternion<T> left, in Quaternion<T> right)
    {
        var denominator        = right.X * right.X + right.Y * right.Y + right.Z * right.Z + right.W * right.W;
        var inverseDenominator = T.One / denominator;

        var inverseX2 = (T.Zero - right.X) * inverseDenominator;
        var inverseY2 = (T.Zero - right.Y) * inverseDenominator;
        var inverseZ2 = (T.Zero - right.Z) * inverseDenominator;
        var inverseW2 = right.W * inverseDenominator;

        var crossX = left.Y * inverseZ2 - left.Z * inverseY2;
        var crossY = left.Z * inverseX2 - left.X * inverseZ2;
        var crossZ = left.X * inverseY2 - left.Y * inverseX2;

        var dotProduct = left.X * inverseX2 + left.Y * inverseY2 + left.Z * inverseZ2;

        Unsafe.SkipInit(out Quaternion<T> result);

        result.X = left.X * inverseW2 + inverseX2 * left.W + crossX;
        result.Y = left.Y * inverseW2 + inverseY2 * left.W + crossY;
        result.Z = left.Z * inverseW2 + inverseZ2 * left.W + crossZ;
        result.W = left.W * inverseW2 - dotProduct;

        return result;
    }

    public static Quaternion<T> operator /(in Quaternion<T> left, T divisor) =>
        new(left.X / divisor, left.Y / divisor, left.Z / divisor, left.W / divisor);
}
