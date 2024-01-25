namespace ThirdParty.Vulkan.Flags;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkAccessFlagBits.html">VkAccessFlagBits</see>
/// </summary>
[Flags]
public enum AccessFlags
{
    IndirectCommandRead                  = 0x00000001,
    IndexRead                            = 0x00000002,
    VertexAttributeRead                  = 0x00000004,
    UniformRead                          = 0x00000008,
    InputAttachmentRead                  = 0x00000010,
    ShaderRead                           = 0x00000020,
    ShaderWrite                          = 0x00000040,
    ColorAttachmentRead                  = 0x00000080,
    ColorAttachmentWrite                 = 0x00000100,
    DepthStencilAttachmentRead           = 0x00000200,
    DepthStencilAttachmentWrite          = 0x00000400,
    TransferRead                         = 0x00000800,
    TransferWrite                        = 0x00001000,
    HostRead                             = 0x00002000,
    HostWrite                            = 0x00004000,
    MemoryRead                           = 0x00008000,
    MemoryWrite                          = 0x00010000,
    None                                 = 0,
    TransformFeedbackWriteExt            = 0x02000000,
    TransformFeedbackCounterReadExt      = 0x04000000,
    TransformFeedbackCounterWriteExt     = 0x08000000,
    ConditionalRenderingReadExt          = 0x00100000,
    ColorAttachmentReadNoncoherentExt    = 0x00080000,
    AccelerationStructureReadKhr         = 0x00200000,
    AccelerationStructureWriteKhr        = 0x00400000,
    FragmentDensityMapReadExt            = 0x01000000,
    FragmentShadingRateAttachmentReadKhr = 0x00800000,
    CommandPreprocessReadNv              = 0x00020000,
    CommandPreprocessWriteNv             = 0x00040000,
    ShadingRateImageReadNv               = FragmentShadingRateAttachmentReadKhr,
    AccelerationStructureReadNv          = AccelerationStructureReadKhr,
    AccelerationStructureWriteNv         = AccelerationStructureWriteKhr,
    NoneKhr                              = None,
}
