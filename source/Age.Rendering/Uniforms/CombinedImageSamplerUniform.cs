using Age.Rendering.Resources;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Uniforms;

public sealed partial record CombinedImageSamplerUniform : Uniform
{
    public override VkDescriptorType Type => VkDescriptorType.CombinedImageSampler;

    public required VkImage       Image       { get; init; }
    public required VkImageLayout ImageLayout { get; init; }
    public required VkImageView   ImageView   { get; init; }
    public required VkSampler     Sampler     { get; init; }
}
