using System.Collections.Immutable;

namespace Age.Rendering.Drawing;

public class Node
{
    private readonly List<Node> children = [];

    public Node? Parent { get; private set; }

    public ImmutableArray<Node> Children => [.. this.children];

    public virtual Node? Root
    {
        get
        {
            var next = this;

            while (next.Parent != null)
            {
                next = next.Parent;
            }

            return next;
        }
    }

    protected virtual void OnUpdate()
    { }

    public void Add(Node child)
    {
        child.Parent = this;
        this.children.Add(child);
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
        this.children.Remove(child);
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
