using System.Diagnostics;

namespace Age.Numerics;

[DebuggerDisplay("\\{ Size: {Size}, Position: {Position}, Rotation: {Rotation} \\}")]
public struct Transform2D
{
    public Size<float>    Size;
    public Vector2<float> Position;
    public float          Rotation;

    public Transform2D(Size<float> size, Vector2<float> position, float rotation) : this()
    {
        this.Size     = size;
        this.Position = position;
        this.Rotation = rotation;
    }

    public Transform2D(float width, float height, float x, float y, float rotation) : this(new(width, height), new(x, y), rotation)
    { }
}
