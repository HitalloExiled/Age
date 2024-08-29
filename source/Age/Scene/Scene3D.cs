namespace Age.Scene;

public abstract class Scene3D : Scene
{
    private readonly List<Camera3D> cameras = [];

    public IEnumerable<Camera3D> Cameras => this.cameras;

    protected override void Connected(NodeTree tree) =>
        tree.Scenes3D.Add(this);

    protected override void ChildAppended(Node child)
    {
        if (child is Camera3D camera)
        {
            this.cameras.Add(camera);
        }
    }

    protected override void ChildRemoved(Node child)
    {
        if (child is Camera3D camera)
        {
            this.cameras.Remove(camera);
        }
    }

    protected override void Disconnected(NodeTree tree) =>
        tree.Scenes3D.Remove(this);
}
