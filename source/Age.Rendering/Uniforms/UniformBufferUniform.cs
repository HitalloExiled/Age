using ThirdParty.Vulkan.Enums;
using Buffer = Age.Rendering.Resources.Buffer;

namespace Age.Rendering.Uniforms;

public sealed record UniformBufferUniform : Uniform
{
    public override VkDescriptorType Type => VkDescriptorType.UniformBuffer;

    public required Buffer Buffer { get; init; }
}
