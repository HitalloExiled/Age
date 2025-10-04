namespace Age.Scenes;

public abstract class Scene : Renderable
{
    public Viewport? Viewport => this.Parent as Viewport;
}
