using Age.Vulkan.Enums;
using Age.Vulkan.Flags;

namespace Age.Vulkan.Types;

/// <summary>
/// Structure specifying a command buffer begin operation.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public unsafe struct VkCommandBufferBeginInfo
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
    /// A bitmask of <see cref="VkCommandBufferUsageFlagBits"/> specifying usage behavior for the command buffer.
    /// </summary>
    public VkCommandBufferUsageFlags flags;

    /// <summary>
    /// A pointer to a <see cref="VkCommandBufferInheritanceInfo"/> structure, used if commandBuffer is a secondary command buffer. If this is a primary command buffer, then this value is ignored.
    /// </summary>
    public VkCommandBufferInheritanceInfo* pInheritanceInfo;

    public VkCommandBufferBeginInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_COMMAND_BUFFER_BEGIN_INFO;
}
