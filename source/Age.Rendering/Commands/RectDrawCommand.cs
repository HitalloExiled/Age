using Age.Numerics;
using Age.Rendering.Drawing;
using Age.Rendering.Enums;
using Age.Rendering.Resources;

namespace Age.Rendering.Commands;

public record RectDrawCommand() : DrawCommand(DrawCommandType.Rect)
{
    public Texture?       Texture { get; init; }
    public Point<float>[] UV      { get; init; } = new Point<float>[4];
    public Color          Color   { get; init; }
    public Sampler?       Sampler { get; init; }
    public BorderStyle?   Border  { get; init; }

    public Rect<float> Rect { get; set; }
}
