namespace ThirdParty.Vulkan.Flags;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkDependencyFlagBits.html">VkDependencyFlagBits</see>
/// </summary>
[Flags]
public enum VkDependencyFlags
{
    ByRegion        = 0x00000001,
    DeviceGroup     = 0x00000004,
    ViewLocal       = 0x00000002,
    FeedbackLoopEXT = 0x00000008,
    ViewLocalKHR    = ViewLocal,
    DeviceGroupKHR  = DeviceGroup,
}
