namespace Age.Scene;

public abstract class Scene3D : Scene
{
    private readonly List<Camera3D> cameras = [];

    public IEnumerable<Camera3D> Cameras => this.cameras;

    protected override void OnConnected() =>
        this.Window!.Tree.Scenes3D.Add(this);

    protected override void OnChildAppended(Node child)
    {
        if (child is Camera3D camera)
        {
            this.cameras.Add(camera);
        }
    }

    protected override void OnChildRemoved(Node child)
    {
        if (child is Camera3D camera)
        {
            this.cameras.Remove(camera);
        }
    }

    protected override void OnDisconnected() =>
        this.Window!.Tree.Scenes3D.Remove(this);
}

public abstract class Scene2D : Scene
{
    public Camera2D Camera
    {
        get;
        set =>field = value ?? Camera2D.Default;
    } = Camera2D.Default;
}
