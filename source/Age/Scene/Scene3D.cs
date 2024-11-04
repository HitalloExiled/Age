namespace Age.Scene;

public abstract class Scene3D : Scene
{
    private readonly List<Camera3D> cameras = [];

    public IEnumerable<Camera3D> Cameras => this.cameras;

    protected override void Connected(RenderTree renderTree) =>
        renderTree.Scenes3D.Add(this);

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

    protected override void Disconnected(RenderTree renderTree) =>
        renderTree.Scenes3D.Remove(this);
}
