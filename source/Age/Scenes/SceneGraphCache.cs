namespace Age.Scenes;

public class SceneGraphCache
{
    private readonly List<Renderable> dirtyTrees = [];

    internal List<Renderable> Nodes     { get; } = [];
    internal List<Viewport>   Viewports { get; } = [];

    public void InvalidatedSubTree(Renderable renderable)
    {
        foreach (var dirty in this.dirtyTrees.ToArray())
        {
            if (dirty.SubtreeRange.Contains(renderable.SubtreeRange))
            {
                return;
            }
            else if (renderable.SubtreeRange.Contains(dirty.SubtreeRange))
            {
                this.dirtyTrees.Remove(dirty);
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
