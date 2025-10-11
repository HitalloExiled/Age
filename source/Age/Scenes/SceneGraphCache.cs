using Age.Core.Extensions;

namespace Age.Scenes;

public class SceneGraphCache
{
    private readonly List<Renderable> dirtyTrees = [];

    internal List<Renderable> Nodes     { get; } = [];
    internal List<Viewport>   Viewports { get; } = [];

    public void InvalidatedSubTree(Renderable renderable)
    {
        foreach (var dirtyTree in this.dirtyTrees.ToArray())
        {
            if (dirtyTree == renderable || (dirtyTree.DirtState.HasFlags(DirtState.Subtree) && (dirtyTree.SubtreeRange.Contains(renderable.SubtreeRange) || dirtyTree.IsAncestor(renderable))))
            {
                return;
            }
            else if (renderable.DirtState.HasFlags(DirtState.Subtree) && (renderable.SubtreeRange.Contains(dirtyTree.SubtreeRange) || renderable.IsAncestor(dirtyTree)))
            {
                this.dirtyTrees.Remove(dirtyTree);
            }
        }

        this.dirtyTrees.Add(renderable);
    }

    internal void Build()
    {
        this.dirtyTrees.Sort(static (left, right) => left.SubtreeRange.Start.CompareTo(right.SubtreeRange.Start));

        foreach (var subtree in this.dirtyTrees)
        {
            Collector.Collect(subtree, this.Nodes);
        }

        this.dirtyTrees.Clear();
    }
}
