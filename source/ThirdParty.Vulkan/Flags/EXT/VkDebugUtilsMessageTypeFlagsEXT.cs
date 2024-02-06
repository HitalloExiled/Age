namespace ThirdParty.Vulkan.Flags.EXT;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkDebugUtilsMessageTypeFlagBitsEXT.html">VkDebugUtilsMessageTypeFlagBitsEXT</see>
/// </summary>
[Flags]
public enum VkDebugUtilsMessageTypeFlagsEXT
{
    General              = 0x00000001,
    Validation           = 0x00000002,
    Performance          = 0x00000004,
    DeviceAddressBinding = 0x00000008,
}
