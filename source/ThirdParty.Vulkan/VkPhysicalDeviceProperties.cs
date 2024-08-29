using ThirdParty.Vulkan.Enums;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPhysicalDeviceProperties.html">VkPhysicalDeviceProperties</see>
/// </summary>
public unsafe struct VkPhysicalDeviceProperties
{
    public uint                             ApiVersion;
    public uint                             DriverVersion;
    public uint                             VendorId;
    public uint                             DeviceId;
    public VkPhysicalDeviceType             DeviceType;
    public fixed byte                       DeviceName[(int)VkConstants.VK_MAX_PHYSICAL_DEVICE_NAME_SIZE];
    public fixed byte                       PipelineCacheUuid[(int)VkConstants.VK_UUID_SIZE];
    public VkPhysicalDeviceLimits           Limits;
    public VkPhysicalDeviceSparseProperties SparseProperties;
}
