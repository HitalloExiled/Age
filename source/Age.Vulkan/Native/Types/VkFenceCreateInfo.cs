using Age.Vulkan.Native.Enums;
using Age.Vulkan.Native.Flags;

namespace Age.Vulkan.Native.Types;

/// <summary>
/// Structure specifying parameters of a newly created fence.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public unsafe struct VkFenceCreateInfo
{
    /// <summary>
    /// A <see cref="VkStructureType"/> value identifying this structure.
    /// </summary>
    public readonly VkStructureType sType;

    /// <summary>
    /// Null or a pointer to a structure extending this structure.
    /// </summary>
    public void* pNext;

    /// <summary>
    /// A bitmask of <see cref="VkFenceCreateFlagBits"/> specifying the initial state and behavior of the fence.
    /// </summary>
    public VkFenceCreateFlags flags;

    public VkFenceCreateInfo() =>
        sType = VkStructureType.VK_STRUCTURE_TYPE_FENCE_CREATE_INFO;
}
