using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Age.Numerics;

[DebuggerDisplay("[{M11}, {M12}], [{M21}, {M22}], [{M31}, {M32}]")]
public record struct Matrix3x2<T> where T : IFloatingPoint<T>, IFloatingPointIeee754<T>, IRootFunctions<T>, ITrigonometricFunctions<T>
{
    public static Matrix3x2<T> Identity => new(Vector3<T>.Right, Vector3<T>.Up, Vector3<T>.Front);

    public T M11, M12;
    public T M21, M22;
    public T M31, M32;

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
        get => Vector2.Angle(Vector3<T>.Right, Unsafe.As<T, Vector2<T>>(ref this.M11).Normalized());
        set => ApplyRotatation(ref this, value, this.Scale, default);
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

            x = x.Normalized() * value.X;
            y = y.Normalized() * value.Y;
        }
    }

    public Vector2<T> Skewing
    {
        readonly get => new(T.Atan(this.M21), T.Atan(this.M12));
        set => ApplySkew(ref this, value, default);
    }

    public Vector2<T> Translation
    {
        get => Unsafe.As<T, Vector2<T>>(ref this.M31);
        set => Unsafe.As<T, Vector2<T>>(ref this.M31) = value;
    }

    public readonly T Determinant => this.M11 * this.M22 - this.M21 * this.M12;

    public readonly bool IsIdentity =>
        this.M11 == T.One  &&
        this.M12 == T.Zero &&
        this.M21 == T.Zero &&
        this.M22 == T.One  &&
        this.M31 == T.Zero &&
        this.M32 == T.Zero;

    public Matrix3x2(in Vector2<T> column1, in Vector2<T> column2, in Vector2<T> column3)
    {
        this.M11 = column1.X;
        this.M12 = column1.Y;
        this.M21 = column2.X;
        this.M22 = column2.Y;
        this.M31 = column3.X;
        this.M32 = column3.Y;
    }

    public Matrix3x2(in Vector2<T> translation, T rotation, in Vector2<T> scale)
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

    private static Matrix3x2<T> ApplyRotatation(ref Matrix3x2<T> matrix, T radians, in Vector2<T> scale, in Vector2<T> origin)
    {
        var fradians = MathF.IEEERemainder(float.CreateChecked(radians), (float)Math.PI * 2f);

        T cos;
        T sin;

        if (fradians > -1.7453294E-05f && fradians < 1.7453294E-05f)
        {
            cos = T.One;
            sin = T.Zero;
        }
        else if (fradians > 1.570779f && fradians < 1.5708138f)
        {
            cos = T.Zero;
            sin = T.One;
        }
        else if (fradians < -3.1415753f || fradians > 3.1415753f)
        {
            cos = -T.One;
            sin = T.Zero;
        }
        else if (fradians > -1.5708138f && fradians < -1.570779f)
        {
            cos = T.Zero;
            sin = -T.One;
        }
        else
        {
            var t = T.CreateChecked(fradians);

            cos = T.Cos(t);
            sin = T.Sin(t);
        }

        var translationX = origin.X * (T.One - cos) + origin.Y * sin;
        var translationY = origin.Y * (T.One - cos) - origin.X * sin;

        matrix.M11 = cos            * scale.X;
        matrix.M12 = sin            * scale.X;
        matrix.M21 = (T.Zero - sin) * scale.Y;
        matrix.M22 = cos            * scale.Y;
        matrix.M31 += translationX;
        matrix.M32 += translationY;

        return matrix;
    }

    public static void ApplySkew(ref Matrix3x2<T> matrix, in Vector2<T> radians, in Vector2<T> origin)
    {
        var skewX = T.Tan(radians.X);
        var skewY = T.Tan(radians.Y);

        var x = (T.Zero - origin.Y) * skewX;
        var y = (T.Zero - origin.X) * skewY;

        matrix.M12 = skewY;
        matrix.M21 = skewX;
        matrix.M31 += x;
        matrix.M32 += y;
    }

    public static Matrix3x2<T> CreateRotated(T radians) =>
        CreateRotated(radians, default);

    public static Matrix3x2<T> CreateRotated(T radians, in Vector2<T> origin)
    {
        Unsafe.SkipInit(out Matrix3x2<T> matrix);

        ApplyRotatation(ref matrix, radians, Vector2<T>.One, origin);

        return matrix;
    }

    public static Matrix3x2<T> CreateScaled(in Vector2<T> scale) =>
        CreateScaled(scale.X, scale.Y);

    public static Matrix3x2<T> CreateScaled(T x, T y)
    {
        Unsafe.SkipInit(out Matrix3x2<T> matrix);

        matrix.M11 = x;
        matrix.M12 = T.Zero;
        matrix.M21 = T.Zero;
        matrix.M22 = y;
        matrix.M31 = T.Zero;
        matrix.M32 = T.Zero;

        return matrix;
    }

    public static Matrix3x2<T> CreateSkewed(T radiansX, T radiansY, Vector2<T> origin) =>
        CreateSkewed(new(radiansX, radiansY), origin);

    public static Matrix3x2<T> CreateSkewed(in Vector2<T> radians, Vector2<T> origin)
    {
        Unsafe.SkipInit(out Matrix3x2<T> matrix);

        matrix.M11 = T.One;
        matrix.M22 = T.One;

        ApplySkew(ref matrix, radians, origin);

        return matrix;
    }

    public static Matrix3x2<T> CreateTranslated(in Vector2<T> translation) =>
        CreateTranslated(translation.X, translation.Y);

    public static Matrix3x2<T> CreateTranslated(T x, T y)
    {
        Unsafe.SkipInit(out Matrix3x2<T> matrix);

        matrix.M11 = T.One;
        matrix.M12 = T.Zero;
        matrix.M21 = T.Zero;
        matrix.M22 = T.One;
        matrix.M31 = x;
        matrix.M32 = y;

        return matrix;
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

    public readonly bool IsAprox(Matrix3x2<T> other) =>
        MathX.IsApprox(this.M11, other.M11)
        && MathX.IsApprox(this.M12, other.M12)
        && MathX.IsApprox(this.M21, other.M21)
        && MathX.IsApprox(this.M22, other.M22)
        && MathX.IsApprox(this.M31, other.M31)
        && MathX.IsApprox(this.M32, other.M32);

    public void Rotate(T radians) =>
        ApplyRotatation(ref this, radians, this.Scale, default);

    public void Rotate(T radians, in Vector2<T> origin) =>
        ApplyRotatation(ref this, radians, this.Scale, origin);

    public void Skew(T radiansX, T radiansY, in Vector2<T> origin) =>
        ApplySkew(ref this, new(radiansX, radiansY), origin);

    public void Skew(in Vector2<T> radians, in Vector2<T> origin) =>
        ApplySkew(ref this, radians, origin);

    public override readonly string ToString() =>
        $"[{this.M11}, {this.M12}], [{this.M21}, {this.M22}], [{this.M31}, {this.M32}]";

    public static Matrix3x2<T> operator *(in Matrix3x2<T> left, in Matrix3x2<T> right)
    {
        Unsafe.SkipInit(out Matrix3x2<T> result);

        result.M11 = left.M11 * right.M11 + left.M12 * right.M21;
        result.M12 = left.M11 * right.M12 + left.M12 * right.M22;
	    result.M21 = left.M21 * right.M11 + left.M22 * right.M21;
        result.M22 = left.M21 * right.M12 + left.M22 * right.M22;
	    result.M31 = left.M31 * right.M11 + left.M32 * right.M21 + right.M31;
        result.M32 = left.M31 * right.M12 + left.M32 * right.M22 + right.M32;

        return result;
    }
}
