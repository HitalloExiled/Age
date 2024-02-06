using ThirdParty.Vulkan.Flags;

namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkQueueFamilyProperties.html">VkQueueFamilyProperties</see>
/// </summary>
public struct VkQueueFamilyProperties
{
    public VkQueueFlags QueueFlags;
    public uint         QueueCount;
    public uint         TimestampValidBits;
    public VkExtent3D   MinImageTransferGranularity;
}
