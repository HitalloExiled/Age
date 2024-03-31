using System.Diagnostics;

namespace Age.Numerics;

[DebuggerDisplay("\\{ Size: {Size}, Position: {Position}, Rotation: {Rotation} \\}")]
public record struct Transform2D
{
    public Vector2<float> Scale = new(1, 1);
    public Vector2<float> Position;
    public float          Rotation;

    public Transform2D() { }

    public Transform2D(Vector2<float> scale, Vector2<float> position, float rotation) : this()
    {
        this.Scale    = scale;
        this.Position = position;
        this.Rotation = rotation;
    }

    public Transform2D(float width, float height, float x, float y, float rotation) : this(new(width, height), new(x, y), rotation)
    { }
}
