namespace ThirdParty.Vulkan.Flags;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPipelineDepthStencilStateCreateFlagBits.html">VkPipelineDepthStencilStateCreateFlagBits</see>
/// </summary>
[Flags]
public enum VkPipelineDepthStencilStateCreateFlags
{
    RasterizationOrderAttachmentDepthAccessEXT   = 0x00000001,
    RasterizationOrderAttachmentStencilAccessEXT = 0x00000002,
    RasterizationOrderAttachmentDepthAccessArm   = RasterizationOrderAttachmentDepthAccessEXT,
    RasterizationOrderAttachmentStencilAccessArm = RasterizationOrderAttachmentStencilAccessEXT,
}
