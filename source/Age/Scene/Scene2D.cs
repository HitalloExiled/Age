namespace Age.Scene;

public class Scene2D : Scene
{
    public Camera2D Camera
    {
        get;
        set =>field = value ?? Camera2D.Default;
    } = Camera2D.Default;

    public override string NodeName => nameof(Scene2D);
}
