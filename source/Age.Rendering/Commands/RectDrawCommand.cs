using Age.Numerics;
using Age.Rendering.Drawing.Styling;
using Age.Rendering.Enums;
using Age.Rendering.Resources;

namespace Age.Rendering.Commands;

public partial record RectDrawCommand() : DrawCommand(DrawCommandType.Rect)
{
    public required SampledTexture SampledTexture { get; init; }

    public Color       Color     { get; set; }
    public ColorMode   ColorMode { get; set; }
    public Rect<float> Rect      { get; set; }
    public Border      Border    { get; set; }
}
