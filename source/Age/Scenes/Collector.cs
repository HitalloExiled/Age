using Age.Commands;
using Age.Core.Extensions;
using Age.Elements;
using Age.Graphs;

namespace Age.Scenes;

internal readonly struct Collector
{
    private static readonly List<Renderable> stage = [];

    public static void Collect(Renderable root, List<Renderable> nodes)
    {
        var subtreeRange = root.SubtreeRange;

        if (root is Viewport viewport)
        {
            CollectViewport(viewport, subtreeRange.Start, stage, nodes);
        }
        else if ((root as Scene ?? root.Scene) is Scene scene)
        {
            var renderContext = scene.Viewport!.RenderContext;

            switch (scene)
            {
                case Scene2D:
                    Collect(root, subtreeRange.Start, renderContext.Buffer2D, stage, nodes);

                    break;

                case Scene3D:
                    Collect(root, subtreeRange.Start, renderContext.Buffer3D, stage, nodes);
                    break;
            }
        }

        var offset = (short)(root.SubtreeRange.End - subtreeRange.End);

        if (offset > 0)
        {
            Node? parent = root;

            do
            {
                parent = parent switch
                {
                    Element element => element.ComposedParentElement,
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
                node.SubtreeRange = node.SubtreeRange.WithOffset(offset);
            }
        }

        nodes.ReplaceRange(subtreeRange, stage.AsSpan());

        stage.Clear();
    }

    public static void Collect(Renderable target, int startIndex, CommandBuffer<Command2D> commandBuffer, List<Renderable> stage, IReadOnlyList<Renderable> nodes)
    {
        using var collector = new Collector2D(target, startIndex, commandBuffer, stage, nodes);

        collector.Collect();
    }

    public static void Collect(Renderable target, int startIndex, CommandBuffer<Command3D> commandBuffer, List<Renderable> stage, IReadOnlyList<Renderable> nodes)
    {
        using var collector = new Collector3D(target, startIndex, commandBuffer, stage, nodes);

        collector.Collect();
    }

    public static void CollectViewport(Viewport viewport, int index, List<Renderable> stage, IReadOnlyList<Renderable> nodes)
    {
        viewport.SubtreeRange = SubtreeRange.CreateWithLength((ushort)(index + stage.Count), 1);

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

        viewport.SubtreeRange = viewport.SubtreeRange.WithEnd((ushort)(index + stage.Count));
    }

    public static void CollectViewport<T>(Viewport viewport, in BoundaryContext<T> context)
    where T : Command =>
        CollectViewport(viewport, context.StartIndex, context.Stage, context.Nodes);
}
