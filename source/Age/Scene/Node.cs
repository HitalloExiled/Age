using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Age.Core;
using Age.Core.Extensions;

namespace Age.Scene;

public abstract partial class Node : Disposable, IEnumerable<Node>, IComparable<Node>
{
    #pragma warning disable IDE0032 // Use auto property
    private NodeTree? tree;
    #pragma warning restore IDE0032 // Use auto property

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

    public NodeFlags NodeFlags { get; protected set; }

    public NodeTree? Tree
    {
        get => this.tree;
        internal set
        {
            if (value != this.tree)
            {
                static void setTree(Node node, NodeTree? tree)
                {
                    var oldTree = node.tree;

                    node.tree = tree;

                    if (tree != null)
                    {
                        node.OnConnected(tree);
                    }
                    else if (oldTree != null)
                    {
                        node.OnDisconnected(oldTree);
                    }
                }

                setTree(this, value);

                foreach (var node in this.GetTraversalEnumerator())
                {
                    setTree(node, value);
                }
            }
        }
    }

    public Node[] Children
    {
        get => [.. this];
        set => this.ReplaceChildren(value);
    }

    [MemberNotNullWhen(true, nameof(Tree))]
    public bool IsConnected => this.Tree != null;

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

    public virtual string? Name { get; set; }

    public abstract string NodeName { get; }

    internal static Node[] SelectBetween(Node start, Node end)
    {
        if (start == end)
        {
            return [];
        }

        var ancestor = GetLowestCommonAncestor(start, end);

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

    public static Node? GetLowestCommonAncestor(Node left, Node right)
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

            while (currentLeft != null || currentRight != null)
            {
                if (currentLeft != null)
                {
                    leftDepth++;
                    currentLeft  = currentLeft.Parent;
                }

                if (currentRight != null)
                {
                    rightDepth++;
                    currentRight  = currentRight.Parent;
                }
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
        if (this.NodeFlags.HasFlags(NodeFlags.Immutable))
        {
            return;
        }

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
        else
        {
            if (this.FirstChild != null)
            {
                this.FirstChild.PreviousSibling = node;
                node.NextSibling = this.FirstChild;

                this.FirstChild = node;
            }
            else
            {
                this.FirstChild = this.LastChild = node;
            }
        }

        node.Parent = this;
        node.Tree   = this.Tree;
        node.OnAdopted(this);

        this.OnChildAppended(node);
    }

    private void AppendOrPrepend(scoped ReadOnlySpan<Node> nodes, bool append)
    {
        if (this.NodeFlags.HasFlags(NodeFlags.Immutable))
        {
            return;
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
            node.Parent = this;
            node.Tree = this.Tree;

            node.OnAdopted(this);
            this.OnChildAppended(node);
        }
    }

    private void InsertAfterOrBefore(Node reference, Node node, bool after)
    {
        if (this.NodeFlags.HasFlags(NodeFlags.Immutable))
        {
            return;
        }

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
            node.NextSibling     = reference.NextSibling;

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
            node.NextSibling     = reference;

            reference.PreviousSibling = node;
        }

        node.Parent = this;
        node.Tree   = this.Tree;

        node.OnAdopted(this);
        this.OnChildAppended(node);
    }

    private void InsertAfterOrBefore(Node reference, scoped ReadOnlySpan<Node> nodes, bool after)
    {
        if (this.NodeFlags.HasFlags(NodeFlags.Immutable))
        {
            return;
        }

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
            node.Parent = this;
            node.Tree   = this.Tree;
            node.OnAdopted(this);

            this.OnChildAppended(node);
        }
    }

    private void RemoveChildren(bool dispose = false)
    {
        if (this.NodeFlags.HasFlags(NodeFlags.Immutable))
        {
            return;
        }

        while (this.LastChild != null)
        {
            var current = this.LastChild;

            this.LastChild = current.PreviousSibling;

            this.LastChild?.NextSibling = null;

            current.PreviousSibling = null;
            current.NextSibling     = null;
            current.Parent          = null;
            current.Tree            = null;

            current.OnRemoved(this);
            this.OnChildRemoved(current);

            if (dispose)
            {
                current.Dispose();
            }
        }

        this.FirstChild = null;
    }

    private void RemoveChildrenInRange(Node start, Node end, bool dispose)
    {
        if (this.NodeFlags.HasFlags(NodeFlags.Immutable))
        {
            return;
        }

        if (start.Parent != this || end.Parent != this)
        {
            throw new InvalidOperationException("Start and end must be child of this node");
        }

        if (start > end)
        {
            (start, end) = (end, start);
        }

        if (start.PreviousSibling == null)
        {
            this.FirstChild = end.NextSibling;
        }
        else
        {
            start.PreviousSibling.NextSibling = end.NextSibling;
        }

        if (end.NextSibling == null)
        {
            this.LastChild = start.PreviousSibling;
        }
        else
        {
            end.NextSibling.PreviousSibling = start.PreviousSibling;
        }

        var next = start;

        while (next != null)
        {
            var current = next;

            next = current.NextSibling;

            current.PreviousSibling = null;
            current.NextSibling     = null;
            current.Parent          = null;
            current.Tree            = null;

            current.OnRemoved(this);
            this.OnChildRemoved(current);

            if (dispose)
            {
                current.Dispose();
            }

            if (current == end)
            {
                break;
            }
        }
    }

    protected override void OnDisposed(bool disposing)
    {
        if (disposing)
        {
            this.OnDisposed();

            foreach (var child in this.GetTraversalEnumerator())
            {
                child.OnDisposed();
            }
        }
    }

    protected virtual void OnAdopted(Node parent) { }
    protected virtual void OnChildAppended(Node child) { }
    protected virtual void OnChildRemoved(Node child) { }
    protected virtual void OnConnected(NodeTree tree) { }
    protected virtual void OnDisconnected(NodeTree tree) { }
    protected virtual void OnDisposed() { }
    protected virtual void OnIndexed() { }
    protected virtual void OnRemoved(Node parent) { }

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
        this.Parent?.RemoveChild(this);

    public void DisposeChildren() =>
        this.RemoveChildren(true);

    public void DisposeChildrenInRange(Node start, Node end) =>
        this.RemoveChildrenInRange(start, end, true);

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

    public bool IsDescendent(Node other)
    {
        var parent = other;

        while (parent != this.Parent)
        {
            parent = parent?.Parent;
        }

        return this.Parent == parent;
    }

    public void PrependChild(Node node) =>
        this.AppendOrPrepend(node, false);

    public void PrependChildren(scoped ReadOnlySpan<Node> nodes) =>
        this.AppendOrPrepend(nodes, false);

    public void RemoveChild(Node node)
    {
        if (this.NodeFlags.HasFlags(NodeFlags.Immutable))
        {
            return;
        }

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

        node.PreviousSibling = null;
        node.NextSibling     = null;
        node.Parent          = null;
        node.Tree            = null;

        node.OnRemoved(this);
        this.OnChildRemoved(node);
    }

    public void RemoveChildren() =>
        this.RemoveChildren(false);

    public void RemoveChildrenInRange(Node start, Node end) =>
        this.RemoveChildrenInRange(start, end, false);

    public void Replace(Node target, Node node)
    {
        if (this.NodeFlags.HasFlags(NodeFlags.Immutable))
        {
            return;
        }

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

        node.Detach();

        if (target.PreviousSibling == null)
        {
            this.FirstChild = node;
        }
        else
        {
            target.PreviousSibling.NextSibling = node;
        }

        if (target.NextSibling == null)
        {
            this.LastChild = node;
        }
        else
        {
            target.NextSibling.PreviousSibling = node;
        }

        node.Parent          = this;
        node.PreviousSibling = target.PreviousSibling;
        node.NextSibling     = target.NextSibling;
        node.Tree            = this.Tree;

        target.Parent          = null;
        target.PreviousSibling = null;
        target.NextSibling     = null;
        target.Tree            = null;

        target.OnRemoved(this);
        this.OnChildRemoved(target);

        node.OnAdopted(this);
        this.OnChildAppended(node);
    }

    public void ReplaceSelf(Node node) =>
        this.Parent?.Replace(this, node);

    public void ReplaceChildren(Node node)
    {
        this.RemoveChildren();
        this.AppendChild(node);
    }

    public void ReplaceChildren(scoped ReadOnlySpan<Node> nodes)
    {
        if (this.FirstChild != null)
        {
            this.RemoveChildren();
        }

        this.AppendChildren(nodes);
    }

    public void ReplaceWith(Node target, scoped ReadOnlySpan<Node> nodes)
    {
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

        var first = nodes[0];
        var last  = nodes[^1];

        if (target.PreviousSibling == null)
        {
            this.FirstChild = first;
        }
        else
        {
            target.PreviousSibling.NextSibling = first;
        }

        first.PreviousSibling = target.PreviousSibling;

        if (nodes.Length > 1)
        {
            first.NextSibling = nodes[1];
            last.PreviousSibling = nodes[^2];
        }

        last.NextSibling = target.NextSibling;

        if (target.NextSibling == null)
        {
            this.LastChild = last;
        }
        else
        {
            target.NextSibling.PreviousSibling = last;
        }

        for (var i = 1; i < nodes.Length - 1; i++)
        {
            nodes[i].PreviousSibling = nodes[i - 1];
            nodes[i].NextSibling     = nodes[i + 1];
        }

        target.Parent          = null;
        target.PreviousSibling = null;
        target.NextSibling     = null;
        target.Tree            = null;

        target.OnRemoved(this);
        this.OnChildRemoved(target);

        foreach (var node in nodes)
        {
            node.Parent = this;
            node.Tree   = this.Tree;
            node.OnAdopted(this);

            this.OnChildAppended(node);
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
