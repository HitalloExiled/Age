using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public static class PipelineMultisampleState
{
    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPipelineMultisampleStateCreateInfo.html">VkPipelineMultisampleStateCreateInfo</see>
    /// </summary>
    public unsafe record CreateInfo : NativeReference<VkPipelineMultisampleStateCreateInfo>
    {
        private SampleMask[] sampleMask = [];

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

        public SampleCountFlags RasterizationSamples
        {
            get => this.PNative->rasterizationSamples;
            init => this.PNative->rasterizationSamples = value;
        }

        public bool SampleShadingEnable
        {
            get => this.PNative->sampleShadingEnable;
            init => this.PNative->sampleShadingEnable = value;
        }

        public float MinSampleShading
        {
            get => this.PNative->minSampleShading;
            init => this.PNative->minSampleShading = value;
        }

        public SampleMask[] SampleMask
        {
            get => this.sampleMask;
            init => Init(ref this.sampleMask, ref this.PNative->pSampleMask, value);
        }

        public bool AlphaToCoverageEnable
        {
            get => this.PNative->alphaToCoverageEnable;
            init => this.PNative->alphaToCoverageEnable = value;
        }

        public bool AlphaToOneEnable
        {
            get => this.PNative->alphaToOneEnable;
            init => this.PNative->alphaToOneEnable = value;
        }
    }
}
