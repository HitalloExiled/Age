using Age.Elements;
using Age.Numerics;
using Age.Rendering.Resources;
using Age.Rendering.Shaders.Canvas;

namespace Age.Commands;

public partial record RectCommand : Command
{
    public required MappedTexture MappedTexture { get; set; }

    internal Layer? Layer { get; set; }

    public CanvasShader.Border Border   { get; set; }
    public Color               Color    { get; set; }
    public CanvasShader.Flags  Flags    { get; set; }
    public uint                ObjectId { get; set; }
    public Rect<float>         Rect     { get; set; }
}
