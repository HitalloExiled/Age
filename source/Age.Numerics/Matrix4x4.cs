using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Age.Numerics;

public record struct Matrix4x4<T> where T : IFloatingPoint<T>, IFloatingPointIeee754<T>, IRootFunctions<T>, ITrigonometricFunctions<T>
{
    public static Matrix4x4<T> Identity => new()
    {
        M11 = T.One,  M12 = T.Zero, M13 = T.Zero, M14 = T.Zero,
        M21 = T.Zero, M22 = T.One,  M23 = T.Zero, M24 = T.Zero,
        M31 = T.Zero, M32 = T.Zero, M33 = T.One,  M34 = T.Zero,
        M41 = T.Zero, M42 = T.Zero, M43 = T.Zero, M44 = T.One,
    };

    public T M11, M12, M13, M14;
	public T M21, M22, M23, M24;
	public T M31, M32, M33, M34;
	public T M41, M42, M43, M44;

    public T this[int row, int column]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly get => ((row * 4) + column) switch
        {
             0 => this.M11,
             1 => this.M12,
             2 => this.M13,
             3 => this.M14,
             4 => this.M21,
             5 => this.M22,
             6 => this.M23,
             7 => this.M24,
             8 => this.M31,
             9 => this.M32,
            10 => this.M33,
            11 => this.M34,
            12 => this.M41,
            13 => this.M42,
            14 => this.M43,
            15 => this.M44,
            _ => throw new IndexOutOfRangeException(),
        };
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            switch ((row * 4) + column)
            {
                case  0: this.M11 = value; break;
                case  1: this.M12 = value; break;
                case  2: this.M13 = value; break;
                case  3: this.M14 = value; break;
                case  4: this.M21 = value; break;
                case  5: this.M22 = value; break;
                case  6: this.M23 = value; break;
                case  7: this.M24 = value; break;
                case  8: this.M31 = value; break;
                case  9: this.M32 = value; break;
                case 10: this.M33 = value; break;
                case 11: this.M34 = value; break;
                case 12: this.M41 = value; break;
                case 13: this.M42 = value; break;
                case 14: this.M43 = value; break;
                case 15: this.M44 = value; break;
                default: throw new IndexOutOfRangeException();
            }
        }
    }

    public Vector3<T> X
    {
        readonly get => new(this.M11, this.M12, this.M13);
        set => Unsafe.As<T, Vector4<T>>(ref this.M11) = value;
    }

    public Vector3<T> Y
    {
        readonly get => new(this.M21, this.M22, this.M23);
        set => Unsafe.As<T, Vector4<T>>(ref this.M21) = value;
    }

    public Vector3<T> Z
    {
        readonly get => new(this.M31, this.M32, this.M33);
        set => Unsafe.As<T, Vector4<T>>(ref this.M31) = value;
    }

    public Vector3<T> W
    {
        readonly get => new(this.M41, this.M42, this.M43);
        set => Unsafe.As<T, Vector4<T>>(ref this.M41) = value;
    }

    public Vector3<T> Translation
    {
        readonly get => this.W;
        set => Unsafe.As<T, Vector3<T>>(ref this.M41) = value;
    }

    public Quaternion<T> Rotation
    {
        readonly get
        {
            Unsafe.SkipInit<Quaternion<T>>(out var result);

            var trace = this.M11 + this.M22 + this.M33;

            if (trace > T.Zero)
            {
                var scalar        = T.Sqrt(trace + T.One);
                var inverseScalar = Math<T>.Half / scalar;

                result.X = (this.M23 - this.M32) * inverseScalar;
                result.Y = (this.M31 - this.M13) * inverseScalar;
                result.Z = (this.M12 - this.M21) * inverseScalar;
                result.W = scalar * Math<T>.Half;
            }
            else if (this.M11 >= this.M22 && this.M11 >= this.M33)
            {
                var scalar        = T.Sqrt(T.One + this.M11 - this.M22 - this.M33);
                var inverseScalar = Math<T>.Half / scalar;

                result.X = Math<T>.Half * scalar;
                result.Y = (this.M12 + this.M21) * inverseScalar;
                result.Z = (this.M13 + this.M31) * inverseScalar;
                result.W = (this.M23 - this.M32) * inverseScalar;
            }
            else if (this.M22 > this.M33)
            {
                var scalar        = T.Sqrt(T.One + this.M22 - this.M11 - this.M33);
                var inverseScalar = Math<T>.Half / scalar;

                result.X = (this.M21 + this.M12) * inverseScalar;
                result.Y = Math<T>.Half * scalar;
                result.Z = (this.M32 + this.M23) * inverseScalar;
                result.W = (this.M31 - this.M13) * inverseScalar;
            }
            else
            {
                var scalar        = T.Sqrt(T.One + this.M33 - this.M11 - this.M22);
                var inverseScalar = Math<T>.Half / scalar;

                result.X = (this.M31 + this.M13) * inverseScalar;
                result.Y = (this.M32 + this.M23) * inverseScalar;
                result.Z = Math<T>.Half * scalar;
                result.W = (this.M12 - this.M21) * inverseScalar;
            }

            return result.Normalized;
        }
        set => ApplyRotatationWithScale(ref this, value, this.Scale);
    }

    public Vector3<T> Scale
    {
        readonly get => new(this.X.Length, this.Y.Length, this.Z.Length);
        set
        {
            ref var x = ref Unsafe.As<T, Vector3<T>>(ref this.M11);
            ref var y = ref Unsafe.As<T, Vector3<T>>(ref this.M21);
            ref var z = ref Unsafe.As<T, Vector3<T>>(ref this.M31);

            x = x.Normalized() * value.X;
            y = y.Normalized() * value.Y;
            z = z.Normalized() * value.Z;
        }
    }

    public readonly T Determinant =>
        (this.M11 * this.M22 * this.M33 * this.M44) - this.M11 * this.M22 * this.M34 * this.M43 - (this.M11 * this.M32 * this.M23 * this.M44) + (this.M11 * this.M32 * this.M24 * this.M43) +
        (this.M11 * this.M42 * this.M23 * this.M34) - this.M11 * this.M42 * this.M24 * this.M33 - (this.M21 * this.M12 * this.M33 * this.M44) + (this.M21 * this.M12 * this.M34 * this.M43) +
        (this.M21 * this.M32 * this.M13 * this.M44) - this.M21 * this.M32 * this.M14 * this.M43 - (this.M21 * this.M42 * this.M13 * this.M34) + (this.M21 * this.M42 * this.M14 * this.M33) +
        (this.M31 * this.M12 * this.M23 * this.M44) - this.M31 * this.M12 * this.M24 * this.M43 - (this.M31 * this.M22 * this.M13 * this.M44) + (this.M31 * this.M22 * this.M14 * this.M43) +
        (this.M31 * this.M42 * this.M13 * this.M24) - this.M31 * this.M42 * this.M14 * this.M23 - (this.M41 * this.M12 * this.M23 * this.M34) + (this.M41 * this.M12 * this.M24 * this.M33) +
        (this.M41 * this.M22 * this.M13 * this.M34) - this.M41 * this.M22 * this.M14 * this.M33 - (this.M41 * this.M32 * this.M13 * this.M24) + (this.M41 * this.M32 * this.M14 * this.M23);

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

    public Matrix4x4(in Vector4<T> column1, in Vector4<T> column2, in Vector4<T> column3, in Vector4<T> column4)
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

    public Matrix4x4(in Vector3<T> position, in Quaternion<T> rotation, in Vector3<T> scale)
    {
        Unsafe.SkipInit(out this);

        ApplyRotatationWithScale(ref this, rotation, scale);

        this.M41 = position.X;
        this.M42 = position.Y;
        this.M43 = position.Z;
        this.M44 = T.One;
    }

    private static void SetDiagonal(ref Matrix4x4<T> matrix, Vector3<T> vector3)
    {
        matrix.M11 = vector3.X;
        matrix.M12 = T.Zero;
        matrix.M13 = T.Zero;

        matrix.M21 = T.Zero;
        matrix.M22 = vector3.Y;
        matrix.M23 = T.Zero;

        matrix.M31 = T.Zero;
        matrix.M32 = T.Zero;
        matrix.M33 = vector3.Z;
    }

    private static void ApplyRotatationWithScale(ref Matrix4x4<T> matrix, in Quaternion<T> rotation, Vector3<T> scale)
    {
        SetDiagonal(ref matrix, scale);

        var xx = rotation.X * rotation.X;
        var xy = rotation.X * rotation.Y;
        var xz = rotation.X * rotation.Z;
        var xw = rotation.X * rotation.W;
        var yy = rotation.Y * rotation.Y;
        var yz = rotation.Y * rotation.Z;
        var yw = rotation.Y * rotation.W;
        var zz = rotation.Z * rotation.Z;
        var zw = rotation.Z * rotation.W;

        var one = T.One;
        var two = T.One + T.One;

        matrix.M11 = one - (two * (yy + zz));
        matrix.M12 = two * (xy + zw);
        matrix.M13 = two * (xz - yw);
        matrix.M14 = T.Zero;

        matrix.M21 = two * (xy - zw);
        matrix.M22 = one - (two * (xx + zz));
        matrix.M23 = two * (yz + xw);
        matrix.M24 = T.Zero;

        matrix.M31 = two * (xz + yw);
        matrix.M32 = two * (yz - xw);
        matrix.M33 = one - (two * (xx + yy));
        matrix.M34 = T.Zero;
    }

    public static Matrix4x4<T> LookingAt(in Vector3<T> eye, in Vector3<T> center, in Vector3<T> up)
    {
        var zaxis = (eye - center).Normalized();
        var xaxis = up.Cross(zaxis).Normalized();
        var yaxis = zaxis.Cross(xaxis);

        Unsafe.SkipInit(out Matrix4x4<T> result);

        result.M11 = xaxis.X;
        result.M12 = yaxis.X;
        result.M13 = zaxis.X;
        result.M14 = T.Zero;

        result.M21 = xaxis.Y;
        result.M22 = yaxis.Y;
        result.M23 = zaxis.Y;
        result.M24 = T.Zero;

        result.M31 = xaxis.Z;
        result.M32 = yaxis.Z;
        result.M33 = zaxis.Z;
        result.M34 = T.Zero;

        result.M41 = T.Zero - xaxis.Dot(eye);
        result.M42 = T.Zero - yaxis.Dot(eye);
        result.M43 = T.Zero - zaxis.Dot(eye);
        result.M44 = T.One;

        return result;
    }

    public static Matrix4x4<T> Perspective(T width, T height, T nearPlaneDistance, T farPlaneDistance)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(nearPlaneDistance, T.Zero);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(farPlaneDistance, T.Zero);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(nearPlaneDistance, farPlaneDistance);

        Unsafe.SkipInit(out Matrix4x4<T> result);

        var two = T.One + T.One;

        result.M11 = two * nearPlaneDistance / width;
        result.M12 = result.M13 = result.M14 = T.Zero;
        result.M22 = two * nearPlaneDistance / height;
        result.M21 = result.M23 = result.M24 = T.Zero;

        result.M33 = T.IsPositiveInfinity(farPlaneDistance) ? (-T.One) : (farPlaneDistance / (nearPlaneDistance - farPlaneDistance));

        result.M31 = result.M32 = T.Zero;
        result.M34 = -T.One;
        result.M41 = result.M42 = result.M44 = T.Zero;
        result.M43 = nearPlaneDistance * result.M33;

        return result;
    }

    public static Matrix4x4<T> PerspectiveFov(T fieldOfView, T aspectRatio, T nearPlaneDistance, T farPlaneDistance)
    {
        if (fieldOfView <= T.Zero || fieldOfView >= Math<T>.PI)
        {
            throw new ArgumentOutOfRangeException(nameof(fieldOfView));
        }

        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(nearPlaneDistance, T.Zero);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(farPlaneDistance, T.Zero);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(nearPlaneDistance, farPlaneDistance);

        var num = T.One / T.Tan(fieldOfView * Math<T>.Half);

        Unsafe.SkipInit(out Matrix4x4<T> result);

        result.M11 = num / aspectRatio;
        result.M12 = result.M13 = result.M14 = T.Zero;
        result.M22 = num;
        result.M21 = result.M23 = result.M24 = T.Zero;
        result.M31 = result.M32 = T.Zero;

        var zz = result.M33 = T.IsPositiveInfinity(farPlaneDistance) ? (-T.One) : (farPlaneDistance / (nearPlaneDistance - farPlaneDistance));

        result.M34 = -T.One;
        result.M41 = result.M42 = result.M44 = T.Zero;
        result.M43 = nearPlaneDistance * zz;

        return result;
    }

    public static Matrix4x4<T> Rotated(in Vector3<T> axis, T angle)
    {
        var sin = T.Sin(angle);
        var cos = T.Cos(angle);

        var xx  = axis.X * axis.X;
        var yy  = axis.Y * axis.Y;
        var zz  = axis.Z * axis.Z;
        var xy  = axis.X * axis.Y;
        var xz  = axis.X * axis.Z;
        var yz  = axis.Y * axis.Z;

        Unsafe.SkipInit(out Matrix4x4<T> result);

        result.M11 = xx + (cos * (T.One - xx));
        result.M12 = xy - (cos * xy) + (sin * axis.Z);
        result.M13 = xz - (cos * xz) - (sin * axis.Y);
        result.M14 = T.Zero;

        result.M21 = xy - (cos * xy) - (sin * axis.Z);
        result.M22 = yy + (cos * (T.One - yy));
        result.M23 = yz - (cos * yz) + (sin * axis.X);
        result.M24 = T.Zero;

        result.M31 = xz - (cos * xz) + (sin * axis.Y);
        result.M32 = yz - (cos * yz) - (sin * axis.X);
        result.M33 = zz + (cos * (T.One - zz));
        result.M34 = T.Zero;

        result.M14 = T.Zero;
        result.M24 = T.Zero;
        result.M34 = T.Zero;
        result.M44 = T.One;

        return result;
    }

    public readonly Matrix4x4<U> Cast<U>() where U : IFloatingPoint<U>, IFloatingPointIeee754<U>, IRootFunctions<U>, ITrigonometricFunctions<U> =>
        new(
            U.CreateChecked(this.M11), U.CreateChecked(this.M12), U.CreateChecked(this.M13), U.CreateChecked(this.M14),
            U.CreateChecked(this.M21), U.CreateChecked(this.M22), U.CreateChecked(this.M23), U.CreateChecked(this.M24),
            U.CreateChecked(this.M31), U.CreateChecked(this.M32), U.CreateChecked(this.M33), U.CreateChecked(this.M34),
            U.CreateChecked(this.M41), U.CreateChecked(this.M42), U.CreateChecked(this.M43), U.CreateChecked(this.M44)
        );

    public readonly Matrix4x4<T> Inverse()
    {
        var determinant = this.Determinant;

        if (Math<T>.IsZeroApprox(determinant))
        {
            return default;
        }

        Unsafe.SkipInit<Matrix4x4<T>>(out var result);

        var reciprocal = T.One / determinant;

        result.M11 = ((this.M22  * this.M33 * this.M44) - (this.M22 * this.M34 * this.M43) - this.M32 * this.M23 * this.M44 + this.M32 * this.M24 * this.M43 + this.M42 * this.M23 * this.M34 - this.M42 * this.M24 * this.M33) * reciprocal;
        result.M12 = ((-this.M12 * this.M33 * this.M44) + (this.M12 * this.M34 * this.M43) + this.M32 * this.M13 * this.M44 - this.M32 * this.M14 * this.M43 - this.M42 * this.M13 * this.M34 + this.M42 * this.M14 * this.M33) * reciprocal;
        result.M13 = ((this.M12  * this.M23 * this.M44) - (this.M12 * this.M24 * this.M43) - this.M22 * this.M13 * this.M44 + this.M22 * this.M14 * this.M43 + this.M42 * this.M13 * this.M24 - this.M42 * this.M14 * this.M23) * reciprocal;
        result.M14 = ((-this.M12 * this.M23 * this.M34) + (this.M12 * this.M24 * this.M33) + this.M22 * this.M13 * this.M34 - this.M22 * this.M14 * this.M33 - this.M32 * this.M13 * this.M24 + this.M32 * this.M14 * this.M23) * reciprocal;

        result.M21 = ((-this.M21 * this.M33 * this.M44) + (this.M21 * this.M34 * this.M43) + this.M31 * this.M23 * this.M44 - this.M31 * this.M24 * this.M43 - this.M41 * this.M23 * this.M34 + this.M41 * this.M24 * this.M33) * reciprocal;
        result.M22 = ((this.M11  * this.M33 * this.M44) - (this.M11 * this.M34 * this.M43) - this.M31 * this.M13 * this.M44 + this.M31 * this.M14 * this.M43 + this.M41 * this.M13 * this.M34 - this.M41 * this.M14 * this.M33) * reciprocal;
        result.M23 = ((-this.M11 * this.M23 * this.M44) + (this.M11 * this.M24 * this.M43) + this.M21 * this.M13 * this.M44 - this.M21 * this.M14 * this.M43 - this.M41 * this.M13 * this.M24 + this.M41 * this.M14 * this.M23) * reciprocal;
        result.M24 = ((this.M11  * this.M23 * this.M34) - (this.M11 * this.M24 * this.M33) - this.M21 * this.M13 * this.M34 + this.M21 * this.M14 * this.M33 + this.M31 * this.M13 * this.M24 - this.M31 * this.M14 * this.M23) * reciprocal;

        result.M31 = ((this.M21  * this.M32 * this.M44) - (this.M21 * this.M34 * this.M42) - this.M31 * this.M22 * this.M44 + this.M31 * this.M24 * this.M42 + this.M41 * this.M22 * this.M34 - this.M41 * this.M24 * this.M32) * reciprocal;
        result.M32 = ((-this.M11 * this.M32 * this.M44) + (this.M11 * this.M34 * this.M42) + this.M31 * this.M12 * this.M44 - this.M31 * this.M14 * this.M42 - this.M41 * this.M12 * this.M34 + this.M41 * this.M14 * this.M32) * reciprocal;
        result.M33 = ((this.M11  * this.M22 * this.M44) - (this.M11 * this.M24 * this.M42) - this.M21 * this.M12 * this.M44 + this.M21 * this.M14 * this.M42 + this.M41 * this.M12 * this.M24 - this.M41 * this.M14 * this.M22) * reciprocal;
        result.M34 = ((-this.M11 * this.M22 * this.M34) + (this.M11 * this.M24 * this.M32) + this.M21 * this.M12 * this.M34 - this.M21 * this.M14 * this.M32 - this.M31 * this.M12 * this.M24 + this.M31 * this.M14 * this.M22) * reciprocal;

        result.M41 = ((-this.M21 * this.M32 * this.M43) + (this.M21 * this.M33 * this.M42) + this.M31 * this.M22 * this.M43 - this.M31 * this.M23 * this.M42 - this.M41 * this.M22 * this.M33 + this.M41 * this.M23 * this.M32) * reciprocal;
        result.M42 = ((this.M11  * this.M32 * this.M43) - (this.M11 * this.M33 * this.M42) - this.M31 * this.M12 * this.M43 + this.M31 * this.M13 * this.M42 + this.M41 * this.M12 * this.M33 - this.M41 * this.M13 * this.M32) * reciprocal;
        result.M43 = ((-this.M11 * this.M22 * this.M43) + (this.M11 * this.M23 * this.M42) + this.M21 * this.M12 * this.M43 - this.M21 * this.M13 * this.M42 - this.M41 * this.M12 * this.M23 + this.M41 * this.M13 * this.M22) * reciprocal;
        result.M44 = ((this.M11  * this.M22 * this.M33) - (this.M11 * this.M23 * this.M32) - this.M21 * this.M12 * this.M33 + this.M21 * this.M13 * this.M32 + this.M31 * this.M12 * this.M23 - this.M31 * this.M13 * this.M22) * reciprocal;

        return result;
    }

    public readonly Matrix4x4<T> Transposed() =>
        new(
            this.M11, this.M21, this.M31, this.M41,
            this.M12, this.M22, this.M32, this.M42,
            this.M13, this.M23, this.M33, this.M43,
            this.M14, this.M24, this.M34, this.M44
        );

    public override readonly string ToString() =>
        string.Create(CultureInfo.InvariantCulture, $"[{this.M11}, {this.M12}, {this.M13}, {this.M14}], [{this.M21}, {this.M22}, {this.M23}, {this.M24}], [{this.M31}, {this.M32}, {this.M33}, {this.M34}], [{this.M41}, {this.M42}, {this.M43}, {this.M44}]");

    public static Matrix4x4<T> operator *(in Matrix4x4<T> a, in Matrix4x4<T> b)
    {
        Unsafe.SkipInit<Matrix4x4<T>>(out var c);

        c.M11 = (a.M11 * b.M11) + (a.M12 * b.M21) + (a.M13 * b.M31) + (a.M14 * b.M41);
        c.M12 = (a.M11 * b.M12) + (a.M12 * b.M22) + (a.M13 * b.M32) + (a.M14 * b.M42);
        c.M13 = (a.M11 * b.M13) + (a.M12 * b.M23) + (a.M13 * b.M33) + (a.M14 * b.M43);
        c.M14 = (a.M11 * b.M14) + (a.M12 * b.M24) + (a.M13 * b.M34) + (a.M14 * b.M44);

        c.M21 = (a.M21 * b.M11) + (a.M22 * b.M21) + (a.M23 * b.M31) + (a.M24 * b.M41);
        c.M22 = (a.M21 * b.M12) + (a.M22 * b.M22) + (a.M23 * b.M32) + (a.M24 * b.M42);
        c.M23 = (a.M21 * b.M13) + (a.M22 * b.M23) + (a.M23 * b.M33) + (a.M24 * b.M43);
        c.M24 = (a.M21 * b.M14) + (a.M22 * b.M24) + (a.M23 * b.M34) + (a.M24 * b.M44);

        c.M31 = (a.M31 * b.M11) + (a.M32 * b.M21) + (a.M33 * b.M31) + (a.M34 * b.M41);
        c.M32 = (a.M31 * b.M12) + (a.M32 * b.M22) + (a.M33 * b.M32) + (a.M34 * b.M42);
        c.M33 = (a.M31 * b.M13) + (a.M32 * b.M23) + (a.M33 * b.M33) + (a.M34 * b.M43);
        c.M34 = (a.M31 * b.M14) + (a.M32 * b.M24) + (a.M33 * b.M34) + (a.M34 * b.M44);

        c.M41 = (a.M41 * b.M11) + (a.M42 * b.M21) + (a.M43 * b.M31) + (a.M44 * b.M41);
        c.M42 = (a.M41 * b.M12) + (a.M42 * b.M22) + (a.M43 * b.M32) + (a.M44 * b.M42);
        c.M43 = (a.M41 * b.M13) + (a.M42 * b.M23) + (a.M43 * b.M33) + (a.M44 * b.M43);
        c.M44 = (a.M41 * b.M14) + (a.M42 * b.M24) + (a.M43 * b.M34) + (a.M44 * b.M44);

        return c;
    }
}
