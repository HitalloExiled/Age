using Age.Vulkan.Native.Enums;
using Age.Vulkan.Native.Flags;

namespace Age.Vulkan.Native.Types;

/// <summary>
/// Structure specifying a global memory barrier.
/// The first <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-dependencies-access-scopes">access scope</see> is limited to access types in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-access-masks">source access mask</see> specified by srcAccessMask.
/// The second <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-dependencies-access-scopes">access scope</see> is limited to access types in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-access-masks">destination access mask</see> specified by dstAccessMask.
/// </summary>
public unsafe struct VkMemoryBarrier
{
    /// <summary>
    /// A VkStructureType value identifying this structure.
    /// </summary>
    public readonly VkStructureType sType;

    /// <summary>
    /// Null or a pointer to a structure extending this structure.
    /// </summary>
    public void* pNext;

    /// <summary>
    /// A bitmask of <see cref="VkAccessFlagBits"/> specifying a source access mask.
    /// </summary>
    public VkAccessFlags srcAccessMask;

    /// <summary>
    /// A bitmask of <see cref="VkAccessFlagBits"/> specifying a destination access mask.
    /// </summary>
    public VkAccessFlags dstAccessMask;

    public VkMemoryBarrier() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_MEMORY_BARRIER;
}
