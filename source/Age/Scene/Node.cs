using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Age.Core;

namespace Age.Scene;

public abstract partial class Node : Disposable, IEnumerable<Node>, IComparable<Node>
{
    #region 8-bytes
    private NodeTree? tree;

    public Node? FirstChild      { get; private set; }
    public Node? LastChild       { get; private set; }
    public Node? NextSibling     { get; private set; }
    public Node? Parent          { get; private set; }
    public Node? PreviousSibling { get; private set; }

    public string? Name { get; set; }
    #endregion

    #region 4-bytes
    internal int Index
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                this.Indexed();
            }
        }
    }

    public NodeFlags Flags { get; protected set; }
    #endregion

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
                        node.Connected(tree);
                    }
                    else if (oldTree != null)
                    {
                        node.Disconnected(oldTree);
                    }
                }

                setTree(this, value);

                foreach (var node in this.Traverse())
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

    public bool HasChildNodes => this.FirstChild != null;

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

    private void AppendOrPrepend(Node node, bool append)
    {
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
        node.Adopted(this);

        this.ChildAppended(node);
    }

    private void AppendOrPrepend(Span<Node> nodes, bool append)
    {
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

            node.Adopted(this);
            this.ChildAppended(node);
        }
    }

    private void InsertAfterOrBefore(Node reference, Node node, bool after)
    {
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

        node.Adopted(this);
        this.ChildAppended(node);
    }

    private void InsertAfterOrBefore(Node reference, Span<Node> nodes, bool after)
    {
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
            node.Adopted(this);

            this.ChildAppended(node);
        }
    }

    private void RemoveChildren(bool dispose = false)
    {
        while (this.LastChild != null)
        {
            var current = this.LastChild;

            if (dispose)
            {
                current.Dispose();
            }

            this.LastChild = current.PreviousSibling;

            if (this.LastChild != null)
            {
                this.LastChild.NextSibling = null;
            }

            current.PreviousSibling = null;
            current.NextSibling     = null;
            current.Parent          = null;
            current.Tree            = null;

            current.Removed(this);
            this.ChildRemoved(current);
        }

        this.FirstChild = null;
    }

    private void RemoveChildrenInRange(Node start, Node end, bool dispose)
    {
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

            if (dispose)
            {
                current.Dispose();
            }

            next = current.NextSibling;

            current.PreviousSibling = null;
            current.NextSibling     = null;
            current.Parent          = null;
            current.Tree            = null;

            current.Removed(this);
            this.ChildRemoved(current);

            if (current == end)
            {
                break;
            }
        }
    }

    protected override void Disposed(bool disposing)
    {
        if (disposing)
        {
            this.Disposed();

            foreach (var child in this.Traverse())
            {
                child.Disposed();
            }
        }
    }

    protected virtual void Adopted(Node parent)
    { }

    protected virtual void Removed(Node parent)
    { }

    protected virtual void ChildAppended(Node child)
    { }

    protected virtual void ChildRemoved(Node child)
    { }

    protected virtual void Connected(NodeTree tree)
    { }

    protected virtual void Disposed()
    { }

    protected virtual void Disconnected(NodeTree tree)
    { }

    protected virtual void Indexed()
    { }

    public void AppendChild(Node node) =>
        this.AppendOrPrepend(node, true);

    public void AppendChildren(Span<Node> nodes) =>
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
        else if (this.Parent == other.Parent)
        {
            if (this == other.PreviousSibling)
            {
                return -1;
            }
            else if (this == other.NextSibling)
            {
                return 1;
            }
            else
            {
                for (var node = this.PreviousSibling; node != null; node = node?.PreviousSibling)
                {
                    if (node == other)
                    {
                        return 1;
                    }
                }

                return -1;
            }
        }
        else if (other.Parent == this || this.Parent == null && other.Parent != null)
        {
            return -1;
        }
        else if (this.Parent == other || this.Parent != null && other.Parent == null)
        {
            return 1;
        }
        else if (this.Parent != null && other.Parent != null)
        {
            return this.Parent.CompareTo(other.Parent);
        }

        return 0;
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

    public IEnumerator<Node> GetEnumerator() =>
        new Enumerator(this);

    public TraverseEnumerator GetTraverseEnumerator() =>
        new(this);

    public void InsertAfter(Node reference, Node node) =>
        this.InsertAfterOrBefore(reference, node, true);

    public void InsertAfterSelf(Node node) =>
        this.Parent?.InsertAfter(this, node);

    public void InsertBefore(Node node, Node reference) =>
        this.InsertAfterOrBefore(reference, node, false);

    public void InsertBeforeSelf(Node node) =>
        this.Parent?.InsertBefore(node, this);

    public void InsertNodesAfterSelf(Span<Node> nodes) =>
        this.Parent?.InsertNodesAfter(this, nodes);

    public void InsertNodesAfter(Node reference, Span<Node> nodes) =>
        this.InsertAfterOrBefore(reference, nodes, true);

    public void InsertNodesBefore(Span<Node> nodes, Node reference) =>
        this.InsertAfterOrBefore(reference, nodes, false);

    public void InsertNodesBeforeSelf(Span<Node> nodes) =>
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

    public void PrependChildren(Span<Node> nodes) =>
        this.AppendOrPrepend(nodes, false);

    public void RemoveChild(Node node)
    {
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

        node.Removed(this);
        this.ChildRemoved(node);
    }

    public void RemoveChildren() =>
        this.RemoveChildren(false);

    public void RemoveChildrenInRange(Node start, Node end) =>
        this.RemoveChildrenInRange(start, end, false);

    public void Replace(Node target, Node node)
    {
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

        target.Removed(this);
        this.ChildRemoved(target);

        node.Adopted(this);
        this.ChildAppended(node);
    }

    public void ReplaceSelf(Node node) =>
        this.Parent?.Replace(this, node);

    public void ReplaceChildren(Node node)
    {
        this.RemoveChildren();
        this.AppendChild(node);
    }

    public void ReplaceChildren(Span<Node> nodes)
    {
        if (this.FirstChild != null)
        {
            this.RemoveChildren();
        }

        this.AppendChildren(nodes);
    }

    public void ReplaceWith(Node target, Span<Node> nodes)
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

        target.Removed(this);
        this.ChildRemoved(target);

        foreach (var node in nodes)
        {
            node.Parent = this;
            node.Tree   = this.Tree;
            node.Adopted(this);

            this.ChildAppended(node);
        }
    }

    public void ReplaceSelfWith(Span<Node> nodes) =>
        this.Parent?.ReplaceWith(this, nodes);

    public IEnumerable<Node> Reverse() =>
        new ReverseEnumerator(this);

    public IEnumerable<Node> Traverse() =>
        new TraverseEnumerator(this);

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
