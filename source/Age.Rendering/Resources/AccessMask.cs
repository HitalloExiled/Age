using ThirdParty.Vulkan.Flags;

namespace Age.Rendering.Resources;

#pragma warning disable CA1069

[Flags]
public enum AccessMask : uint
{
    None                                 = VkAccessFlags.None,
    NoneKHR                              = VkAccessFlags.NoneKHR,
    IndirectCommandRead                  = VkAccessFlags.IndirectCommandRead,
    IndexRead                            = VkAccessFlags.IndexRead,
    VertexAttributeRead                  = VkAccessFlags.VertexAttributeRead,
    UniformRead                          = VkAccessFlags.UniformRead,
    InputAttachmentRead                  = VkAccessFlags.InputAttachmentRead,
    ShaderRead                           = VkAccessFlags.ShaderRead,
    ShaderWrite                          = VkAccessFlags.ShaderWrite,
    ColorAttachmentRead                  = VkAccessFlags.ColorAttachmentRead,
    ColorAttachmentWrite                 = VkAccessFlags.ColorAttachmentWrite,
    DepthStencilAttachmentRead           = VkAccessFlags.DepthStencilAttachmentRead,
    DepthStencilAttachmentWrite          = VkAccessFlags.DepthStencilAttachmentWrite,
    TransferRead                         = VkAccessFlags.TransferRead,
    TransferWrite                        = VkAccessFlags.TransferWrite,
    HostRead                             = VkAccessFlags.HostRead,
    HostWrite                            = VkAccessFlags.HostWrite,
    MemoryRead                           = VkAccessFlags.MemoryRead,
    MemoryWrite                          = VkAccessFlags.MemoryWrite,
    CommandPreprocessReadNV              = VkAccessFlags.CommandPreprocessReadNV,
    CommandPreprocessWriteNV             = VkAccessFlags.CommandPreprocessWriteNV,
    ColorAttachmentReadNoncoherentEXT    = VkAccessFlags.ColorAttachmentReadNoncoherentEXT,
    ConditionalRenderingReadEXT          = VkAccessFlags.ConditionalRenderingReadEXT,
    AccelerationStructureReadKHR         = VkAccessFlags.AccelerationStructureReadKHR,
    AccelerationStructureReadNV          = VkAccessFlags.AccelerationStructureReadNV,
    AccelerationStructureWriteKHR        = VkAccessFlags.AccelerationStructureWriteKHR,
    AccelerationStructureWriteNV         = VkAccessFlags.AccelerationStructureWriteNV,
    FragmentShadingRateAttachmentReadKHR = VkAccessFlags.FragmentShadingRateAttachmentReadKHR,
    ShadingRateImageReadNV               = VkAccessFlags.ShadingRateImageReadNV,
    FragmentDensityMapReadEXT            = VkAccessFlags.FragmentDensityMapReadEXT,
    TransformFeedbackWriteEXT            = VkAccessFlags.TransformFeedbackWriteEXT,
    TransformFeedbackCounterReadEXT      = VkAccessFlags.TransformFeedbackCounterReadEXT,
    TransformFeedbackCounterWriteEXT     = VkAccessFlags.TransformFeedbackCounterWriteEXT,
}
