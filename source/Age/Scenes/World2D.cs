using Age.Commands;
using Age.Graphs;

namespace Age.Scenes;

public class World2D : Renderable
{
    internal CommandBuffer<Command2D> CommandBuffer { get; } = [];

    public override string NodeName => nameof(World2D);
}
