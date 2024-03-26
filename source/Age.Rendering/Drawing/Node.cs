using System.Diagnostics;

namespace Age.Rendering.Drawing;

[DebuggerDisplay("Index: {Index}, Children: {children.Count}, Parent: {Parent != null}")]
public class Node
{
    private readonly List<Node> children = [];

    public Node? this[int index] => index > -1 && index < this.children.Count ? this.children[index] : null;
    public Node? PreviousSibling => this.Parent?[this.Index - 1];
    public Node? NextSibling     => this.Parent?[this.Index + 1];

    public int   Index  { get; private set; }
    public Node? Parent { get; private set; }

    protected virtual void OnChildAdded(Node child)
    { }

    protected virtual void OnChildRemoved(Node child)
    { }

    protected virtual void OnUpdate()
    { }

    public void Add(Node child)
    {
        if (child.Parent != this)
        {
            child.Parent = this;
            child.Index  = this.children.Count;

            this.children.Add(child);

            this.OnChildAdded(child);
        }
    }

    public IEnumerable<Node> Enumerate(bool topDown = false)
    {
        foreach (var child in this.children)
        {
            if (topDown)
            {
                yield return child;
            }

            foreach (var item in child.Enumerate(topDown))
            {
                yield return item;
            }

            if (!topDown)
            {
                yield return child;
            }
        }
    }

    public IEnumerable<T> Enumerate<T>(bool topDown = false) where T : Node
    {
        foreach (var node in this.Enumerate(topDown))
        {
            if (node is T t)
            {
                yield return t;
            }
        }
    }

    public void Remove() =>
        this.Parent?.Remove(this);

    public void Remove(Node child)
    {
        child.Parent = null;
        child.Index  = -1;

        this.children.Remove(child);

        this.OnChildRemoved(child);
    }

    public void Update()
    {
        foreach (var child in this.children)
        {
            child.Update();
        }

        this.OnUpdate();
    }
}
