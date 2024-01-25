namespace ThirdParty.Vulkan.Flags;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkDependencyFlagBits.html">VkDependencyFlagBits</see>
/// </summary>
[Flags]
public enum DependencyFlags
{
    ByRegion        = 0x00000001,
    DeviceGroup     = 0x00000004,
    ViewLocal       = 0x00000002,
    FeedbackLoopExt = 0x00000008,
    ViewLocalKhr    = ViewLocal,
    DeviceGroupKhr  = DeviceGroup,
}
