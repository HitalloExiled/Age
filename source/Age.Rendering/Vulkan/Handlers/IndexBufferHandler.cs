using Age.Vulkan.Enums;

namespace Age.Rendering.Vulkan.Handlers;

public class IndexBufferHandler
{
    public required BufferHandler Buffer { get; init; }
    public required uint          Size   { get; init; }
    public required VkIndexType   Type   { get; init; }
}
