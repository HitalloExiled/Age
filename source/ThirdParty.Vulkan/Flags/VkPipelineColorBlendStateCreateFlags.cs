namespace ThirdParty.Vulkan.Flags;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/PipelineColorBlendStateCreateFlagBits.html">PipelineColorBlendStateCreateFlagBits</see>
/// </summary>
[Flags]
public enum VkPipelineColorBlendStateCreateFlags
{
    RasterizationOrderAttachmentAccessEXT = 0x00000001,
    RasterizationOrderAttachmentAccessArm = RasterizationOrderAttachmentAccessEXT,
}
