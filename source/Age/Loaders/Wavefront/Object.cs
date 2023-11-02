namespace Age.Loaders.Wavefront;

public record Object(string Name)
{
    public Mesh Mesh { get; init; } = new();
}
