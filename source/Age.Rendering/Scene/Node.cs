using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Age.Rendering.Scene;

[DebuggerDisplay("NodeName: {NodeName}, Name: {Name}, IsConnected: {IsConnected}")]
public abstract partial class Node : IEnumerable<Node>
{
    private NodeTree? tree;

    internal uint ObjectId { get; set; }

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
        private set
        {
            if (value != this.tree)
            {
                this.tree = value;

                for (var child = this.FirstChild; child != null; child = child.NextSibling)
                {
                    child.Tree = this.tree;
                }

                if (this.tree != null)
                {
                    this.tree.Nodes.Add(this);

                    this.ObjectId = (uint)this.tree.Nodes.Count;

                    this.OnConnected();
                }
                else
                {
                    this.ObjectId = 0;
                    this.OnDisconnected();
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

    protected virtual void OnConnected()
    { }

    protected virtual void OnDisconnected()
    { }

    protected virtual void OnChildRemoved(Node child)
    { }

    protected virtual void OnInitialize()
    { }

    protected virtual void OnPreUpdate(double deltaTime)
    { }

    protected virtual void OnPostUpdate(double deltaTime)
    { }

    protected virtual void OnUpdate(double deltaTime)
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

            child.Tree = this is NodeTree tree ? tree : this.Tree;

            this.OnChildAppended(child);
            child.OnAdopted();
        }
    }

    public void AppendChildren(IEnumerable<Node> children)
    {
        foreach (var child in children)
        {
            this.AppendChild(child);
        }
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

    public void Remove() =>
        this.Parent?.RemoveChild(this);

    public void RemoveChildren()
    {
        if (this.FirstChild != null)
        {
            var next = this.FirstChild;

            do
            {
                var child = next;

                next = child.NextSibling;

                child.PreviousSibling = null;
                child.NextSibling     = null;
                child.Parent          = null;
                child.Tree            = null;

                this.OnChildRemoved(child);
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

            child.Tree            = null;
            child.PreviousSibling = null;
            child.NextSibling     = null;
            child.Parent          = null;

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

    public IEnumerable<Node> Traverse(bool topDown = false) =>
        new TraverseEnumerator(this, topDown);

    public IEnumerable<T> Traverse<T>(bool topDown = false) where T : Node
    {
        foreach (var node in this.Traverse(topDown))
        {
            if (node is T t)
            {
                yield return t;
            }
        }
    }

    public override string ToString() =>
        $"<{this.NodeName} name='{this.Name}'>";

    public void Initialize()
    {
        foreach (var child in this)
        {
            child.Initialize();
        }

        this.OnInitialize();
    }

    public void Update(double deltaTime, Action<Node>? callback = null)
    {
        this.OnPreUpdate(deltaTime);

        foreach (var child in this)
        {
            child.Update(deltaTime, callback);
        }

        this.OnUpdate(deltaTime);

        this.OnPostUpdate(deltaTime);

        callback?.Invoke(this);
    }
}
