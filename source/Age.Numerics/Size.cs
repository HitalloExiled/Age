using System.Diagnostics;
using System.Numerics;

namespace Age.Numerics;

[DebuggerDisplay("\\{ Width: {Width}, Height: {Height} \\}")]
public record struct Size<T> where T : INumber<T>
{
    public T Height;
    public T Width;

    public Size(T width, T height)
    {
        this.Width  = width;
        this.Height = height;
    }
}
