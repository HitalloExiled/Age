using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

/// <inheritdoc cref="VkMemoryHeap" />
public unsafe record MemoryHeap : NativeReference<VkMemoryHeap>
{
    public ulong Size            => this.PNative->size;
    public MemoryHeapFlags Flags => this.PNative->flags;

    internal MemoryHeap(in VkMemoryHeap value) : base(value) { }
}
