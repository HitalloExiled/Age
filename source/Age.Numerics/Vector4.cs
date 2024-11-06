using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Age.Numerics;

public record struct Vector4<T> where T : IFloatingPoint<T>, IFloatingPointIeee754<T>, IRootFunctions<T>, ITrigonometricFunctions<T>
{
    public T X;
    public T Y;
    public T Z;
    public T W;

    public Vector4(T x, T y, T z, T w)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
        this.W = w;
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

    public override readonly string ToString() =>
        string.Create(CultureInfo.InvariantCulture, $"{{ X: {this.X}, Y: {this.Y}, Z: {this.Z}, W: {this.W} }}");

    public readonly Vector2<T> ToVector2() =>
        new(this.X, this.Y);

    public readonly Vector3<T> ToVector3() =>
        new(this.X, this.Y, this.Z);
}
