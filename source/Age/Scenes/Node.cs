using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using Age.Core;
using Age.Core.Extensions;

namespace Age.Scenes;

public abstract partial class Node : Disposable, IEnumerable<Node>, IComparable<Node>
{
    private NodeFlags nodeFlags;

    internal int Index
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                this.OnIndexed();
            }
        }
    }

    public Node? FirstChild      { get; private set; }
    public Node? LastChild       { get; private set; }
    public Node? NextSibling     { get; private set; }
    public Node? Parent          { get; private set; }
    public Node? PreviousSibling { get; private set; }

    public bool IsConnected { get; private set; }

    public Node[] Children
    {
        get => [.. this];
        set => this.ReplaceChildren(value);
    }

    [MemberNotNullWhen(false, nameof(FirstChild), nameof(LastChild))]
    public bool IsLeaf => this.FirstChild == null;

    public Node Root
    {
        get
        {
            var current = this;

            while (current.Parent != null)
            {
                current = current.Parent;
            }

            return current;
        }
    }

    public bool IsChildrenUpdatesSuspended => this.nodeFlags.HasFlags(NodeFlags.ChildrenUpdatesSuspended);
    public bool IsSealed                   => this.nodeFlags.HasFlags(NodeFlags.Sealed);
    public bool IsUpdatesSuspended         => this.nodeFlags.HasFlags(NodeFlags.UpdatesSuspended);

    public virtual string? Name { get; set; }

    public abstract string NodeName { get; }

    public Scene? Scene { get; internal set; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ClearParenting(Node parent, Node node, bool dispose)
    {
        InvokeDetachingCallbacks(parent, node);

        if (parent.IsConnected)
        {
            node.PropagateOnDisconnecting();
        }

        if (dispose)
        {
            node.Dispose();
        }

        node.Parent          = null;
        node.PreviousSibling = null;
        node.NextSibling     = null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void InvokeAttachedCallbacks(Node parent, Node node)
    {
        parent.OnChildAttachedInternal(node);
        parent.OnChildAttached(node);

        node.OnAttachedInternal();
        node.OnAttached();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void InvokeConnectedCallbacks(Node node)
    {
        node.OnConnectedInternal();
        node.OnConnected();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void InvokeDetachingCallbacks(Node parent, Node node)
    {
        parent.OnChildDetachingInternal(node);
        parent.OnChildDetaching(node);

        node.OnDetachingInternal();
        node.OnDetaching();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void InvokeDisconnectingCallbacks(Node node)
    {
        node.OnDisconnectingInternal();
        node.OnDisconnecting();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void InvokeDisposedCallbacks(Node node)
    {
        node.OnDisposedInternal();
        node.OnDisposed();
    }

    internal static Node[] SelectBetween(Node start, Node end)
    {
        if (start == end)
        {
            return [];
        }

        var ancestor = GetCommonAncestor(start, end);

        var (left, right) = start < end
            ? (start, end)
            : (end, start);

        var leftNodes  = new List<Node>();
        var rightNodes = new List<Node>();

        while (left!.Parent != ancestor)
        {
            for (var nextSibling = left.NextSibling; nextSibling != null; nextSibling = nextSibling.NextSibling)
            {
                leftNodes.Add(nextSibling);
            }

            left = left.Parent;
        }

        while (right!.Parent != ancestor)
        {
            for (var previousSibling = right.PreviousSibling; previousSibling != null; previousSibling = previousSibling.PreviousSibling)
            {
                rightNodes.Add(previousSibling);
            }

            right = right.Parent;
        }

        rightNodes.Reverse();

        return [..leftNodes, ..rightNodes];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SetParenting(Node parent, Node node)
    {
        node.Parent = parent;

        InvokeAttachedCallbacks(parent, node);

        if (parent.IsConnected)
        {
            node.PropagateOnConnected();
        }
    }

    public static Node? GetCommonAncestor(Node left, Node right)
    {
        if (left.Parent == right.Parent)
        {
            return left.Parent;
        }
        else if (left == right.Parent)
        {
            return left;
        }
        else if (left.Parent == right)
        {
            return right;
        }
        else
        {
            var leftDepth  = 0;
            var rightDepth = 0;

            var currentLeft  = left.Parent;
            var currentRight = right.Parent;

            while (currentLeft != null)
            {
                leftDepth++;
                currentLeft  = currentLeft.Parent;
            }

            while (currentRight != null)
            {
                rightDepth++;
                currentRight  = currentRight.Parent;
            }

            currentLeft  = left;
            currentRight = right;

            while (leftDepth > rightDepth)
            {
                currentLeft = currentLeft?.Parent;
                leftDepth--;
            }

            while (leftDepth < rightDepth)
            {
                currentRight = currentRight?.Parent;
                rightDepth--;
            }

            while (currentLeft != currentRight)
            {
                currentLeft  = currentLeft?.Parent;
                currentRight = currentRight?.Parent;
            }

            return currentLeft;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() =>
        this.GetEnumerator();

    IEnumerator<Node> IEnumerable<Node>.GetEnumerator() =>
        this.GetEnumerator();

    private void AppendOrPrepend(Node node, bool append)
    {
        this.ThowIfSealed();

        if (node == this)
        {
            throw new InvalidOperationException("Cant add node to itself");
        }

        node.Detach();

        if (append)
        {
            if (this.LastChild != null)
            {
                this.LastChild.NextSibling = node;
                node.PreviousSibling = this.LastChild;

                this.LastChild = node;
            }
            else
            {
                this.FirstChild = this.LastChild = node;
            }
        }
        else if (this.FirstChild != null)
        {
            this.FirstChild.PreviousSibling = node;
            node.NextSibling = this.FirstChild;

            this.FirstChild = node;
        }
        else
        {
            this.FirstChild = this.LastChild = node;
        }

        SetParenting(this, node);
    }

    private void AppendOrPrepend(scoped ReadOnlySpan<Node> nodes, bool append)
    {
        this.ThowIfSealed();

        if (nodes.Length == 0)
        {
            return;
        }

        foreach (var node in nodes)
        {
            if (node == this)
            {
                throw new InvalidOperationException("Cant add node to itself");
            }

            node.Detach();
        }

        var first = nodes[0];
        var last  = nodes[^1];

        if (append)
        {
            if (this.LastChild == null)
            {
                this.FirstChild = first;
            }
            else
            {
                this.LastChild.NextSibling = first;
                first.PreviousSibling = this.LastChild;
            }

            this.LastChild = last;
        }
        else
        {
            if (this.FirstChild == null)
            {
                this.LastChild = last;
            }
            else
            {
                this.FirstChild.PreviousSibling = last;
                last.NextSibling = this.FirstChild;
            }

            this.FirstChild = first;
        }

        if (nodes.Length > 1)
        {
            first.NextSibling = nodes[1];
            last.PreviousSibling = nodes[^2];
        }

        for (var i = 1; i < nodes.Length - 1; i++)
        {
            nodes[i].PreviousSibling = nodes[i - 1];
            nodes[i].NextSibling = nodes[i + 1];
        }

        foreach (var node in nodes)
        {
            SetParenting(this, node);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ThowIfSealed()
    {
        if (this.IsSealed)
        {
            throw new InvalidOperationException("Cannot modify a sealed node.");
        }
    }

    private void DetachChildren(bool dispose = false)
    {
        this.ThowIfSealed();

        for (var current = this.LastChild; current != null; current = this.LastChild)
        {
            var previousSibling = current.PreviousSibling;

            ClearParenting(this, current, dispose);

            this.LastChild = previousSibling;
            previousSibling?.NextSibling = null;
        }

        this.FirstChild = null;
    }

    private void DetachChildrenInRange(Node start, Node end, bool dispose)
    {
        this.ThowIfSealed();

        if (start.Parent != this || end.Parent != this)
        {
            throw new InvalidOperationException("Start and end must be child of this node");
        }

        if (start > end)
        {
            (start, end) = (end, start);
        }

        var startPrevious = start.PreviousSibling;
        var endNext       = end.NextSibling;

        for (Node? current = end, previous; ; current = previous)
        {
            previous = current!.PreviousSibling;

            ClearParenting(this, current, dispose);

            previous?.NextSibling = endNext;

            if (current == start)
            {
                break;
            }
        }

        if (startPrevious == null)
        {
            this.FirstChild = endNext;
        }
        else
        {
            startPrevious.NextSibling = endNext;
        }

        if (endNext == null)
        {
            this.LastChild = startPrevious;
        }
        else
        {
            endNext.PreviousSibling = startPrevious;
        }
    }

    private void InsertAfterOrBefore(Node reference, Node node, bool after)
    {
        this.ThowIfSealed();

        if (reference.Parent != this)
        {
            throw new InvalidOperationException("Reference node is not child of this node");
        }

        if (node == this)
        {
            throw new InvalidOperationException("Cant add node to itself");
        }

        if (node == reference)
        {
            throw new InvalidOperationException($"Cant intset node {(after ? "after" : "before")} itself");
        }

        node.Detach();

        if (after)
        {
            if (reference.NextSibling == null)
            {
                this.LastChild = node;
            }
            else
            {
                reference.NextSibling.PreviousSibling = node;
            }

            node.PreviousSibling = reference;
            node.NextSibling = reference.NextSibling;

            reference.NextSibling = node;
        }
        else
        {
            if (reference.PreviousSibling == null)
            {
                this.FirstChild = node;
            }
            else
            {
                reference.PreviousSibling.NextSibling = node;
            }

            node.PreviousSibling = reference.PreviousSibling;
            node.NextSibling = reference;

            reference.PreviousSibling = node;
        }

        SetParenting(this, node);
    }

    private void InsertAfterOrBefore(Node reference, scoped ReadOnlySpan<Node> nodes, bool after)
    {
        this.ThowIfSealed();

        if (reference.Parent != this)
        {
            throw new InvalidOperationException("Reference node is not child of this node");
        }

        if (nodes.Length == 0)
        {
            return;
        }

        foreach (var node in nodes)
        {
            if (node == this)
            {
                throw new InvalidOperationException("Cant add node to itself");
            }

            if (node == reference)
            {
                throw new InvalidOperationException($"Cant intset node {(after ? "after" : "before")} itself");
            }

            node.Detach();
        }

        var first = nodes[0];
        var last  = nodes[^1];

        if (after)
        {
            first.PreviousSibling = reference;

            if (reference.NextSibling == null)
            {
                this.LastChild = last;
            }
            else
            {
                reference.NextSibling.PreviousSibling = last;
            }

            last.NextSibling = reference.NextSibling;

            reference.NextSibling = first;
        }
        else
        {
            if (reference.PreviousSibling == null)
            {
                this.FirstChild = first;
            }
            else
            {
                reference.PreviousSibling.NextSibling = first;
            }

            first.PreviousSibling = reference.PreviousSibling;

            last.NextSibling = reference;

            reference.PreviousSibling = last;
        }

        if (nodes.Length > 1)
        {
            first.NextSibling = nodes[1];
            last.PreviousSibling = nodes[^2];
        }

        for (var i = 1; i < nodes.Length - 1; i++)
        {
            nodes[i].PreviousSibling = nodes[i - 1];
            nodes[i].NextSibling     = nodes[i + 1];
        }

        foreach (var node in nodes)
        {
            SetParenting(this, node);
        }
    }

    private void PropagateOnConnected()
    {
        apply(this);

        var enumerator = this.GetTraversalEnumerator();

        while (enumerator.MoveNext())
        {
            apply(enumerator.Current);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void apply(Node node)
        {
            node.IsConnected = true;

            node.Scene = node.Parent is Scene scene ? scene : node.Parent?.Scene;

            InvokeConnectedCallbacks(node);
        }
    }

    private void PropagateOnDisconnecting()
    {
        apply(this);

        var enumerator = this.GetTraversalEnumerator();

        while (enumerator.MoveNext())
        {
            apply(enumerator.Current);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void apply(Node node)
        {
            InvokeDisconnectingCallbacks(node);

            node.IsConnected = false;
            node.Scene       = null;
        }
    }

    private protected virtual void OnAttachedInternal() { }
    private protected virtual void OnChildAttachedInternal(Node node) { }
    private protected virtual void OnChildDetachingInternal(Node node) { }
    private protected virtual void OnChildDetachedInternal(Node node) { }
    private protected virtual void OnConnectedInternal() { }
    private protected virtual void OnDisconnectingInternal() { }
    private protected virtual void OnDisposedInternal() { }
    private protected virtual void OnIndexed() { }
    private protected virtual void OnDetachingInternal() { }
    private protected virtual void OnDetachedInternal() { }

    protected sealed override void OnDisposed(bool disposing)
    {
        if (disposing)
        {
            InvokeDisposedCallbacks(this);

            foreach (var child in this.GetTraversalEnumerator())
            {
                child.Dispose();
            }
        }
    }

    protected void Seal() =>
        this.nodeFlags |= NodeFlags.Sealed;

    protected void Unseal() =>
        this.nodeFlags &= ~NodeFlags.Sealed;

    protected void SuspendUpdates() =>
        this.nodeFlags |= NodeFlags.UpdatesSuspended;

    protected void ResumeUpdates() =>
        this.nodeFlags &= ~NodeFlags.UpdatesSuspended;

    protected void SuspendChildrenUpdates() =>
        this.nodeFlags |= NodeFlags.ChildrenUpdatesSuspended;

    protected void ResumeChildrenUpdates() =>
        this.nodeFlags &= ~NodeFlags.ChildrenUpdatesSuspended;

    protected virtual void OnAttached() { }
    protected virtual void OnChildAttached(Node node) { }
    protected virtual void OnChildDetaching(Node node) { }
    protected virtual void OnChildDetached(Node node) { }
    protected virtual void OnConnected() { }
    protected virtual void OnDisconnecting() { }
    protected virtual void OnDisposed() { }
    protected virtual void OnDetaching() { }
    protected virtual void OnDetached() { }

    internal void Connect()
    {
        if (!this.IsConnected)
        {
            if (this.Parent != null)
            {
                throw new InvalidOperationException();
            }

            this.PropagateOnConnected();
        }
    }

    internal void Disconnect()
    {
        if (this.IsConnected)
        {
            if (this.Parent != null)
            {
                throw new InvalidOperationException();
            }

            this.PropagateOnDisconnecting();
        }
    }

    public void AppendChild(Node node) =>
        this.AppendOrPrepend(node, true);

    public void AppendChildren(scoped ReadOnlySpan<Node> nodes) =>
        this.AppendOrPrepend(nodes, true);

    public int CompareTo(Node? other)
    {
        if (other == null)
        {
            return 1;
        }
        else if (this == other)
        {
            return 0;
        }

        var left  = this;
        var right = other;

        if (left.Parent != right.Parent)
        {
            var leftDepth  = left.GetDepth();
            var rightDepth = right.GetDepth();

            while (leftDepth > rightDepth)
            {
                if (left.Parent == right)
                {
                    return 1;
                }

                left = left.Parent!;
                leftDepth--;
            }

            while (leftDepth < rightDepth)
            {
                if (right.Parent == left)
                {
                    return -1;
                }

                right = right.Parent!;
                rightDepth--;
            }

            while (left.Parent != right.Parent)
            {
                left  = left.Parent!;
                right = right.Parent!;
            }
        }

        if (left.Parent == right.Parent)
        {
            if (left.Parent == null)
            {
                throw new InvalidOperationException("Can't compare an root node to another");
            }

            if (left == right.NextSibling)
            {
                return 1;
            }

            if (left != right.PreviousSibling)
            {
                for (var node = left.PreviousSibling; node != null; node = node?.PreviousSibling)
                {
                    if (node == right)
                    {
                        return 1;
                    }
                }
            }
        }

        return -1;
    }

    public void Detach() =>
        this.Parent?.DetachChild(this);

    public void DetachChild(Node node)
    {
        this.ThowIfSealed();

        if (node.Parent != this)
        {
            throw new InvalidOperationException("Node is not child of this node");
        }

        if (node.PreviousSibling == null)
        {
            this.FirstChild = node.NextSibling;
        }
        else
        {
            node.PreviousSibling.NextSibling = node.NextSibling;
        }

        if (node.NextSibling == null)
        {
            this.LastChild = node.PreviousSibling;
        }
        else
        {
            node.NextSibling.PreviousSibling = node.PreviousSibling;
        }

        ClearParenting(this, node, false);
    }

    public void DetachChildren() =>
        this.DetachChildren(false);

    public void DetachChildrenInRange(Node start, Node end) =>
        this.DetachChildrenInRange(start, end, false);

    public void DisposeChildren() =>
        this.DetachChildren(true);

    public void DisposeChildrenInRange(Node start, Node end) =>
        this.DetachChildrenInRange(start, end, true);

    public int GetDepth()
    {
        var depth = 0;

        var node = this.Parent;

        while (node != null)
        {
            depth++;
            node = node.Parent;
        }

        return depth;
    }

    public Enumerator GetEnumerator() =>
        new(this);

    public TraversalEnumerator GetTraversalEnumerator() =>
        new(this);

    public void InsertAfter(Node reference, Node node) =>
        this.InsertAfterOrBefore(reference, node, true);

    public void InsertAfterSelf(Node node) =>
        this.Parent?.InsertAfter(this, node);

    public void InsertBefore(Node node, Node reference) =>
        this.InsertAfterOrBefore(reference, node, false);

    public void InsertBeforeSelf(Node node) =>
        this.Parent?.InsertBefore(node, this);

    public void InsertNodesAfterSelf(scoped ReadOnlySpan<Node> nodes) =>
        this.Parent?.InsertNodesAfter(this, nodes);

    public void InsertNodesAfter(Node reference, scoped ReadOnlySpan<Node> nodes) =>
        this.InsertAfterOrBefore(reference, nodes, true);

    public void InsertNodesBefore(scoped ReadOnlySpan<Node> nodes, Node reference) =>
        this.InsertAfterOrBefore(reference, nodes, false);

    public void InsertNodesBeforeSelf(scoped ReadOnlySpan<Node> nodes) =>
        this.Parent?.InsertNodesBefore(nodes, this);

    public bool IsAncestor(Node other)
    {
        if (this == other)
        {
            return false;
        }

        var parent = other.Parent;

        while (parent != this)
        {
            if (parent == null)
            {
                return false;
            }

            parent = parent.Parent;
        }

        return true;
    }

    public bool IsDescendent(Node other) =>
        other.IsAncestor(this);

    public void PrependChild(Node node) =>
        this.AppendOrPrepend(node, false);

    public void PrependChildren(scoped ReadOnlySpan<Node> nodes) =>
        this.AppendOrPrepend(nodes, false);

    public void Replace(Node target, Node node)
    {
        this.ThowIfSealed();

        if (target.Parent != this)
        {
            throw new InvalidOperationException("Target is not child of this node");
        }

        if (node == this)
        {
            throw new InvalidOperationException("Cant add node to itself");
        }

        if (node == target)
        {
            throw new InvalidOperationException("Cant replace node by itself");
        }

        var previous = target.PreviousSibling;
        var next = target.NextSibling;

        ClearParenting(this, target, false);

        node.Detach();

        if (previous == null)
        {
            this.FirstChild = node;
        }
        else
        {
            previous.NextSibling = node;
        }

        if (next == null)
        {
            this.LastChild = node;
        }
        else
        {
            next.PreviousSibling = node;
        }

        node.PreviousSibling = previous;
        node.NextSibling = next;

        SetParenting(this, node);
    }

    public void ReplaceSelf(Node node) =>
        this.Parent?.Replace(this, node);

    public void ReplaceChildren(Node node)
    {
        this.DetachChildren();
        this.AppendChild(node);
    }

    public void ReplaceChildren(scoped ReadOnlySpan<Node> nodes)
    {
        if (this.FirstChild != null)
        {
            this.DetachChildren();
        }

        this.AppendChildren(nodes);
    }

    public void ReplaceWith(Node target, scoped ReadOnlySpan<Node> nodes)
    {
        this.ThowIfSealed();

        if (target.Parent != this)
        {
            throw new InvalidOperationException("Target is not child of this node");
        }

        if (nodes.Length == 0)
        {
            return;
        }

        foreach (var node in nodes)
        {
            if (node == this)
            {
                throw new InvalidOperationException("Cant add node to itself");
            }

            if (node == target)
            {
                throw new InvalidOperationException("Cant replace node by itself");
            }

            node.Detach();
        }

        var previous = target.PreviousSibling;
        var next     = target.NextSibling;

        ClearParenting(this, target, false);

        var first = nodes[0];
        var last  = nodes[^1];

        if (previous == null)
        {
            this.FirstChild = first;
        }
        else
        {
            previous.NextSibling = first;
        }

        first.PreviousSibling = previous;

        if (nodes.Length > 1)
        {
            first.NextSibling = nodes[1];
            last.PreviousSibling = nodes[^2];
        }

        last.NextSibling = next;

        if (next == null)
        {
            this.LastChild = last;
        }
        else
        {
            next.PreviousSibling = last;
        }

        for (var i = 1; i < nodes.Length - 1; i++)
        {
            nodes[i].PreviousSibling = nodes[i - 1];
            nodes[i].NextSibling     = nodes[i + 1];
        }

        foreach (var node in nodes)
        {
            SetParenting(this, node);
        }
    }

    public void ReplaceSelfWith(scoped ReadOnlySpan<Node> nodes) =>
        this.Parent?.ReplaceWith(this, nodes);

    public virtual void Initialize()
    { }

    public virtual void LateUpdate()
    { }

    public virtual void Update()
    { }

    public override string ToString()
    {
        var builder = new StringBuilder(255);

        builder.Append('<');
        builder.Append(this.NodeName);

        if (!string.IsNullOrEmpty(this.Name))
        {
            builder.Append($" name='{this.Name}'");
        }

        builder.Append('>');

        return builder.ToString();
    }

    public static bool operator >(Node left, Node right) => left.CompareTo(right) == 1;
    public static bool operator <(Node left, Node right) => left.CompareTo(right) == -1;
}
