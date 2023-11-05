namespace Age.Rendering.Vulkan.Handlers;

public class VertexBufferHandler
{
    public required BufferHandler Buffer { get; init; }
    public required ulong         Size   { get; init; }
}
