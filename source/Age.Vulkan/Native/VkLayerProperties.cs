namespace Age.Vulkan.Native;

/// <summary>
/// Structure specifying layer properties.
/// </summary>
public unsafe struct VkLayerProperties
{
    /// <summary>
    /// An array of <see cref="Vk.VK_MAX_EXTENSION_NAME_SIZE"/> char containing a null-terminated UTF-8 string which is the name of the layer. Use this name in the ppEnabledLayerNames array passed in the <see cref="VkInstanceCreateInfo"/> structure to enable this layer for an instance.
    /// </summary>
    public fixed byte layerName[(int)Vk.VK_MAX_EXTENSION_NAME_SIZE];

    /// <summary>
    /// The Vulkan version the layer was written to, encoded as described in https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#extendingvulkan-coreversions-versionnumbers.
    /// </summary>
    public uint specVersion;

    /// <summary>
    /// The version of this layer. It is an integer, increasing with backward compatible changes.
    /// </summary>
    public uint implementationVersion;

    /// <summary>
    /// An array of <see cref="Vk.VK_MAX_DESCRIPTION_SIZE"/> char containing a null-terminated UTF-8 string which provides additional details that can be used by the application to identify the layer.
    /// </summary>
    public fixed byte description[(int)Vk.VK_MAX_DESCRIPTION_SIZE];
}
