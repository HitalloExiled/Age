namespace Age.Numerics;

public record struct Transform2D
{
    public static Transform2D Identity => new();

    private Matrix3x2<float> matrix;

    public readonly Matrix3x2<float> Matrix => this.matrix;

    public Vector2<float> Position
    {
        readonly get => this.matrix.Translation;
        set          => this.matrix.Translation = value;
    }

    public float Rotation
    {
        readonly get => this.matrix.Rotation;
        set          => this.matrix.Rotation = value;
    }

    public Vector2<float> Scale
    {
        readonly get => this.matrix.Scale;
        set          => this.matrix.Scale = value;
    }

    public Transform2D() =>
        this.matrix = Matrix3x2<float>.Identity;

    public Transform2D(in Matrix3x2<float> matrix) =>
        this.matrix = matrix;

    public Transform2D(in Vector2<float> position, float rotation, in Vector2<float> scale) =>
        this.matrix = new(position, rotation, scale);

    public Transform2D(float positionX, float positionY, float rotation, float scaleX, float scaleY) : this(new(positionX, positionY), rotation, new(scaleX, scaleY))
    { }

    public static Transform2D CreateRotated(float radians) =>
        new(Matrix3x2<float>.CreateRotated(radians));

    public static Transform2D CreateScaled(float scale) =>
        new(Matrix3x2<float>.CreateScaled(scale));

    public static Transform2D CreateScaled(float scaleX, float scaleY) =>
        new(Matrix3x2<float>.CreateScaled(scaleX, scaleY));

    public static Transform2D CreateScaled(in Vector2<float> scale) =>
        new(Matrix3x2<float>.CreateScaled(scale));

    public static Transform2D CreateTranslated(float translationX, float translationY) =>
        new(Matrix3x2<float>.CreateTranslated(new(translationX, translationY)));

    public static Transform2D CreateTranslated(in Vector2<float> translation) =>
        new(Matrix3x2<float>.CreateTranslated(translation));

    public readonly Transform2D Inverse() =>
        new(this.matrix.Inverse());

    public override readonly string ToString() =>
        this.matrix.ToString();

    public static Transform2D operator *(in Transform2D left, in Transform2D right) =>
        new(left.matrix * right.matrix);

    public static implicit operator Matrix3x2<float>(in Transform2D transform) =>
        transform.matrix;
}
