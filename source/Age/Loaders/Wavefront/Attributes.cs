using Age.Numerics;

namespace Age.Loaders.Wavefront;

public record Attributes
{
    public List<Color>          Colors    { get; init; } = [];
    public List<Group>          Groups    { get; init; } = [];
    public List<Material>       Materials { get; init; } = [];
    public List<Vector3<float>> Normals   { get; init; } = [];
    public List<Vector3<float>> TexCoords { get; init; } = [];
    public List<Vector4<float>> Vertices  { get; init; } = [];
};
