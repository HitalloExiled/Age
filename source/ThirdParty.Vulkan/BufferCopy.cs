using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkBufferCopy.html">VkBufferCopy</see>
/// </summary>
public struct BufferCopy
{
    public ulong SrcOffset { get; set; }
    public ulong DstOffset { get; set; }
    public ulong Size      { get; set; }

    public static implicit operator VkBufferCopy(BufferCopy value) =>
        new()
        {
            dstOffset = value.DstOffset,
            size      = value.Size,
            srcOffset = value.SrcOffset,
        };
}
