namespace Age.Scenes;

public sealed class Camera2D : Spatial2D
{
    public static Camera2D Default { get; } = new();

    public override string NodeName => nameof(Camera2D);

    public float Rotation { get; set; }
    public float Zoom     { get; set; }

    public void MakeCurrent() =>
        this.Scene?.Viewport?.Camera2D = this;
}
