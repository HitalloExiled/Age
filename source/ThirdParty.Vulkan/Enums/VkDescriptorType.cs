namespace ThirdParty.Vulkan.Enums;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkDescriptorType.html">VkDescriptorType</see>
/// </summary>
public enum VkDescriptorType
{
    Sampler                  = 0,
    CombinedImageSampler     = 1,
    SampledImage             = 2,
    StorageImage             = 3,
    UniformTexelBuffer       = 4,
    StorageTexelBuffer       = 5,
    UniformBuffer            = 6,
    StorageBuffer            = 7,
    UniformBufferDynamic     = 8,
    StorageBufferDynamic     = 9,
    InputAttachment          = 10,
    InlineUniformBlock       = 1000138000,
    AccelerationStructureKHR = 1000150000,
    AccelerationStructureNV  = 1000165000,
    SampleWeightImageQCOM    = 1000440000,
    BlockMatchImageQCOM      = 1000440001,
    MutableEXT               = 1000351000,
    InlineUniformBlockEXT    = InlineUniformBlock,
    MutableValve             = MutableEXT,
}
