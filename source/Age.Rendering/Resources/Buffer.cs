using Age.Rendering.Vulkan;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Flags;

namespace Age.Rendering.Resources;

public class Buffer(VkBuffer instance) : Resource<VkBuffer>
{

    public required Allocation         Allocation { get; init; }
    public required VkBufferUsageFlags Usage      { get; init; }

    public override VkBuffer Instance => instance;

    private static void Copy(Buffer source, Buffer destination)
    {
        var commandBuffer = VulkanRenderer.Singleton.BeginSingleTimeCommands();

        var copyRegion = new VkBufferCopy
        {
            Size = source.Allocation.Size,
        };

        commandBuffer.Instance.CopyBuffer(source, destination, copyRegion);

        VulkanRenderer.Singleton.EndSingleTimeCommands(commandBuffer);
    }

    protected override void Disposed()
    {
        this.Allocation.Dispose();
        this.Instance.Dispose();
    }

    public void CopyFrom(Buffer source) =>
        Copy(source, this);

    public void CopyTo(Buffer destination) =>
        Copy(this, destination);

    public void Update<T>(T data) where T : unmanaged =>
        this.Update([data]);

    public void Update<T>(Span<T> data) where T : unmanaged
    {
        var stagingBuffer = VulkanRenderer.Singleton.CreateBuffer(this.Allocation.Size, VkBufferUsageFlags.TransferSrc, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent);

        stagingBuffer.Allocation.Memory.Write(0, 0, data);

        Copy(stagingBuffer, this);

        stagingBuffer.Dispose();
    }
}
