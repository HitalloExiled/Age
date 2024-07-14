using Age.Rendering.Resources;
using Age.Rendering.Scene.Resources;

namespace Age.Rendering.Commands;

public record MeshCommand : Command
{
    public required IndexBuffer  IndexBuffer  { get; set; }
    public required VertexBuffer VertexBuffer { get; set; }
    public required Mesh         Mesh         { get; set; }
}
