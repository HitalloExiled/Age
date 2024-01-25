namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPipelineColorBlendAttachmentState.html">VkPipelineColorBlendAttachmentState</see>
/// </summary>
public struct VkPipelineColorBlendAttachmentState
{
    public VkBool32              blendEnable;
    public VkBlendFactor         srcColorBlendFactor;
    public VkBlendFactor         dstColorBlendFactor;
    public VkBlendOp             colorBlendOp;
    public VkBlendFactor         srcAlphaBlendFactor;
    public VkBlendFactor         dstAlphaBlendFactor;
    public VkBlendOp             alphaBlendOp;
    public VkColorComponentFlags colorWriteMask;
}
