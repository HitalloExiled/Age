using Age.Commands;
using Age.Core.Extensions;
using System.Runtime.CompilerServices;

namespace Age.Scenes;

public abstract class Renderable : Node
{
#pragma warning disable IDE0032 // Use auto property
    private bool visible = true;
#pragma warning restore IDE0032 // Use auto property

    internal DirtState DirtState { get; set; }

    internal ShortRange SubtreeRange
    {
        get;
        set
        {
            var indexHasChanged = field.Start != value.Start;

            field = value;

            if (indexHasChanged)
            {
                this.OnIndexChangedInternal();
            }
        }
    }

    internal int Index => this.SubtreeRange.Start;

    public bool Visible
    {
        get => this.visible;
        set
        {
            if (this.visible != value)
            {
                this.visible = value;

                this.OnVisibilityChangedInternal();
                this.OnVisibilityChanged();
                this.UpdateDirtyState(DirtState.Subtree);
            }
        }
    }

    private protected virtual void OnIndexChangedInternal() { }
    private protected virtual void OnVisibilityChangedInternal() { }

    private protected override void OnChildAttachedInternal(Node node)
    {
        base.OnChildAttachedInternal(node);

        this.UpdateDirtyState(DirtState.Subtree);
    }

    private protected override void OnChildDetachingInternal(Node node)
    {
        base.OnChildDetachingInternal(node);

        this.UpdateDirtyState(DirtState.Subtree);
    }

    protected virtual void OnVisibilityChanged() { }

    internal void UpdateDirtyState(DirtState dirtState)
    {
        var isPristine = this.DirtState == default;

        this.DirtState |= dirtState;

        if (isPristine && this.IsConnected)
        {
            (this as Window ?? this.Scene?.Viewport?.Window)?.Tree.InvalidatedSubTree(this);
        }
    }

    internal void MarkCommandsDirty() =>
        this.UpdateDirtyState(DirtState.Commands);

    internal void MarkSubtreeDirty() =>
        this.UpdateDirtyState(DirtState.Subtree);

    internal void MakeSubtreeStatePristine() =>
        this.DirtState = default;
}

public abstract class Renderable<T> : Renderable where T : Command
{
    private readonly List<T> commands = [];

    internal CommandRange CommandRange { get; set; }

    internal ReadOnlySpan<T> Commands => this.commands.AsSpan();

    internal T? SingleCommand
    {
        get => this.commands.Count == 1 ? this.commands[0] : null;
        set
        {
            if (value == null)
            {
                this.commands.Clear();
            }
            else if (this.commands.Count == 1)
            {
                this.commands[0] = value;
            }
            else
            {
                this.commands.Clear();
                this.commands.Add(value);
            }

            this.MarkCommandsDirty();
        }
    }

    private protected void AddCommand(T command)
    {
        this.commands.Add(command);

        this.MarkCommandsDirty();
    }

    private protected void AllocateCommands<TCommand, TNode>(int count, CommandPool<TCommand, TNode> pool, bool reset = false)
    where TCommand : Command<TNode>, new()
    where TNode    : Node
    {
        this.commands.EnsureCapacity(count);

        if (count < this.commands.Count)
        {
            this.ReleaseCommands(this.commands.Count - count, pool);
        }
        else if (count > this.commands.Count)
        {
            if (reset)
            {
                resetAll(this.commands);
            }

            var tail = this.commands.Count;

            this.commands.SetCount(count);

            var span = this.commands.AsSpan();

            for (var i = tail; i < span.Length; i++)
            {
                span[i] = Unsafe.As<T>(pool.Get(Unsafe.As<TNode>(this)));
            }

            this.MarkCommandsDirty();
        }
        else if (reset)
        {
            resetAll(this.commands);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void resetAll(List<T> commands)
        {
            foreach (var command in commands)
            {
                command.Reset();
            }
        }
    }

    private protected void ClearCommands()
    {
        if (this.commands.Count > 0)
        {
            this.commands.Clear();

            this.MarkCommandsDirty();
        }
    }

    private protected void InsertCommand(int index, T command)
    {
        this.commands.Insert(index, command);

        this.MarkCommandsDirty();
    }

    private protected void ReleaseCommands<TCommand, TNode>(int count, CommandPool<TCommand, TNode> pool)
    where TCommand : Command<TNode>, new()
    where TNode    : Node
    {
        if (count > 0)
        {
            var span  = this.commands.AsSpan();
            var start = span.Length - count;

            for (var i = start; i < span.Length; i++)
            {
                pool.Return(Unsafe.As<TCommand>(span[i]));

                span[i] = default!;
            }

            this.commands.SetCount(start);

            this.MarkCommandsDirty();
        }
    }

    private protected void RemoveCommand(T command)
    {
        if (this.commands.Remove(command))
        {
            this.MarkCommandsDirty();
        }
    }

    private protected void RemoveCommandAt(Index index) =>
        this.RemoveCommandAt(index.GetOffset(this.commands.Count));

    private protected void RemoveCommandAt(int index)
    {
        this.commands.RemoveAt(index);

        this.MarkCommandsDirty();
    }
}
