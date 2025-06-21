using System.Globalization;
using System.Numerics;

namespace Age.Numerics;

public record struct Vector4<T> where T : IFloatingPoint<T>, IFloatingPointIeee754<T>, IRootFunctions<T>, ITrigonometricFunctions<T>
{
    public T X;
    public T Y;
    public T Z;
    public T W;

    public T this[int index]
    {
        readonly get => index switch
        {
            0 => this.X,
            1 => this.Y,
            2 => this.Z,
            3 => this.W,
            _ => throw new IndexOutOfRangeException(),
        };
        set
        {
            switch (index)
            {
                case 0: this.X = value; break;
                case 1: this.Y = value; break;
                case 2: this.Z = value; break;
                case 3: this.W = value; break;
                default: throw new IndexOutOfRangeException();
            }
        }
    }

    public Vector4(T x, T y, T z, T w)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
        this.W = w;
    }

    public override readonly string ToString() =>
        string.Create(CultureInfo.InvariantCulture, $"{{ X: {this.X}, Y: {this.Y}, Z: {this.Z}, W: {this.W} }}");

    public readonly Vector2<T> ToVector2() =>
        new(this.X, this.Y);

    public readonly Vector3<T> ToVector3() =>
        new(this.X, this.Y, this.Z);
}
