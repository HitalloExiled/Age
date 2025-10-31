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
        private void Collect(Element subtree)
        {
            var subtreeRange = subtree.SubtreeRange;

            var boundaryRange = new ShortRange(subtreeRange.End, subtree.Scene!.SubtreeRange.End);

            if (subtree.DirtState == DirtState.Commands)
            {
                this.CollectNodeCommands(subtree, subtreeRange, boundaryRange);
            }
            else
            {
                this.CollectElement(subtree);
                this.ApplyOffsetAndUpdateBuffer(subtree, boundaryRange);
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
            this.CollectElementSubtree(subtree);
            this.EndSubtreeRange(subtree);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CollectElementSubtree(Element subtree)
        {
            Debug.Assert(composedTreeStack.Count == 0);

            var composedTreeTraversal = subtree.GetComposedTreeTraversalEnumerator(composedTreeStack);

            composedTreeTraversal.SubtreeTraversed += this.OnSubtreeTraversed;

            while (composedTreeTraversal.MoveNext())
            {
                this.StartSubtreeRange(composedTreeTraversal.Current);
            }
        }

        protected override void ApplyOffset(Renderable<Command2D> subtree, ShortRange boundaryRange, int offset, bool offsetAncestors)
        {
            if (offset != 0)
            {
                boundaryRange = boundaryRange.WithClamp(this.Nodes.Count);

                if (offsetAncestors)
                {
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
