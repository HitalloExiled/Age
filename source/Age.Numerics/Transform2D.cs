namespace Age.Numerics;

public record struct Transform2D
{
    public static Transform2D Identity => new();

    private Matrix3x2<float> matrix;

    public readonly Matrix3x2<float> Matrix   => this.matrix;
    public readonly Vector2<float>   Position => this.matrix.Translation;
    public readonly float            Rotation => this.matrix.Rotation;
    public readonly Vector2<float>   Scale    => this.matrix.Scale;

    public Transform2D() =>
        this.matrix = Matrix3x2<float>.Identity;

    public Transform2D(in Matrix3x2<float> matrix) =>
        this.matrix = matrix;

    public Transform2D(in Vector2<float> position, float rotation, in Vector2<float> scale) =>
        this.matrix = new(position, rotation, scale);

    public Transform2D(float positionX, float positionY, float rotation, float scaleX, float scaleY) : this(new(positionX, positionY), rotation, new(scaleX, scaleY))
    { }

    public static Transform2D Rotated(float radians) =>
        new(Matrix3x2<float>.Rotated(radians));

    public static Transform2D Scaled(float scale) =>
        new(Matrix3x2<float>.Scaled(scale));

    public static Transform2D Scaled(float scaleX, float scaleY) =>
        new(Matrix3x2<float>.Scaled(scaleX, scaleY));

    public static Transform2D Scaled(in Vector2<float> scale) =>
        new(Matrix3x2<float>.Scaled(scale));

    public static Transform2D Translated(float translationX, float translationY) =>
        new(Matrix3x2<float>.Translated(new(translationX, translationY)));

    public static Transform2D Translated(in Vector2<float> translation) =>
        new(Matrix3x2<float>.Translated(translation));

    public readonly Transform2D Inverse() =>
        new(this.matrix.Inverse());

    public override readonly string ToString() =>
        this.matrix.ToString();

    public static Transform2D operator *(in Transform2D left, in Transform2D right) =>
        new(left.matrix * right.matrix);

    public static Vector2<float> operator *(in Transform2D left, in Vector2<float> right) =>
        left.matrix * right;

    public static Vector2<float> operator *(in Vector2<float> left, in Transform2D right) =>
        left * right.matrix;

    public static implicit operator Matrix3x2<float>(in Transform2D transform) =>
        transform.matrix;

    public static implicit operator Transform2D(in Matrix3x2<float> matrix) =>
        new(matrix);
}
