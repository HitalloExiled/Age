

using System.Diagnostics;

namespace Age.Numerics;

public record struct Transform3D
{
    private Matrix4x4<float> matrix;

    public Vector3<float> Position
    {
        readonly get => this.matrix.Translation;
        set          => this.matrix.Translation = value;
    }

    public Quaternion<float> Rotation
    {
        readonly get => this.matrix.Rotation;
        set =>          this.matrix.Rotation = value;
    }

    public Vector3<float> Scale
    {
        readonly get => this.matrix.Scale;
        set          => this.matrix.Scale = value;
    }

    public Vector3<float> Right   => this.matrix.X.ToVector3();
    public Vector3<float> Up      => this.matrix.Y.ToVector3();
    public Vector3<float> Forward => this.matrix.Z.ToVector3();

    public Transform3D() =>
        this.matrix = Matrix4x4<float>.Identity;

    public Transform3D(in Matrix4x4<float> matrix) =>
        this.matrix = matrix;

    public Transform3D(in Vector3<float> position, Quaternion<float> rotation, Vector3<float> scale) =>
        this.matrix = new(position, rotation, scale);

    public Transform3D(in Vector3<float> position) =>
        this.matrix = new(position, Quaternion<float>.Identity, Vector3<float>.One);

    public Transform3D(in Vector3<float> position, Quaternion<float> rotation) =>
        this.matrix = new(position, rotation, Vector3<float>.One);

    public readonly Transform3D LookingAt(Vector3<float> target, Vector3<float> up) =>
        new(Matrix4x4<float>.LookingAt(this.Position, target, up).Inverse());

    public static Transform3D Translated(Vector3<float> offset) =>
        new(offset, Quaternion<float>.Identity, Vector3<float>.One);

    public readonly Transform3D Inverse() =>
        new(this.matrix.Inverse());

    public override readonly string ToString() =>
        this.matrix.ToString();

    public static Transform3D operator *(in Transform3D left, in Transform3D right) =>
        new(left.matrix * right.matrix);

    public static implicit operator Matrix4x4<float>(in Transform3D transform) =>
        transform.matrix;

    public static implicit operator Transform3D(in Matrix4x4<float> matrix) =>
        new(matrix);
}
