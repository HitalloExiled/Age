using Age.Commands;
using Age.Scenes;
using Age.Shaders;

namespace Age.Resources;

public sealed class Mesh : Spatial3D
{
    public override string NodeName => nameof(Mesh);

    public Material Material { get; set; } = new();

    public Mesh(ReadOnlySpan<GeometryShader.Vertex> vertices, scoped ReadOnlySpan<uint> indices)
    {
        var command = CommandPool.MeshCommand.Get(this, CommandFilter.Color);

        command.IndexBuffer   = new(indices);
        command.VertexBuffer  = new(vertices);

        this.SingleCommand = command;
    }

    private protected override void OnDisposedInternal()
    {
        if (this.SingleCommand is MeshCommand command)
        {
            command.IndexBuffer.Dispose();
            command.VertexBuffer.Dispose();

            CommandPool.MeshCommand.Return(command);
        }

        this.Material.Dispose();
    }
}
