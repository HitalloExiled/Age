namespace ThirdParty.Vulkan.Flags;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkSampleCountFlagBits.html">VkSampleCountFlagBits</see>
/// </summary>
[Flags]
public enum VkSampleCountFlags
{
    N1 = 0x00000001,
    N2 = 0x00000002,
    N4 = 0x00000004,
    N8 = 0x00000008,
    N16 = 0x00000010,
    N32 = 0x00000020,
    N64 = 0x00000040,
}
