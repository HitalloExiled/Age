using System.Numerics;

namespace Age.Numerics;

public record struct Line<T> where T : IFloatingPoint<T>, IRootFunctions<T>, ITrigonometricFunctions<T>, IPowerFunctions<T>
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
        $"{{ Head: {this.A}, Tail: {this.B} }}";

    public readonly T CrossProduct(in Vector2<T> point) =>
        (point.X - this.A.X) * (this.B.Y - this.A.Y) - (point.Y - this.A.Y) * (this.B.X - this.A.X);

    public readonly T DistanceTo(in Vector2<T> point)
    {
        var two = T.One + T.One;

        var numerator   = T.Abs(this.CrossProduct(point));
        var denominator = T.Sqrt(T.Pow(this.B.Y - this.A.Y, two) + T.Pow(this.B.X - this.A.X, two));

        return numerator / denominator;
    }
}
