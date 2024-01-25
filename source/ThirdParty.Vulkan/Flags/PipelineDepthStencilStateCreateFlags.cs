namespace ThirdParty.Vulkan.Flags;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPipelineDepthStencilStateCreateFlagBits.html">VkPipelineDepthStencilStateCreateFlagBits</see>
/// </summary>
[Flags]
public enum PipelineDepthStencilStateCreateFlags
{
    RasterizationOrderAttachmentDepthAccessExt   = 0x00000001,
    RasterizationOrderAttachmentStencilAccessExt = 0x00000002,
    RasterizationOrderAttachmentDepthAccessArm   = RasterizationOrderAttachmentDepthAccessExt,
    RasterizationOrderAttachmentStencilAccessArm = RasterizationOrderAttachmentStencilAccessExt,
}
