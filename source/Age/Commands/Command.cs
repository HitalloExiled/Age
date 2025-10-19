using System.Diagnostics.CodeAnalysis;
using Age.Core.Interfaces;
using Age.Elements;
using Age.Scenes;

namespace Age.Commands;

public abstract record Command : IPoolable
{
    internal StencilLayer? StencilLayer { get; set; }

    public CommandFilter CommandFilter { get; set; }
    public ulong         Metadata      { get; set; }
    public long          UserData      { get; set; }

    public virtual void Reset()
    {
        this.CommandFilter = default;
        this.Metadata      = default;
        this.StencilLayer  = default;
        this.UserData      = default;
    }
}

public abstract record Command<T> : Command where T : Node
{
    [AllowNull]
    public T Owner { get; internal set; }

    public override void Reset()
    {
        base.Reset();

        this.Owner = default;
    }
}
