using Age.Numerics;
using Age.Rendering.Enums;
using Age.Rendering.Resources;
using Age.Rendering.Shaders;

namespace Age.Rendering.Commands;

public partial record RectDrawCommand() : DrawCommand(DrawCommandType.Rect)
{
    public required SampledTexture SampledTexture { get; init; }

    public Color               Color     { get; set; }
    public ColorMode           ColorMode { get; set; }
    public Rect<float>         Rect      { get; set; }
    public CanvasShader.Border Border    { get; set; }
}
