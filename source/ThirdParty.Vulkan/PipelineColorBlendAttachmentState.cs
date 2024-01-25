using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPipelineColorBlendAttachmentState.html">VkPipelineColorBlendAttachmentState</see>
/// </summary>
public unsafe record PipelineColorBlendAttachmentState : NativeReference<VkPipelineColorBlendAttachmentState>
{
    public bool BlendEnable
    {
        get => this.PNative->blendEnable;
        init => this.PNative->blendEnable = value;
    }

    public BlendFactor SrcColorBlendFactor
    {
        get => this.PNative->srcColorBlendFactor;
        init => this.PNative->srcColorBlendFactor = value;
    }

    public BlendFactor DstColorBlendFactor
    {
        get => this.PNative->dstColorBlendFactor;
        init => this.PNative->dstColorBlendFactor = value;
    }

    public BlendOp ColorBlendOp
    {
        get => this.PNative->colorBlendOp;
        init => this.PNative->colorBlendOp = value;
    }

    public BlendFactor SrcAlphaBlendFactor
    {
        get => this.PNative->srcAlphaBlendFactor;
        init => this.PNative->srcAlphaBlendFactor = value;
    }

    public BlendFactor DstAlphaBlendFactor
    {
        get => this.PNative->dstAlphaBlendFactor;
        init => this.PNative->dstAlphaBlendFactor = value;
    }

    public BlendOp AlphaBlendOp
    {
        get => this.PNative->alphaBlendOp;
        init => this.PNative->alphaBlendOp = value;
    }

    public ColorComponentFlags ColorWriteMask
    {
        get => this.PNative->colorWriteMask;
        init => this.PNative->colorWriteMask = value;
    }
}
