using Age.Elements;
using Age.Rendering.Shaders.Canvas;

namespace Age.Commands;

public abstract record Command
{
    #region 8-bytes
    internal StencilLayer? StencilLayer { get; set; }
    #endregion

    #region 4-bytes
    public uint                 ObjectId { get; set; }
    public CanvasShader.Variant Variant  { get; set; }
    #endregion
}
