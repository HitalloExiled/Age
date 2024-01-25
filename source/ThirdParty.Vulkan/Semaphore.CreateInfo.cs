using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public partial class Semaphore
{
    /// <inheritdoc cref="VkSemaphoreCreateInfo" />
    public unsafe record CreateInfo : NativeReference<VkSemaphoreCreateInfo>
    {
        public nint Next
        {
            get => (nint)this.PNative->pNext;
            init => this.PNative->pNext = value.ToPointer();
        }

        public uint Flags
        {
            get => this.PNative->flags;
            init => this.PNative->flags = value;
        }

    }
}
