using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public static class PipelineRasterizationState
{
    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPipelineRasterizationStateCreateInfo.html">VkPipelineRasterizationStateCreateInfo</see>
    /// </summary>
    public unsafe record CreateInfo : NativeReference<VkPipelineRasterizationStateCreateInfo>
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

        public bool DepthClampEnable
        {
            get => this.PNative->depthClampEnable;
            init => this.PNative->depthClampEnable = value;
        }

        public bool RasterizerDiscardEnable
        {
            get => this.PNative->rasterizerDiscardEnable;
            init => this.PNative->rasterizerDiscardEnable = value;
        }

        public PolygonMode PolygonMode
        {
            get => this.PNative->polygonMode;
            init => this.PNative->polygonMode = value;
        }

        public CullModeFlags CullMode
        {
            get => this.PNative->cullMode;
            init => this.PNative->cullMode = value;
        }

        public FrontFace FrontFace
        {
            get => this.PNative->frontFace;
            init => this.PNative->frontFace = value;
        }

        public bool DepthBiasEnable
        {
            get => this.PNative->depthBiasEnable;
            init => this.PNative->depthBiasEnable = value;
        }

        public float DepthBiasConstantFactor
        {
            get => this.PNative->depthBiasConstantFactor;
            init => this.PNative->depthBiasConstantFactor = value;
        }

        public float DepthBiasClamp
        {
            get => this.PNative->depthBiasClamp;
            init => this.PNative->depthBiasClamp = value;
        }

        public float DepthBiasSlopeFactor
        {
            get => this.PNative->depthBiasSlopeFactor;
            init => this.PNative->depthBiasSlopeFactor = value;
        }

        public float LineWidth
        {
            get => this.PNative->lineWidth;
            init => this.PNative->lineWidth = value;
        }
    }
}
