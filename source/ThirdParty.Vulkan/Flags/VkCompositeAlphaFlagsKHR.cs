namespace ThirdParty.Vulkan.Flags;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkCompositeAlphaFlagBitsKHR.html">VkCompositeAlphaFlagBitsKHR</see>
/// </summary>
[Flags]
public enum VkCompositeAlphaFlagsKHR
{
    Opaque         = 0x00000001,
    PreMultiplied  = 0x00000002,
    PostMultiplied = 0x00000004,
    Inherit        = 0x00000008,
}
