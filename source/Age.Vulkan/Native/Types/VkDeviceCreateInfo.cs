using Age.Vulkan.Native.Enums;
using Age.Vulkan.Native.Flags;

namespace Age.Vulkan.Native.Types;

/// <summary>
/// Structure specifying parameters of a newly created device.
/// </summary>
public unsafe struct VkDeviceCreateInfo
{
    /// <summary>
    /// A <see cref="VkStructureType"/> value identifying this structure.
    /// </summary>
    public VkStructureType sType;

    /// <summary>
    /// NULL or a pointer to a structure extending this structure.
    /// </summary>
    public void* pNext;

    /// <summary>
    /// Reserved for future use.
    /// </summary>
    public VkDeviceCreateFlags flags;

    /// <summary>
    /// The unsigned integer size of the pQueueCreateInfos array. Refer to the Queue Creation section below for further details.
    /// </summary>
    public uint queueCreateInfoCount;

    /// <summary>
    /// A pointer to an array of <see cref="VkDeviceQueueCreateInfo"/> structures describing the queues that are requested to be created along with the logical device. Refer to the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#devsandqueues-queue-creation">Queue Creation</see> section below for further details.
    /// </summary>
    public VkDeviceQueueCreateInfo* pQueueCreateInfos;

    /// <summary>
    /// Is deprecated and ignored.
    /// </summary>
    [Obsolete("Ignored")]
    public uint enabledLayerCount;

    /// <summary>
    /// Is deprecated and ignored. See https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#extendingvulkan-layers-devicelayerdeprecation.
    /// </summary>
    [Obsolete("Ignored")]
    public byte** ppEnabledLayerNames;

    /// <summary>
    /// The number of device extensions to enable.
    /// </summary>
    public uint enabledExtensionCount;

    /// <summary>
    /// A pointer to an array of <see cref="enabledExtensionCount"/> null-terminated UTF-8 strings containing the names of extensions to enable for the created device. See the https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#extendingvulkan-extensions section for further details.
    /// </summary>
    public byte** ppEnabledExtensionNames;

    /// <summary>
    /// NULL or a pointer to a <see cref="VkPhysicalDeviceFeatures"/> structure containing boolean indicators of all the features to be enabled. Refer to the Features section for further details.
    /// </summary>
    public VkPhysicalDeviceFeatures* pEnabledFeatures;
}
