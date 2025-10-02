namespace Age.Scenes;

public abstract class Scene : Renderable
{
    public Viewport? Viewport => this.Parent as Viewport;

    internal void NotifySubTreeChange(Node node) =>
        this.Viewport?.Window?.Tree.InvalidateNodeSubTree(node, DirtState.SubTree);
}
