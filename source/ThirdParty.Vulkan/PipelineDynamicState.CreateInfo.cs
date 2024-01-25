using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public partial class PipelineDynamicState
{
    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPipelineDynamicStateCreateInfo.html">VkPipelineDynamicStateCreateInfo</see>
    /// </summary>
    public unsafe record CreateInfo : NativeReference<VkPipelineDynamicStateCreateInfo>
    {
        private VkDynamicState[] dynamicStates = [];

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

        public uint DynamicStateCount
        {
            get => this.PNative->dynamicStateCount;
            init => this.PNative->dynamicStateCount = value;
        }

        public DynamicState[] DynamicStates
        {
            get => this.dynamicStates;
            init => Init(ref this.dynamicStates, ref this.PNative->pDynamicStates, ref this.PNative->dynamicStateCount, value);
        }

        protected override void OnFinalize() =>
            Free(ref this.PNative->pDynamicStates);
    }
}
