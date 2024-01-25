using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public partial class DescriptorSetLayout
{
    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkDescriptorSetLayoutCreateInfo.html">VkDescriptorSetLayoutCreateInfo</see>
    /// </summary>
    public unsafe record CreateInfo : NativeReference<VkDescriptorSetLayoutCreateInfo>
    {
        private DescriptorSetLayoutBinding[] bindings = [];

        public nint Next
        {
            get => (nint)this.PNative->pNext;
            init => this.PNative->pNext = value.ToPointer();
        }

        public VkDescriptorSetLayoutCreateFlags Flags
        {
            get => this.PNative->flags;
            init => this.PNative->flags = value;
        }

        public DescriptorSetLayoutBinding[] Bindings
        {
            get => this.bindings;
            init => Init(ref this.bindings, ref this.PNative->pBindings, ref this.PNative->bindingCount, value);
        }

        protected override void OnFinalize() =>
            Free(ref this.PNative->pBindings);
    }
}
