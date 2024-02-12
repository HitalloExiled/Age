using Age.Rendering.Resources;

namespace Age.Rendering.Vulkan.Uniforms;

public partial record CombinedImageSamplerUniform
{
    public record Image
    {
        public required Sampler Sampler { get; init; }
        public required Texture Texture { get; init; }
    }
}
