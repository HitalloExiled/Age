using Age.Numerics;

namespace Age.Drawing.Styling;

public record struct BorderSide
{
    public uint  Thickness;
    public Color Color;

    public BorderSide(uint thickness, Color color)
    {
        this.Thickness = thickness;
        this.Color     = color;
    }
}
