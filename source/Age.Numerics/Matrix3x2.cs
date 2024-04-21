using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Age.Numerics;

[DebuggerDisplay("[{M11}, {M12}], [{M21}, {M22}], [{M31}, {M32}]")]
public record struct Matrix3x2<T> where T : IFloatingPoint<T>, IFloatingPointIeee754<T>, IRootFunctions<T>, ITrigonometricFunctions<T>
{
    public static Matrix3x2<T> Identity => new(Vector3<T>.Right, Vector3<T>.Up, Vector3<T>.Front);

    public T M11;
    public T M12;
    public T M21;
    public T M22;
    public T M31;
    public T M32;

    public T this[int column, int row]
    {
        get
        {
            Common.ThrowIfOutOfRange(0, 2, column);
            Common.ThrowIfOutOfRange(0, 2, row);

		    return Unsafe.Add(ref this.M11, column * 3 + row);
        }
        set
        {
            Common.ThrowIfOutOfRange(0, 2, row);
            Common.ThrowIfOutOfRange(0, 2, column);

            Unsafe.Add(ref this.M11, column * 3 + row) = value;
        }
    }

    public Vector2<T> X
    {
        get => Unsafe.As<T, Vector2<T>>(ref this.M11);
        set => Unsafe.As<T, Vector2<T>>(ref this.M11) = value;
    }

    public Vector2<T> Y
    {
        get => Unsafe.As<T, Vector2<T>>(ref this.M21);
        set => Unsafe.As<T, Vector2<T>>(ref this.M21) = value;
    }

    public Vector2<T> Z
    {
        get => Unsafe.As<T, Vector2<T>>(ref this.M31);
        set => Unsafe.As<T, Vector2<T>>(ref this.M31) = value;
    }

    public T Rotation
    {
        get => Vector2.Angle(Unsafe.As<T, Vector2<T>>(ref this.M11).Normalized, Vector3<T>.Right);
        set
        {
            var scale = this.Scale;

            ref var x = ref Unsafe.As<T, Vector2<T>>(ref this.M11);
            ref var y = ref Unsafe.As<T, Vector2<T>>(ref this.M21);

            var cos = T.Cos(value);
            var sin = T.Sin(value);

            x = new Vector2<T>(cos,  sin) * scale.X;
            y = new Vector2<T>(-sin, cos) * scale.Y;
        }
    }

    public Vector2<T> Scale
    {
        get
        {
            ref var x = ref Unsafe.As<T, Vector2<T>>(ref this.M11);
            ref var y = ref Unsafe.As<T, Vector2<T>>(ref this.M21);

            return new(x.Length, y.Length);
        }
        set
        {
            ref var x = ref Unsafe.As<T, Vector2<T>>(ref this.M11);
            ref var y = ref Unsafe.As<T, Vector2<T>>(ref this.M21);

            x = x.Normalized * value.X;
            y = y.Normalized * value.Y;
        }
    }

    public Vector2<T> Translation
    {
        get => Unsafe.As<T, Vector2<T>>(ref this.M31);
        set => Unsafe.As<T, Vector2<T>>(ref this.M31) = value;
    }

    public readonly T Determinant => this.M11 * this.M22 - this.M21 * this.M12;

    public readonly bool IsIdentity =>
        this.M11 == T.One  &&
        this.M22 == T.One  &&
        this.M21 == T.Zero &&
        this.M31 == T.Zero &&
        this.M12 == T.Zero &&
        this.M32 == T.Zero;

    public Matrix3x2(Vector2<T> column1, Vector2<T> column2, Vector2<T> column3)
    {
        this.M11 = column1.X;
        this.M12 = column1.Y;
        this.M21 = column2.X;
        this.M22 = column2.Y;
        this.M31 = column3.X;
        this.M32 = column3.Y;
    }

    public Matrix3x2(Vector2<T> translation, T rotation, Vector2<T> scale)
    {
        Unsafe.SkipInit(out this);

        ref var x = ref Unsafe.As<T, Vector2<T>>(ref this.M11);
        ref var y = ref Unsafe.As<T, Vector2<T>>(ref this.M21);
        ref var z = ref Unsafe.As<T, Vector2<T>>(ref this.M31);

        var cos = T.Cos(rotation);
        var sin = T.Sin(rotation);

        x = new Vector2<T>(cos,  sin) * scale.X;
        y = new Vector2<T>(-sin, cos) * scale.Y;
        z = translation;
    }

    public readonly Matrix3x2<T> Inverse()
    {
        var determinant = this.Determinant;

        if (MathX.IsZeroApprox(determinant))
        {
            return default;
        }

        Unsafe.SkipInit<Matrix3x2<T>>(out var result);

        var reciprocal = T.One / determinant;

        result.M11 = this.M22  * reciprocal;
        result.M12 = -this.M12 * reciprocal;
        result.M21 = -this.M21 * reciprocal;
        result.M22 = this.M11  * reciprocal;
        result.M31 = -(this.M31 * result.M11 + this.M32 * result.M21);
        result.M32 = -(this.M31 * result.M12 + this.M32 * result.M22);

        return result;
    }

    public override readonly string ToString() =>
        $"[{this.M11}, {this.M12}], [{this.M21}, {this.M22}], [{this.M31}, {this.M32}]";

    public static Matrix3x2<T> operator *(in Matrix3x2<T> a, in Matrix3x2<T> b)
    {
        Unsafe.SkipInit<Matrix3x2<T>>(out var c);

        c.M11 = a.M11 * b.M11 + a.M21 * b.M12;
        c.M12 = a.M12 * b.M11 + a.M22 * b.M12;
        c.M21 = a.M11 * b.M21 + a.M21 * b.M22;
        c.M22 = a.M12 * b.M21 + a.M22 * b.M22;
        c.M31 = a.M11 * b.M31 + a.M21 * b.M32 + a.M31;
        c.M32 = a.M12 * b.M31 + a.M22 * b.M32 + a.M32;

        return c;
    }
}
