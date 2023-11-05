using Age.Numerics;
using Age.Vulkan.Native.Types;
using Age.Vulkan.Native.Types.KHR;

namespace Age.Rendering.Vulkan.Handlers;

public record WindowHandler
{
    private Size<uint> size;

    public required VkSemaphore[] Semaphores { get; init; }
    public required VkSurfaceKHR  Surface    { get; init; }

    public required Size<uint> Size
    {
        get => this.size;
        set
        {
            this.FramebufferResized = this.size != default && value != this.size;

            this.size = value;
        }
    }

    public required SwapchainHandler Swapchain { get; set; }

    public uint CurrentBuffer      { get; set; }
    public bool FramebufferResized { get; set; }

}
