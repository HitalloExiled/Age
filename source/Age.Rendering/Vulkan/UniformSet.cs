using Age.Rendering.Vulkan.Handlers;
using Age.Vulkan.Types;

namespace Age.Rendering.Vulkan;

public record UniformSet
{
    public required DescriptorPoolHandler DescriptorPool { get; init; }
    public required VkDescriptorSet[]     DescriptorSets { get; init; }
    public required ShaderHandler         Shader         { get; init; }
}
