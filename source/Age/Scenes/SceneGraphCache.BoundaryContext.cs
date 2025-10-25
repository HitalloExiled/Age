using Age.Commands;
using Age.Core.Extensions;
using Age.Core;
using System.Runtime.CompilerServices;
using Age.Graphs;

namespace Age.Scenes;

internal partial class SceneGraphCache
{
    public readonly struct BoundaryContext<TCommand>(
        int                       index,
        CommandRange              commandRange,
        CommandBuffer<TCommand>   commandBuffer,
        List<Renderable>          stage,
        List<Renderable>          nodes
    ) : IDisposable
    where TCommand : Command
    {
        private static readonly ListPool<TCommand> commandListPool = new();

        public CommandBuffer<TCommand> CommandBuffer { get; } = commandBuffer;
        public List<TCommand>          Commands      { get; } = commandListPool.Get();
        public List<Renderable>        Nodes         { get; } = nodes;
        public List<Renderable>        Stage         { get; } = stage;

        public CommandRange CommandRange { get; } = commandRange.WithClamp((ushort)commandBuffer.Commands.Length);
        public int          Index        { get; } = index;

        public readonly ushort SubtreeOffset => (ushort)(this.Index + this.Stage.Count);
        public readonly ushort CommandOffset => (ushort)(this.CommandRange.Pre.Start + this.Commands.Count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void AccumulateCommands(Renderable<TCommand> renderable)
        {
            this.CollectCommands(renderable.Commands);

            renderable.CommandRange = renderable.CommandRange.WithPreEnd(this.CommandOffset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void CollectCommands(ReadOnlySpan<TCommand> source) =>
            this.Commands.AddRange(source);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void CollectCommands(Renderable<TCommand> renderable)
        {
            renderable.CommandRange = new CommandRange(this.CommandOffset);

            this.CollectCommands(renderable.Commands);

            renderable.CommandRange = renderable.CommandRange.WithPreEnd(this.CommandOffset);
        }

        public readonly CommandRange CreateCommandRange() =>
            new(this.CommandOffset);

        public readonly CommandRange WithPreEnd(in CommandRange commandRange) =>
            commandRange.WithPreEnd(this.CommandOffset);

        public readonly CommandRange WithPostEnd(in CommandRange commandRange) =>
            commandRange.WithPostEnd(this.CommandOffset);

        public readonly CommandRange WithPostStart(in CommandRange commandRange) =>
            commandRange.WithPostStart(this.CommandOffset);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void StartSubtreeRange(Renderable renderable)
        {
            renderable.MakeSubtreePristine();

            renderable.SubtreeRange = ShortRange.CreateWithLength(this.SubtreeOffset, 1);

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
            renderable.SubtreeRange = renderable.SubtreeRange.WithEnd(this.SubtreeOffset);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void EndSubtreeRange(Renderable<TCommand> renderable, bool extendCommandRange)
        {
            this.EndSubtreeRange(renderable);

            if (extendCommandRange)
            {
                renderable.CommandRange = renderable.CommandRange.WithPost(this.CommandOffset);
            }
        }

        public readonly void Dispose() =>
            commandListPool.Return(this.Commands);

        public void UpdateBuffer(Range range) =>
            this.CommandBuffer.ReplaceCommandRange(range, this.Commands.AsSpan());
    }
}
