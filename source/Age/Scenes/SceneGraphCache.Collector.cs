using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Age.Commands;
using Age.Core;
using Age.Core.Extensions;
using Age.Core.Interfaces;
using Age.Elements;
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
                    Node? parent = subtree;

                    do
                    {
                        parent = parent switch
                        {
                            Element element => element.ComposedParent,
                            _ => parent.Parent,
                        };

                        if (parent is Renderable renderable)
                        {
                            renderable.SubtreeRange = renderable.SubtreeRange.WithEndOffset(offset);
                        }
                    }
                    while (parent != null);

                    foreach (var node in nodes.AsSpan(subtreeRange.End))
                    {
                        if (node.IsConnected)
                        {
                            node.SubtreeRange = node.SubtreeRange.WithOffset(offset);
                        }
                        else
                        {
                            Console.WriteLine($"Updating {subtree}(connected: {subtree.IsConnected})[{subtreeRange}], {node} is disconnected");
                        }
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

            viewport.MakeSubtreePristine();
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
        }

        protected virtual void ApplyOffset(Renderable<TCommand> subtree, ShortRange boundaryRange, int offset)
        {
            if (offset != 0)
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
                        case Renderable<TCommand> parentRenderable:
                            parentRenderable.CommandRange = parentRenderable.CommandRange.WithPostOffset(offset);
                            break;
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

            this.ApplyOffset(subtree, boundaryRange, offset);

            this.UpdateBuffer(this.CommandRange.Pre);
        }

        protected virtual void Collect(Renderable subtree)
        {
            switch (subtree)
            {
                case Renderable<TCommand> renderable3D:
                    this.Collect(renderable3D);

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
            var boundaryRange = new ShortRange(subtree.SubtreeRange.End, subtree.Scene!.SubtreeRange.End);

            if (subtree.DirtState == DirtState.Commands)
            {
                this.AccumulateCommands(subtree);

                subtree.MakeSubtreePristine();
            }
            else
            {
                this.StartSubtreeRange(subtree, true);

                this.CollectSubtree(subtree);

                this.EndSubtreeRange(subtree, true);
            }

            this.ApplyOffsetAndUpdateBuffer(subtree, boundaryRange);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void CollectCommands(ReadOnlySpan<TCommand> source) =>
            this.Commands.AddRange(source);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void CollectCommands(Renderable<TCommand> renderable)
        {
            renderable.CommandRange = new CommandRange(this.CommandOffset);

            if (this.ScopeIsVisible)
            {
                this.CollectCommands(renderable.Commands);

                renderable.CommandRange = renderable.CommandRange.WithPreEnd(this.CommandOffset);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CollectSubtree(Renderable subtree)
        {
            var traversal = subtree.GetTraversalEnumerator();

            traversal.SubtreeTraversed += this.OnSubtreeTraversed;

            while (traversal.MoveNext())
            {
                this.Collect(ref traversal);
            }
        }

        protected virtual void Collect(ref Node.TraversalEnumerator traversal)
        {
            switch (traversal.Current)
            {
                case Viewport viewport:
                    Collector.CollectViewport(viewport, this);

                    traversal.SkipToNextSibling();
                    break;

                case Renderable<TCommand> renderableT:
                    this.StartSubtreeRange(renderableT, true);
                    break;

                case Renderable renderable:
                    this.StartSubtreeRange(renderable);
                    break;

                default:
                    traversal.SkipToNextSibling();

                    break;
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
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void EndSubtreeRange(Renderable<TCommand> renderable, bool extendCommandRange)
        {
            this.EndSubtreeRange(renderable);

            if (extendCommandRange)
            {
                renderable.CommandRange = renderable.CommandRange.WithPost(this.CommandOffset);
            }
        }

        protected void OnSubtreeTraversed(Node node)
        {
            switch (node)
            {
                case Renderable<TCommand> renderableT:
                    this.EndSubtreeRange(renderableT, true);
                    break;

                case Renderable renderable:
                    this.EndSubtreeRange(renderable);
                    break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void StartSubtreeRange(Renderable renderable)
        {
            renderable.MakeSubtreePristine();

            renderable.SubtreeRange = ShortRange.CreateWithLength(this.SubtreeOffset, 1);

            this.Stage.Add(renderable);

            if (this.ScopeIsVisible && !renderable.Visible)
            {
                this.StartHiddenScope(renderable);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void StartSubtreeRange(Renderable<TCommand> renderable, bool collectCommands)
        {
            this.StartSubtreeRange(renderable);

            if (collectCommands)
            {
                this.CollectCommands(renderable);
            }
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
