using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPipelineColorBlendAttachmentState.html">VkPipelineColorBlendAttachmentState</see>
/// </summary>
public struct VkPipelineColorBlendAttachmentState
{
    public VkBool32              BlendEnable;
    public VkBlendFactor         SrcColorBlendFactor;
    public VkBlendFactor         DstColorBlendFactor;
    public VkBlendOp             ColorBlendOp;
    public VkBlendFactor         SrcAlphaBlendFactor;
    public VkBlendFactor         DstAlphaBlendFactor;
    public VkBlendOp             AlphaBlendOp;
    public VkColorComponentFlags ColorWriteMask;
}
