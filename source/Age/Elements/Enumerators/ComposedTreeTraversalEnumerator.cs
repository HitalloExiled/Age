using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Age.Scene;
using StackEntry = (Age.Elements.Slot Slot, int Index);

namespace Age.Elements.Enumerators;

internal struct ComposedTreeTraversalEnumerator : IEnumerator<Layoutable>, IEnumerable<Layoutable>
{
    #region 8-bytes
    private readonly Element           root;
    private readonly Stack<StackEntry> stack;
    private readonly Action<Element>?  parentCallback;

    private Layoutable current;
    #endregion

    #region 1-byte
    private bool skipToNextSibling;
    #endregion

    public ComposedTreeTraversalEnumerator(Element root, Stack<StackEntry>? stack = null, Action<Element>? parentCallback = null)
    {
        this.root           = root;
        this.stack          = stack ?? [];
        this.parentCallback = parentCallback;

        this.Reset();
    }

    public readonly Layoutable Current => this.current!;

    readonly object IEnumerator.Current => this.Current;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly bool IsAssignedToCurrentSlot(Node node) =>
        this.stack.Count > 0 && node is Layoutable layoutable && this.stack.Peek().Slot == layoutable.AssignedSlot;

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
                return GetLayoutableOrSkip(slot.FirstChild);
            }
        }
        else if (node is Element element && element.ShadowTree != null)
        {
            return GetLayoutableOrSkip(element.ShadowTree.FirstChild);
        }

        return GetLayoutableOrSkip(node.FirstChild);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly Layoutable? GetNextSibling(Node node, out Element parent)
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

        parent = node.Parent is ShadowTree shadowTree ? shadowTree.Host : (Element)node.Parent!;

        return GetLayoutableOrSkip(node.NextSibling);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Layoutable? GetLayoutableOrSkip(Node? node)
    {
        while (true)
        {
            if (node is Layoutable layoutable)
            {
                return layoutable;
            }

            if (node?.NextSibling == null)
            {
                return null;
            }

            node = node.NextSibling;
        }
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
        if (!this.skipToNextSibling && this.GetFirstChild(this.current) is Layoutable firstChild)
        {
            this.current = firstChild;

            return true;
        }

        this.skipToNextSibling = false;

        while (this.current != this.root)
        {
            if (this.GetNextSibling(this.current, out var parent) is Layoutable nextSibling)
            {
                this.current = nextSibling;

                return true;
            }

            this.parentCallback?.Invoke(parent);

            this.current = parent;
        }

        return false;
    }

    [MemberNotNull(nameof(current))]
    public void Reset()
    {
        this.skipToNextSibling = false;
        this.current           = this.root;

        this.stack.Clear();
    }

    public void SkipToNextSibling() =>
        this.skipToNextSibling = true;
}
