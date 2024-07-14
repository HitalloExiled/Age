using Age.Numerics;
using Age.Rendering.Resources;

namespace Age.Rendering.Scene;

public class Camera3D : Node3D
{
    public override string NodeName { get; } = nameof(Camera3D);

    public float Far  { get; set; } = 50;
    public float FoV  { get; set; } = Angle.Radians(45);
    public float Near { get; set; } = 0.1f;

    public List<RenderTarget> RenderTargets { get; }  = [];

    public void LookAt(Node3D node, Vector3<float> up) =>
        this.Transform = this.Transform.LookingAt(node.Transform.Position, up);

    public void LookAt(Vector3<float> target, Vector3<float> up) =>
        this.Transform = this.Transform.LookingAt(target, up);
}
