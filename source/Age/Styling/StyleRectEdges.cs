namespace Age.Styling;

public record struct StyleRectEdges
{
    public Unit? Top;
    public Unit? Right;
    public Unit? Bottom;
    public Unit? Left;

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
