using Age.Vulkan.Native.Enums;
using Age.Vulkan.Native.Flags;

namespace Age.Vulkan.Native.Types;

/// <summary>
/// Structure specifying parameters of a newly created device queue.
/// </summary>
public unsafe struct VkDeviceQueueCreateInfo
{
    /// <summary>
    /// A VkStructureType value identifying this structure.
    /// </summary>
    public readonly VkStructureType sType;

    /// <summary>
    /// NULL or a pointer to a structure extending this structure.
    /// </summary>
    public void* pNext;

    /// <summary>
    /// A bitmask indicating behavior of the queues.
    /// </summary>
    public VkDeviceQueueCreateFlags flags;

    /// <summary>
    /// An unsigned integer indicating the index of the queue family in which to create the queues on this device. This index corresponds to the index of an element of the pQueueFamilyProperties array that was returned by <see cref="Vk.GetPhysicalDeviceQueueFamilyProperties"/>.
    /// </summary>
    public uint queueFamilyIndex;

    /// <summary>
    /// An unsigned integer specifying the number of queues to create in the queue family indicated by queueFamilyIndex, and with the behavior specified by flags.
    /// </summary>
    public uint queueCount;

    /// <summary>
    /// A pointer to an array of queueCount normalized floating point values, specifying priorities of work that will be submitted to each created queue. See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#devsandqueues-priority">Queue Priority</see> for more information.
    /// </summary>
    public float* pQueuePriorities;

    public VkDeviceQueueCreateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_DEVICE_QUEUE_CREATE_INFO;
}
