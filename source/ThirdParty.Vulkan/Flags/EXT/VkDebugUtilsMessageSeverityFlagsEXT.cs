namespace ThirdParty.Vulkan.Flags.EXT;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkDebugUtilsMessageSeverityFlagBitsEXT.html">VkDebugUtilsMessageSeverityFlagBitsEXT</see>
/// </summary>
[Flags]
public enum VkDebugUtilsMessageSeverityFlagsEXT
{
    Verbose = 0x00000001,
    Info    = 0x00000010,
    Warning = 0x00000100,
    Error   = 0x00001000,
}
