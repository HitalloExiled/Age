namespace Age.Scenes;

public class Scene3D : Scene
{
    private readonly List<Camera3D> cameras = [];

    public IReadOnlyList<Camera3D> Cameras => this.cameras;

    public override string NodeName => nameof(Scene3D);
}
