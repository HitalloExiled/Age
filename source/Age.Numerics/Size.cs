using System.Diagnostics;
using System.Numerics;

namespace Age.Numerics;

[DebuggerDisplay("\\{ Width: {Width}, Height: {Height} \\}")]
public record struct Size<T> where T : INumber<T>
{
    public T Width;
    public T Height;

    public Size(T width, T height)
    {
        this.Width  = width;
        this.Height = height;
    }

    public readonly Size<U> Cast<U>() where U : INumber<U> =>
        new(U.CreateChecked(this.Width), U.CreateChecked(this.Height));
}
