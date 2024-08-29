namespace Age.Resources.Loaders.Wavefront;

public record Face
{
    public int              Group        { get; init; } = -1;
    public List<VertexData> Indices      { get; init; } = [];
    public int              Material     { get; init; } = -1;
    public bool             ShadedSmooth { get; init; }
}
