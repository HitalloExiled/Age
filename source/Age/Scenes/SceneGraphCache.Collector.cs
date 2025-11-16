using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Age.Commands;
using Age.Core;
using Age.Core.Extensions;
using Age.Core.Interfaces;
using Age.Graphs;

namespace Age.Scenes;

internal partial class SceneGraphCache
{
    internal static class Collector
    {
        private static readonly ObjectPool<Collector2D> collector2DPool = new();
        private static readonly ObjectPool<Collector3D> collector3DPool = new();
        private static readonly List<Renderable>        stage           = [];

        private static void Collect(Renderable target, int index, CommandBuffer<Command2D> commandBuffer, List<Renderable> stage, List<Renderable> nodes)
        {
            var collector = collector2DPool.Get();

            collector.Collect(target, index, commandBuffer, stage, nodes);

            collector2DPool.Return(collector);
        }

        private static void Collect(Renderable target, int index, CommandBuffer<Command3D> commandBuffer, List<Renderable> stage, List<Renderable> nodes)
        {
            var collector = collector3DPool.Get();

            collector.Collect(target, index, commandBuffer, stage, nodes);

            collector3DPool.Return(collector);
        }

        public static void Collect(Renderable subtree, List<Renderable> nodes)
        {
            var subtreeRange = subtree.SubtreeRange.WithClamp(nodes.Count);

            var state = subtree.DirtState;

            if (subtree is Viewport viewport)
            {
                CollectViewport(viewport, subtreeRange.Start, stage, nodes);
            }
            else if ((subtree as Scene ?? subtree.Scene) is Scene scene)
            {
                var renderContext = scene.Viewport!.RenderContext;

                switch (scene)
                {
                    case Scene2D:
                        Collect(subtree, subtreeRange.Start, renderContext.Buffer2D, stage, nodes);

                        break;

                    case Scene3D:
                        Collect(subtree, subtreeRange.Start, renderContext.Buffer3D, stage, nodes);
                        break;
                }
            }

            if (state.HasFlags(DirtState.Subtree))
            {
                var offset = subtree.SubtreeRange.End - subtreeRange.End;

                if (offset != 0)
                {
                    Node? current = subtree;

                    while (true)
                    {
                        current = current.CompositeParent;

                        if (current == null)
                        {
                            break;
                        }

                        if (current is Renderable renderable)
                        {
                            renderable.SubtreeRange = renderable.SubtreeRange.WithEndOffset(offset);
                        }
                    }

                    foreach (var node in nodes.AsSpan(subtreeRange.End))
                    {
                        node.SubtreeRange = node.SubtreeRange.WithOffset(offset);
                    }
                }

                nodes.ReplaceRange(subtreeRange, stage.AsSpan());

                stage.Clear();
            }
        }

        public static void CollectViewport(Viewport viewport, int index, List<Renderable> stage, List<Renderable> nodes)
        {
            viewport.SubtreeRange = ShortRange.CreateWithLength(index + stage.Count, 1);

            stage.Add(viewport);

            foreach (var child in viewport)
            {
                switch (child)
                {
                    case Scene2D scene2D:
                        Collect(scene2D, index, viewport.RenderContext.Buffer2D, stage, nodes);

                        break;

                    case Scene3D scene3D:
                        Collect(scene3D, index, viewport.RenderContext.Buffer3D, stage, nodes);

                        break;
                }
            }

            viewport.SubtreeRange = viewport.SubtreeRange.WithEnd(index + stage.Count);

            viewport.MakeSubtreeStatePristine();
        }

        public static void CollectViewport<T>(Viewport viewport, Collector<T> context)
        where T : Command =>
            CollectViewport(viewport, context.Index, context.Stage, context.Nodes);
    }

    public abstract class Collector<TCommand> : IPoolable where TCommand : Command
    {
        private Renderable? hiddenSubtree;

        public List<TCommand> Commands { get; } = [];

        [AllowNull]
        public CommandBuffer<TCommand> CommandBuffer { get; private set; }

        public CommandRange CommandRange { get; private set; }

        public int Index { get; private set; }

        [AllowNull]
        public List<Renderable> Nodes { get; private set; }

        [AllowNull]
        public List<Renderable> Stage { get; private set; }

        public int  CommandOffset  => this.CommandRange.Pre.Start + this.Commands.Count;
        public int  SubtreeOffset  => this.Index + this.Stage.Count;
        public bool ScopeIsVisible => this.hiddenSubtree == null;

        private void EndHiddenScope() =>
            this.hiddenSubtree = null;

        private void StartHiddenScope(Renderable renderable) =>
            this.hiddenSubtree = renderable;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void AccumulateCommands(Renderable<TCommand> renderable)
        {
            this.CollectCommands(renderable.Commands);

            renderable.CommandRange = renderable.CommandRange.WithPreEnd(this.CommandOffset);
            renderable.CommandRange = renderable.CommandRange.WithPostEnd(this.CommandOffset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void AccumulatePreCommands(Renderable<TCommand> renderable, out int offset)
        {
            this.CollectCommands(renderable.PreCommands);

            offset = this.CommandOffset - this.CommandRange.Pre.End;

            renderable.CommandRange = renderable.CommandRange.WithPreEndAndPostOffset(this.CommandOffset, offset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void AccumulatePostCommands(Renderable<TCommand> renderable, out int offset)
        {
            var tail = this.Commands.Count;

            this.CollectCommands(renderable.PostCommands);

            var difference = this.Commands.Count - tail - renderable.CommandRange.Post.Length;

            renderable.CommandRange = renderable.CommandRange.WithPostResize(difference);

            offset = renderable.CommandRange.Post.End - this.CommandRange.Post.End;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void CollectPreCommands(Renderable<TCommand> renderable)
        {
            renderable.CommandRange = new(this.CommandOffset);

            if (this.ScopeIsVisible)
            {
                this.CollectCommands(renderable.PreCommands);

                renderable.CommandRange = renderable.CommandRange.WithPreEnd(this.CommandOffset);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void CollectPostCommands(Renderable<TCommand> renderable)
        {
            renderable.CommandRange = renderable.CommandRange.WithPostStart(this.CommandOffset);

            if (this.ScopeIsVisible)
            {
                this.CollectCommands(renderable.PostCommands);

                renderable.CommandRange = renderable.CommandRange.WithPostEnd(this.CommandOffset);
            }
        }

        protected virtual void ApplyOffset(Renderable<TCommand> subtree, ShortRange boundaryRange, int offset, bool offsetAncestors)
        {
            if (offset != 0)
            {
                boundaryRange = boundaryRange.WithClamp(this.Nodes.Count);

                if (offsetAncestors)
                {
                    Node? current = subtree;

                    while (true)
                    {
                        current = current.CompositeParent;

                        if (current == null || current is Scene)
                        {
                            break;
                        }

                        if (current is Renderable<TCommand> renderable)
                        {
                            renderable.CommandRange = renderable.CommandRange.WithPostOffset(offset);
                        }
                    }
                }

                foreach (var node in this.Nodes.AsSpan(boundaryRange))
                {
                    if (node is Renderable<TCommand> renderable)
                    {
                        renderable.CommandRange = renderable.CommandRange.WithOffset(offset);
                    }
                }
            }
        }

        protected virtual void ApplyOffsetAndUpdateBuffer(Renderable<TCommand> subtree, ShortRange boundaryRange)
        {
            var offset = subtree.CommandRange.Post.End - this.CommandRange.Post.End;

            this.ApplyOffset(subtree, boundaryRange, offset, true);
            this.UpdateBuffer(this.CommandRange);
        }

        protected virtual void Collect(Renderable subtree)
        {
            switch (subtree)
            {
                case Renderable<TCommand> renderable:
                    this.Collect(renderable);

                    break;

                default:
                    this.Collect((Scene)subtree);

                    break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Collect<T>(T scene) where T : Scene
        {
            this.StartSubtreeRange(scene);
            this.CollectSubtree(scene);
            this.EndSubtreeRange(scene);
            this.UpdateBuffer(0..);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Collect(Renderable<TCommand> subtree)
        {
            var subtreeRange = subtree.SubtreeRange;

            var boundaryRange = new ShortRange(subtreeRange.End, subtree.Scene!.SubtreeRange.End);

            if (subtree.DirtState == DirtState.Commands)
            {
                this.AccumulatePreCommands(subtree, out var preOffset);

                this.ApplyOffset(subtree, new(subtreeRange.Start + 1, subtreeRange.End), preOffset, false);

                var pre = this.Commands.AsSpan();

                var postStart = this.Commands.Count;

                this.AccumulatePostCommands(subtree, out var postOffset);

                var postCount = this.Commands.Count - postStart;

                this.ApplyOffset(subtree, boundaryRange, postOffset, true);

                var post = this.Commands.AsSpan(postStart, postCount);

                this.CommandBuffer.ReplaceCommandRange(this.CommandRange.Post, post);
                this.CommandBuffer.ReplaceCommandRange(this.CommandRange.Pre,  pre);

                subtree.MakeSubtreeStatePristine();
            }
            else
            {
                this.StartSubtreeRange(subtree);
                this.CollectSubtree(subtree);
                this.EndSubtreeRange(subtree);
                this.ApplyOffsetAndUpdateBuffer(subtree, boundaryRange);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void CollectCommands(ReadOnlySpan<TCommand> source) =>
            this.Commands.AddRange(source);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CollectSubtree(Renderable subtree)
        {
            var traversal = subtree.GetCompositeTraversalEnumerator();

            traversal.SubtreeTraversed += this.OnSubtreeTraversed;

            while (traversal.MoveNext())
            {
                switch (traversal.Current)
                {
                    case Viewport viewport:
                        Collector.CollectViewport(viewport, this);

                        traversal.SkipToNextSibling();
                        break;

                    case Renderable<TCommand> renderableT:
                        this.StartSubtreeRange(renderableT);
                        break;

                    case Renderable renderable:
                        this.StartSubtreeRange(renderable);
                        break;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void EndSubtreeRange(Renderable renderable)
        {
            renderable.SubtreeRange = renderable.SubtreeRange.WithEnd(this.SubtreeOffset);

            if (this.hiddenSubtree == renderable)
            {
                this.EndHiddenScope();
            }

            renderable.MakeSubtreeStatePristine();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void EndSubtreeRange(Renderable<TCommand> renderable)
        {
            this.CollectPostCommands(renderable);
            this.EndSubtreeRange(Unsafe.As<Renderable>(renderable));
        }

        protected void OnSubtreeTraversed(Node node)
        {
            switch (node)
            {
                case Renderable<TCommand> renderableT:
                    this.EndSubtreeRange(renderableT);
                    break;

                case Renderable renderable:
                    this.EndSubtreeRange(renderable);
                    break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void StartSubtreeRange(Renderable renderable)
        {
            renderable.SubtreeRange = ShortRange.CreateWithLength(this.SubtreeOffset, 1);

            this.Stage.Add(renderable);

            if (this.ScopeIsVisible && !renderable.Visible)
            {
                this.StartHiddenScope(renderable);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void StartSubtreeRange(Renderable<TCommand> renderable)
        {
            this.StartSubtreeRange(Unsafe.As<Renderable>(renderable));
            this.CollectPreCommands(renderable);
        }

        protected void UpdateBuffer(Range range) =>
            this.CommandBuffer.ReplaceCommandRange(range, this.Commands.AsSpan());

        public void Collect(Renderable subtree, int index, CommandBuffer<TCommand> commandBuffer, List<Renderable> stage, List<Renderable> nodes)
        {
            this.Index         = index;
            this.CommandBuffer = commandBuffer;
            this.Stage         = stage;
            this.Nodes         = nodes;

            this.CommandRange = subtree is Renderable<TCommand> renderable
                ? renderable.CommandRange.WithClamp(commandBuffer.Commands.Length)
                : new(0, commandBuffer.Commands.Length);

            this.Collect(subtree);
        }

        public void Reset()
        {
            this.Commands.Clear();

            this.CommandBuffer = default;
            this.CommandRange  = default;
            this.Index         = default;
            this.Nodes         = default;
            this.Stage         = default;
        }
    }
}
