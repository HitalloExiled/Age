using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Age.Numerics;

[DebuggerDisplay("[{Row1}], [{Row2}], [{Row3}], [{Row4}]")]
public struct Matrix4x4<T> where T : IFloatingPoint<T>, IRootFunctions<T>, ITrigonometricFunctions<T>
{
    public static Matrix4x4<T> Identity => new(
        new(T.One,  T.Zero, T.Zero, T.Zero),
        new(T.Zero, T.One,  T.Zero, T.Zero),
        new(T.Zero, T.Zero, T.One,  T.Zero),
        new(T.Zero, T.Zero, T.Zero, T.One)
    );

    public Vector4<T> Row1;
    public Vector4<T> Row2;
    public Vector4<T> Row3;
    public Vector4<T> Row4;

    public Vector4<T> this[int index]
    {
        get
        {
            Common.ThrowIfOutOfRange(0, 3, index);

            return Unsafe.Add(ref this.Row1, index);
        }

        set
        {
            Common.ThrowIfOutOfRange(0, 3, index);

            Unsafe.Add(ref this.Row1, index) = value;
        }
    }

    public T this[int row, int column]
    {
        get
        {
            Common.ThrowIfOutOfRange(0, 3, row);
            Common.ThrowIfOutOfRange(0, 3, column);

            return Unsafe.Add(ref this.Row1, row)[column];
        }

        set
        {
            Common.ThrowIfOutOfRange(0, 3, row);
            Common.ThrowIfOutOfRange(0, 3, column);

            Unsafe.Add(ref this.Row1, row)[column] = value;
        }
    }

    public readonly bool IsIdentity =>
        this.Row1[0] == T.One &&
        this.Row2[1] == T.One &&
        this.Row3[2] == T.One &&
        this.Row4[3] == T.One &&
        this.Row1[1] == T.Zero &&
        this.Row1[2] == T.Zero &&
        this.Row1[3] == T.Zero &&
        this.Row2[0] == T.Zero &&
        this.Row2[2] == T.Zero &&
        this.Row2[3] == T.Zero &&
        this.Row3[0] == T.Zero &&
        this.Row3[1] == T.Zero &&
        this.Row3[3] == T.Zero &&
        this.Row4[0] == T.Zero &&
        this.Row4[1] == T.Zero &&
        this.Row4[2] == T.Zero;

    public Matrix4x4(T value)
    {
        this.Row1 = new(value, T.Zero, T.Zero, T.Zero);
        this.Row2 = new(T.Zero, value, T.Zero, T.Zero);
        this.Row3 = new(T.Zero, T.Zero, value, T.Zero);
        this.Row4 = new(T.Zero, T.Zero, T.Zero, value);
    }

    public Matrix4x4(Vector4<T> row1, Vector4<T> row2, Vector4<T> row3, Vector4<T> row4)
    {
        this.Row1 = row1;
        this.Row2 = row2;
        this.Row3 = row3;
        this.Row4 = row4;
    }

    public static Matrix4x4<T> LookAt(Vector3<T> cameraPosition, Vector3<T> cameraTarget, Vector3<T> cameraUpVector)
    {
        var forward = (cameraPosition - cameraTarget).Normalized;
        var side    = cameraUpVector.Cross(forward).Normalized;
        var up      = forward.Cross(side);

        var identity = Identity;

        identity[0, 0] = side.X;
        identity[0, 1] = up.X;
        identity[0, 2] = forward.X;
        identity[1, 0] = side.Y;
        identity[1, 1] = up.Y;
        identity[1, 2] = forward.Y;
        identity[2, 0] = side.Z;
        identity[2, 1] = up.Z;
        identity[2, 2] = forward.Z;
        identity[3, 0] = T.Zero - side.Dot(cameraPosition);
        identity[3, 1] = T.Zero - up.Dot(cameraPosition);
        identity[3, 2] = T.Zero - forward.Dot(cameraPosition);

        return identity;
    }

    public static Matrix4x4<T> Perspective(T width, T height, T nearPlaneDistance, T farPlaneDistance)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(nearPlaneDistance, T.Zero);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(farPlaneDistance, T.Zero);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(nearPlaneDistance, farPlaneDistance);

        var result = new Matrix4x4<T>();

        var two = T.CreateChecked(2);

        result[0, 0] = two * nearPlaneDistance / width;
        result[0, 1] = result[0, 2] = result[0, 3] = T.Zero;
        result[1, 1] = two * nearPlaneDistance / height;
        result[1, 0] = result[1, 2] = result[1, 3] = T.Zero;

        var num = result[2, 2] = T.IsPositiveInfinity(farPlaneDistance) ? (-T.One) : (farPlaneDistance / (nearPlaneDistance - farPlaneDistance));

        result[2, 0] = result[2, 1] = T.Zero;
        result[2, 3] = -T.One;
        result[3, 0] = result[3, 1] = result[3, 3] = T.Zero;
        result[3, 2] = nearPlaneDistance * num;

        return result;
    }

    public readonly Matrix4x4<T> Rotate(Vector3<T> axis, T angle)
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

        identity[0, 0] = xx + cos * (T.One - xx);
        identity[0, 1] = xy - cos * xy + sin * z;
        identity[0, 2] = xz - cos * xz - sin * y;
        identity[1, 0] = xy - cos * xy - sin * z;
        identity[1, 1] = yy + cos * (T.One - yy);
        identity[1, 2] = yz - cos * yz + sin * x;
        identity[2, 0] = xz - cos * xz + sin * y;
        identity[2, 1] = yz - cos * yz - sin * x;
        identity[2, 2] = zz + cos * (T.One - zz);

        return identity;
    }
}
