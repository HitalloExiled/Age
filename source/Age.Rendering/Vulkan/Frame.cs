using ThirdParty.Vulkan;

namespace Age.Rendering.Vulkan;

public record struct Frame
{
    public bool            BufferPrepared;
    public VkCommandBuffer CommandBuffer;
    public VkCommandPool   CommandPool;
    public VkFence         Fence;
    public ushort          Index;
 }
