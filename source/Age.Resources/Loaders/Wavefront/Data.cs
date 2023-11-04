namespace Age.Resources.Loaders.Wavefront;

public record Data
{
    public Attributes   Attributes { get; init; } = new();
    public List<Object> Objects    { get; init; } = [];
}
