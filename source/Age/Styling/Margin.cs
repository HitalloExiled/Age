namespace Age.Styling;

public record struct Margin
{
    public Unit? Top;
    public Unit? Right;
    public Unit? Bottom;
    public Unit? Left;

    public Margin(Unit? top, Unit? right, Unit? bottom, Unit? left)
    {
        this.Top    = top;
        this.Right  = right;
        this.Bottom = bottom;
        this.Left   = left;
    }

    public Margin(Unit? horizontal, Unit? vertical) : this(vertical, horizontal, vertical, horizontal)
    { }

    public Margin(Unit? value) : this(value, value, value, value)
    { }
}
