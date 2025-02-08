using Age.Elements;

namespace Age.Commands;

public abstract record Command
{
    #region 8-bytes
    internal StencilLayer? StencilLayer { get; set; }
    #endregion

    #region 4-bytes
    public uint            ObjectId        { get; set; }
    public PipelineVariant PipelineVariant { get; set; }
    #endregion
}
