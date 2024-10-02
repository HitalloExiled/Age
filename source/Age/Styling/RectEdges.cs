namespace Age.Styling;

public record struct RectEdges
{
    public Unit? Top;
    public Unit? Right;
    public Unit? Bottom;
    public Unit? Left;

    public RectEdges(Unit? top, Unit? right, Unit? bottom, Unit? left)
    {
        this.Top    = top;
        this.Right  = right;
        this.Bottom = bottom;
        this.Left   = left;
    }

    public RectEdges(Unit? horizontal, Unit? vertical) : this(vertical, horizontal, vertical, horizontal)
    { }

    public RectEdges(Unit? value) : this(value, value, value, value)
    { }
}
