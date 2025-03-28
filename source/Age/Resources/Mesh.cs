using Age.Commands;
using Age.Rendering.Vulkan;
using Age.Scene;

using static Age.Shaders.GeometryShader;

namespace Age.Resources;

public sealed class Mesh : Spatial3D
{
    public override string NodeName { get; } = nameof(Mesh);

    public Material Material { get; set; } = new();

    public Mesh(scoped ReadOnlySpan<Vertex> vertices, scoped ReadOnlySpan<uint> indices)
    {
        var command = CommandPool.MeshCommand.Get();

        command.VertexBuffer = VulkanRenderer.Singleton.CreateVertexBuffer(vertices);
        command.IndexBuffer  = VulkanRenderer.Singleton.CreateIndexBuffer(indices);
        command.Mesh         = this;

        this.SingleCommand = command;
    }

    protected override void Disposed()
    {
        if (this.SingleCommand is MeshCommand command)
        {
            command.IndexBuffer.Dispose();
            command.VertexBuffer.Dispose();

            CommandPool.MeshCommand.Return(command);
        }
    }
}
