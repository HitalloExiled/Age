namespace ThirdParty.Vulkan.Enums;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkDescriptorType.html">VkDescriptorType</see>
/// </summary>
public enum DescriptorType
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
    AccelerationStructureKhr = 1000150000,
    AccelerationStructureNv  = 1000165000,
    SampleWeightImageQcom    = 1000440000,
    BlockMatchImageQcom      = 1000440001,
    MutableExt               = 1000351000,
    InlineUniformBlockExt    = InlineUniformBlock,
    MutableValve             = MutableExt,
}
