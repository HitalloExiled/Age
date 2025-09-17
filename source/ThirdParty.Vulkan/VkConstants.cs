namespace ThirdParty.Vulkan;

public static class VkConstants
{
    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VK_ATTACHMENT_UNUSED.html">VK_ATTACHMENT_UNUSED</see>
    /// </summary>
    public const uint VK_ATTACHMENT_UNUSED = uint.MaxValue;

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VK_MAX_DESCRIPTION_SIZE.html">VK_MAX_DESCRIPTION_SIZE</see>
    /// </summary>
    public const uint VK_MAX_DESCRIPTION_SIZE = 256;

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VK_MAX_EXTENSION_NAME_SIZE.html">VK_MAX_EXTENSION_NAME_SIZE</see>
    /// </summary>
    public const uint VK_MAX_EXTENSION_NAME_SIZE = 256;

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VK_MAX_MEMORY_HEAPS.html">VK_MAX_MEMORY_HEAPS</see>
    /// </summary>
    public const uint VK_MAX_MEMORY_HEAPS = 16;

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VK_MAX_MEMORY_TYPES.html">VK_MAX_MEMORY_TYPES</see>
    /// </summary>
    public const uint VK_MAX_MEMORY_TYPES = 32;

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VK_MAX_PHYSICAL_DEVICE_NAME_SIZE.html">VK_MAX_PHYSICAL_DEVICE_NAME_SIZE</see>
    /// </summary>
    public const uint VK_MAX_PHYSICAL_DEVICE_NAME_SIZE = 256;

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VK_QUEUE_FAMILY_IGNORED.html">VK_QUEUE_FAMILY_IGNORED</see>
    /// </summary>
    public const uint VK_QUEUE_FAMILY_IGNORED = uint.MaxValue;

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VK_SUBPASS_EXTERNAL.html">VK_SUBPASS_EXTERNAL</see>
    /// </summary>
    public const uint VK_SUBPASS_EXTERNAL = uint.MaxValue;

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VK_UUID_SIZE.html">VK_UUID_SIZE</see>
    /// </summary>
    public const uint VK_UUID_SIZE = 16;

    public const string VK_LAYER_KHRONOS_VALIDATION = "VK_LAYER_KHRONOS_validation";
}
