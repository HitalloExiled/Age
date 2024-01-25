using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public partial class Buffer
{
    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkBufferCreateInfo.html">VkBufferCreateInfo</see>
    /// </summary>
    public unsafe record CreateInfo : NativeReference<VkBufferCreateInfo>
    {
        private uint[] queueFamilyIndices = [];

        public nint Next
        {
            get => (nint)this.PNative->pNext;
            init => this.PNative->pNext = value.ToPointer();
        }

        public BufferCreateFlags Flags

        {
            get => this.PNative->flags;
            init => this.PNative->flags = value;
        }

        public ulong Size
        {
            get => this.PNative->size;
            init => this.PNative->size = value;
        }

        public BufferUsageFlags Usage
        {
            get => this.PNative->usage;
            init => this.PNative->usage = value;
        }

        public SharingMode SharingMode
        {
            get => this.PNative->sharingMode;
            init => this.PNative->sharingMode = value;
        }

        public uint[] QueueFamilyIndices
        {
            get => this.queueFamilyIndices;
            init => Init(ref this.queueFamilyIndices, ref this.PNative->pQueueFamilyIndices, ref this.PNative->queueFamilyIndexCount, value);
        }

        protected override void OnFinalize() =>
            Free(ref this.PNative->pQueueFamilyIndices);
    }
}
