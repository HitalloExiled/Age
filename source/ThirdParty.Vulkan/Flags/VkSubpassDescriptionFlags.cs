namespace ThirdParty.Vulkan.Flags;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkSubpassDescriptionFlagBits.html">VkSubpassDescriptionFlagBits</see>
/// </summary>
[Flags]
public enum VkSubpassDescriptionFlags
{
    PerViewAttributesNVX                         = 0x00000001,
    PerViewPositionXOnlyNVX                      = 0x00000002,
    FragmentRegionQCOM                           = 0x00000004,
    ShaderResolveQCOM                            = 0x00000008,
    RasterizationOrderAttachmentColorAccessEXT   = 0x00000010,
    RasterizationOrderAttachmentDepthAccessEXT   = 0x00000020,
    RasterizationOrderAttachmentStencilAccessEXT = 0x00000040,
    EnableLegacyDitheringEXT                     = 0x00000080,
    RasterizationOrderAttachmentColorAccessArm   = RasterizationOrderAttachmentColorAccessEXT,
    RasterizationOrderAttachmentDepthAccessArm   = RasterizationOrderAttachmentDepthAccessEXT,
    RasterizationOrderAttachmentStencilAccessArm = RasterizationOrderAttachmentStencilAccessEXT,
}
