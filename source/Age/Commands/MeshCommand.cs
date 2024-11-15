using Age.Rendering.Resources;
using Age.Resources;

namespace Age.Commands;

public sealed record MeshCommand : Command
{
    public required IndexBuffer  IndexBuffer  { get; set; }
    public required VertexBuffer VertexBuffer { get; set; }
    public required Mesh         Mesh         { get; set; }
}
