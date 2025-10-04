using System.Data.Common;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Age.Commands;
using Age.Core;
using Age.Core.Extensions;
using Age.Elements;
using Age.Graphs;
using Age.Numerics;

namespace Age.Scenes;

public sealed partial class RenderTree
{
    private readonly List<Renderable> dirtyTrees = [];
    private readonly List<Viewport>   viewports  = [];

    internal List<Renderable> Nodes { get; } = [];

    private void BuildIndexAndCollectCommands()
    {
        this.dirtyTrees.Sort(static (left, right) => left.SubtreeRange.Start.CompareTo(right.SubtreeRange.Start));

        foreach (var subtree in this.dirtyTrees)
        {
            Collector.Collect(subtree, this.Nodes);
        }

        this.dirtyTrees.Clear();
    }

    public void InvalidatedSubTree(Renderable renderable)
    {
        foreach (var dirty in this.dirtyTrees)
        {
            if (dirty.SubtreeRange.Contains(renderable.SubtreeRange))
            {
                return;
            }
        }

        this.dirtyTrees.Add(renderable);
    }
}
