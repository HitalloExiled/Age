using Age.Numerics;
using Age.Rendering.Resources;
using Age.Rendering.Shaders.Canvas;

namespace Age.Commands;

public partial record RectCommand : Command
{
    #region 8-bytes
    public required MappedTexture MappedTexture { get; set; }
    #endregion

    #region 4-bytes
    public CanvasShader.Border Border   { get; set; }
    public Color               Color    { get; set; }
    public CanvasShader.Flags  Flags    { get; set; }
    public uint                ObjectId { get; set; }
    public Rect<float>         Rect     { get; set; }
    #endregion
}
