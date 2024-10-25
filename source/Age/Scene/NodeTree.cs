using Age.Core;

namespace Age.Scene;

public abstract class NodeTree : Disposable
{
    public event Action? Updated;

    protected Stack<Node> Stack { get; } = [];

    public Root Root { get; }

    protected NodeTree() =>
        this.Root = new() { Tree = this };

    private void CallInitialize()
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

    private void CallLateUpdate()
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

    private void CallUpdate()
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
        this.CallInitialize();
        this.CallLateUpdate();
    }

    public virtual void Update()
    {
        this.CallUpdate();
        this.CallLateUpdate();

        this.Updated?.Invoke();
    }
}
