using Age.Numerics;
using Age.Shaders;
using Age.Resources;

namespace Age.Commands;

public record RectCommand : Command
{
    #region 8-bytes
    public MappedTexture MappedTexture { get; set; } = MappedTexture.Default;
    #endregion

    #region 4-bytes
    public CanvasShader.Border  Border { get; set; }
    public Color                Color  { get; set; }
    public CanvasShader.Flags   Flags  { get; set; }
    public Rect<float>          Rect   { get; set; }
    #endregion
}
