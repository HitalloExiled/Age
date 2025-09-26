using System.Runtime.CompilerServices;
using Age.Commands;
using Age.Core.Extensions;
using Age.Elements;
using Age.Numerics;

namespace Age.Scene;

public sealed partial class RenderTree
{
    private readonly List<Command2D> commands2D = [];
    private readonly List<Command3D> commands3D = [];

    private void BuildIndexAndCollectCommands()
    {
        var index = 0;

        var traversalEnumerator = this.Window.GetTraversalEnumerator();

        while (traversalEnumerator.MoveNext())
        {
            if (traversalEnumerator.Current is Renderable renderable && renderable.Visible)
            {
                updateIndex(renderable);

                if (renderable is Canvas canvas)
                {
                    traversalEnumerator.SkipToNextSibling();

                    collectSpatial2D(canvas);

                    var composedTreeTraversalEnumerator = canvas.GetComposedTreeTraversalEnumerator(this.composedTreeStack, gatherElementPostCommands);

                    while (composedTreeTraversalEnumerator.MoveNext())
                    {
                        if (composedTreeTraversalEnumerator.Current.Visible)
                        {
                            updateIndex(composedTreeTraversalEnumerator.Current);

                            if (composedTreeTraversalEnumerator.Current is Element element)
                            {
                                collectElementPreCommands(element);
                            }
                            else if (composedTreeTraversalEnumerator.Current is Layoutable layoutable)
                            {
                                collectLayoutable(composedTreeTraversalEnumerator.Current);
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
                    collectSpatial2D(spatial2D);
                }
                else if (renderable is Spatial3D spatial3D)
                {
                    collectSpatial3D(spatial3D);
                }
            }
        }

        if (index < this.Nodes.Count)
        {
            this.Nodes.RemoveRange(index, this.Nodes.Count - index);
        }

        // this.command2DEntries.AsSpan().TimSort(static (left, right) => left.Command.ZIndex.CompareTo(right.Command.ZIndex));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void collect2DCommands(ReadOnlySpan<Command2D> commands, Transform2D transform)
        {
            foreach (var command in commands)
            {
                command.Transform = command.LocalTransform * transform;

                this.commands2D.Add(command);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void collectElementPreCommands(Element element) =>
            collect2DCommands(element.PreCommands, element.CachedTransformWithOffset);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void gatherElementPostCommands(Element element) =>
            collect2DCommands(element.PostCommands, element.CachedTransformWithOffset);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void collectLayoutable(Layoutable layoutable) =>
            collect2DCommands(layoutable.Commands.AsSpan(), layoutable.CachedTransformWithOffset);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void collectSpatial2D(Spatial2D spatial2D) =>
            collect2DCommands(spatial2D.Commands.AsSpan(), spatial2D.CachedTransform);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void collectSpatial3D(Spatial3D spatial3D)
        {
            var transform = (Matrix4x4<float>)spatial3D.CachedTransform;

            this.commands3D.AddRange(spatial3D.Commands);
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

    private void ResetCache()
    {
        this.commands2D.Clear();
        this.commands3D.Clear();
    }
}
