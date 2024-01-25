namespace ThirdParty.Vulkan.Flags;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkCullModeFlagBits.html">VkCullModeFlagBits</see>
/// </summary>
[Flags]
public enum CullModeFlags
{
    None         = 0,
    Front        = 0x00000001,
    Back         = 0x00000002,
    FrontAndBack = 0x00000003,
}
