using Age.Vulkan.Enums;
using Age.Vulkan.Flags;

namespace Age.Vulkan.Types;

/// <summary>
/// Structure specifying parameters of a newly created semaphore.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public unsafe struct VkSemaphoreCreateInfo
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
    /// Reserved for future use.
    /// </summary>
    public VkSemaphoreCreateFlags flags;

    public VkSemaphoreCreateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_SEMAPHORE_CREATE_INFO;
}
