using Age.Commands;
using Age.Core.Extensions;
using Age.Elements;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Scenes;

internal partial class SceneGraphCache
{
    public sealed class Collector2D : Collector<Command2D>
    {
        private static readonly Stack<(Slot, int)> composedTreeStack = [];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AccumulateElementPreCommands(Element element)
        {
            this.CollectCommands(element.PreCommands);

            element.CommandRange = element.CommandRange.WithPreEnd(this.CommandOffset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AccumulateElementPostCommands(Element element)
        {
            var colorIndex  = this.Commands.Count;

            this.CollectCommands(element.PostCommands);

            var difference = this.Commands.Count - colorIndex - element.CommandRange.Post.Length;

            element.CommandRange = element.CommandRange.WithPostResize(difference);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CollectElementPreCommands(Element element)
        {
            element.CommandRange = new(this.CommandOffset);

            if (this.ScopeIsVisible)
            {
                this.CollectCommands(element.PreCommands);

                element.CommandRange = element.CommandRange.WithPreEnd(this.CommandOffset);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CollectElementPostCommands(Element element)
        {
            element.CommandRange = element.CommandRange.WithPostStart(this.CommandOffset);

            if (this.ScopeIsVisible)
            {
                this.CollectCommands(element.PostCommands);

                element.CommandRange = element.CommandRange.WithPostEnd(this.CommandOffset);
            }
        }

        private void OnComposedSubtreeTraversed(Element element)
        {
            this.CollectElementPostCommands(element);

            this.EndSubtreeRange(element, false);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Collect(Element subtree)
        {
            var subtreeRange = subtree.SubtreeRange;

            var boundaryRange = new ShortRange(subtreeRange.End, subtree.Scene!.SubtreeRange.End);

            if (subtree.DirtState == DirtState.Commands)
            {
                this.AccumulateElementPreCommands(subtree);

                var preOffset  = subtree.CommandRange.Pre.End  - this.CommandRange.Pre.End;

                this.ApplyOffset(subtree, new(subtreeRange.Start + 1, subtreeRange.End), preOffset);

                this.CommandBuffer.ReplaceCommandRange(this.CommandRange.Pre, this.Commands.AsSpan());

                var postColorIndex  = this.Commands.Count;

                this.AccumulateElementPostCommands(subtree);

                var postColorOffset  = subtree.CommandRange.Post.End  - this.CommandRange.Post.End;

                this.ApplyOffset(subtree, boundaryRange, postColorOffset);

                var postColorCount  = this.Commands.Count - postColorIndex;

                this.CommandBuffer.ReplaceCommandRange(this.CommandRange.Post, this.Commands.AsSpan(postColorIndex, postColorCount));

                subtree.MakeSubtreePristine();
            }
            else
            {
                this.CollectElement(subtree);

                var offset = subtree.CommandRange.Post.End - this.CommandRange.Post.End;

                this.ApplyOffset(subtree, boundaryRange, offset);

                this.UpdateBuffer(this.CommandRange.FullRange);
            }
        }

        protected override void Collect(ref Node.TraversalEnumerator traversal)
        {
            switch (traversal.Current)
            {
                case Element element:
                    this.CollectElement(element);

                    traversal.SkipToNextSibling();
                    break;

                default:
                    base.Collect(ref traversal);
                    break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CollectElement(Element subtree)
        {
            this.StartSubtreeRange(subtree);
            this.CollectElementPreCommands(subtree);

            this.CollectElementSubtree(subtree);

            this.CollectElementPostCommands(subtree);
            this.EndSubtreeRange(subtree);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CollectElementSubtree(Element subtree)
        {
            Debug.Assert(composedTreeStack.Count == 0);

            var composedTreeTraversal = subtree.GetComposedTreeTraversalEnumerator(composedTreeStack);

            composedTreeTraversal.SubtreeTraversed += this.OnComposedSubtreeTraversed;

            while (composedTreeTraversal.MoveNext())
            {
                this.StartSubtreeRange(composedTreeTraversal.Current);

                if (composedTreeTraversal.Current is Layoutable layoutable)
                {
                    if (layoutable is Element element)
                    {
                        this.CollectElementPreCommands(element);
                    }
                    else
                    {
                        this.CollectCommands(layoutable);
                    }
                }
            }
        }

        protected override void ApplyOffset(Renderable<Command2D> subtree, ShortRange boundaryRange, int offset)
        {
            if (offset != 0)
            {
                boundaryRange = boundaryRange.WithClamp(this.Nodes.Count);

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

                foreach (var node in this.Nodes.AsSpan(boundaryRange))
                {
                    if (node is Renderable<Command2D> renderable)
                    {
                        renderable.CommandRange = renderable.CommandRange.WithOffset(offset);
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void Collect(Renderable target)
        {
            switch (target)
            {
                case Element element:
                    this.Collect(element);

                    break;

                default:
                    base.Collect(target);

                    break;
            }
        }
    }
}
