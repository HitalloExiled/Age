using Age.Rendering.Resources;
using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Vulkan.Uniforms;

public partial record CombinedImageSamplerUniform : Uniform
{
    public override VkDescriptorType Type => VkDescriptorType.CombinedImageSampler;

    public required Texture Texture { get; init; }
    public required Sampler Sampler { get; init; }
}
