using Age.Numerics;

namespace Age.Styling;

public record StyleRectEdges
{
    public Unit? Top    { get; init; }
    public Unit? Right  { get; init; }
    public Unit? Bottom { get; init; }
    public Unit? Left   { get; init; }

    public StyleRectEdges() { }

    public StyleRectEdges(Unit? top, Unit? right, Unit? bottom, Unit? left)
    {
        this.Top    = top;
        this.Right  = right;
        this.Bottom = bottom;
        this.Left   = left;
    }

    public StyleRectEdges(Unit? horizontal, Unit? vertical) : this(vertical, horizontal, vertical, horizontal)
    { }

    public StyleRectEdges(Unit? value) : this(value, value, value, value)
    { }
}
