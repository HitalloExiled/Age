using Age.Vulkan.Native.Types;

namespace Age.Rendering.Vulkan;

public record struct Frame
{
    public VkCommandBuffer CommandBuffer;
    public VkCommandPool   CommandPool;
    public ushort          Index;
 }
