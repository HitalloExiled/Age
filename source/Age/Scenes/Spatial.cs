using Age.Commands;

namespace Age.Scenes;

public abstract class Spatial<TCommand, TMatrix> : Renderable<TCommand>
where TCommand : Command
where TMatrix : unmanaged
{
    public abstract TMatrix CachedMatrix { get; }
    public abstract TMatrix Matrix       { get; }
}
