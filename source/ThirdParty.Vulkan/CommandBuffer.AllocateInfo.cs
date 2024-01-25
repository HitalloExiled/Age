using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public unsafe partial class CommandBuffer
{    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkCommandBufferAllocateInfo.html">VkCommandBufferAllocateInfo</see>
    /// </summary>
    public unsafe record AllocateInfo : NativeReference<VkCommandBufferAllocateInfo>
    {
        public nint Next
        {
            get => (nint)this.PNative->pNext;
            init => this.PNative->pNext = value.ToPointer();
        }

        public VkCommandBufferLevel Level
        {
            get => this.PNative->level;
            init => this.PNative->level = value;
        }

        public uint CommandBufferCount
        {
            get => this.PNative->commandBufferCount;
            init => this.PNative->commandBufferCount = value;
        }

        internal void SetCommandPool(VkCommandPool commandPool) =>
            this.PNative->commandPool = commandPool;
    }
}
