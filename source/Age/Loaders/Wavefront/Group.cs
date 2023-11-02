namespace Age.Loaders.Wavefront;

public record Group(string Name)
{
    public List<Face> Faces    { get; init; } = [];
    public List<Line> Lines    { get; init; } = [];
    public List<int>  Vertices { get; init; } = [];
}
