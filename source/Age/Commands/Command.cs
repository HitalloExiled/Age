using Age.Elements;

namespace Age.Commands;

public abstract record Command
{
    #region 8-bytes
    public ulong           ObjectId     { get; set; }
    internal StencilLayer? StencilLayer { get; set; }
    #endregion

    #region 4-bytes
    public PipelineVariant PipelineVariant { get; set; }
    #endregion
}
