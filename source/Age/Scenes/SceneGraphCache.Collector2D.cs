using Age.Commands;
using Age.Core.Extensions;
using Age.Elements;
using Age.Graphs;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Scenes;

internal partial class SceneGraphCache
{
    public readonly struct Collector2D : IDisposable
    {
        private static readonly Stack<(Slot, int)> composedTreeStack = [];

        private readonly BoundaryContext<Command2D> context;
        private readonly Renderable                 target;

        public Collector2D(Renderable target, int startIndex, CommandBuffer<Command2D> commandBuffer, List<Renderable> stage, List<Renderable> nodes)
        {
            var commandRange = target is Renderable<Command2D> renderable2D
                ? renderable2D.CommandRange
                : new(0, commandBuffer.Commands.Length);

            this.context = new(startIndex, commandRange, commandBuffer, stage, nodes);
            this.target  = target;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly void AccumulateElementPreCommands(Element element)
        {
            this.context.CollectCommands(element.PreCommands);

            element.CommandRange = this.context.WithPreEnd(element.CommandRange);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly void AccumulateElementPostCommands(Element element)
        {
            var colorIndex  = this.context.Commands.Count;

            this.context.CollectCommands(element.PostCommands);

            var difference = this.context.Commands.Count - colorIndex - element.CommandRange.Post.Length;

            element.CommandRange = element.CommandRange.WithPostResize(difference);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly void CollectElementPreCommands(Element element)
        {
            element.CommandRange = this.context.CreateCommandRange();

            this.context.CollectCommands(element.PreCommands);

            element.CommandRange = this.context.WithPreEnd(element.CommandRange);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly void CollectElementPostCommands(Element element)
        {
            element.CommandRange = this.context.WithPostStart(element.CommandRange);

            this.context.CollectCommands(element.PostCommands);

            element.CommandRange = this.context.WithPostEnd(element.CommandRange);
        }

        private readonly void OnComposedSubtreeTraversed(Element element)
        {
            this.CollectElementPostCommands(element);

            this.context.EndSubtreeRange(element, false);
        }

        private readonly void OnSubtreeTraversed(Node node)
        {
            switch (node)
            {
                case Renderable<Command2D> renderable2D:
                    this.context.EndSubtreeRange(renderable2D, true);
                    break;

                case Renderable renderable:
                    this.context.EndSubtreeRange(renderable);
                    break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly void Collect(Element subtree)
        {
            var subtreeRange = subtree.SubtreeRange;

            var boundaryRange = new ShortRange(subtreeRange.End, subtree.Scene!.SubtreeRange.End);

            if (subtree.DirtState == DirtState.Commands)
            {
                this.AccumulateElementPreCommands(subtree);

                var preOffset  = subtree.CommandRange.Pre.End  - this.context.CommandRange.Pre.End;

                this.ApplyOffset(subtree, new(subtreeRange.Start + 1, subtreeRange.End), preOffset);

                this.context.CommandBuffer.ReplaceCommandRange(this.context.CommandRange.Pre, this.context.Commands.AsSpan());

                var postColorIndex  = this.context.Commands.Count;

                this.AccumulateElementPostCommands(subtree);

                var postColorOffset  = subtree.CommandRange.Post.End  - this.context.CommandRange.Post.End;

                this.ApplyOffset(subtree, boundaryRange, postColorOffset);

                var postColorCount  = this.context.Commands.Count - postColorIndex;

                this.context.CommandBuffer.ReplaceCommandRange(this.context.CommandRange.Post, this.context.Commands.AsSpan(postColorIndex, postColorCount));

                subtree.MakeSubtreePristine();
            }
            else
            {
                this.CollectElement(subtree);

                var colorOffset  = subtree.CommandRange.Post.End  - this.context.CommandRange.Post.End;

                this.ApplyOffset(subtree, boundaryRange, colorOffset);

                this.context.UpdateBuffer(this.context.CommandRange.FullRange);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly void CollectSubtree(Renderable subtree)
        {
            var traversal = subtree.GetTraversalEnumerator();

            traversal.SubtreeTraversed += this.OnSubtreeTraversed;

            while (traversal.MoveNext())
            {
                if (traversal.Current is Renderable renderable && renderable.Visible)
                {
                    switch (traversal.Current)
                    {
                        case Viewport viewport:
                            Collector.CollectViewport(viewport, this.context);

                            traversal.SkipToNextSibling();
                            break;

                        case Element element:
                            this.CollectElement(element);

                            traversal.SkipToNextSibling();
                            break;

                        case Renderable<Command2D> renderable2D:
                            this.context.StartSubtreeRange(renderable2D, true);
                            break;

                        default:
                            this.context.StartSubtreeRange(renderable);
                            break;
                    }
                }
                else
                {
                    traversal.SkipToNextSibling();
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CollectElement(Element subtree)
        {
            this.context.StartSubtreeRange(subtree);
            this.CollectElementPreCommands(subtree);

            this.CollectElementSubtree(subtree);

            this.CollectElementPostCommands(subtree);
            this.context.EndSubtreeRange(subtree);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly void CollectElementSubtree(Element subtree)
        {
            Debug.Assert(composedTreeStack.Count == 0);

            var composedTreeTraversal = subtree.GetComposedTreeTraversalEnumerator(composedTreeStack);

            composedTreeTraversal.SubtreeTraversed += this.OnComposedSubtreeTraversed;

            while (composedTreeTraversal.MoveNext())
            {
                if (composedTreeTraversal.Current.Visible)
                {
                    this.context.StartSubtreeRange(composedTreeTraversal.Current);

                    if (composedTreeTraversal.Current is Layoutable layoutable)
                    {
                        if (layoutable is Element element)
                        {
                            this.CollectElementPreCommands(element);

                            if (element.IsComposedLeaf)
                            {
                                this.OnComposedSubtreeTraversed(element);
                            }
                        }
                        else
                        {
                            this.context.CollectCommands(layoutable);
                        }
                    }
                }
                else
                {
                    composedTreeTraversal.SkipToNextSibling();
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly void Collect(Scene2D scene)
        {
            this.context.StartSubtreeRange(scene);

            this.CollectSubtree(scene);

            this.context.EndSubtreeRange(scene);
            this.context.UpdateBuffer(0..);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly void Collect(Renderable<Command2D> subtree)
        {
            var boundaryRange = new ShortRange(subtree.SubtreeRange.End, subtree.Scene!.SubtreeRange.End);

            if (subtree.DirtState == DirtState.Commands)
            {
                this.context.AccumulateCommands(subtree);

                subtree.MakeSubtreePristine();
            }
            else
            {
                this.context.StartSubtreeRange(subtree, true);

                this.CollectSubtree(subtree);

                this.context.EndSubtreeRange(subtree, true);
            }

            this.ApplyOffsetAndUpdateBuffer(subtree, boundaryRange);
        }

        private void ApplyOffset(Renderable<Command2D> subtree, ShortRange boundaryRange, int offset)
        {
            if (offset != 0)
            {
                boundaryRange = boundaryRange.WithClamp(this.context.Nodes.Count);

                Node? parent = subtree;

                while (true)
                {
                    parent = parent is Element element ? element.ComposedParent : parent.Parent;

                    if (parent == null || parent is Scene)
                    {
                        break;
                    }

                    if (parent is Renderable<Command2D> parentRenderable)
                    {
                        parentRenderable.CommandRange = parentRenderable.CommandRange.WithPostOffset(offset);
                    }
                }

                foreach (var node in this.context.Nodes.AsSpan(boundaryRange))
                {
                    if (node is Renderable<Command2D> renderable)
                    {
                        renderable.CommandRange = renderable.CommandRange.WithOffset(offset);
                    }
                }
            }
        }

        private void ApplyOffsetAndUpdateBuffer(Renderable<Command2D> subtree, ShortRange boundaryRange)
        {
            var offset = subtree.CommandRange.Post.End - this.context.CommandRange.Post.End;

            this.ApplyOffset(subtree, boundaryRange, offset);

            this.context.UpdateBuffer(this.context.CommandRange.Pre);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Collect()
        {
            switch (this.target)
            {
                case Element element:
                    this.Collect(element);

                    break;

                case Renderable<Command2D> renderable2D:
                    this.Collect(renderable2D);

                    break;

                default:
                    this.Collect((Scene2D)this.target);

                    break;
            }
        }

        public readonly void Dispose() =>
            this.context.Dispose();
    }
}
