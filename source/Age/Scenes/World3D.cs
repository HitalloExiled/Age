using Age.Commands;
using Age.Graphs;

namespace Age.Scenes;

public class World3D : Renderable
{
    internal CommandBuffer<Command3D> CommandBuffer { get; } = [];

    public override string NodeName => nameof(World3D);
}
