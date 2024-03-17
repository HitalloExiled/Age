using Age.Numerics;

namespace Age.Rendering.Drawing;

public record BorderStyle
{
    public uint  Size   { get; set; }
    public uint  Radius { get; set; }
    public Color Color  { get; set; }
}
