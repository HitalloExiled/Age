using Age.Commands;
using Age.Core;
using Age.Core.Extensions;

namespace Age.Scenes;

public abstract class Renderable : Node
{
    public bool Visible { get; set; } = true;

    public Range<ushort> SubtreeCommandRenge { get; set; }
}

public abstract class Renderable<T> : Renderable where T : Command
{
    private readonly List<T> commands = [];

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

            this.NotifyCommandChanges();
        }
    }

    private protected void AddCommand(T command)
    {
        this.commands.Add(command);

        this.NotifyCommandChanges();
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

            this.NotifyCommandChanges();
        }
    }

    private protected void ClearCommands()
    {
        if (this.commands.Count > 0)
        {
            this.commands.Clear();

            this.NotifyCommandChanges();
        }
    }

    private protected void InsertCommand(int index, T command)
    {
        this.commands.Insert(index, command);

        this.NotifyCommandChanges();
    }

    private protected void NotifyCommandChanges()
    {
        if (!this.IsConnected)
        {
            return;
        }

        this.Scene!.Viewport!.Window!.Tree.InvalidateNodeSubTree(this, DirtState.Commands);
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

            this.NotifyCommandChanges();
        }
    }

    private protected void RemoveCommand(T command)
    {
        if (this.commands.Remove(command))
        {
            this.NotifyCommandChanges();
        }
    }

    private protected void RemoveCommandAt(Index index) =>
        this.RemoveCommandAt(index.GetOffset(this.commands.Count));

    private protected void RemoveCommandAt(int index)
    {
        this.commands.RemoveAt(index);

        this.NotifyCommandChanges();
    }

    internal ReadOnlySpan<T> GetCommands() =>
        this.commands.AsSpan();
}
