namespace Age.Vulkan.Native.Flags.EXT;

/// <summary>
/// Bitmask specifying which severities of events cause a debug messenger callback.
/// </summary>
/// <remarks>Provided by VK_EXT_debug_utils</remarks>
[Flags]
public enum VkDebugUtilsMessageSeverityFlagBitsEXT
{
    /// <summary>
    /// Specifies the most verbose output indicating all diagnostic messages from the Vulkan loader, layers, and drivers should be captured.
    /// </summary>
    VK_DEBUG_UTILS_MESSAGE_SEVERITY_VERBOSE_BIT_EXT = 0x00000001,

    /// <summary>
    /// Specifies an informational message such as resource details that may be handy when debugging an application.
    /// </summary>
    VK_DEBUG_UTILS_MESSAGE_SEVERITY_INFO_BIT_EXT = 0x00000010,

    /// <summary>
    /// Specifies use of Vulkan that may expose an app bug. Such cases may not be immediately harmful, such as a fragment shader outputting to a location with no attachment. Other cases may point to behavior that is almost certainly bad when unintended such as using an image whose memory has not been filled. In general if you see a warning but you know that the behavior is intended/desired, then simply ignore the warning.
    /// </summary>
    VK_DEBUG_UTILS_MESSAGE_SEVERITY_WARNING_BIT_EXT = 0x00000100,

    /// <summary>
    /// Specifies that the application has violated a valid usage condition of the specification.
    /// </summary>
    VK_DEBUG_UTILS_MESSAGE_SEVERITY_ERROR_BIT_EXT = 0x00001000,
}