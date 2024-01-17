using Age.Numerics;

namespace Age.Rendering.Drawing;

public record Style
{
    public int        FontSize { get; set; }
    public Point<int> Position { get; set; }
    public Size<uint> Size     { get; set; }
}
