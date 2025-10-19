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
    List<Renderable>          nodes
) : IDisposable
where TCommand : Command
{
    private static readonly ListPool<TCommand> commandListPool = new();

    public CommandBuffer<TCommand>   CommandBuffer { get; } = commandBuffer;
    public List<TCommand>            Colors        { get; } = commandListPool.Get();
    public List<TCommand>            Encodes       { get; } = commandListPool.Get();
    public CommandRange              CommandRange  { get; } = commandRange.WithClamp((ushort)commandBuffer.Colors.Length, (ushort)commandBuffer.Encodes.Length);
    public List<Renderable>          Nodes         { get; } = nodes;
    public List<Renderable>          Stage         { get; } = stage;
    public int                       StartIndex    { get; } = startIndex;

    public readonly ushort Index        => (ushort)(this.StartIndex + this.Stage.Count);
    public readonly ushort ColorOffset  => (ushort)(this.CommandRange.Color.Pre.Start + this.Colors.Count);
    public readonly ushort EncodeOffset => (ushort)(this.CommandRange.Encode.Pre.Start + this.Encodes.Count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void AccumulateCommands(Renderable<TCommand> renderable)
    {
        this.CollectCommands(renderable.Commands);

        renderable.CommandRange = renderable.CommandRange.WithPreEnd(this.ColorOffset, this.EncodeOffset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void CollectCommands(ReadOnlySpan<TCommand> source)
    {
        foreach (var command in source)
        {
            if (command.CommandFilter.HasFlags(CommandFilter.Color))
            {
                this.Colors.Add(command);
            }

            if (command.CommandFilter.HasFlags(CommandFilter.Encode))
            {
                this.Encodes.Add(command);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void CollectCommands(Renderable<TCommand> renderable)
    {
        renderable.CommandRange = new CommandRange(new(this.ColorOffset), new(this.EncodeOffset));

        this.CollectCommands(renderable.Commands);

        renderable.CommandRange = renderable.CommandRange.WithPreEnd(this.ColorOffset, this.EncodeOffset);
    }

    public readonly CommandRange CreateCommandRange() =>
        new(new(this.ColorOffset), new(this.EncodeOffset));

    public readonly CommandRange WithPreEnd(in CommandRange commandRange) =>
        commandRange.WithPreEnd(this.ColorOffset, this.EncodeOffset);

    public readonly CommandRange WithPostEnd(in CommandRange commandRange) =>
        commandRange.WithPostEnd(this.ColorOffset, this.EncodeOffset);

    public readonly CommandRange WithPostStart(in CommandRange commandRange) =>
        commandRange.WithPostStart(this.ColorOffset, this.EncodeOffset);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void StartSubtreeRange(Renderable renderable)
    {
        renderable.MakeSubtreePristine();

        renderable.SubtreeRange = ShortRange.CreateWithLength(this.Index, 1);

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
            renderable.CommandRange = renderable.CommandRange.WithPost(this.ColorOffset, this.EncodeOffset);
        }
    }

    public readonly void Dispose()
    {
        commandListPool.Return(this.Colors);
        commandListPool.Return(this.Encodes);
    }

    public void UpdateBuffer(Range color, Range encode)
    {
        this.CommandBuffer.ReplaceColorCommandRange(color, this.Colors.AsSpan());
        this.CommandBuffer.ReplaceEncodeCommandRange(encode, this.Encodes.AsSpan());
    }
}
