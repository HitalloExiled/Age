using Age.Numerics;
using Age.Rendering.Enums;

namespace Age.Rendering.Commands;

public record RectDrawCommand() : DrawCommand(DrawCommandType.Rect)
{
    public required Rect<float> Rect    { get; set; }
    public required Texture     Texture { get; set; }
}
