using System.Runtime.CompilerServices;
using Age.Core.Extensions;
using Age.Elements;
using Age.Numerics;

namespace Age.Scene;

public sealed partial class RenderTree
{
    private readonly List<Command2DEntry> command2DEntries = [];
    private readonly List<Command3DEntry> command3DEntries = [];

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
        // this.command3DEntries.AsSpan().TimSort(static (left, right) => left.Command.ZIndex.CompareTo(right.Command.ZIndex));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void collectElementPreCommands(Element element)
        {
            var transform = element.CachedTransformWithOffset;

            foreach (var command in element.PreCommands)
            {
                var entry = new Command2DEntry(command, transform);

                this.command2DEntries.Add(entry);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void gatherElementPostCommands(Element element)
        {
            var transform = element.CachedTransformWithOffset;

            foreach (var command in element.PostCommands)
            {
                var entry = new Command2DEntry(command, transform);

                this.command2DEntries.Add(entry);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void collectLayoutable(Layoutable layoutable)
        {
            var transform = layoutable.CachedTransformWithOffset;

            foreach (var command in layoutable.Commands)
            {
                var entry = new Command2DEntry(command, transform);

                this.command2DEntries.Add(entry);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void collectSpatial2D(Spatial2D spatial2D)
        {
            var transform = spatial2D.CachedTransform;

            foreach (var command in spatial2D.Commands)
            {
                var entry = new Command2DEntry(command, transform);

                this.command2DEntries.Add(entry);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void collectSpatial3D(Spatial3D spatial3D)
        {
            var transform = (Matrix4x4<float>)spatial3D.CachedTransform;

            foreach (var command in spatial3D.Commands)
            {
                var entry = new Command3DEntry(command, transform);

                this.command3DEntries.Add(entry);
            }
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
        this.command2DEntries.Clear();
        this.command3DEntries.Clear();
    }
}
