using Age.Core;
using ThirdParty.Vulkan.Flags;

namespace Age.Rendering.Resources;

public abstract class VertexBuffer(Buffer buffer, uint size) : Disposable
{
    public Buffer Buffer => buffer;
    public uint   Size   => size;
}

public sealed class VertexBuffer<T> : VertexBuffer where T : unmanaged
{
    private unsafe static Buffer CreateBuffer(ReadOnlySpan<T> data)
    {
        var size   = (ulong)(data.Length * sizeof(T));
        var buffer = new Buffer(
            size,
            VkBufferUsageFlags.TransferDst | VkBufferUsageFlags.VertexBuffer,
            VkMemoryPropertyFlags.DeviceLocal
        );

        buffer.Update(data);

        return buffer;
    }

    public VertexBuffer(ReadOnlySpan<T> data) : base(CreateBuffer(data), (uint)data.Length)
    { }

    protected override void OnDisposed(bool disposing)
    {
        if (disposing)
        {
            this.Buffer.Dispose();
        }
    }

    public void Update(T data) =>
        this.Buffer.Update(data);

    public void Update(scoped ReadOnlySpan<T> data) =>
        this.Buffer.Update(data);
}
