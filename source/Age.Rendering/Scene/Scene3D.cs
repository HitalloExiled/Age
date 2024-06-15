namespace Age.Rendering.Scene;

public abstract class Scene3D : Scene
{
    public Camera3D? Camera { get; set; }

    protected override void OnConnected() =>
        this.Tree!.Scenes3D.Add(this);

    protected override void OnDisconnected(SceneTree tree) =>
        tree.Scenes3D.Remove(this);
}
