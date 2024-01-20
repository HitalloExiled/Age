using Age.Rendering.Vulkan.Handlers;
using Age.Vulkan.Types;

namespace Age.Rendering.Vulkan.Uniforms;

public partial record CombinedImageSamplerUniform
{
    public record Image
    {
        public required VkSampler      Sampler { get; init; }
        public required TextureHandler Texture { get; init; }
    }
}
