using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkBufferMemoryBarrier.html">VkBufferMemoryBarrier</see>
/// </summary>
public unsafe struct VkBufferMemoryBarrier
{
    public readonly VkStructureType SType;

    public void*              PNext;
    public VkAccessFlags      SrcAccessMask;
    public VkAccessFlags      DstAccessMask;
    public uint               SrcQueueFamilyIndex;
    public uint               DstQueueFamilyIndex;
    public VkHandle<VkBuffer> Buffer;
    public VkDeviceSize       Offset;
    public VkDeviceSize       Size;

    public VkBufferMemoryBarrier() =>
        this.SType = VkStructureType.BufferMemoryBarrier;
}
