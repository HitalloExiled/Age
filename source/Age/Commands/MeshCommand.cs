using System.Diagnostics.CodeAnalysis;
using Age.Rendering.Resources;
using Age.Resources;
using Age.Shaders;

namespace Age.Commands;

public sealed record MeshCommand : Command3D
{
    [AllowNull]
    public IndexBuffer32 IndexBuffer { get; set; }

    [AllowNull]
    public Mesh Onwer { get; set; }

    [AllowNull]
    public VertexBuffer<GeometryShader.Vertex> VertexBuffer { get; set; }

    public override void Reset()
    {
        base.Reset();

        this.IndexBuffer  = default;
        this.Onwer        = default;
        this.VertexBuffer = default;
    }
}
