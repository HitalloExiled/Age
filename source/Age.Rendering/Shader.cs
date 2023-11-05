using Age.Rendering.Vulkan.Handlers;

namespace Age.Rendering;

public record Shader
{
    public required ShaderHandler Handler { get; init; }
}
