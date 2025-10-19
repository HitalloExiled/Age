using Age.Commands;
using Age.Core;
using Age.Core.Extensions;

namespace Age.Scenes;

public abstract class Renderable : Node
{
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
                this.OnIndexChanged();
            }
        }
    }

    internal int Index => this.SubtreeRange.Start;

    public bool Visible { get; set; } = true;

    private protected virtual void OnIndexChanged() { }

    private protected override void OnChildAttachedInternal(Node node)
    {
        base.OnChildAttachedInternal(node);

        this.MakeSubtreeDirty(DirtState.Subtree);
    }

    private protected override void OnChildDetachedInternal(Node node)
    {
        base.OnChildDetachedInternal(node);

        this.MakeSubtreeDirty(DirtState.Subtree);
    }

    private protected void MarkCommandsDirty() =>
        this.MakeSubtreeDirty(DirtState.Commands);

    internal void MakeSubtreeDirty(DirtState dirtState)
    {
        var isPristine = this.DirtState == default;

        this.DirtState |= dirtState;

        if (isPristine && this.IsConnected)
        {
            (this as Window ?? this.Scene?.Viewport?.Window)?.Tree.InvalidatedSubTree(this);
        }
    }

    internal void MakeSubtreePristine() =>
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

    private protected void AllocateCommands<U>(int count, ObjectPool<U> pool) where U : Command2D
    {
        this.commands.EnsureCapacity(count);

        if (count < this.commands.Count)
        {
            this.ReleaseCommands(this.commands.Count - count, pool);
        }
        else
        {
            var previousCount = this.commands.Count;

            this.commands.SetCount(count);

            var span = this.commands.AsSpan();

            for (var i = previousCount; i < span.Length; i++)
            {
                span[i] = (T)(Command)pool.Get();
            }

            this.MarkCommandsDirty();
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

    private protected void ReleaseCommands<U>(int count, ObjectPool<U> pool) where U : Command2D
    {
        if (count > 0)
        {
            var span  = this.commands.AsSpan();
            var start = span.Length - count;

            for (var i = start; i < span.Length; i++)
            {
                pool.Return((U)(Command)span[i]);

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
