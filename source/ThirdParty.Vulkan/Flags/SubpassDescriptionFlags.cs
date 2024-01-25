namespace ThirdParty.Vulkan.Flags;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkSubpassDescriptionFlagBits.html">VkSubpassDescriptionFlagBits</see>
/// </summary>
[Flags]
public enum SubpassDescriptionFlags
{
    PerViewAttributesNvx                         = 0x00000001,
    PerViewPositionXOnlyNvx                      = 0x00000002,
    FragmentRegionQcom                           = 0x00000004,
    ShaderResolveQcom                            = 0x00000008,
    RasterizationOrderAttachmentColorAccessExt   = 0x00000010,
    RasterizationOrderAttachmentDepthAccessExt   = 0x00000020,
    RasterizationOrderAttachmentStencilAccessExt = 0x00000040,
    EnableLegacyDitheringExt                     = 0x00000080,
    RasterizationOrderAttachmentColorAccessArm   = RasterizationOrderAttachmentColorAccessExt,
    RasterizationOrderAttachmentDepthAccessArm   = RasterizationOrderAttachmentDepthAccessExt,
    RasterizationOrderAttachmentStencilAccessArm = RasterizationOrderAttachmentStencilAccessExt,
}
