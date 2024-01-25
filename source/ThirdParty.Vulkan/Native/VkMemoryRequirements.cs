namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkMemoryRequirements.html">VkMemoryRequirements</see>
/// </summary>
public struct VkMemoryRequirements
{
    public VkDeviceSize size;
    public VkDeviceSize alignment;
    public uint         memoryTypeBits;
}
