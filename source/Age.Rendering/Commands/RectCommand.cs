using Age.Numerics;
using Age.Rendering.Drawing.Styling;
using Age.Rendering.Resources;
using Age.Rendering.Shaders.Canvas;

namespace Age.Rendering.Commands;

public partial record RectCommand : Command
{
    public Border             Border         { get; set; }
    public Color              Color          { get; set; }
    public CanvasShader.Flags Flags          { get; set; }
    public uint               ObjectId       { get; set; }
    public Rect<float>        Rect           { get; set; }
    public SampledTexture     SampledTexture { get; set; }
}
