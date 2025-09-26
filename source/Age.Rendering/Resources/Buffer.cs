using Age.Rendering.Vulkan;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Flags;

namespace Age.Rendering.Resources;

public sealed class Buffer : Resource<VkBuffer>
{
    internal override VkBuffer Instance { get; }

    public Allocation         Allocation { get; }
    public VkBufferUsageFlags Usage      { get; }

    public ulong Size => this.Allocation.Size;

    public Buffer(ulong size, VkBufferUsageFlags usage, VkMemoryPropertyFlags properties)
    {
        var context = VulkanRenderer.Singleton.Context;

        var bufferCreateInfo = new VkBufferCreateInfo
        {
            Size  = size,
            Usage = usage,
        };

        var device = context.Device;

        var buffer = device.CreateBuffer(bufferCreateInfo);

        buffer.GetMemoryRequirements(out var memRequirements);

        var memoryType = context.FindMemoryType(memRequirements.MemoryTypeBits, properties);

        var memoryAllocateInfo = new VkMemoryAllocateInfo
        {
            AllocationSize  = memRequirements.Size,
            MemoryTypeIndex = memoryType
        };

        var memory = device.AllocateMemory(memoryAllocateInfo);

        buffer.BindMemory(memory, 0);

        this.Instance = buffer;

        this.Allocation = new()
        {
            Alignment  = memRequirements.Alignment,
            Memory     = memory,
            Memorytype = memoryType,
            Offset     = 0,
            Size       = size,
        };

        this.Usage = usage;
    }

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

    protected override void OnDisposed()
    {
        this.Allocation.Dispose();
        this.Instance.Dispose();
    }

    public void CopyFrom(Buffer source) =>
        Copy(source, this);

    public void CopyTo(Buffer destination) =>
        Copy(this, destination);

    public void Map(out nint handle) =>
        this.Allocation.Memory.Map(0, (uint)this.Allocation.Size, 0, out handle);

    public void Map(uint flags, out nint handle) =>
        this.Allocation.Memory.Map(0, (uint)this.Allocation.Size, flags, out handle);

    public void Unmap() =>
        this.Allocation.Memory.Unmap();

    public void Update<T>(T data) where T : unmanaged =>
        this.Update([data]);

    public void Update<T>(scoped ReadOnlySpan<T> data) where T : unmanaged
    {
        var stagingBuffer = new Buffer(this.Allocation.Size, VkBufferUsageFlags.TransferSrc, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent);

        stagingBuffer.Allocation.Memory.Write(0, 0, data);

        Copy(stagingBuffer, this);

        stagingBuffer.Dispose();
    }
}
