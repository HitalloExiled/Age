using System.Diagnostics.CodeAnalysis;
using Age.Rendering.Resources;
using Age.Shaders;

namespace Age.Commands;

public sealed record MeshCommand : Command3D
{
    [AllowNull]
    public IndexBuffer32 IndexBuffer { get; set; }

    [AllowNull]
    public VertexBuffer<Geometry3DShader.Vertex> VertexBuffer { get; set; }

    public override void Reset()
    {
        base.Reset();

        this.IndexBuffer  = default;
        this.VertexBuffer = default;
    }
}
