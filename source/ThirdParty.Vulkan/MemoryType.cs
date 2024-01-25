using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

/// <inheritdoc cref="VkMemoryType" />
public unsafe record MemoryType : NativeReference<VkMemoryType>
{
    public MemoryPropertyFlags PropertyFlags => this.PNative->propertyFlags;
    public uint                HeapIndex     => this.PNative->heapIndex;

    internal MemoryType(in VkMemoryType value) : base(value) { }
}
