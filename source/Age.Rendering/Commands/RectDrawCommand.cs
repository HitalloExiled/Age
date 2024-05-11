using Age.Numerics;
using Age.Rendering.Drawing.Styling;
using Age.Rendering.Enums;
using Age.Rendering.Resources;
using Age.Rendering.Shaders.Canvas;

namespace Age.Rendering.Commands;

public partial record RectDrawCommand() : DrawCommand(DrawCommandType.Rect)
{
    public required SampledTexture SampledTexture { get; init; }

    public uint               ObjectId { get; set; }
    public Border             Border   { get; set; }
    public Color              Color    { get; set; }
    public CanvasShader.Flags Flags    { get; set; }
    public Rect<float>        Rect     { get; set; }
}
