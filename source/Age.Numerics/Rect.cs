using System.Diagnostics;
using System.Numerics;

namespace Age.Numerics;

[DebuggerDisplay("\\{ Size: {Size}, Position: {Position} \\}")]
public record struct Rect<T> where T : INumber<T>
{
    public Size<T>  Size;
    public Point<T> Position;

    public Rect(Size<T> size, Point<T> position)
    {
        this.Size     = size;
        this.Position = position;
    }
}
