using Age.Commands;
using Age.Rendering.Vulkan;
using Age.Scene;

using static Age.Rendering.Shaders.GeometryShader;

namespace Age.Resources;

public sealed class Mesh : Node3D
{
    public override string NodeName { get; } = nameof(Mesh);

    public Material Material { get; set; } = new();

    public Mesh(Span<Vertex> vertices, Span<uint> indices) =>
        this.SingleCommand = new MeshCommand
        {
            VertexBuffer = VulkanRenderer.Singleton.CreateVertexBuffer(vertices),
            IndexBuffer  = VulkanRenderer.Singleton.CreateIndexBuffer(indices),
            Mesh         = this,
        };

    protected override void Disposed()
    {
        if (this.SingleCommand is MeshCommand command)
        {
            command.IndexBuffer.Dispose();
            command.VertexBuffer.Dispose();
        }
    }
}
