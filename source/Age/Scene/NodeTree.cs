using Age.Core;

namespace Age.Scene;

public abstract class NodeTree : Disposable
{
    public event Action? Updated;

    protected Stack<Node> Stack { get; } = [];

    internal List<Timer> Timers { get; } = [];

    public Root Root { get; }

    protected NodeTree() =>
        this.Root = new() { Tree = this };

    private void InitializeTree()
    {
        this.Stack.Push(this.Root);

        while (this.Stack.Count > 0)
        {
            var current = this.Stack.Pop();

            if (!current.Flags.HasFlag(NodeFlags.IgnoreUpdates))
            {
                current.Initialize();

                foreach (var node in current.Reverse())
                {
                    this.Stack.Push(node);
                }
            }
        }
    }

    private void LateUpdateTree()
    {
        this.Stack.Push(this.Root);

        while (this.Stack.Count > 0)
        {
            var current = this.Stack.Pop();

            if (!current.Flags.HasFlag(NodeFlags.IgnoreUpdates))
            {
                current.LateUpdate();

                foreach (var node in current.Reverse())
                {
                    this.Stack.Push(node);
                }
            }
        }
    }

    private void UpdateTimers()
    {
        foreach (var timer in this.Timers)
        {
            timer.Update();
        }
    }

    private void UpdateTree()
    {
        this.Stack.Push(this.Root);

        while (this.Stack.Count > 0)
        {
            var current = this.Stack.Pop();

            if (!current.Flags.HasFlag(NodeFlags.IgnoreUpdates))
            {
                current.Update();

                foreach (var node in current.Reverse())
                {
                    this.Stack.Push(node);
                }
            }
        }
    }

    public virtual void Initialize()
    {
        this.InitializeTree();
        this.LateUpdateTree();
    }

    public virtual void Update()
    {
        this.UpdateTimers();
        this.UpdateTree();
        this.LateUpdateTree();

        this.Updated?.Invoke();
    }
}
