using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

/// <inheritdoc cref="VkMemoryHeap" />
public record MemoryHeap : NativeReference<VkMemoryHeap>
{
    internal MemoryHeap(in VkMemoryHeap value) : base(value) { }
}
