namespace ThirdParty.Vulkan.Enums;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPhysicalDeviceType.html">VkPhysicalDeviceType</see>
/// </summary>
public enum VkPhysicalDeviceType
{
    Other         = 0,
    IntegratedGpu = 1,
    DiscreteGpu   = 2,
    VirtualGpu    = 3,
    Cpu           = 4,
}
