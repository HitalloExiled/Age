using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public static class PipelineDepthStencilState
{
    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPipelineDepthStencilStateCreateInfo.html">VkPipelineDepthStencilStateCreateInfo</see>
    /// </summary>
    public unsafe record CreateInfo : NativeReference<VkPipelineDepthStencilStateCreateInfo>
    {
        private StencilOpState? back;
        private StencilOpState? front;

        public nint Next
        {
            get => (nint)this.PNative->pNext;
            init => this.PNative->pNext = value.ToPointer();
        }

        public PipelineDepthStencilStateCreateFlags Flags
        {
            get => this.PNative->flags;
            init => this.PNative->flags = value;
        }

        public bool DepthTestEnable
        {
            get => this.PNative->depthTestEnable;
            init => this.PNative->depthTestEnable = value;
        }

        public bool DepthWriteEnable
        {
            get => this.PNative->depthWriteEnable;
            init => this.PNative->depthWriteEnable = value;
        }

        public CompareOp DepthCompareOp
        {
            get => this.PNative->depthCompareOp;
            init => this.PNative->depthCompareOp = value;
        }

        public bool DepthBoundsTestEnable
        {
            get => this.PNative->depthBoundsTestEnable;
            init => this.PNative->depthBoundsTestEnable = value;
        }

        public bool StencilTestEnable
        {
            get => this.PNative->stencilTestEnable;
            init => this.PNative->stencilTestEnable = value;
        }

        public StencilOpState? Front
        {
            get => this.front;
            init => this.PNative->front = this.front = value;
        }

        public StencilOpState? Back
        {
            get => this.back;
            init => this.PNative->back = this.back = value;
        }

        public float MinDepthBounds
        {
            get => this.PNative->minDepthBounds;
            init => this.PNative->minDepthBounds = value;
        }

        public float MaxDepthBounds
        {
            get => this.PNative->maxDepthBounds;
            init => this.PNative->maxDepthBounds = value;
        }
    }
}
