using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public unsafe partial class PhysicalDevice
{
    /// <inheritdoc cref="VkPhysicalDeviceMemoryProperties" />
    public record MemoryProperties : NativeReference<VkPhysicalDeviceMemoryProperties>
    {
        private MemoryType[] memoryTypes            = [];
        private MemoryHeap[] memoryHeapsMemoryTypes = [];

        public MemoryType[] MemoryTypes => Get(ref this.memoryTypes,            (VkMemoryType*)this.PNative->memoryTypes, this.PNative->memoryTypeCount, static x => new(x));
        public MemoryHeap[] MemoryHeaps => Get(ref this.memoryHeapsMemoryTypes, (VkMemoryHeap*)this.PNative->memoryHeaps, this.PNative->memoryHeapCount, static x => new(x));

        internal MemoryProperties(in VkPhysicalDeviceMemoryProperties value) : base(value) { }
    }
}
