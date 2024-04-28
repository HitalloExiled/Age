using Age.Numerics;

namespace Age.Rendering.Drawing.Styling;

public record struct Border
{
    public BorderSide   Top;
    public BorderSide   Right;
    public BorderSide   Bottom;
    public BorderSide   Left;
    public BorderRadius Radius;

    public Border(uint thickness, uint radius, Color color)
    {
        this.Top    = new(thickness, color);
        this.Right  = new(thickness, color);
        this.Bottom = new(thickness, color);
        this.Left   = new(thickness, color);
    }
}
