namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPhysicalDeviceProperties.html">VkPhysicalDeviceProperties</see>
/// </summary>
public unsafe struct VkPhysicalDeviceProperties
{
    public uint                             apiVersion;
    public uint                             driverVersion;
    public uint                             vendorID;
    public uint                             deviceID;
    public VkPhysicalDeviceType             deviceType;
    public fixed byte                       deviceName[(int)Constants.VK_MAX_PHYSICAL_DEVICE_NAME_SIZE];
    public fixed byte                       pipelineCacheUUID[(int)Constants.VK_UUID_SIZE];
    public VkPhysicalDeviceLimits           limits;
    public VkPhysicalDeviceSparseProperties sparseProperties;
}
