using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public partial class DescriptorPool
{
    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkDescriptorPoolCreateInfo.html">VkDescriptorPoolCreateInfo</see>
    /// </summary>
    public unsafe record CreateInfo : NativeReference<VkDescriptorPoolCreateInfo>
    {
        private DescriptorPoolSize[] poolSizes = [];
        public nint Next
        {
            get => (nint)this.PNative->pNext;
            init => this.PNative->pNext = value.ToPointer();
        }

        public VkDescriptorPoolCreateFlags Flags
        {
            get => this.PNative->flags;
            init => this.PNative->flags = value;
        }
        public uint MaxSets
        {
            get => this.PNative->maxSets;
            init => this.PNative->maxSets = value;
        }

        public DescriptorPoolSize[] PoolSizes
        {
            get => this.poolSizes;
            init => Init(ref this.poolSizes, ref this.PNative->pPoolSizes, ref this.PNative->poolSizeCount, value);
        }

        protected override void OnFinalize() =>
            Free(ref this.PNative->pPoolSizes);
    }
}
