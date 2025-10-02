using System.Diagnostics;
using System.Runtime.CompilerServices;
using Age.Commands;
using Age.Core;
using Age.Core.Extensions;
using Age.Elements;
using Age.Graphs;
using Age.Numerics;

namespace Age.Scenes;

[Flags]
public enum DirtState
{
    None     = 0,
    SubTree  = 1 << 0,
    Commands = 1 << 1,
    Indexes  = 1 << 2,
}

public sealed partial class RenderTree
{
    private DirtState dirtState;

    private readonly List<Range<short>> commandsDirtyRanges = [];
    private readonly Stack<(Slot, int)> composedTreeStack   = [];
    private readonly HashSet<Node>      dirtyTrees          = [];
    private readonly List<Viewport>     viewports           = [];

    internal List<Node> Nodes { get; } = new(256);

    private void BuildIndexAndCollectCommands()
    {
        this.viewports.Clear();
        this.Window.RenderContext.Reset();

        var index = 0;

        void onSubtreeTraversed(Node node)
        {
            if (index > node.SubTreeRange.Start)
            {
                node.SubTreeRange = node.SubTreeRange.WithEnd((ushort)index);
            }
        }

        var traversalEnumerator = this.Window.GetTraversalEnumerator();

        traversalEnumerator.SubtreeTraversed += onSubtreeTraversed;

        while (traversalEnumerator.MoveNext())
        {
            if (traversalEnumerator.Current is Viewport viewport)
            {
                this.viewports.Add(viewport);

                viewport.RenderContext.Reset();
            }
            else if (traversalEnumerator.Current is Renderable renderable && renderable.Visible)
            {
                var context = renderable.Scene!.Viewport!.RenderContext;

                updateIndex(renderable);

                if (renderable is Canvas canvas)
                {
                    traversalEnumerator.SkipToNextSibling();

                    collectSpatial2D(context, canvas);

                    Debug.Assert(this.composedTreeStack.Count == 0);

                    void onComposedSubtreeTraversed(Element element)
                    {
                        onSubtreeTraversed(element);

                        collect2DCommands(context, element.PostCommands, element.CachedTransformWithOffset);
                    }

                    var composedTreeTraversalEnumerator = canvas.GetComposedTreeTraversalEnumerator(this.composedTreeStack);

                    composedTreeTraversalEnumerator.SubtreeTraversed += onComposedSubtreeTraversed;

                    while (composedTreeTraversalEnumerator.MoveNext())
                    {
                        if (composedTreeTraversalEnumerator.Current.Visible)
                        {
                            updateIndex(composedTreeTraversalEnumerator.Current);

                            if (composedTreeTraversalEnumerator.Current is Element element)
                            {
                                collectElementPreCommands(context, element);
                            }
                            else if (composedTreeTraversalEnumerator.Current is Layoutable layoutable)
                            {
                                collectLayoutable(context, composedTreeTraversalEnumerator.Current);
                            }
                        }
                        else
                        {
                            composedTreeTraversalEnumerator.SkipToNextSibling();
                        }
                    }
                }
                else if (renderable is Spatial2D spatial2D)
                {
                    collectSpatial2D(context, spatial2D);
                }
                else if (renderable is Spatial3D spatial3D)
                {
                    collectSpatial3D(context, spatial3D);
                }
            }
        }

        if (index < this.Nodes.Count)
        {
            this.Nodes.RemoveRange(index, this.Nodes.Count - index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void collect2DCommands(RenderContext context, ReadOnlySpan<Command2D> commands, Transform2D transform)
        {
            foreach (var command in commands)
            {
                command.Transform = command.LocalTransform * transform;

                context.Buffer2D.AddCommand(command);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void collectElementPreCommands(RenderContext context, Element element) =>
            collect2DCommands(context, element.PreCommands, element.CachedTransformWithOffset);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void collectLayoutable(RenderContext context, Layoutable layoutable) =>
            collect2DCommands(context, layoutable.GetCommands(), layoutable.CachedTransformWithOffset);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void collectSpatial2D(RenderContext context, Spatial2D spatial2D) =>
            collect2DCommands(context, spatial2D.GetCommands(), spatial2D.CachedTransform);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void collectSpatial3D(RenderContext context, Spatial3D spatial3D)
        {
            var transform = (Matrix4x4<float>)spatial3D.CachedTransform;

            context.Buffer3D.AddCommandRange(spatial3D.GetCommands());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void updateIndex(Node current)
        {
            current.SubTreeRange = new((ushort)index);

            if (index == this.Nodes.Count)
            {
                this.Nodes.Add(current);
            }
            else
            {
                this.Nodes[index] = current;
            }

            index++;
        }
    }

    public void InvalidateNodeSubTree(Node node, DirtState dirtState)
    {
        foreach (var range in this.dirtyTrees)
        {
            if (range.SubTreeRange.Contains(node.SubTreeRange))
            {
                return;
            }
        }

        this.dirtState |= dirtState;

        this.dirtyTrees.Add(node);
    }
}
