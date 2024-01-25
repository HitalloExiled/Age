namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkQueueFamilyProperties.html">VkQueueFamilyProperties</see>
/// </summary>
public struct VkQueueFamilyProperties
{
    public VkQueueFlags queueFlags;
    public uint         queueCount;
    public uint         timestampValidBits;
    public VkExtent3D   minImageTransferGranularity;
}
