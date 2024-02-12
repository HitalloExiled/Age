using Age.Rendering.Enums;

using Buffer = Age.Rendering.Resources.Buffer;

namespace Age.Rendering.Vulkan.Uniforms;

public record UniformBufferUniform : Uniform
{
    public override UniformType Type => UniformType.UniformBuffer;

    public required Buffer Buffer { get; init; }
}
