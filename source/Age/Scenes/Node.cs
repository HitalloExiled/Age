using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using Age.Core;
using Age.Core.Extensions;

namespace Age.Scenes;

public abstract partial class Node : Disposable, IComparable<Node>
{
    private NodeFlags nodeFlags;
    private bool hasStarted;

    internal protected Node? ShadowHost { get; private set; }
    internal protected Node? ShadowRoot { get; private set; }

    internal protected Node? CompositeParent => this.ShadowHost ?? this.Parent;

    internal ushort CompositeDepth { get; private set; }

    public Node? FirstChild      { get; private set; }
    public Node? LastChild       { get; private set; }
    public Node? NextSibling     { get; private set; }
    public Node? Parent          { get; private set; }
    public Node? PreviousSibling { get; private set; }

    public ushort Depth { get; private set; }

    public bool IsConnected { get; private set; }
    public Node[] Children
    {
        get => [.. this];
        set => this.ReplaceChildren(value);
    }

    public virtual string? Name { get; set; }

    public Scene? Scene { get; private set; }

    [MemberNotNullWhen(false, nameof(FirstChild), nameof(LastChild))]
    public bool IsLeaf => this.FirstChild == null;

    public bool IsCompositeLeaf => (this.ShadowRoot ?? this.FirstChild) == null;

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
    public bool IsSlotted                  => this.nodeFlags.HasFlags(NodeFlags.Slotted);
    public bool IsUpdatesSuspended         => this.nodeFlags.HasFlags(NodeFlags.UpdatesSuspended);

    public abstract string NodeName { get; }

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

    protected static void ReplaceSlot<T>(T slot, ref T? field, T? value) where T : Node
    {
        if (field != value)
        {
            ReplaceSlot(slot, field, value);

            field = value;
        }
    }

    protected static void ReplaceSlot(Node slot, Node? previous, Node? next)
    {
        if (next?.IsSlotted == true)
        {
            throw new InvalidOperationException($"Node {next} already slloted");
        }

        var current = previous ?? slot;

        if (current.Parent is not Node parent)
        {
            return;
        }

        var parentIsSealed = parent.IsSealed;

        if (parentIsSealed)
        {
            parent.Unseal();
        }

        if (next == null)
        {
            if (previous != null)
            {
                previous.ReplaceSelf(slot);
                previous.ClearSlotted();
            }
        }
        else
        {
            next.SetSlotted();

            current.ReplaceSelf(next);

            previous?.ClearSlotted();
        }

        if (parentIsSealed)
        {
            parent.Seal();
        }
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
        var leftParent  = left.CompositeParent;
        var rightParent = right.CompositeParent;

        if (leftParent == rightParent)
        {
            return left.Parent;
        }
        else if (left == rightParent)
        {
            return left;
        }
        else if (left.Parent == right)
        {
            return right;
        }
        else
        {
            var leftDepth  = left?.CompositeDepth;
            var rightDepth = right?.CompositeDepth;

            var currentLeft  = left;
            var currentRight = right;

            while (leftDepth > rightDepth)
            {
                currentLeft = currentLeft?.CompositeParent;
                leftDepth--;
            }

            while (leftDepth < rightDepth)
            {
                currentRight = currentRight?.CompositeParent;
                rightDepth--;
            }

            while (currentLeft != currentRight)
            {
                currentLeft  = currentLeft?.CompositeParent;
                currentRight = currentRight?.CompositeParent;
            }

            return currentLeft;
        }
    }

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

    private void AppendOrPrepend(ReadOnlySpan<Node> nodes, bool append)
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

    private void InsertAfterOrBefore(Node reference, ReadOnlySpan<Node> nodes, bool after)
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

        var enumerator = this.GetCompositeTraversalEnumerator();

        while (enumerator.MoveNext())
        {
            apply(enumerator.Current);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void apply(Node node)
        {
            var compositeParent = node.CompositeParent;
            var parent          = node.Parent;

            node.CompositeDepth = (ushort)((compositeParent?.CompositeDepth ?? -1) + 1);
            node.Depth          = (ushort)((parent?.Depth ?? -1) + 1);
            node.IsConnected    = true;
            node.Scene          = node as Scene ?? compositeParent?.Scene;

            InvokeConnectedCallbacks(node);
        }
    }

    private void PropagateOnDisconnecting()
    {
        apply(this);

        var enumerator = this.GetCompositeTraversalEnumerator();

        while (enumerator.MoveNext())
        {
            apply(enumerator.Current);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void apply(Node node)
        {
            InvokeDisconnectingCallbacks(node);

            node.CompositeDepth = default;
            node.Depth          = default;
            node.hasStarted     = default;
            node.IsConnected    = default;
            node.Scene          = default;
        }
    }

    private protected virtual void OnAttachedInternal() { }
    private protected virtual void OnChildAttachedInternal(Node child) { }
    private protected virtual void OnChildDetachingInternal(Node child) { }
    private protected virtual void OnConnectedInternal() { }
    private protected virtual void OnDetachingInternal() { }
    private protected virtual void OnDisconnectingInternal() { }
    private protected virtual void OnDisposedInternal() { }

    protected sealed override void OnDisposed(bool disposing)
    {
        if (disposing)
        {
            InvokeDisposedCallbacks(this);

            foreach (var child in this.GetCompositeTraversalEnumerator())
            {
                child.Dispose();
            }
        }
    }

    [MemberNotNull(nameof(ShadowRoot))]
    protected void AttachShadowRoot(Node shadowRoot)
    {
        if (this.ShadowRoot != null)
        {
            throw new InvalidOperationException("A shadow root is already attached to this node.");
        }

        if (shadowRoot.ShadowHost != null)
        {
            throw new InvalidOperationException($"The specified shadow root is already attached to another host ('{shadowRoot.ShadowHost}').");
        }

        this.ShadowRoot = shadowRoot;
        shadowRoot.ShadowHost = this;

        this.OnChildAttachedInternal(shadowRoot);
        this.OnChildAttached(shadowRoot);

        shadowRoot.OnAttachedInternal();
        shadowRoot.OnAttached();

        if (this.IsConnected)
        {
            shadowRoot.Connect();
        }
    }

    protected void DetachShadowRoot()
    {
        if (this.ShadowRoot == null)
        {
            throw new InvalidOperationException();
        }

        this.OnChildDetachingInternal(this.ShadowRoot);
        this.OnChildDetaching(this.ShadowRoot);

        this.ShadowRoot.OnDetachingInternal();
        this.ShadowRoot.OnDetaching();

        if (this.IsConnected)
        {
            this.ShadowRoot.Disconnect();
        }

        this.ShadowHost = null;
    }

    protected void SetSlotted() =>
        this.nodeFlags |= NodeFlags.Slotted;

    protected void ClearSlotted() =>
        this.nodeFlags &= ~NodeFlags.Slotted;

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
    protected virtual void OnChildAttached(Node child) { }
    protected virtual void OnChildDetaching(Node child) { }
    protected virtual void OnConnected() { }
    protected virtual void OnDetaching() { }
    protected virtual void OnDisconnecting() { }
    protected virtual void OnDisposed() { }
    protected virtual void OnStart() { }

    internal static ComposedPath GetCompositePathBetween(Node left, Node right)
    {
        var leftToAncestor  = new List<Node>();
        var rightToAncestor = new List<Node>();

        GetCompositePathBetween(leftToAncestor, rightToAncestor, left, right);

        return new(leftToAncestor, rightToAncestor);
    }

    internal static void GetCompositePathBetween(List<Node> leftToAncestor, List<Node> rightToAncestor, Node left, Node right)
    {
        const string ERROR_MESSAGE = "The specified elements do not share a common ancestor in the composed tree.";

        leftToAncestor.Clear();
        rightToAncestor.Clear();

        leftToAncestor.Add(left);
        rightToAncestor.Add(right);

        var leftCompositeParent  = left.CompositeParent;
        var rightCompositeParent = right.CompositeParent;

        if (leftCompositeParent == rightCompositeParent)
        {
            if (leftCompositeParent == null)
            {
                throw new InvalidOperationException(ERROR_MESSAGE);
            }

            leftToAncestor.Add(leftCompositeParent);
            rightToAncestor.Add(leftCompositeParent);
        }
        else if (left == rightCompositeParent)
        {
            rightToAncestor.Add(left);
        }
        else if (leftCompositeParent == right)
        {
            leftToAncestor.Add(right);
        }
        else
        {
            var currentLeft  = left;
            var currentRight = right;

            var leftDepth  = left.CompositeDepth;
            var rightDepth = right.CompositeDepth;

            while (leftDepth > rightDepth)
            {
                currentLeft = currentLeft.CompositeParent!;
                leftDepth--;

                leftToAncestor.Add(currentLeft);
            }

            while (leftDepth < rightDepth)
            {
                currentRight = currentRight.CompositeParent!;
                rightDepth--;

                rightToAncestor.Add(currentRight);
            }

            while (currentLeft != currentRight)
            {
                currentLeft  = currentLeft.CompositeParent;
                currentRight = currentRight.CompositeParent;

                if (currentLeft == null || currentRight == null)
                {
                    leftToAncestor.Clear();
                    rightToAncestor.Clear();

                    throw new InvalidOperationException(ERROR_MESSAGE);
                }

                leftToAncestor.Add(currentLeft);
                rightToAncestor.Add(currentRight);
            }
        }
    }

    internal void Connect()
    {
        if (this.IsConnected)
        {
            throw new InvalidOperationException("Subtree already connected.");
        }

        if (this.Parent != null)
        {
            throw new InvalidOperationException("Only subtree root can call Connect.");
        }

        this.PropagateOnConnected();
    }

    internal void Disconnect()
    {
        if (!this.IsConnected)
        {
            throw new InvalidOperationException("Subtree already disconnected.");
        }

        if (this.Parent != null)
        {
            throw new InvalidOperationException("Only subtree root can call Disconnect.");
        }

        this.PropagateOnDisconnecting();
    }

    internal CompositeEnumerator GetCompositeEnumerator() =>
        new(this);

    internal CompositeTraversalEnumerator GetCompositeTraversalEnumerator() =>
        new(this);

    internal void InvokeStart()
    {
        if (!this.hasStarted)
        {
            this.OnStart();

            this.hasStarted = true;
        }
    }

    public void AppendChild(Node node) =>
        this.AppendOrPrepend(node, true);

    public void AppendChildren(ReadOnlySpan<Node> nodes) =>
        this.AppendOrPrepend(nodes, true);

    public UnsealedScope CreateUnsealedScope() =>
        new(this);

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

        var leftParent  = left.CompositeParent!;
        var rightParent = right.CompositeParent!;

        if (leftParent != rightParent)
        {
            var leftDepth  = left.CompositeDepth;
            var rightDepth = right.CompositeDepth;

            while (leftDepth > rightDepth)
            {
                leftParent = left.CompositeParent!;

                if (leftParent == right)
                {
                    return 1;
                }

                left = leftParent;
                leftDepth--;
            }

            while (leftDepth < rightDepth)
            {
                rightParent = right.CompositeParent!;

                if (rightParent == left)
                {
                    return -1;
                }

                right = rightParent;
                rightDepth--;
            }

            while ((leftParent = left.CompositeParent!) != (rightParent = right.CompositeParent!))
            {
                left  = leftParent;
                right = rightParent;
            }
        }

        if (leftParent == rightParent)
        {
            if (leftParent == null)
            {
                throw new InvalidOperationException("Can't compare an root node to another");
            }

            if (left == right.NextSibling || right.ShadowHost != null)
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

    public Enumerator GetEnumerator() =>
        new(this);

    public TraversalEnumerator GetTraversalEnumerator() =>
        new(this);

    public void InsertAfter(Node reference, Node node) =>
        this.InsertAfterOrBefore(reference, node, true);

    public void InsertAfterSelf(Node node) =>
        this.Parent?.InsertAfter(this, node);

    public void InsertBefore(Node reference, Node node) =>
        this.InsertAfterOrBefore(reference, node, false);

    public void InsertBeforeSelf(Node node) =>
        this.Parent?.InsertBefore(node, this);

    public void InsertNodesAfterSelf(ReadOnlySpan<Node> nodes) =>
        this.Parent?.InsertNodesAfter(this, nodes);

    public void InsertNodesAfter(Node reference, ReadOnlySpan<Node> nodes) =>
        this.InsertAfterOrBefore(reference, nodes, true);

    public void InsertNodesBefore(ReadOnlySpan<Node> nodes, Node reference) =>
        this.InsertAfterOrBefore(reference, nodes, false);

    public void InsertNodesBeforeSelf(ReadOnlySpan<Node> nodes) =>
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

    public bool IsCompositeAncestor(Node other)
    {
        if (this == other)
        {
            return false;
        }

        var parent = other.CompositeParent;

        while (parent != this)
        {
            if (parent == null)
            {
                return false;
            }

            parent = parent.CompositeParent;
        }

        return true;
    }

    public bool IsCompositeDescendent(Node other) =>
        other.IsCompositeAncestor(this);

    public bool IsDescendent(Node other) =>
        other.IsAncestor(this);

    public void PrependChild(Node node) =>
        this.AppendOrPrepend(node, false);

    public void PrependChildren(ReadOnlySpan<Node> nodes) =>
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
        var next     = target.NextSibling;

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
        node.NextSibling     = next;

        SetParenting(this, node);
    }

    public void ReplaceSelf(Node node) =>
        this.Parent?.Replace(this, node);

    public void ReplaceChildren(Node node)
    {
        this.DetachChildren();
        this.AppendChild(node);
    }

    public void ReplaceChildren(ReadOnlySpan<Node> nodes)
    {
        if (this.FirstChild != null)
        {
            this.DetachChildren();
        }

        this.AppendChildren(nodes);
    }

    public void ReplaceWith(Node target, ReadOnlySpan<Node> nodes)
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

    public void ReplaceSelfWith(ReadOnlySpan<Node> nodes) =>
        this.Parent?.ReplaceWith(this, nodes);

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
