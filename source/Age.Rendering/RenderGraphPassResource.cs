namespace Age.Rendering;

public abstract class RenderGraphPassResource
{
    public required string       Name { get; init; }
    public required ResourceType Type { get; init; }
}
