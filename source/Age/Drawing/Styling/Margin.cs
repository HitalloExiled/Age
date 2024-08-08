namespace Age.Drawing.Styling;

public record struct Margin
{
    public uint Top;
    public uint Right;
    public uint Bottom;
    public uint Left;

    public readonly uint Horizontal => this.Left + this.Right;
    public readonly uint Vertical   => this.Top + this.Bottom;

    public Margin(uint top, uint right, uint bottom, uint left)
    {
        this.Top    = top;
        this.Right  = right;
        this.Bottom = bottom;
        this.Left   = left;
    }

    public Margin(uint horizontal, uint vertical) : this(vertical, horizontal, vertical, horizontal)
    { }

    public Margin(uint value) : this(value, value, value, value)
    { }
}
