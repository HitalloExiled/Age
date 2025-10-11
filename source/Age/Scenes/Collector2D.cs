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

    public Collector2D(Renderable target, int startIndex, CommandBuffer<Command2D> commandBuffer, List<Renderable> stage, List<Renderable> nodes)
    {
        var commandRange = target is Renderable<Command2D> renderable2D
            ? renderable2D.CommandRange
            : new(
                new(0, (ushort)commandBuffer.Colors.Length),
                new(0, (ushort)commandBuffer.Encodes.Length)
            );

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
        var colorIndex  = this.context.Colors.Count;
        var encodeIndex = this.context.Encodes.Count;

        this.context.CollectCommands(element.PostCommands);

        var colorDiff  = (short)(element.CommandRange.Color.Post.Length - (this.context.Colors.Count - colorIndex));
        var encodeDiff = (short)(element.CommandRange.Encode.Post.Length - (this.context.Encodes.Count - encodeIndex));

        element.CommandRange = element.CommandRange.WithPostResize(colorDiff, encodeDiff);
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

            var preColorOffset  = (short)(subtree.CommandRange.Color.Pre.End  - this.context.CommandRange.Color.Pre.End);
            var preEncodeOffset = (short)(subtree.CommandRange.Encode.Pre.End - this.context.CommandRange.Encode.Pre.End);

            this.ApplyOffset(subtree, new((ushort)(subtreeRange.Start + 1), subtreeRange.End), preColorOffset, preEncodeOffset);

            this.context.CommandBuffer.ReplaceColorCommandRange(this.context.CommandRange.Color.Pre, this.context.Colors.AsSpan());
            this.context.CommandBuffer.ReplaceEncodeCommandRange(this.context.CommandRange.Encode.Pre, this.context.Encodes.AsSpan());

            var postColorIndex  = this.context.Colors.Count;
            var postEncodeIndex = this.context.Encodes.Count;

            this.AccumulateElementPostCommands(subtree);

            var postColorOffset  = (short)(subtree.CommandRange.Color.Post.End  - this.context.CommandRange.Color.Post.End);
            var postEncodeOffset = (short)(subtree.CommandRange.Encode.Post.End - this.context.CommandRange.Encode.Post.End);

            this.ApplyOffset(subtree, boundaryRange, postColorOffset, postEncodeOffset);

            var postColorCount  = this.context.Colors.Count - postColorIndex;
            var postEncodeCount = this.context.Encodes.Count - postEncodeIndex;

            this.context.CommandBuffer.ReplaceColorCommandRange(this.context.CommandRange.Color.Post, this.context.Colors.AsSpan(postColorIndex, postColorCount));
            this.context.CommandBuffer.ReplaceEncodeCommandRange(this.context.CommandRange.Encode.Post, this.context.Encodes.AsSpan(postEncodeIndex, postEncodeCount));

            subtree.MakeSubtreePristine();
        }
        else
        {
            this.CollectElement(subtree);
            this.ApplyOffsetAndUpdateBuffer(subtree, boundaryRange);
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
        this.context.UpdateBuffer(true);
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

    private void ApplyOffset(Renderable<Command2D> subtree, ShortRange boundaryRange, short colorOffset, short encodeOffset)
    {
        if (colorOffset != 0 || encodeOffset != 0)
        {
            boundaryRange = boundaryRange.WithClamp((ushort)this.context.Nodes.Count);

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
                    parentRenderable.CommandRange = parentRenderable.CommandRange.WithPostOffset(colorOffset, encodeOffset);
                }
            }

            foreach (var node in this.context.Nodes.AsSpan(boundaryRange))
            {
                if (node is Renderable<Command2D> renderable)
                {
                    renderable.CommandRange = renderable.CommandRange.WithOffset(colorOffset, encodeOffset);
                }
            }
        }
    }

    private void ApplyOffsetAndUpdateBuffer(Renderable<Command2D> subtree, ShortRange boundaryRange)
    {
        var colorOffset  = (short)(subtree.CommandRange.Color.Post.End  - this.context.CommandRange.Color.Post.End);
        var encodeOffset = (short)(subtree.CommandRange.Encode.Post.End - this.context.CommandRange.Encode.Post.End);

        this.ApplyOffset(subtree, boundaryRange, colorOffset, encodeOffset);

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

        #if DEBUG
        asset(this.context.CommandBuffer.Colors);
        asset(this.context.CommandBuffer.Encodes);

        static void asset(ReadOnlySpan<Command2D> commands)
        {
            for (var i = 0; i < commands.Length; i++)
            {
                if (commands[i].Owner == null)
                {
                    throw new InvalidOperationException();
                }
            }
        }
        #endif
    }

    public readonly void Dispose() =>
        this.context.Dispose();
}
