namespace Age.Scene;

public class Scene3D : Scene
{
    private readonly List<Camera3D> cameras = [];

    public IReadOnlyList<Camera3D> Cameras => this.cameras;

    public override string NodeName => nameof(Scene3D);

    private protected override void OnConnectedInternal() =>
        this.Window!.Tree.Scenes3D.Add(this);

    private protected override void OnChildAttachedInternal(Node child)
    {
        base.OnChildAttachedInternal(child);

        if (child is Camera3D camera)
        {
            this.cameras.Add(camera);
        }
    }

    private protected override void OnChildDetachingInternal(Node child)
    {
        base.OnChildDetachingInternal(child);

        if (child is Camera3D camera)
        {
            this.cameras.Remove(camera);
        }
    }

    private protected override void OnDisconnectingInternal()
    {
        base.OnDisconnecting();

        this.Window!.Tree.Scenes3D.Remove(this);
    }
}
