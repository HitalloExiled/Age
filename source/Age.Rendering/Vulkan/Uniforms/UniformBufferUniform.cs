using Age.Rendering.Enums;
using Age.Rendering.Vulkan.Handlers;

namespace Age.Rendering.Vulkan.Uniforms;

public record UniformBufferUniform : Uniform
{
    public override UniformType Type => UniformType.UniformBuffer;

    public required BufferHandler Buffer { get; init; }
}
