using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Age.Numerics;

[DebuggerDisplay("[{Column1}], [{Column2}], [{Column3}], [{Column4}]")]
public struct Matrix4x4<T> where T : IFloatingPoint<T>, IRootFunctions<T>, ITrigonometricFunctions<T>
{
    public static Matrix4x4<T> Identity => new(
        new(T.One,  T.Zero, T.Zero, T.Zero),
        new(T.Zero, T.One,  T.Zero, T.Zero),
        new(T.Zero, T.Zero, T.One,  T.Zero),
        new(T.Zero, T.Zero, T.Zero, T.One)
    );

    public T M11;
	public T M12;
	public T M13;
	public T M14;
	public T M21;
	public T M22;
	public T M23;
	public T M24;
	public T M31;
	public T M32;
	public T M33;
	public T M34;
	public T M41;
	public T M42;
	public T M43;
	public T M44;

    public T this[int row, int column]
    {
        get
        {
            Common.ThrowIfOutOfRange(0, 3, row);
            Common.ThrowIfOutOfRange(0, 3, column);

		    return Unsafe.Add(ref Unsafe.As<T, Vector4<T>>(ref this.M11), row)[column];
        }
        set
        {
            Common.ThrowIfOutOfRange(0, 3, row);
            Common.ThrowIfOutOfRange(0, 3, column);

            Unsafe.Add(ref Unsafe.As<T, Vector4<T>>(ref this.M11), row)[column] = value;
        }
    }

    public readonly bool IsIdentity =>
        this.M11 == T.One  &&
        this.M22 == T.One  &&
        this.M33 == T.One  &&
        this.M44 == T.One  &&
        this.M12 == T.Zero &&
        this.M13 == T.Zero &&
        this.M14 == T.Zero &&
        this.M21 == T.Zero &&
        this.M23 == T.Zero &&
        this.M24 == T.Zero &&
        this.M31 == T.Zero &&
        this.M32 == T.Zero &&
        this.M34 == T.Zero &&
        this.M41 == T.Zero &&
        this.M42 == T.Zero &&
        this.M43 == T.Zero;

    public Matrix4x4(T m11, T m12, T m13, T m14, T m21, T m22, T m23, T m24, T m31, T m32, T m33, T m34, T m41, T m42, T m43, T m44)
    {
        this.M11 = m11;
        this.M12 = m12;
        this.M13 = m13;
        this.M14 = m14;
        this.M21 = m21;
        this.M22 = m22;
        this.M23 = m23;
        this.M24 = m24;
        this.M31 = m31;
        this.M32 = m32;
        this.M33 = m33;
        this.M34 = m34;
        this.M41 = m41;
        this.M42 = m42;
        this.M43 = m43;
        this.M44 = m44;
    }

    public Matrix4x4(Vector4<T> column1, Vector4<T> column2, Vector4<T> column3, Vector4<T> column4)
    {
        this.M11 = column1.X;
        this.M12 = column2.X;
        this.M13 = column3.X;
        this.M14 = column4.X;
        this.M21 = column1.Y;
        this.M22 = column2.Y;
        this.M23 = column3.Y;
        this.M24 = column4.Y;
        this.M31 = column1.Z;
        this.M32 = column2.Z;
        this.M33 = column3.Z;
        this.M34 = column4.Z;
        this.M41 = column1.W;
        this.M42 = column2.W;
        this.M43 = column3.W;
        this.M44 = column4.W;
    }

    public static Matrix4x4<T> LookAt(Vector3<T> eye, Vector3<T> center, Vector3<T> up)
    {
        var zaxis = (eye - center).Normalized;
        var xaxis = up.Cross(zaxis).Normalized;
        var yaxis = zaxis.Cross(xaxis);

        var identity = Identity;

        identity.M11 = xaxis.X;
        identity.M12 = yaxis.X;
        identity.M13 = zaxis.X;
        identity.M21 = xaxis.Y;
        identity.M22 = yaxis.Y;
        identity.M23 = zaxis.Y;
        identity.M31 = xaxis.Z;
        identity.M32 = yaxis.Z;
        identity.M33 = zaxis.Z;
        identity.M41 = T.Zero - xaxis.Dot(eye);
        identity.M42 = T.Zero - yaxis.Dot(eye);
        identity.M43 = T.Zero - zaxis.Dot(eye);

        return identity;
    }

    public static Matrix4x4<T> Perspective(T width, T height, T nearPlaneDistance, T farPlaneDistance)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(nearPlaneDistance, T.Zero);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(farPlaneDistance, T.Zero);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(nearPlaneDistance, farPlaneDistance);

        Unsafe.SkipInit(out Matrix4x4<T> result);

        var two = T.CreateChecked(2);

        result.M11 = two * nearPlaneDistance / width;
        result.M12 = result[0, 2] = result[0, 3] = T.Zero;
        result.M22 = two * nearPlaneDistance / height;
        result.M21 = result[1, 2] = result[1, 3] = T.Zero;

        var num = result[2, 2] = T.IsPositiveInfinity(farPlaneDistance) ? (-T.One) : (farPlaneDistance / (nearPlaneDistance - farPlaneDistance));

        result.M31 = result[2, 1] = T.Zero;
        result.M34 = -T.One;
        result.M41 = result[3, 1] = result[3, 3] = T.Zero;
        result.M43 = nearPlaneDistance * num;

        return result;
    }

    public static Matrix4x4<T> PerspectiveFov(T fieldOfView, T aspectRatio, T nearPlaneDistance, T farPlaneDistance)
    {
        if (fieldOfView <= T.Zero || fieldOfView >= T.CreateSaturating(Math.PI))
        {
            throw new ArgumentOutOfRangeException(nameof(fieldOfView));
        }

        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(nearPlaneDistance, T.Zero);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(farPlaneDistance, T.Zero);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(nearPlaneDistance, farPlaneDistance);

        var num = T.One / T.Tan(fieldOfView * T.CreateSaturating(0.5));

        Unsafe.SkipInit(out Matrix4x4<T> result);

        result.M11 = num / aspectRatio;
        result.M12 = result[0, 2] = result[0, 3] = T.Zero;
        result.M22 = num;
        result.M21 = result[1, 2] = result[1, 3] = T.Zero;
        result.M31 = result[2, 1] = T.Zero;

        var zz = result[2, 2] = T.IsPositiveInfinity(farPlaneDistance) ? (-T.One) : (farPlaneDistance / (nearPlaneDistance - farPlaneDistance));

        result.M34 = -T.One;
        result.M41 = result[3, 1] = result[3, 3] = T.Zero;
        result.M43 = nearPlaneDistance * zz;

        return result;
    }

    public static Matrix4x4<T> Rotate(Vector3<T> axis, T angle)
    {
        var x   = axis.X;
        var y   = axis.Y;
        var z   = axis.Z;
        var sin = T.Sin(angle);
        var cos = T.Cos(angle);
        var xx  = x * x;
        var yy  = y * y;
        var zz  = z * z;
        var xy  = x * y;
        var xz  = x * z;
        var yz  = y * z;

        var identity = Identity;

        identity.M11 = xx + cos * (T.One - xx);
        identity.M12 = xy - cos * xy + sin * z;
        identity.M13 = xz - cos * xz - sin * y;
        identity.M21 = xy - cos * xy - sin * z;
        identity.M22 = yy + cos * (T.One - yy);
        identity.M23 = yz - cos * yz + sin * x;
        identity.M31 = xz - cos * xz + sin * y;
        identity.M32 = yz - cos * yz - sin * x;
        identity.M33 = zz + cos * (T.One - zz);

        return identity;
    }
}
