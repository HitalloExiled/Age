using Age.Core.Interfaces;
using Age.Elements;

namespace Age.Commands;

public abstract record Command : IPoolable
{
    internal StencilLayer? StencilLayer { get; set; }

    public ulong           ObjectId        { get; set; }
    public PipelineVariant PipelineVariant { get; set; }

    public virtual void Reset()
    {
        this.StencilLayer    = default;
        this.ObjectId        = default;
        this.PipelineVariant = default;
    }
}
