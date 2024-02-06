using ThirdParty.Vulkan.Flags;

namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkMemoryBarrier.html">VkMemoryBarrier</see>
/// </summary>
public unsafe struct VkMemoryBarrier
{
    public readonly VkStructureType SType;

    public void*         PNext;
    public VkAccessFlags SrcAccessMask;
    public VkAccessFlags DstAccessMask;

    public VkMemoryBarrier() =>
        this.SType = VkStructureType.MemoryBarrier;
}
