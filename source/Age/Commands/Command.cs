using Age.Core.Interfaces;
using Age.Elements;

namespace Age.Commands;

public abstract record Command : IPoolable
{
    internal StencilLayer? StencilLayer { get; set; }

    public long            Metadata        { get; set; }
    public ulong           ObjectId        { get; set; }
    public PipelineVariant PipelineVariant { get; set; }
    public int             ZIndex          { get; set; }

    public virtual void Reset()
    {
        this.Metadata        = default;
        this.StencilLayer    = default;
        this.ObjectId        = default;
        this.PipelineVariant = default;
        this.ZIndex          = default;
    }
}
