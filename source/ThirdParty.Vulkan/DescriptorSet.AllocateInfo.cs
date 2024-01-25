using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public partial class DescriptorSet
{
    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkDescriptorSetAllocateInfo.html">VkDescriptorSetAllocateInfo</see>
    /// </summary>
    public unsafe record AllocateInfo : NativeReference<VkDescriptorSetAllocateInfo>
    {
        private DescriptorSetLayout[] setLayouts = [];

        public nint Next
        {
            get => (nint)this.PNative->pNext;
            init => this.PNative->pNext = value.ToPointer();
        }

        public DescriptorSetLayout[] SetLayouts
        {
            get => this.setLayouts;
            init => Init(ref this.setLayouts, ref this.PNative->pSetLayouts, ref this.PNative->descriptorSetCount, value);
        }

        protected override void OnFinalize() =>
            Free(ref this.PNative->pSetLayouts);
    }
}
