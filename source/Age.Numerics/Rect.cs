using System.Numerics;

namespace Age.Numerics;

public record struct Rect<T> where T : IFloatingPoint<T>
{
    public Size<T>  Size;
    public Point<T> Position;

    public Rect(Size<T> size, Point<T> position)
    {
        this.Size     = size;
        this.Position = position;
    }
}
