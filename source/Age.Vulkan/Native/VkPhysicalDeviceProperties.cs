namespace Age.Vulkan.Native;

/// <summary>
/// Structure specifying physical device properties.
/// </summary>
public unsafe struct VkPhysicalDeviceProperties
{
    /// <summary>
    /// the version of Vulkan supported by the device, encoded as described in https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#extendingvulkan-coreversions-versionnumbers.
    /// </summary>
    public uint apiVersion;

    /// <summary>
    /// the vendor-specified version of the driver.
    /// </summary>
    public uint driverVersion;

    /// <summary>
    /// a unique identifier for the vendor (see below) of the physical device.
    /// </summary>
    public uint vendorID;

    /// <summary>
    /// a unique identifier for the physical device among devices available from the vendor.
    /// </summary>
    public uint deviceID;

    /// <summary>
    /// a <see cref="VkPhysicalDeviceType"/> specifying the type of device.
    /// </summary>
    public VkPhysicalDeviceType deviceType;

    /// <summary>
    /// an array of <see cref="Vk.VK_MAX_PHYSICAL_DEVICE_NAME_SIZE"/> char containing a null-terminated UTF-8 string which is the name of the device.
    /// </summary>
    /// <returns></returns>
    public fixed byte deviceName[(int)Vk.VK_MAX_PHYSICAL_DEVICE_NAME_SIZE];

    /// <summary>
    /// an array of <see cref="Vk.VK_UUID_SIZE"/> byte values representing a universally unique identifier for the device.
    /// </summary>
    /// <returns></returns>
    public fixed byte pipelineCacheUUID[(int)Vk.VK_UUID_SIZE];

    /// <summary>
    /// The <see cref="VkPhysicalDeviceLimits"/> structure specifying device-specific limits of the physical device. See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#limits">Limits</see> for details.
    /// </summary>
    public VkPhysicalDeviceLimits limits;

    /// <summary>
    /// the <see cref="VkPhysicalDeviceSparseProperties"/> structure specifying various sparse related properties of the physical device. See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#sparsememory-physicalprops">Sparse Properties</see> for details.
    /// </summary>
    public VkPhysicalDeviceSparseProperties sparseProperties;
}
