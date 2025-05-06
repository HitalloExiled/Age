using Age.Numerics;

namespace Age.Styling;

public record struct BorderSide
{
    public Color Color;
    public ushort  Thickness;

    public BorderSide(ushort thickness, Color color)
    {
        this.Thickness = thickness;
        this.Color     = color;
    }

    public override readonly string ToString() =>
        $"[{this.Thickness}, ${this.Color}]";
}
