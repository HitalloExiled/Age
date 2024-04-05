using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Age.Numerics;

[DebuggerDisplay("X: {X}, Y: {Y}, Z: {Z}, W: {W}")]
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

    public readonly T Angle => T.CreateChecked(2) * T.Acos(this.W);

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

    public readonly Quaternion<T> Inverse
    {
        get
        {
            var invMagnitudeSquared = T.One / (this.W * this.W + this.X * this.X + this.Y * this.Y + this.Z * this.Z);

            return new Quaternion<T>(
                this.W * invMagnitudeSquared,
                -this.X * invMagnitudeSquared,
                -this.Y * invMagnitudeSquared,
                -this.Z * invMagnitudeSquared
            );
        }
    }

    public readonly T Length        => T.Sqrt(this.LengthSquared);
    public readonly T LengthSquared => Quaternion.Dot(this, this);

    public readonly Quaternion<T> Normalized
    {
        get
        {
            var length = this.Length;
            return new(this.X / length, this.Y / length, this.Z / length, this.W / length);
        }
    }

    public Quaternion(T x, T y, T z, T w)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
        this.W = w;
    }

    public Quaternion(Vector3<T> vector, T scalar) : this(vector.X, vector.Y, vector.Z, scalar) { }

    public readonly T Dot(Quaternion<T> other) =>
        Quaternion.Dot(this, other);

    public static Quaternion<T> FromAngleAxis(Vector3<T> axis, T angle)
    {
        var halfAngle    = angle / T.CreateChecked(2);
        var sinHalfAngle = T.Sin(halfAngle);

        return new Quaternion<T>(axis.X * sinHalfAngle, axis.Y * sinHalfAngle, axis.Z * sinHalfAngle, T.Cos(halfAngle));
    }

    // public static Quaternion<T> FromRotationMatrix(in Matrix3x3<T> matrix)
    // {
    //     var trace = matrix.M11 + matrix.M22 + matrix.M33;

    //     Unsafe.SkipInit<Quaternion<T>>(out var result);

    //     if (trace > T.Zero)
    //     {
    //         var s = T.Sqrt(trace + T.One) * T.CreateChecked(2);
    //         result.X = (matrix.M32 - matrix.M32) / s;
    //         result.Y = (matrix.M31 - matrix.M31) / s;
    //         result.Z = (matrix.M12 - matrix.M21) / s;
    //         result.W = T.CreateChecked(0.25) * s;
    //     }
    //     else if (matrix.M11 > matrix.M22 && matrix.M11 > matrix.M33)
    //     {
    //         var s = T.Sqrt(T.One + matrix.M11 - matrix.M22 - matrix.M33) * T.CreateChecked(2);
    //         result.X = T.CreateChecked(0.25) * s;
    //         result.Y = (matrix.M21 + matrix.M12) / s;
    //         result.Z = (matrix.M31 + matrix.M31) / s;
    //         result.W = (matrix.M32 - matrix.M32) / s;
    //     }
    //     else if (matrix.M22 > matrix.M33)
    //     {
    //         var s = T.Sqrt(T.One + matrix.M22 - matrix.M11 - matrix.M33) * T.CreateChecked(2);
    //         result.X = (matrix.M21 + matrix.M12) / s;
    //         result.Y = T.CreateChecked(0.25) * s;
    //         result.Z = (matrix.M32 + matrix.M32) / s;
    //         result.W = (matrix.M31 - matrix.M31) / s;
    //     }
    //     else
    //     {
    //         var s = T.Sqrt(T.One + matrix.M33 - matrix.M11 - matrix.M22) * T.CreateChecked(2);
    //         result.X = (matrix.M31 + matrix.M31) / s;
    //         result.Y = (matrix.M32 + matrix.M32) / s;
    //         result.Z = T.CreateChecked(0.25) * s;
    //         result.W = (matrix.M12 - matrix.M21) / s;
    //     }

    //     return result;
    // }

    // public readonly Matrix3x3<T> ToRotationMatrix()
    // {
    //     Unsafe.SkipInit<Matrix3x3<T>>(out var rotationMatrix);

    //     var xx = this.X * this.X;
    //     var xy = this.X * this.Y;
    //     var xz = this.X * this.Z;
    //     var xw = this.X * this.W;
    //     var yy = this.Y * this.Y;
    //     var yz = this.Y * this.Z;
    //     var yw = this.Y * this.W;
    //     var zz = this.Z * this.Z;
    //     var zw = this.Z * this.W;

    //     var one = T.One;
    //     var two = T.CreateChecked(2);

    //     rotationMatrix.M11 = one - two * (yy + zz);
    //     rotationMatrix.M21 = two * (xy - zw);
    //     rotationMatrix.M31 = two * (xz + yw);
    //     rotationMatrix.M12 = two * (xy + zw);
    //     rotationMatrix.M22 = one - two * (xx + zz);
    //     rotationMatrix.M32 = two * (yz - xw);
    //     rotationMatrix.M31 = two * (xz - yw);
    //     rotationMatrix.M32 = two * (yz + xw);
    //     rotationMatrix.M33 = one - two * (xx + yy);

    //     return rotationMatrix;
    // }

    public override readonly string ToString()
        => $"{{ X = {this.X}, Y = {this.Y}, Z = {this.Z}, W = {this.W} }}";

    public static Quaternion<T> operator *(Quaternion<T> left, Quaternion<T> right) =>
        new(
            left.W * right.W - left.X * right.X - left.Y * right.Y - left.Z * right.Z,
            left.W * right.X + left.X * right.W + left.Y * right.Z - left.Z * right.Y,
            left.W * right.Y - left.X * right.Z + left.Y * right.W + left.Z * right.X,
            left.W * right.Z + left.X * right.Y - left.Y * right.X + left.Z * right.W
        );

    public static Quaternion<T> operator /(Quaternion<T> left, Quaternion<T> right) =>
        left * right.Inverse;
}

public static class Quaternion
{
    public static T Dot<T>(Quaternion<T> left, Quaternion<T> right) where T : IFloatingPoint<T>, IFloatingPointIeee754<T>, IRootFunctions<T>, ITrigonometricFunctions<T> =>
        left.X * right.X + left.Y * right.Y + left.Z * right.Z + left.W * right.W;
}
