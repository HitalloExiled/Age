using Age.Core.Interfaces;
using Age.Elements;

namespace Age.Commands;

public abstract record Command : IPoolable
{
    internal StencilLayer? StencilLayer { get; set; }

    public CommandFilter CommandFilter { get; set; }
    public long          Metadata      { get; set; }
    public ulong         ObjectId      { get; set; }

    public virtual void Reset()
    {
        this.CommandFilter = default;
        this.Metadata      = default;
        this.ObjectId      = default;
        this.StencilLayer  = default;
    }
}
