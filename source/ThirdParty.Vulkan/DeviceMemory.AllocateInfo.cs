using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkMemoryAllocateInfo.html">VkMemoryAllocateInfo</see>
/// </summary>
public unsafe partial class DeviceMemory
{
    public record AllocateInfo : NativeReference<VkMemoryAllocateInfo>
    {
        public nint Next
        {
            get => (nint)this.PNative->pNext;
            init => this.PNative->pNext = value.ToPointer();
        }

        public ulong AllocationSize
        {
            get => this.PNative->allocationSize;
            init => this.PNative->allocationSize = value;
        }

        public uint MemoryTypeIndex
        {
            get => this.PNative->memoryTypeIndex;
            init => this.PNative->memoryTypeIndex = value;
        }
    }
}
