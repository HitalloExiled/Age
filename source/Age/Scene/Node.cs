using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Age.Scene;

public abstract partial class Node : IEnumerable<Node>, IComparable<Node>
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
                this.IndexChanged();
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
                        node.Index = -1;

                        node.Disconnected(oldTree);
                    }
                }

                setTree(this, value);

                foreach (var child in this.Traverse())
                {
                    setTree(child, value);
                }
            }
        }
    }

    [MemberNotNullWhen(true, nameof(Tree))]
    public bool IsConnected => this.Tree != null;

    IEnumerator IEnumerable.GetEnumerator() =>
        this.GetEnumerator();

    protected virtual void Adopted()
    { }

    protected virtual void ChildAppended(Node child)
    { }

    protected virtual void ChildRemoved(Node child)
    { }

    protected virtual void Connected(NodeTree tree)
    { }

    protected virtual void Destroyed()
    { }

    protected virtual void Disconnected(NodeTree tree)
    { }

    protected virtual void IndexChanged() { }

    public void AppendChild(Node child)
    {
        if (child == this)
        {
            throw new InvalidOperationException("Cant add node to itself");
        }

        if (child.Parent != this)
        {
            child.Parent = this;

            if (this.LastChild != null)
            {
                this.LastChild.NextSibling = child;
                child.PreviousSibling = this.LastChild;

                this.LastChild = child;
            }
            else
            {
                this.FirstChild = this.LastChild = child;
            }

            child.Tree = this.Tree;

            child.Adopted();
            this.ChildAppended(child);
        }
    }

    public void AppendChildren(IEnumerable<Node> children)
    {
        foreach (var child in children)
        {
            this.AppendChild(child);
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
                for (var node = this.PreviousSibling; node != this.FirstChild; node = node?.PreviousSibling)
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

    public void Destroy()
    {
        this.Destroyed();

        foreach (var child in this.Traverse())
        {
            child.Destroyed();
        }
    }

    public IEnumerator<Node> GetEnumerator() =>
        new Enumerator(this);

    public void RemoveChildren()
    {
        if (this.FirstChild != null)
        {
            var next = this.FirstChild;

            do
            {
                var current = next;

                next = current.NextSibling;

                current.PreviousSibling = null;
                current.NextSibling     = null;
                current.Parent          = null;
                current.Tree            = null;

                this.ChildRemoved(current);
            }
            while (next != null);

            this.FirstChild = null;
            this.LastChild  = null;
        }
    }

    public void RemoveChild(Node child)
    {
        if (child.Parent == this)
        {
            if (child == this.FirstChild)
            {
                this.FirstChild = child.NextSibling;
            }

            if (child == this.LastChild)
            {
                this.LastChild = child.PreviousSibling;
            }

            if (child.PreviousSibling != null)
            {
                child.PreviousSibling.NextSibling = child.NextSibling;

                if (child.NextSibling != null)
                {
                    child.NextSibling.PreviousSibling = child.PreviousSibling.NextSibling;
                }
            }
            else if (child.NextSibling != null)
            {
                child.NextSibling.PreviousSibling = null;
            }

            child.PreviousSibling = null;
            child.NextSibling     = null;
            child.Parent          = null;
            child.Tree            = null;

            this.ChildRemoved(child);
        }
    }

    public void RemoveChildrenInRange(Node start, Node end)
    {
        if (start.Parent != this || end.Parent != this)
        {
            return;
        }

        var next = start;

        do
        {
            var current = next;

            next = current.NextSibling;

            current.PreviousSibling = null;
            current.NextSibling     = null;
            current.Parent          = null;
            current.Tree            = null;

            this.ChildRemoved(current);

            if (current == end)
            {
                break;
            }
        }
        while (next != null);

        this.FirstChild = null;
        this.LastChild  = null;
    }

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


}
