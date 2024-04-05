using System.Diagnostics;

namespace Age.Numerics;

[DebuggerDisplay("\\{ Position: {Position}, Rotation: {Rotation}, Scale: {Scale} \\}")]
public record struct Transform2D
{
    private Matrix3x2<float> matrix;

    public Vector2<float> Position
    {
        readonly get => this.matrix.Translation;
        set          => this.matrix.Translation = new(value.X, value.Y);
    }

    public float Rotation
    {
        readonly get => this.matrix.Rotation;
        set          => this.matrix.Rotation = value;
    }

    public Vector2<float> Scale
    {
        readonly get => this.matrix.Scale;
        set          => this.matrix.Scale = new(value.X, value.Y);
    }

    public Transform2D() =>
        this.matrix = Matrix3x2<float>.Identity;

    public Transform2D(Matrix3x2<float> matrix) =>
        this.matrix = matrix;

    public Transform2D(Vector2<float> position, float rotation, Vector2<float> scale) =>
        this.matrix = new(position, rotation, scale);

    public Transform2D(float positionX, float positionY, float rotation, float scaleX, float scaleY) : this(new(positionX, positionY), rotation, new(scaleX, scaleY))
    { }

    public readonly Transform2D Inverse() =>
        new(this.matrix.Inverse());

    public static Transform2D operator *(in Transform2D left, in Transform2D right) =>
        new(left.matrix * right.matrix);
}
