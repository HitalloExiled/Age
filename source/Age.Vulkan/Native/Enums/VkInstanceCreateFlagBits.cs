namespace Age.Vulkan.Native.Enums;

/// <summary>
/// Bitmask specifying behavior of the instance.
/// </summary>
[Flags]
public enum VkInstanceCreateFlagBits : uint
{
    /// <summary>
    /// Specifies that the instance will enumerate available Vulkan Portability-compliant physical devices and groups in addition to the Vulkan physical devices and groups that are enumerated by default.
    /// </summary>
    /// <remarks>Provided by VK_KHR_portability_enumeration</remarks>
    VK_INSTANCE_CREATE_ENUMERATE_PORTABILITY_BIT_KHR = 0x00000001,
}
