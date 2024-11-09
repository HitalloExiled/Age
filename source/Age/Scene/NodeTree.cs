using Age.Core;

namespace Age.Scene;

public abstract class NodeTree : Disposable
{
    public event Action? Updated;

    internal List<Timer> Timers { get; } = [];

    public Root Root { get; }

    protected NodeTree() =>
        this.Root = new() { Tree = this };

    private void InitializeTree()
    {
        var enumerator = this.Root.GetTraverseEnumerator();

        while (enumerator.MoveNext())
        {
            var current = enumerator.Current;

            if (current.Flags.HasFlag(NodeFlags.IgnoreUpdates))
            {
                enumerator.SkipToNextSibling();
            }
            else
            {
                current.Initialize();
            }
        }
    }

    private void LateUpdateTree()
    {
        var enumerator = this.Root.GetTraverseEnumerator();

        while (enumerator.MoveNext())
        {
            var current = enumerator.Current;

            if (current.Flags.HasFlag(NodeFlags.IgnoreUpdates))
            {
                enumerator.SkipToNextSibling();
            }
            else
            {
                current.LateUpdate();
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
        var enumerator = this.Root.GetTraverseEnumerator();

        while (enumerator.MoveNext())
        {
            var current = enumerator.Current;

            if (current.Flags.HasFlag(NodeFlags.IgnoreUpdates))
            {
                enumerator.SkipToNextSibling();
            }
            else
            {
                current.Update();
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
