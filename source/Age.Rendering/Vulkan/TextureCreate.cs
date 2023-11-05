using Age.Rendering.Enums;

namespace Age.Rendering.Vulkan;

public record TextureCreate
{
    public required uint[]      Data        { get; init; }
    public required uint        Depth       { get; init; }
    public required DataFormat  Format      { get; init; }
    public required uint        Height      { get; init; }
    public required TextureType TextureType { get; init; }
    public required uint        Width       { get; init; }
}
