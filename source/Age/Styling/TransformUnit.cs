using Age.Numerics;

namespace Age.Styling;

public record TransformUnit
{
    public PointUnit?    Position { get; init; }
    public float         Rotation { get; init; }
    public Point<float>  Scale    { get; init; } = new(1);

    public override string ToString() =>
        $"Position: {this.Position}, Rotation: {this.Rotation}, Scale: {this.Scale}";
}
