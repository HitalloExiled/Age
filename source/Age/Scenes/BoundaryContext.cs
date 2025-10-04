using Age.Commands;
using Age.Core.Extensions;
using Age.Core;
using System.Runtime.CompilerServices;
using Age.Graphs;

namespace Age.Scenes;

public readonly struct BoundaryContext<TCommand>(
    int                       startIndex,
    CommandRange              commandRange,
    CommandBuffer<TCommand>   commandBuffer,
    List<Renderable>          stage,
    IReadOnlyList<Renderable> nodes
) : IDisposable
where TCommand : Command
{
    private static readonly ListPool<TCommand> commandListPool = new();


    public CommandBuffer<TCommand>   CommandBuffer { get; } = commandBuffer;
    public List<TCommand>            Colors        { get; } = commandListPool.Get();
    public List<TCommand>            Indices       { get; } = commandListPool.Get();
    public CommandRange              CommandRange  { get; } = commandRange;
    public IReadOnlyList<Renderable> Nodes         { get; } = nodes;
    public List<Renderable>          Stage         { get; } = stage;
    public int                       StartIndex    { get; } = startIndex;

    public readonly ushort Index       => (ushort)(this.StartIndex + this.Stage.Count);
    public readonly ushort ColorOffset => (ushort)(this.CommandRange.Color.Start + this.Colors.Count);
    public readonly ushort IndexOffset => (ushort)(this.CommandRange.Index.Start + this.Indices.Count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void CollectCommands(ReadOnlySpan<TCommand> source)
    {
        foreach (var command in source)
        {
            if (command.CommandFilter.HasFlags(CommandFilter.Color))
            {
                this.Colors.Add(command);
            }

            if (command.CommandFilter.HasFlags(CommandFilter.Index))
            {
                this.Indices.Add(command);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void CollectCommands(Renderable<TCommand> renderable)
    {
        renderable.CommandRange = new CommandRange(new(this.ColorOffset), new(this.IndexOffset));

        this.CollectCommands(renderable.GetCommands());

        renderable.CommandRange = renderable.CommandRange.WithEnd(this.ColorOffset, this.IndexOffset);
    }

    public readonly CommandRange CreateCommandRange() =>
        new(new(this.ColorOffset), new(this.IndexOffset));

    public readonly CommandRange WithEnd(in CommandRange commandRange) =>
        commandRange.WithEnd(this.ColorOffset, this.IndexOffset);

    public readonly CommandRange GetCommandRangeWithExtend(in CommandRange commandRange) =>
        commandRange.WithExtend(this.ColorOffset, this.IndexOffset);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void StartSubtreeRange(Renderable renderable)
    {
        renderable.SubtreeRange = SubtreeRange.CreateWithLength(this.Index, 1);

        this.Stage.Add(renderable);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void StartSubtreeRange(Renderable<TCommand> renderable, bool collectCommands)
    {
        this.StartSubtreeRange(renderable);

        if (collectCommands)
        {
            this.CollectCommands(renderable);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void EndSubtreeRange(Renderable renderable) =>
        renderable.SubtreeRange = renderable.SubtreeRange.WithEnd(this.Index);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void EndSubtreeRange(Renderable<TCommand> renderable, bool extendCommandRange)
    {
        this.EndSubtreeRange(renderable);

        if (extendCommandRange)
        {
            renderable.CommandRange = renderable.CommandRange.WithExtend(this.ColorOffset, this.IndexOffset);
        }
    }

    public readonly void Dispose()
    {
        commandListPool.Return(this.Colors);
        commandListPool.Return(this.Indices);
    }

    public void UpdateBuffer(bool all)
    {
        if (all)
        {
            this.CommandBuffer.ReplaceColorCommandRange(0.., this.Colors.AsSpan());
            this.CommandBuffer.ReplaceIndexCommandRange(0.., this.Indices.AsSpan());
        }
        else
        {
            this.CommandBuffer.ReplaceColorCommandRange(this.CommandRange.Color.Range, this.Colors.AsSpan());
            this.CommandBuffer.ReplaceIndexCommandRange(this.CommandRange.Index.Range, this.Indices.AsSpan());
        }
    }
}
