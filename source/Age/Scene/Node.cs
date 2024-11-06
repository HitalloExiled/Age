using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Age.Core;

namespace Age.Scene;

public abstract partial class Node : Disposable, IEnumerable<Node>, IComparable<Node>
{
    private NodeTree? tree;

    private int index;

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

    public Node? FirstChild      { get; private set; }
    public Node? LastChild       { get; private set; }
    public Node? NextSibling     { get; private set; }
    public Node? Parent          { get; private set; }
    public Node? PreviousSibling { get; private set; }

    public Node[] Children => [..this];

    public abstract string NodeName { get; }

    public NodeFlags Flags { get; protected set; }

    public string? Name { get; set; }

    public bool Visible { get; set; } = true;

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

    [MemberNotNullWhen(true, nameof(Tree))]
    public bool IsConnected => this.Tree != null;

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
        if (node == this)
        {
            throw new InvalidOperationException("Cant add node to itself");
        }

        if (reference.Parent != this)
        {
            throw new InvalidOperationException("Bla bla bla");
        }

        var hasSameParent = node.Parent == this;

        if (!hasSameParent)
        {
            node.Detach();

            node.Parent = this;
        }

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

        if (!hasSameParent)
        {
            node.Tree = this.Tree;

            node.Adopted();
            this.ChildAppended(node);
        }
    }

    private void Insert(Node reference, Span<Node> nodes, bool before)
    {
        if (reference.Parent != this)
        {
            throw new InvalidOperationException("Bla bla bla");
        }

        for (var i = 0; i < nodes.Length ; i++)
        {
            var node = nodes[i];

            if (node == this)
            {
                throw new InvalidOperationException("Cant add node to itself");
            }

            var hasSameParent = node.Parent == this;

            if (!hasSameParent)
            {
                node.Detach();

                node.Parent = this;
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

            if (!hasSameParent)
            {
                node.Tree = this.Tree;

                node.Adopted();
                this.ChildAppended(node);
            }
        }
    }

    private void RemoveChildren(bool dispose = false)
    {
        var next = this.FirstChild;

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
        }

        this.FirstChild = null;
        this.LastChild  = null;
    }

    private void RemoveChildren(Node start, Node end, bool dispose)
    {
        if (start.Parent != this || end.Parent != this)
        {
            return;
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

    protected virtual void Disposed() { }
    protected virtual void Disconnected(NodeTree tree)
    { }

    protected virtual void Indexed() { }

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
        for (var i = 0; i < nodes.Length; i++)
        {
            var node = nodes[i];

            if (node == this)
            {
                throw new InvalidOperationException("Cant add node to itself");
            }

            if (node.Parent != this)
            {
                node.Detach();

                node.Parent = this;

                if (i == 0)
                {
                    if (this.LastChild == null)
                    {
                        this.FirstChild = node;
                    }
                    else
                    {
                        this.LastChild.NextSibling = node;
                    }
                }
                else
                {
                    nodes[i - 1].NextSibling = node;
                    node.PreviousSibling = nodes[i - 1];
                }

                if (i == nodes.Length - 1)
                {
                    this.LastChild = node;
                }

                node.Tree = this.Tree;

                node.Adopted();
                this.ChildAppended(node);
            }
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

    public void Replace(Node oldNode, Node newNode)
    {
        if (newNode == this)
        {
            throw new InvalidOperationException("Cant add node to itself");
        }

        if (oldNode.Parent != this)
        {
            throw new InvalidOperationException("Bla bla bla");
        }

        newNode.Detach();

        if (oldNode.PreviousSibling == null)
        {
            this.FirstChild = newNode;
        }
        else
        {
            oldNode.PreviousSibling.NextSibling = newNode;
        }

        if (oldNode.NextSibling == null)
        {
            this.LastChild = newNode;
        }
        else
        {
            oldNode.NextSibling.PreviousSibling = newNode;
        }

        newNode.Parent          = this;
        newNode.PreviousSibling = oldNode.PreviousSibling;
        newNode.NextSibling     = oldNode.NextSibling;
        newNode.Tree            = this.Tree;

        oldNode.Parent          = null;
        oldNode.PreviousSibling = null;
        oldNode.NextSibling     = null;
        oldNode.Tree            = null;

        oldNode.Removed();
        this.ChildRemoved(oldNode);

        newNode.Adopted();
        this.ChildAppended(newNode);
    }

    public void RemoveChild(Node node)
    {
        if (node.Parent == this)
        {
            if (node == this.FirstChild)
            {
                this.FirstChild = node.NextSibling;
            }

            if (node == this.LastChild)
            {
                this.LastChild = node.PreviousSibling;
            }

            if (node.PreviousSibling != null)
            {
                node.PreviousSibling.NextSibling = node.NextSibling;

                if (node.NextSibling != null)
                {
                    node.NextSibling.PreviousSibling = node.PreviousSibling.NextSibling;
                }
            }
            else if (node.NextSibling != null)
            {
                node.NextSibling.PreviousSibling = null;
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

    public virtual void Initialize()
    { }

    public virtual void LateUpdate()
    { }

    public virtual void Update()
    { }

    public bool IsDescendent(Node other)
    {
        var parent = other;

        while (parent != this.Parent)
        {
            parent = parent?.Parent;
        }

        return this.Parent == parent;
    }

    public static bool operator >(Node left, Node right) => left.CompareTo(right) == 1;
    public static bool operator <(Node left, Node right) => left.CompareTo(right) == -1;
}
