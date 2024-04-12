using Age.Numerics;
using Age.Rendering.Enums;
using Age.Rendering.Resources;

namespace Age.Rendering.Commands;

public record RectDrawCommand() : DrawCommand(DrawCommandType.Rect)
{
    public required SampledTexture SampledTexture { get; init; }

    public Color       Color { get; set; }
    public Rect<float> Rect  { get; set; }
}
