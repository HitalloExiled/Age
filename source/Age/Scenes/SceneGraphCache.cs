using Age.Elements;

namespace Age.Scenes;

internal partial class SceneGraphCache
{
    private readonly List<Renderable> dirtySubtrees = [];

    internal List<Renderable> Nodes     { get; } = [];
    internal List<Viewport>   Viewports { get; } = [];

    public void InvalidatedSubTree(Renderable renderable)
    {
        foreach (var dirtyTree in this.dirtySubtrees.ToArray())
        {
            if (dirtyTree == renderable)
            {
                return;
            }

            if ((renderable is Layoutable layoutable && Layoutable.IsComposedAncestor(dirtyTree, layoutable)) || dirtyTree.IsAncestor(renderable))
            {
                dirtyTree.DirtState |= DirtState.Subtree;

                return;
            }
            else if ((dirtyTree is Layoutable layoutableDirtyTree && Layoutable.IsComposedAncestor(renderable, layoutableDirtyTree)) || renderable.IsAncestor(dirtyTree))
            {
                renderable.DirtState |= DirtState.Subtree;

                this.dirtySubtrees.Remove(dirtyTree);
            }
        }

        this.dirtySubtrees.Add(renderable);
    }

    internal void Build()
    {
        this.dirtySubtrees.Sort();

        foreach (var subtree in this.dirtySubtrees)
        {
            Collector.Collect(subtree, this.Nodes);
        }

        this.dirtySubtrees.Clear();
    }
}
