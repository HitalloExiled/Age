using Age.Rendering.Vulkan;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Flags;

namespace Age.Rendering.Resources;

public class Buffer(VulkanRenderer renderer, VkBuffer value) : Resource<VkBuffer>(renderer, value)
{
    public required Allocation         Allocation { get; init; }
    public required VkBufferUsageFlags Usage      { get; init; }

    protected override void OnDispose()
    {
        this.Allocation.Dispose();
        this.Value.Dispose();
    }

    public void Update<T>(T data) where T : unmanaged =>
        this.Update([data]);

    public void Update<T>(Span<T> data) where T : unmanaged
    {
        var stagingBuffer = this.Renderer.CreateBuffer(this.Allocation.Size, VkBufferUsageFlags.TransferSrc, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent);

        stagingBuffer.Allocation.Memory.Write(0, 0, data);

        this.Renderer.CopyBuffer(stagingBuffer, this, this.Allocation.Size);

        stagingBuffer.Dispose();
    }
}
