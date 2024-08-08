using Age.Core;
using Age.Numerics;
using ThirdParty.Vulkan;

namespace Age.Rendering.Resources;

public class Surface : Disposable
{
    public static List<Surface> Entries { get; } = [];

    public required VkSemaphore[] Semaphores { get; init; }
    public required VkSurfaceKHR  Value      { get; init; }

    public required Size<uint> Size      { get; set; }
    public required Swapchain  Swapchain { get; set; }

    public uint CurrentBuffer { get; set; }
    public bool Hidden        { get; set; }
    public bool HasChanged    { get; set; }

    public Surface() =>
        Entries.Add(this);

    protected override void OnDispose()
    {
        Entries.Remove(this);

        foreach (var semaphore in this.Semaphores)
        {
            semaphore.Dispose();
        }

        this.Swapchain.Dispose();
        this.Value.Dispose();
    }
}
