using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Age.Numerics;

[DebuggerDisplay("X: {X}, Y: {Y}, Z: {Z}, W: {W}")]
public record struct Vector4<T> where T :  IFloatingPoint<T>, IRootFunctions<T>, ITrigonometricFunctions<T>
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

    public static implicit operator Vector2<T>(Vector4<T> vector) =>
        new(vector.X, vector.Y);

    public static implicit operator Vector3<T>(Vector4<T> vector) =>
        new(vector.X, vector.Y, vector.Z);

    public override readonly string ToString()
        => $"{{ X = {this.X}, Y = {this.Y}, Z = {this.Z}, W = {this.W} }}";
}
