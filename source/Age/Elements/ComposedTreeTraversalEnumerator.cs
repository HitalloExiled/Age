using System.Collections;
using System.Runtime.CompilerServices;
using Age.Scene;
using StackEntry = (Age.Elements.Slot Slot, int Index);

namespace Age.Elements;

internal struct ComposedTreeTraversalEnumerator : IEnumerator<Layoutable>, IEnumerable<Layoutable>
{
    #region 8-bytes
    private readonly Element           root;
    private readonly Stack<StackEntry> stack;

    private Layoutable? current;
    #endregion

    #region 1-byte
    private bool skipToNextSibling;
    #endregion

    public ComposedTreeTraversalEnumerator(Element root, Stack<StackEntry>? stack = null)
    {
        this.root  = root;
        this.stack = stack ?? [];

        this.Reset();
    }

    public readonly Layoutable Current => this.current!;

    readonly object IEnumerator.Current => this.Current;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly bool IsAssignedToCurrentSlot(Node node) =>
        this.stack.Count > 0 && node is Layoutable layoutable && this.stack.Peek().Slot == layoutable.AssignedSlot;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly bool IsAssignedToCurrentSlot(Layoutable layoutable) =>
        this.stack.Count > 0 && this.stack.Peek().Slot == layoutable.AssignedSlot;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly Layoutable? GetFirstChild(Node node)
    {
        if (node is Slot slot)
        {
            if (slot.Nodes.Count > 0)
            {
                this.stack.Push((slot, 0));

                return slot.Nodes[0];
            }
            else
            {
                return this.GetLayoutableOrSkip(slot.FirstChild);
            }
        }
        else if (node is Element element && element.ShadowTree != null)
        {
            return this.GetLayoutableOrSkip(element.ShadowTree.FirstChild);
        }

        return this.GetLayoutableOrSkip(node.FirstChild);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly Layoutable? GetNextSibling(Node node, out Node parent)
    {
        if (this.IsAssignedToCurrentSlot(node))
        {
            var (slot, index) = this.stack.Pop();

            parent = slot;

            if (++index < slot.Nodes.Count)
            {
                this.stack.Push((slot, index));

                return slot.Nodes[index];
            }

            return null;
        }

        parent = node.Parent!;

        if (this.GetLayoutableOrSkip(node.NextSibling) is Layoutable nextSibling)
        {
            return nextSibling;
        }

        if (parent is ShadowTree shadowTree)
        {
            parent = shadowTree.Host;

            // return this.GetLayoutableOrSkip(shadowTree.Host.FirstChild);
        }

        return null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly Layoutable? GetLayoutableOrSkip(Node? node)
    {
        do
        {
            if (node is Layoutable layoutable/*  && (layoutable.AssignedSlot == null || this.IsAssignedToCurrentSlot(layoutable)) */)
            {
                return layoutable;
            }

            if (node?.NextSibling == null)
            {
                return null;
            }

            node = node.NextSibling;
        }
        while (true);
    }

    readonly IEnumerator IEnumerable.GetEnumerator() =>
        this.GetEnumerator();

    readonly IEnumerator<Layoutable> IEnumerable<Layoutable>.GetEnumerator() =>
        this.GetEnumerator();

    public readonly void Dispose()
    { }

    public readonly ComposedTreeTraversalEnumerator GetEnumerator() => this;

    public bool MoveNext()
    {
        if (!this.skipToNextSibling && this.GetFirstChild(this.current!) is Layoutable firstChild)
        {
            this.current = firstChild;

            return true;
        }

        this.skipToNextSibling = false;

        Node node = this.current!;

        do
        {
            if (this.GetNextSibling(node, out var parent) is Layoutable nextSibling)
            {
                this.current = nextSibling;

                return true;
            }

            if (parent == this.root)
            {
                return false;
            }

            node = parent;
        }
        while (node != null);

        return false;
    }

    public void Reset()
    {
        this.skipToNextSibling = false;
        this.current           = this.root;

        this.stack.Clear();
    }

    public void SkipToNextSibling() =>
        this.skipToNextSibling = true;
}
