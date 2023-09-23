using Age.Vulkan.Native.Enums;
using Age.Vulkan.Native.Flags;

namespace Age.Vulkan.Native.Types;

/// <summary>
/// <para>Structure specifying the parameters of a newly created buffer object.</para>
/// <para>If a <see cref="VkBufferUsageFlags2CreateInfoKHR"/> structure is present in the pNext chain, <see cref="VkBufferUsageFlags2CreateInfoKHR.usage"/> from that structure is used instead of usage from this structure.</para>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public unsafe struct VkBufferCreateInfo
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
    /// A bitmask of <see cref="VkBufferCreateFlagBits"/> specifying additional parameters of the buffer.
    /// </summary>
    public VkBufferCreateFlags flags;

    /// <summary>
    /// The size in bytes of the buffer to be created.
    /// </summary>
    public VkDeviceSize size;

    /// <summary>
    /// A bitmask of VkBufferUsageFlagBits specifying allowed usages of the buffer.
    /// </summary>
    public VkBufferUsageFlags usage;

    /// <summary>
    /// A <see cref="VkSharingMode"/> value specifying the sharing mode of the buffer when it will be accessed by multiple queue families.
    /// </summary>
    public VkSharingMode sharingMode;

    /// <summary>
    /// The number of entries in the pQueueFamilyIndices array.
    /// </summary>
    public uint queueFamilyIndexCount;

    /// <summary>
    /// A pointer to an array of queue families that will access this buffer. It is ignored if sharingMode is not <see cref="VkSharingMode.VK_SHARING_MODE_CONCURRENT"/>.
    /// </summary>
    public uint* pQueueFamilyIndices;

    public VkBufferCreateInfo() =>
        sType = VkStructureType.VK_STRUCTURE_TYPE_BUFFER_CREATE_INFO;
}
