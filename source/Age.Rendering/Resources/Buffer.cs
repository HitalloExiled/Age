using Age.Rendering.Vulkan;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Flags;

namespace Age.Rendering.Resources;

public class Buffer(VulkanRenderer renderer, VkBuffer value) : Resource<VkBuffer>(renderer, value)
{
    public required Allocation         Allocation { get; init; }
    public required VkBufferUsageFlags Usage      { get; init; }

    private void Copy(Buffer source, Buffer destination)
    {
        var commandBuffer = this.Renderer.BeginSingleTimeCommands();

        var copyRegion = new VkBufferCopy
        {
            Size = source.Allocation.Size,
        };

        commandBuffer.Value.CopyBuffer(source, destination, copyRegion);

        this.Renderer.EndSingleTimeCommands(commandBuffer);
    }

    protected override void OnDispose()
    {
        this.Allocation.Dispose();
        this.Value.Dispose();
    }

    public void CopyFrom(Buffer source) =>
        this.Copy(source, this);

    public void CopyTo(Buffer destination) =>
        this.Copy(this, destination);

    public void Update<T>(T data) where T : unmanaged =>
        this.Update([data]);

    public void Update<T>(Span<T> data) where T : unmanaged
    {
        var stagingBuffer = this.Renderer.CreateBuffer(this.Allocation.Size, VkBufferUsageFlags.TransferSrc, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent);

        stagingBuffer.Allocation.Memory.Write(0, 0, data);

        this.Copy(stagingBuffer, this);

        stagingBuffer.Dispose();
    }
}
