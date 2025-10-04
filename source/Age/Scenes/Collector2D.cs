using Age.Commands;
using Age.Core.Extensions;
using Age.Elements;
using Age.Graphs;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Scenes;

public readonly struct Collector2D : IDisposable
{
    private static readonly Stack<(Slot, int)> composedTreeStack = [];

    private readonly BoundaryContext<Command2D> context;
    private readonly Renderable                 target;

    public Collector2D(Renderable target, int startIndex, CommandBuffer<Command2D> commandBuffer, List<Renderable> stage, IReadOnlyList<Renderable> nodes)
    {
        var commandRange = target is Renderable<Command2D> renderable2D
            ? renderable2D.CommandRange
            : new(
                new(0, 0, (ushort)commandBuffer.Colors.Length),
                new(0, 0, (ushort)commandBuffer.Indices.Length)
            );

        this.context = new(startIndex, commandRange, commandBuffer, stage, nodes);
        this.target  = target;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly void CollectElementPreCommands(Element element)
    {
        element.PreCommandRange = this.context.CreateCommandRange();

        this.context.CollectCommands(element.PreCommands);

        element.PreCommandRange = this.context.WithEnd(element.PreCommandRange);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly void CollectElementPostCommands(Element element)
    {
        element.PostCommandRange = this.context.CreateCommandRange();

        this.context.CollectCommands(element.PostCommands);

        element.PostCommandRange = this.context.WithEnd(element.PostCommandRange);
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
        var boundaryRange = subtree.SubtreeRange.End..subtree.Scene!.SubtreeRange.End;

        this.CollectElement(subtree);

        this.UpdateBuffer(subtree, boundaryRange);
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
        this.context.UpdateBuffer(true);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly void Collect(Renderable<Command2D> subtree)
    {
        var boundaryRange = subtree.SubtreeRange.End..subtree.Scene!.SubtreeRange.End;

        this.context.StartSubtreeRange(subtree, true);

        this.CollectSubtree(subtree);

        this.context.EndSubtreeRange(subtree, true);
        this.UpdateBuffer(subtree, boundaryRange);
    }

    private void UpdateBuffer(Renderable<Command2D> subtree, Range boundaryRange)
    {
        var colorOffset = (short)(subtree.CommandRange.Color.Extend - this.context.CommandRange.Color.Extend);
        var indexOffset = (short)(subtree.CommandRange.Index.Extend - this.context.CommandRange.Index.Extend);

        if (colorOffset > 0 || indexOffset > 0)
        {
            Node? parent = subtree;

            while (true)
            {
                parent = parent is Element element ? element.ComposedParentElement : parent.Parent;

                if (parent == null || parent is Scene)
                {
                    break;
                }

                switch (parent)
                {
                    case Element parentElement:
                        parentElement.PostCommandRange = parentElement.PostCommandRange.WithExtendOffset(colorOffset, indexOffset);
                        break;
                    case Renderable<Command2D> parentRenderable:
                        parentRenderable.CommandRange = parentRenderable.CommandRange.WithExtendOffset(colorOffset, indexOffset);
                        break;
                }
            }

            foreach (var node in Unsafe.As<List<Renderable>>(this.context.Nodes).AsSpan(boundaryRange))
            {
                if (node is Element element)
                {
                    element.PreCommandRange = element.PreCommandRange.WithOffset(colorOffset, indexOffset);
                    element.PostCommandRange = element.PostCommandRange.WithOffset(colorOffset, indexOffset);
                }
                else if (node is Renderable<Command2D> renderable)
                {
                    renderable.CommandRange = renderable.CommandRange.WithOffset(colorOffset, indexOffset);
                }
            }
        }

        this.context.UpdateBuffer(false);
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
