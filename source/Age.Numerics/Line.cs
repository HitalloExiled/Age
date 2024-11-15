using System.Numerics;

namespace Age.Numerics;

public record struct Line<T> where T : IFloatingPoint<T>, IFloatingPointIeee754<T>, IRootFunctions<T>, ITrigonometricFunctions<T>, IPowerFunctions<T>
{
    public Vector2<T> A;
    public Vector2<T> B;

    public readonly T Lenght => (this.A - this.B).Length;

    public Line() { }

    public Line(in Vector2<T> a, in Vector2<T> b)
    {
        this.A = a;
        this.B = b;
    }

    public override readonly string ToString() =>
        $"{{ A: {this.A}, B: {this.B} }}";

    public readonly T CrossProduct(in Vector2<T> point) =>
        Vector2<T>.CrossProduct(point - this.A, this.B - this.A);

    public readonly T DistanceTo(in Vector2<T> point) =>
        T.Abs(this.CrossProduct(point)) / this.Lenght;
}
