using System.Numerics;
using Age.Core;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace Age.Rendering.Resources;

public abstract class IndexBuffer(Buffer buffer, uint size, VkIndexType type) : Disposable
{
    public Buffer      Buffer => buffer;
    public uint        Size   => size;
    public VkIndexType Type   => type;
}

public abstract class IndexBuffer<T> : IndexBuffer where T : unmanaged, INumber<T>
{
    private static unsafe Buffer CreateBuffer(ReadOnlySpan<T> indices)
    {
        var bufferSize = (ulong)(sizeof(T) * indices.Length);

        var buffer = new Buffer(
            bufferSize,
            VkBufferUsageFlags.TransferDst | VkBufferUsageFlags.IndexBuffer,
            VkMemoryPropertyFlags.DeviceLocal
        );

        buffer.Update([..indices]);

        return buffer;
    }

    protected IndexBuffer(ReadOnlySpan<T> indices, VkIndexType indexType) : base(CreateBuffer(indices), (uint)indices.Length, indexType) { }

    protected override void OnDisposed(bool disposing)
    {
        if (disposing)
        {
            this.Buffer.Dispose();
        }
    }

    public void Update(T data) =>
        this.Buffer.Update(data);

    public void Update(ReadOnlySpan<T> data) =>
        this.Buffer.Update(data);
}

public class IndexBuffer8(ReadOnlySpan<sbyte> indices) : IndexBuffer<sbyte>(indices, VkIndexType.Uint8EXT);
public class IndexBuffer16(ReadOnlySpan<ushort> indices) : IndexBuffer<ushort>(indices, VkIndexType.Uint16);
public class IndexBuffer32(ReadOnlySpan<uint> indices) : IndexBuffer<uint>(indices, VkIndexType.Uint32);
