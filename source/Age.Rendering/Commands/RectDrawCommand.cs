using Age.Numerics;
using Age.Rendering.Drawing;
using Age.Rendering.Enums;
using Age.Rendering.Resources;

namespace Age.Rendering.Commands;

public record RectDrawCommand() : DrawCommand(DrawCommandType.Rect)
{
    public required SampledTexture SampledTexture { get; init; }

    public BorderStyle? Border { get; set; }
    public Color        Color  { get; set; }
    public Rect<float>  Rect   { get; set; }
}
