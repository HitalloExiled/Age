using Age.Core.Extensions;
using Age.Commands;

namespace Age.Graphs;

public sealed class CommandBuffer<T> : List<T> where T : Command
{
    public Span<T> Commands => this.AsSpan();
}

public class RenderContext
{
    private readonly CommandBuffer<Command2D> buffer2D = [];
    private readonly CommandBuffer<Command3D> buffer3D = [];

    private CommandBuffer<Command2D>? buffer2DOverride;
    private CommandBuffer<Command3D>? buffer3DOverride;

    public CommandBuffer<Command2D> Buffer2D => this.buffer2DOverride ?? this.buffer2D;
    public CommandBuffer<Command3D> Buffer3D => this.buffer3DOverride ?? this.buffer3D;
    public CommandBuffer<Command2D> UIBuffer { get; } = [];

    public void ClearOverride2D() =>
        this.buffer2DOverride = null;

    public void ClearOverride3D() =>
        this.buffer3DOverride = null;

    public void Override2D(RenderContext renderContext) =>
        this.buffer2DOverride = renderContext.Buffer2D;

    public void Override3D(RenderContext renderContext) =>
        this.buffer3DOverride = renderContext.Buffer3D;

    public void Clear()
    {
        this.buffer2D.Clear();
        this.buffer3D.Clear();
    }
}
