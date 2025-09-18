using Age.Numerics;

namespace Age.Scene;

public sealed class Camera3D : Spatial3D
{
    public override string NodeName => nameof(Camera3D);

    public float Far  { get; set; } = 50;
    public float FoV  { get; set; } = Angle.DegreesToRadians(45f);
    public float Near { get; set; } = 0.1f;

    public void LookAt(Spatial3D node, Vector3<float> up) =>
        this.Transform = this.Transform.LookingAt(node.Transform.Position, up);

    public void LookAt(Vector3<float> target, Vector3<float> up) =>
        this.Transform = this.Transform.LookingAt(target, up);
}
