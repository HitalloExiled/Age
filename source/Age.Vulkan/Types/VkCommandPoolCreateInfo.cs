using Age.Vulkan.Enums;
using Age.Vulkan.Flags;

namespace Age.Vulkan.Types;

/// <summary>
/// Structure specifying parameters of a newly created command pool.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public unsafe struct VkCommandPoolCreateInfo
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
    /// A bitmask of <see cref="VkCommandPoolCreateFlagBits"/> indicating usage behavior for the pool and command buffers allocated from it.
    /// </summary>
    public VkCommandPoolCreateFlags flags;

    /// <summary>
    /// Sesignates a queue family as described in section <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#devsandqueues-queueprops">Queue Family Properties</see>. All command buffers allocated from this command pool must be submitted on queues from the same queue family.
    /// </summary>
    public uint queueFamilyIndex;

    public VkCommandPoolCreateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_COMMAND_POOL_CREATE_INFO;
}
