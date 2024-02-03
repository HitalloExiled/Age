using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkBufferCopy.html">VkBufferCopy</see>
/// </summary>
public unsafe record BufferCopy : NativeReference<VkBufferCopy>
{
    public ulong SrcOffset
    {
        get => this.PNative->srcOffset;
        init => this.PNative->srcOffset = value;
    }

    public ulong DstOffset
    {
        get => this.PNative->dstOffset;
        init => this.PNative->dstOffset = value;
    }

    public ulong Size
    {
        get => this.PNative->size;
        init => this.PNative->size = value;
    }
}
