using Age.Core;
using Age.Core.Extensions;

namespace Age.Scene;

public abstract class NodeTree(Node root) : Disposable
{
    public event Action? Updated;

    private readonly Queue<Action> updatesQueue = [];

    internal List<Timer> Timers { get; } = [];

    public bool IsDirty { get; private set; }

    public Node Root { get; } = root;

    private void InitializeTree()
    {
        var enumerator = this.Root.GetTraversalEnumerator();

        while (enumerator.MoveNext())
        {
            var current = enumerator.Current;

            if (current.NodeFlags.HasFlags(NodeFlags.IgnoreUpdates))
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
        var enumerator = this.Root.GetTraversalEnumerator();

        while (enumerator.MoveNext())
        {
            var current = enumerator.Current;

            if (current.NodeFlags.HasFlags(NodeFlags.IgnoreUpdates))
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
        var enumerator = this.Root.GetTraversalEnumerator();

        while (enumerator.MoveNext())
        {
            var current = enumerator.Current;

            if (current.NodeFlags.HasFlags(NodeFlags.IgnoreUpdates))
            {
                enumerator.SkipToNextSibling();
            }
            else
            {
                current.Update();

                if (current.NodeFlags.HasFlags(NodeFlags.IgnoreChildrenUpdates))
                {
                    enumerator.SkipToNextSibling();
                }
            }
        }
    }

    internal void AddDeferredUpdate(Action action)
    {
        this.updatesQueue.Enqueue(action);
        this.MakeDirty();
    }

    internal void MakePristine() =>
        this.IsDirty = false;

    public virtual void Initialize()
    {
        this.InitializeTree();
        this.LateUpdateTree();
    }

    public void MakeDirty() =>
        this.IsDirty = true;

    public virtual void Update()
    {
        this.UpdateTimers();
        this.UpdateTree();
        this.LateUpdateTree();

        this.Updated?.Invoke();

        while (this.updatesQueue.Count > 0)
        {
            this.updatesQueue.Dequeue().Invoke();
        }
    }
}
