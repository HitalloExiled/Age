using Age.Rendering.Resources;
using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Uniforms;

public sealed partial record CombinedImageSamplerUniform : Uniform
{
    public override VkDescriptorType Type => VkDescriptorType.CombinedImageSampler;

    public required VkImageLayout ImageLayout { get; init; }
    public required Texture       Texture     { get; init; }
}
