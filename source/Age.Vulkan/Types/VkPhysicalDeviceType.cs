namespace Age.Vulkan.Types;

/// <summary>
/// Supported physical device types.
/// </summary>
/// <remarks>The physical device type is advertised for informational purposes only, and does not directly affect the operation of the system. However, the device type may correlate with other advertised properties or capabilities of the system, such as how many memory heaps there are.</remarks>
public enum VkPhysicalDeviceType
{
    /// <summary>
    /// The device does not match any other available types.
    /// </summary>
    VK_PHYSICAL_DEVICE_TYPE_OTHER = 0,

    /// <summary>
    /// The device is typically one embedded in or tightly coupled with the host.
    /// </summary>
    VK_PHYSICAL_DEVICE_TYPE_INTEGRATED_GPU = 1,

    /// <summary>
    /// The device is typically a separate processor connected to the host via an interlink.
    /// </summary>
    VK_PHYSICAL_DEVICE_TYPE_DISCRETE_GPU = 2,

    /// <summary>
    /// The device is typically a virtual node in a virtualization environment.
    /// </summary>
    VK_PHYSICAL_DEVICE_TYPE_VIRTUAL_GPU = 3,

    /// <summary>
    /// The device is typically running on the same processors as the host.
    /// </summary>
    VK_PHYSICAL_DEVICE_TYPE_CPU = 4,
}
