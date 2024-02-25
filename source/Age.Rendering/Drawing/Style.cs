using Age.Numerics;

namespace Age.Rendering.Drawing;

public record Style
{
    public ushort     FontSize { get; set; }
    public Point<int> Position { get; set; }
    public Size<uint> Size     { get; set; }
    public Color      Color    { get; set; }
}
