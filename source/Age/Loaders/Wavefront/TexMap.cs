using Age.Numerics;

namespace Age.Loaders.Wavefront;

public record TextureMap
{
    public string         ImagePath      { get; set; } = "";
    public Vector3<float> Translation    { get; set; }
    public Vector3<float> Scale          { get; set; } = new(1);
    public ProjectionType ProjectionType { get; set; } = ProjectionType.Flat;
}
