using System.Diagnostics.CodeAnalysis;
using Age.Rendering.Resources;
using Age.Resources;

namespace Age.Commands;

public sealed record MeshCommand : Command
{
    [AllowNull]
    public IndexBuffer IndexBuffer { get; set; }

    [AllowNull]
    public Mesh Mesh { get; set; }

    [AllowNull]
    public VertexBuffer VertexBuffer { get; set; }

    public override void Reset()
    {
        base.Reset();

        this.IndexBuffer  = default;
        this.Mesh         = default;
        this.VertexBuffer = default;
    }
}
