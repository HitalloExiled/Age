using Age.Commands;
using Age.Core.Extensions;
using Age.Elements;
using Age.Graphs;

namespace Age.Scenes;

internal partial class SceneGraphCache
{
    internal readonly struct Collector
    {
        private static readonly List<Renderable> stage = [];

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

        public static void Collect(Renderable target, int startIndex, CommandBuffer<Command2D> commandBuffer, List<Renderable> stage, List<Renderable> nodes)
        {
            using var collector = new Collector2D(target, startIndex, commandBuffer, stage, nodes);

            collector.Collect();
        }

        public static void Collect(Renderable target, int startIndex, CommandBuffer<Command3D> commandBuffer, List<Renderable> stage, List<Renderable> nodes)
        {
            using var collector = new Collector3D(target, startIndex, commandBuffer, stage, nodes);

            collector.Collect();
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
        }

        public static void CollectViewport<T>(Viewport viewport, in BoundaryContext<T> context)
        where T : Command =>
            CollectViewport(viewport, context.StartIndex, context.Stage, context.Nodes);
    }
}
