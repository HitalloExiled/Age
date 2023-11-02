namespace Age.Loaders.Wavefront;

public record Mesh
{
    public List<Face>   Faces    { get; init; } = [];
    public List<Line>   Lines    { get; init; } = [];
    public HashSet<int> Vertices { get; init; } = [];
}
