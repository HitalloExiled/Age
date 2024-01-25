namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkMemoryType.html">VkMemoryType</see>
/// </summary>
public struct VkMemoryType
{
    public VkMemoryPropertyFlags propertyFlags;
    public uint                  heapIndex;
}
