using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Age.Scene;

[DebuggerDisplay("NodeName: {NodeName}, Name: {Name}, IsConnected: {IsConnected}")]
public abstract partial class Node : IEnumerable<Node>
{
    private NodeTree? tree;

    internal int Index { get; set; }

    public Node? FirstChild      { get; private set; }
    public Node? LastChild       { get; private set; }
    public Node? NextSibling     { get; private set; }
    public Node? Parent          { get; private set; }
    public Node? PreviousSibling { get; private set; }

    public Node[] Children => [..this];

    public abstract string NodeName { get; }

    public string? Name { get; set; }

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
                        node.Index = tree.Nodes.Count;

                        tree.Nodes.Add(node);

                        node.OnConnected(tree);
                    }
                    else if (oldTree != null)
                    {
                        node.Index = -1;

                        node.OnDisconnected(oldTree);
                    }

                    if (node.Index > -1)
                    {
                        oldTree?.Nodes.RemoveAt(node.Index);
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

    protected virtual void OnAdopted()
    { }

    protected virtual void OnChildAppended(Node child)
    { }

    protected virtual void OnConnected(NodeTree tree)
    { }

    protected virtual void OnDisconnected(NodeTree tree)
    { }

    protected virtual void OnChildRemoved(Node child)
    { }

    protected virtual void OnDestroy()
    { }

    public void AppendChild(Node child)
    {
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

            child.OnAdopted();
            this.OnChildAppended(child);
        }
    }

    public void AppendChildren(IEnumerable<Node> children)
    {
        foreach (var child in children)
        {
            this.AppendChild(child);
        }
    }

    public void Destroy()
    {
        foreach (var child in this.Traverse())
        {
            child.OnDestroy();
        }

        this.OnDestroy();
    }

    public IEnumerable<T> Enumerate<T>() where T : Node
    {
        foreach (var node in this)
        {
            if (node is T t)
            {
                yield return t;
            }
        }
    }

    public IEnumerator<Node> GetEnumerator() =>
        new Enumerator(this);

    public virtual void Initialize()
    { }

    public virtual void LateUpdate()
    { }

    public void Remove() =>
        this.Parent?.RemoveChild(this);

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

                this.OnChildRemoved(current);
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

            child.tree  = null;

            foreach (var node in this.Traverse())
            {
                node.tree = null;
            }

            this.OnChildRemoved(child);
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
            var child = next;

            next = child.NextSibling;

            child.PreviousSibling = null;
            child.NextSibling     = null;
            child.Parent          = null;
            child.Tree            = null;

            this.OnChildRemoved(child);

            if (child == end)
            {
                break;
            }
        }
        while (next != null);

        this.FirstChild = null;
        this.LastChild  = null;
    }

    public IEnumerable<Node> Traverse() =>
        new TraverseEnumerator(this);

    public override string ToString() =>
        $"<{this.NodeName} name='{this.Name}'>";

    public virtual void Update(double deltaTime)
    { }
}
