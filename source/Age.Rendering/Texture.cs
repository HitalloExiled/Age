using Age.Rendering.Enums;
using Age.Rendering.Vulkan.Handlers;

namespace Age.Rendering;

public record Texture
{
    public required uint           Depth       { get; init; }
    public required TextureHandler Handler     { get; init; }
    public required Image          Image       { get; init; }
    public required Sampler        Sampler     { get; init; }
    public required TextureType    TextureType { get; init; }
}
