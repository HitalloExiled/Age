using Age.Core;

namespace Age.Scene;

public abstract class NodeTree : Disposable
{
    public event Action? Updated;

    #region 8-bytes
    private readonly Queue<Action> updatesQueue = [];

    internal List<Timer> Timers { get; } = [];

    public Root Root { get; }
    #endregion

    #region 1-bytes
    public bool IsDirty { get; private set; }
    #endregion

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

                if (current.Flags.HasFlag(NodeFlags.IgnoreChildrenUpdates))
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

    public void MakeDirty() =>
        this.IsDirty = true;

    internal void MakePristine() =>
        this.IsDirty = false;

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

        while (this.updatesQueue.Count > 0)
        {
            this.updatesQueue.Dequeue().Invoke();
        }
    }
}
