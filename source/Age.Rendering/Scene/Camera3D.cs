using Age.Rendering.Resources;

namespace Age.Rendering.Scene;

public class Camera3D : Node3D
{
    public override string NodeName { get; } = nameof(Camera3D);

    public float Far  { get; set; }
    public float FoV  { get; set; }
    public float Near { get; set; }

    public RenderTarget? RenderTarget { get; set; }
}
