using Age.Commands;
using Age.Core.Extensions;
using Age.Graphs;
using System.Runtime.CompilerServices;

namespace Age.Scenes;

internal partial class SceneGraphCache
{
    public readonly struct Collector3D : IDisposable
    {
        private readonly BoundaryContext<Command3D> context;
        private readonly Renderable                 target;

        public Collector3D(Renderable target, int startIndex, CommandBuffer<Command3D> commandBuffer, List<Renderable> stage, List<Renderable> nodes)
        {
            var commandRange = target is Renderable<Command3D> renderable3D
                ? renderable3D.CommandRange
                : new(0, commandBuffer.Commands.Length);

            this.context = new(startIndex, commandRange, commandBuffer, stage, nodes);
            this.target  = target;
        }

        private readonly void OnSubtreeTraversed(Node node)
        {
            switch (node)
            {
                case Renderable<Command3D> renderable3D:
                    this.context.EndSubtreeRange(renderable3D, true);
                    break;

                case Renderable renderable:
                    this.context.EndSubtreeRange(renderable);
                    break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly void CollectSubtree(Renderable subtree)
        {
            var traversal = subtree.GetTraversalEnumerator();

            traversal.SubtreeTraversed += this.OnSubtreeTraversed;

            while (traversal.MoveNext())
            {
                switch (traversal.Current)
                {
                    case Viewport viewport:
                        Collector.CollectViewport(viewport, this.context);

                        traversal.SkipToNextSibling();
                        break;

                    case Renderable<Command3D> renderable3D:
                        this.context.StartSubtreeRange(renderable3D, true);
                        break;

                    case Renderable renderable:
                        this.context.StartSubtreeRange(renderable);
                        break;

                    default:
                        traversal.SkipToNextSibling();
                        break;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly void Collect(Scene3D scene)
        {
            this.context.StartSubtreeRange(scene);

            this.CollectSubtree(scene);

            this.context.EndSubtreeRange(scene);
            this.context.UpdateBuffer(0..);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly void Collect(Renderable<Command3D> subtree)
        {
            var boundaryRange = new ShortRange(subtree.SubtreeRange.Start, subtree.Scene!.SubtreeRange.End);

            this.context.StartSubtreeRange(subtree, true);

            this.CollectSubtree(subtree);

            this.context.EndSubtreeRange(subtree, true);
            this.UpdateBuffer(subtree, boundaryRange);
        }

        public void UpdateBuffer(Renderable<Command3D> subtree, ShortRange boundaryRange)
        {
            var offset = subtree.CommandRange.Post.End - this.context.CommandRange.Post.End;

            boundaryRange = boundaryRange.WithClamp(this.context.Nodes.Count);

            if (offset > 0)
            {
                Node? parent = subtree;

                while (true)
                {
                    parent = parent.Parent;

                    if (parent == null || parent is Scene)
                    {
                        break;
                    }

                    switch (parent)
                    {
                        case Renderable<Command3D> parentRenderable:
                            parentRenderable.CommandRange = parentRenderable.CommandRange.WithPostOffset(offset);
                            break;
                    }
                }

                foreach (var node in this.context.Nodes.AsSpan(boundaryRange))
                {
                    if (node is Renderable<Command3D> renderable)
                    {
                        renderable.CommandRange = renderable.CommandRange.WithOffset(offset);
                    }
                }
            }

            this.context.UpdateBuffer(this.context.CommandRange.Pre);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Collect()
        {
            switch (this.target)
            {
                case Renderable<Command3D> renderable3D:
                    this.Collect(renderable3D);

                    break;

                default:
                    this.Collect((Scene3D)this.target);

                    break;
            }
        }

        public readonly void Dispose() =>
            this.context.Dispose();
    }
}
