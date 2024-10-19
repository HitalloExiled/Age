using Age.Elements;

namespace Age.Commands;

public abstract record Command
{
    internal StencilLayer? StencilLayer { get; set; }
    public required int Id { get; set; }
}
