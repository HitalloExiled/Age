using Age.Rendering.Enums;

namespace Age.Rendering.Vulkan.Uniforms;

public partial record CombinedImageSamplerUniform : Uniform
{
    public override UniformType Type => UniformType.CombinedImageSampler;

    public required List<Image> Images { get; init; } = [];
}
