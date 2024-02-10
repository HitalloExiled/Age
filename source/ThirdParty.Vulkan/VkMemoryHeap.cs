using ThirdParty.Vulkan.Flags;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkMemoryHeap.html">VkMemoryHeap</see>
/// </summary>
public struct VkMemoryHeap
{
    public VkDeviceSize      Size;
    public VkMemoryHeapFlags Flags;
}
