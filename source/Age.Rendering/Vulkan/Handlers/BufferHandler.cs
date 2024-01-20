using Age.Vulkan.Flags;
using Age.Vulkan.Types;

namespace Age.Rendering.Vulkan.Handlers;

public record BufferHandler
{
    public required Allocation         Allocation { get; init; }
    public required VkBuffer           Handler    { get; init; }
    public required VkBufferUsageFlags Usage      { get; init; }
}
