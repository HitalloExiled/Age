using Age.Vulkan.Native.Enums;

namespace Age.Vulkan.Native.Types;

/// <summary>
/// Structure specifying the allocation parameters for command buffer object.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public unsafe struct VkCommandBufferAllocateInfo
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
    /// The command pool from which the command buffers are allocated.
    /// </summary>
    public VkCommandPool commandPool;

    /// <summary>
    /// A <see cref="VkCommandBufferLevel"/> value specifying the command buffer level.
    /// </summary>
    public VkCommandBufferLevel level;

    /// <summary>
    /// The number of command buffers to allocate from the pool.
    /// </summary>
    public uint commandBufferCount;

    public VkCommandBufferAllocateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_COMMAND_BUFFER_ALLOCATE_INFO;
}
