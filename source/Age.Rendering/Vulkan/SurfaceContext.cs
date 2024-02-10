using Age.Numerics;
using Age.Rendering.Vulkan.Handlers;
using ThirdParty.Vulkan;

namespace Age.Rendering.Vulkan;

public record SurfaceContext
{
    public required VkSemaphore[] Semaphores { get; init; }
    public required VkSurfaceKHR  Surface    { get; init; }

    public required Size<uint>       Size      { get; set; }
    public required SwapchainHandler Swapchain { get; set; }

    public uint CurrentBuffer { get; set; }
    public bool Hidden        { get; set; }
}
