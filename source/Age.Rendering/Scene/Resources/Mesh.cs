using Age.Rendering.Commands;
using static Age.Rendering.Shaders.GeometryShader;

namespace Age.Rendering.Scene.Resources;

public class Mesh : Node3D, IDisposable
{
    private bool disposed;

    public override string NodeName { get; } = nameof(Mesh);

    public Material Material { get; set; } = new();

    public Mesh(Span<Vertex> vertices, Span<uint> indices) =>
        this.SingleCommand = new MeshCommand
        {
            VertexBuffer = Renderer.CreateVertexBuffer(vertices),
            IndexBuffer  = Renderer.CreateIndexBuffer(indices),
            Mesh         = this,
        };

    protected override void OnDestroy() =>
        this.Dispose();

    public void Dispose()
    {
        if (!this.disposed)
        {
            if (this.SingleCommand is MeshCommand command)
            {
                command.IndexBuffer.Dispose();
                command.VertexBuffer.Dispose();
            }

            this.disposed = true;
        }

        GC.SuppressFinalize(this);
    }
}