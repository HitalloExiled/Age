
namespace Age.Numerics;

public record struct Transform3D
{
    private Matrix4x4<float> matrix;

    public Transform3D() =>
        this.matrix = Matrix4x4<float>.Identity;

    public Transform3D(in Matrix4x4<float> matrix) =>
        this.matrix = matrix;

    public Transform3D(in Vector3<float> position) =>
        this.matrix = new(position);

    public static Transform3D Translated(Vector3<float> offset) =>
        new(offset);

    public readonly Transform3D Inverse() =>
        new(this.matrix.Inverse());

    public static Transform3D operator *(Transform3D left, Transform3D right) =>
        new(left.matrix * right.matrix);

    public static implicit operator Matrix4x4<float>(in Transform3D transform) =>
        transform.matrix;
}
