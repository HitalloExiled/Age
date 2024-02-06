namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkLayerProperties.html">VkLayerProperties</see>
/// </summary>
public unsafe struct VkLayerProperties
{
    public fixed byte LayerName[(int)VkConstants.VK_MAX_EXTENSION_NAME_SIZE];
    public uint       SpecVersion;
    public uint       ImplementationVersion;
    public fixed byte Description[(int)VkConstants.VK_MAX_DESCRIPTION_SIZE];
}
