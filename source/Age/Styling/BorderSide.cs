using Age.Numerics;

namespace Age.Styling;

public record struct BorderSide
{
    public uint  Thickness;
    public Color Color;

    public BorderSide(uint thickness, Color color)
    {
        this.Thickness = thickness;
        this.Color     = color;
    }

    public override readonly string ToString() =>
        $"[{this.Thickness}, ${this.Color}]";
}
