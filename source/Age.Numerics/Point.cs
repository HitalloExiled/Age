using System.Diagnostics;
using System.Numerics;

namespace Age.Numerics;

[DebuggerDisplay("X: {X}, Y: {Y}")]
public record struct Point<T> where T : INumber<T>
{
    public T X;
    public T Y;

    public Point(T x, T y)
    {
        this.X = x;
        this.Y = y;
    }

    public readonly Point<U> Cast<U>() where U : INumber<U> =>
        new(U.CreateChecked(this.X), U.CreateChecked(this.Y));
}
