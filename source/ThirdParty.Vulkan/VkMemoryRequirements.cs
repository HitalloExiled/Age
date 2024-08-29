namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkMemoryRequirements.html">VkMemoryRequirements</see>
/// </summary>
public struct VkMemoryRequirements
{
    public VkDeviceSize Size;
    public VkDeviceSize Alignment;
    public uint         MemoryTypeBits;
}
