using System.Diagnostics;
using System.Runtime.CompilerServices;
using Age.Commands;
using Age.Core.Extensions;
using Age.Elements;
using Age.Graphs;
using Age.Numerics;

namespace Age.Scenes;

public sealed partial class RenderTree
{
    private readonly Stack<(Slot, int)> composedTreeStack = [];
    private readonly List<Viewport>     viewports         = [];

    internal List<Node> Nodes { get; } = new(256);

    private void BuildIndexAndCollectCommands()
    {
        this.viewports.Clear();
        this.Window.RenderContext.Reset();

        var index = 0;

        var traversalEnumerator = this.Window.GetTraversalEnumerator();

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

                    void onSubtreeTraversed(Element element) =>
                        collect2DCommands(context, element.PostCommands, element.CachedTransformWithOffset);

                    var composedTreeTraversalEnumerator = canvas.GetComposedTreeTraversalEnumerator(this.composedTreeStack);

                    composedTreeTraversalEnumerator.SubtreeTraversed += onSubtreeTraversed;

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

        // this.command2DEntries.AsSpan().TimSort(static (left, right) => left.Command.ZIndex.CompareTo(right.Command.ZIndex));

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
            collect2DCommands(context, layoutable.Commands.AsSpan(), layoutable.CachedTransformWithOffset);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void collectSpatial2D(RenderContext context, Spatial2D spatial2D) =>
            collect2DCommands(context, spatial2D.Commands.AsSpan(), spatial2D.CachedTransform);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void collectSpatial3D(RenderContext context, Spatial3D spatial3D)
        {
            var transform = (Matrix4x4<float>)spatial3D.CachedTransform;

            context.Buffer3D.AddCommandRange(spatial3D.Commands.AsSpan());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void updateIndex(Node current)
        {
            current.Index = index;

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
}
