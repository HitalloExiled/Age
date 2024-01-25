using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public partial class PipelineLayout
{
    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPipelineLayoutCreateInfo.html">VkPipelineLayoutCreateInfo</see>
    /// </summary>
    public unsafe record CreateInfo : NativeReference<VkPipelineLayoutCreateInfo>
    {
        private DescriptorSetLayout[] setLayouts         = [];
        private PushConstantRange[]   pushConstantRanges = [];

        public nint Next
        {
            get => (nint)this.PNative->pNext;
            init => this.PNative->pNext = value.ToPointer();
        }

        public PipelineLayoutCreateFlags Flags
        {
            get => this.PNative->flags;
            init => this.PNative->flags = value;
        }

        public DescriptorSetLayout[] SetLayouts
        {
            get => this.setLayouts;
            init => Init(ref this.setLayouts, ref this.PNative->pSetLayouts, ref this.PNative->setLayoutCount, value);
        }

        public PushConstantRange[] PushConstantRanges
        {
            get => this.pushConstantRanges;
            init => Init(ref this.pushConstantRanges, ref this.PNative->pPushConstantRanges, ref this.PNative->pushConstantRangeCount, value);
        }

        protected override void OnFinalize()
        {
            Free(ref this.PNative->pSetLayouts);
            Free(ref this.PNative->pPushConstantRanges);
        }
    }
}
