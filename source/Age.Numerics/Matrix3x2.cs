using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Age.Numerics;

public record struct Matrix3x2<T> where T : IFloatingPoint<T>, IFloatingPointIeee754<T>, IRootFunctions<T>, ITrigonometricFunctions<T>
{
    public static Matrix3x2<T> Identity => new(Vector3<T>.Right, Vector3<T>.Up, Vector3<T>.Front);

    public T M11, M12;
    public T M21, M22;
    public T M31, M32;

    public T this[int column, int row]
    {
        readonly get => (row * 2 + column) switch
        {
            0 => this.M11,
            1 => this.M12,
            2 => this.M21,
            3 => this.M22,
            4 => this.M31,
            5 => this.M32,
            _ => throw new IndexOutOfRangeException(),
        };
        set
        {
            switch (row * 2 + column)
            {
                case 0: this.M11 = value; break;
                case 1: this.M12 = value; break;
                case 2: this.M21 = value; break;
                case 3: this.M22 = value; break;
                case 4: this.M31 = value; break;
                case 5: this.M32 = value; break;
                default: throw new IndexOutOfRangeException();
            }
        }
    }

    public Vector2<T> X
    {
        readonly get => new(this.M11, this.M12);
        set => Unsafe.As<T, Vector2<T>>(ref this.M11) = value;
    }

    public Vector2<T> Y
    {
        readonly get => new(this.M21, this.M22);
        set => Unsafe.As<T, Vector2<T>>(ref this.M21) = value;
    }

    public Vector2<T> Z
    {
        readonly get => new(this.M31, this.M32);
        set => Unsafe.As<T, Vector2<T>>(ref this.M31) = value;
    }

    public T Rotation
    {
        readonly get
        {
            var scale = this.Scale;

            return T.Atan2(this.M21 / scale.Y, this.M11 / scale.X);
        }
        set
        {
            var scale = this.Scale;

            var cos = T.Cos(value);
            var sin = T.Sin(value);

            this.M11 = cos  * scale.X;
            this.M12 = -sin * scale.X;
            this.M21 = sin  * scale.Y;
            this.M22 = cos  * scale.Y;
        }
    }

    public Vector2<T> Scale
    {
        readonly get => new(this.X.Length, this.Y.Length);
        set
        {
            ref var x = ref Unsafe.As<T, Vector2<T>>(ref this.M11);
            ref var y = ref Unsafe.As<T, Vector2<T>>(ref this.M21);

            x = x.Normalized() * value;
            y = y.Normalized() * value;
        }
    }

    public Vector2<T> Translation
    {
        readonly get => this.Z;
        set => Unsafe.As<T, Vector2<T>>(ref this.M31) = value;
    }

    public readonly T Determinant => this.M11 * this.M22 - this.M12 * this.M21;

    public readonly bool IsIdentity =>
        this.M11 == T.One  &&
        this.M12 == T.Zero &&
        this.M21 == T.Zero &&
        this.M22 == T.One  &&
        this.M31 == T.Zero &&
        this.M32 == T.Zero;

    public readonly bool IsOrthonormalized => this.M11 * this.M21 + this.M12 * this.M22 == T.Zero;

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

        var cos = T.Cos(rotation);
        var sin = T.Sin(rotation);

        this.M11 = cos  * scale.X;
        this.M12 = -sin * scale.Y;
        this.M21 = sin  * scale.X;
        this.M22 = cos  * scale.Y;
        this.M31 = translation.X;
        this.M32 = translation.Y;
    }

    public static Matrix3x2<T> CreateRotated(T radians) =>
        CreateRotated(radians, default);

    public static Matrix3x2<T> CreateRotated(T radians, in Vector2<T> origin)
    {
        Unsafe.SkipInit(out Matrix3x2<T> matrix);

        var cos = T.Cos(radians);
        var sin = T.Sin(radians);

        var translationX = origin.X * (T.One - cos) + origin.Y * sin;
        var translationY = origin.Y * (T.One - cos) - origin.X * sin;

        matrix.M11 = cos;
        matrix.M12 = -sin;
        matrix.M21 = sin;
        matrix.M22 = cos;
        matrix.M31 += translationX;
        matrix.M32 += translationY;

        return matrix;
    }

    public static Matrix3x2<T> CreateScaled(T scale) =>
        CreateScaled(new Vector2<T>(scale));

    public static Matrix3x2<T> CreateScaled(T scaleX, T scaleY) =>
        CreateScaled(new Vector2<T>(scaleX, scaleY));

    public static Matrix3x2<T> CreateScaled(in Vector2<T> scale)
    {
        Unsafe.SkipInit(out Matrix3x2<T> matrix);

        matrix.M11 = scale.X;
        matrix.M12 = T.Zero;
        matrix.M21 = T.Zero;
        matrix.M22 = scale.Y;
        matrix.M31 = T.Zero;
        matrix.M32 = T.Zero;

        return matrix;
    }

    public static Matrix3x2<T> CreateSkewed(T radiansX, T radiansY) =>
        CreateSkewed(radiansX, radiansY, default);

    public static Matrix3x2<T> CreateSkewed(in Vector2<T> radians) =>
        CreateSkewed(radians, default);

    public static Matrix3x2<T> CreateSkewed(T radiansX, T radiansY, Vector2<T> origin) =>
        CreateSkewed(new(radiansX, radiansY), origin);

    public static Matrix3x2<T> CreateSkewed(in Vector2<T> radians, Vector2<T> origin)
    {
        Unsafe.SkipInit(out Matrix3x2<T> matrix);

        var skewX = T.Tan(radians.X);
        var skewY = T.Tan(radians.Y);

        var x = (T.Zero - origin.Y) * radians.X;
        var y = (T.Zero - origin.X) * radians.Y;

        matrix.M11 = T.One;
        matrix.M12 = skewY;
        matrix.M21 = skewX;
        matrix.M22 = T.One;
        matrix.M31 += x;
        matrix.M32 += y;

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

        if (Math<T>.IsZeroApprox(determinant))
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
        Math<T>.IsApprox(this.M11, other.M11)
        && Math<T>.IsApprox(this.M12, other.M12)
        && Math<T>.IsApprox(this.M21, other.M21)
        && Math<T>.IsApprox(this.M22, other.M22)
        && Math<T>.IsApprox(this.M31, other.M31)
        && Math<T>.IsApprox(this.M32, other.M32);

    public void Rotate(T radians) =>
        this.Rotate(radians, default);

    public void Rotate(T radians, in Vector2<T> origin)
    {
        var cos = T.Cos(radians);
        var sin = T.Sin(radians);

        var translationX = origin.X * (T.One - cos) + origin.Y * sin;
        var translationY = origin.Y * (T.One - cos) - origin.X * sin;

        var scale = this.Scale;

        this.M11 = cos  * scale.X;
        this.M12 = -sin * scale.Y;
        this.M21 = sin  * scale.X;
        this.M22 = cos  * scale.Y;
        this.M31 += translationX;
        this.M32 += translationY;
    }

    public override readonly string ToString() =>
        string.Create(CultureInfo.InvariantCulture, $"[{this.M11}, {this.M12}], [{this.M21}, {this.M22}], [{this.M31}, {this.M32}]");

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

    public static Vector2<T> operator *(in Matrix3x2<T> matrix, in Vector2<T> vector)
    {
        Unsafe.SkipInit(out Vector2<T> result);

        result.X = vector.X * matrix.M11 + vector.Y * matrix.M21 + matrix.M31;
        result.Y = vector.X * matrix.M12 + vector.Y * matrix.M22 + matrix.M32;

        return result;
    }
}
