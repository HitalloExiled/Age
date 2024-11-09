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
    private int index;

    public NodeFlags Flags { get; protected set; }
    #endregion

    #region 1-byte
    public bool Visible { get; set; } = true;
    #endregion

    internal int Index
    {
        get => this.index;
        set
        {
            if (this.index != value)
            {
                this.index = value;
                this.Indexed();
            }
        }
    }

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

    public Node[] Children => [..this];

    [MemberNotNullWhen(true, nameof(Tree))]
    public bool IsConnected => this.Tree != null;

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

    private void Insert(Node reference, Node node, bool before)
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
            throw new InvalidOperationException($"Cant intset node {(before ? "before" : "after")} itself");
        }

        node.Detach();

        if (before)
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
        else
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

        node.Parent = this;
        node.Tree   = this.Tree;

        node.Adopted();
        this.ChildAppended(node);
    }

    private void Insert(Node reference, Span<Node> nodes, bool before)
    {
        if (reference.Parent != this)
        {
            throw new InvalidOperationException("Reference node is not child of this node");
        }

        foreach (var node in nodes)
        {
            node.Detach();
        }

        for (var i = 0; i < nodes.Length ; i++)
        {
            var node = nodes[i];

            if (node == this)
            {
                throw new InvalidOperationException("Cant add node to itself");
            }

            if (node == reference)
            {
                throw new InvalidOperationException($"Cant intset node {(before ? "before" : "after")} itself");
            }

            if (i == 0)
            {
                if (before)
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
                }
                else
                {
                    node.PreviousSibling = reference;
                }

                if (nodes.Length > 1)
                {
                    node.NextSibling = nodes[i + 1];
                }
            }
            else if (i < nodes.Length - 1)
            {
                node.PreviousSibling = nodes[i - 1];
                node.NextSibling     = nodes[i + 1];
            }

            if (i == nodes.Length - 1)
            {
                if (i > 0)
                {
                    node.PreviousSibling = nodes[i - 1];
                }

                if (before)
                {
                    node.NextSibling = reference;

                    reference.PreviousSibling = node;
                }
                else
                {
                    if (reference.NextSibling == null)
                    {
                        this.LastChild = node;
                    }
                    else
                    {
                        reference.NextSibling.PreviousSibling = node;
                    }

                    node.NextSibling = reference.NextSibling;

                    reference.NextSibling = nodes[0];
                }
            }

            node.Parent = this;
        }

        foreach (var node in nodes)
        {
            node.Tree = this.Tree;
            node.Adopted();

            this.ChildAppended(node);
        }
    }

    private void RemoveChildren(bool dispose = false)
    {
        while (this.FirstChild != null)
        {
            var current = this.FirstChild;

            if (dispose)
            {
                current.Dispose();
            }

            this.FirstChild = current.NextSibling;

            current.PreviousSibling = null;
            current.NextSibling     = null;
            current.Parent          = null;
            current.Tree            = null;

            current.Removed();
            this.ChildRemoved(current);
        }

        this.LastChild = null;
    }

    private void RemoveChildren(Node start, Node end, bool dispose)
    {
        if (start.Parent != this || end.Parent != this)
        {
            throw new Exception("Start and end must be child of this node");
        }

        if (start > end)
        {
            (start, end) = (end, start);
        }

        if (start.PreviousSibling != null)
        {
            start.PreviousSibling.NextSibling = end.NextSibling;

            if (end.NextSibling != null)
            {
                end.NextSibling.PreviousSibling = start.PreviousSibling.NextSibling;
            }
        }
        else if (end.NextSibling != null)
        {
            end.NextSibling.PreviousSibling = null;
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

            current.Removed();
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

    protected virtual void Adopted()
    { }

    protected virtual void Removed()
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

    public void AppendChild(Node node)
    {
        if (node == this)
        {
            throw new InvalidOperationException("Cant add node to itself");
        }

        if (node.Parent != this)
        {
            node.Detach();

            node.Parent = this;

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

            node.Tree = this.Tree;

            node.Adopted();
            this.ChildAppended(node);
        }
    }

    public void AppendChildren(Span<Node> nodes)
    {
        foreach (var node in nodes)
        {
            node.Detach();
        }

        for (var i = 0; i < nodes.Length; i++)
        {
            var node = nodes[i];

            if (node == this)
            {
                throw new InvalidOperationException("Cant add node to itself");
            }

            if (i == 0)
            {
                if (this.FirstChild == null)
                {
                    this.FirstChild = node;
                }
                else
                {
                    node.PreviousSibling = this.LastChild;
                }

                if (nodes.Length > 1)
                {
                    node.NextSibling = nodes[i + 1];
                }

            }
            else if (i < nodes.Length - 1)
            {
                node.PreviousSibling = nodes[i - 1];
                node.NextSibling     = nodes[i + 1];
            }

            if (i == nodes.Length - 1)
            {
                if (i > 0)
                {
                    node.PreviousSibling = nodes[i - 1];
                }

                this.LastChild = node;
            }
        }

        foreach (var node in nodes)
        {
            node.Parent = this;
            node.Tree   = this.Tree;

            node.Adopted();
            this.ChildAppended(node);
        }
    }

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

    public void DisposeChildren(Node start, Node end) =>
        this.RemoveChildren(start, end, true);

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
        this.Insert(reference, node, false);

    public void InsertAfter(Node reference, Span<Node> nodes) =>
        this.Insert(reference, nodes, false);

    public void InsertBefore(Node node, Node reference) =>
        this.Insert(reference, node, true);

    public void InsertBefore(Span<Node> nodes, Node reference) =>
        this.Insert(reference, nodes, true);

    public bool IsDescendent(Node other)
    {
        var parent = other;

        while (parent != this.Parent)
        {
            parent = parent?.Parent;
        }

        return this.Parent == parent;
    }

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

        target.Removed();
        this.ChildRemoved(target);

        node.Adopted();
        this.ChildAppended(node);
    }

    public void Replace(Node target, Span<Node> nodes)
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
        }

        for (var i = 1; i < nodes.Length - 1; i++)
        {
            nodes[i].PreviousSibling = nodes[i - 1];
            nodes[i].NextSibling     = nodes[i + 1];
        }

        if (nodes.Length > 1)
        {
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

        target.Parent          = null;
        target.PreviousSibling = null;
        target.NextSibling     = null;
        target.Tree            = null;

        target.Removed();
        this.ChildRemoved(target);

        foreach (var node in nodes)
        {
            node.Parent = this;
            node.Tree   = this.Tree;
            node.Adopted();

            this.ChildAppended(node);
        }
    }

    public void RemoveChild(Node node)
    {
        if (node.Parent == this)
        {
            if (node == this.FirstChild)
            {
                this.FirstChild = node.NextSibling;
            }

            if (node.PreviousSibling != null)
            {
                node.PreviousSibling.NextSibling = node.NextSibling;

                if (node.NextSibling != null)
                {
                    node.NextSibling.PreviousSibling = node.PreviousSibling;
                }
            }
            else if (node.NextSibling != null)
            {
                node.NextSibling.PreviousSibling = node.PreviousSibling;
            }

            if (node == this.LastChild)
            {
                this.LastChild = node.PreviousSibling;
            }

            node.PreviousSibling = null;
            node.NextSibling     = null;
            node.Parent          = null;
            node.Tree            = null;

            node.Removed();
            this.ChildRemoved(node);
        }
    }

    public void RemoveChildren() =>
        this.RemoveChildren(false);

    public void RemoveChildrenInRange(Node start, Node end) =>
        this.RemoveChildren(start, end, false);

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
