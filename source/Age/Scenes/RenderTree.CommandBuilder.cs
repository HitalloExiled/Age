namespace Age.Scenes;

public sealed partial class RenderTree
{
    private readonly List<Viewport>  viewports       = [];
    private readonly SceneGraphCache sceneGraphCache = new();

    private bool viewportDirty;

    internal List<Renderable> Nodes => this.sceneGraphCache.Nodes;

    private void BuildIndexAndCollectCommands()
    {
        if (this.viewportDirty)
        {
            this.viewports.Sort();
        }

        this.sceneGraphCache.Build();
    }

    public void InvalidatedSubTree(Renderable renderable) =>
        this.sceneGraphCache.InvalidatedSubTree(renderable);

    public void AddViewport(Viewport viewport)
    {
        this.viewports.Add(viewport);

        this.viewportDirty = true;
    }

    public void RemoveViewport(Viewport viewport)
    {
        this.viewports.Remove(viewport);

        this.viewportDirty = true;
    }
}
