namespace ThirdParty.Vulkan.Enums;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkSystemAllocationScope.html">VkSystemAllocationScope</see>
/// </summary>
public enum VkSystemAllocationScope
{
    Command  = 0,
    Object   = 1,
    Cache    = 2,
    Device   = 3,
    Instance = 4,
}
