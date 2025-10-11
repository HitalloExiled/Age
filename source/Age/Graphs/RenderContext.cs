using Age.Commands;
using Age.Core.Extensions;

namespace Age.Graphs;

public class CommandBuffer<T> where T : Command
{
    private readonly List<T> colors  = [];
    private readonly List<T> encodes = [];

    public ReadOnlySpan<T> Colors => this.colors.AsSpan();
    public ReadOnlySpan<T> Encodes  => this.encodes.AsSpan();

    public void AddColorCommand(T command) =>
        this.colors.Add(command);

    public void AddColorCommandRange(ReadOnlySpan<T> command) =>
        this.colors.AddRange(command);

    public void AddIndexCommand(T command) =>
        this.encodes.Add(command);

    public void AddEncodeCommandRange(ReadOnlySpan<T> command) =>
        this.encodes.AddRange(command);

    public void ReplaceColorCommandRange(Range range, ReadOnlySpan<T> command) =>
        this.colors.ReplaceRange(range, command);

    public void ReplaceEncodeCommandRange(Range range, ReadOnlySpan<T> command) =>
        this.encodes.ReplaceRange(range, command);

     public void Reset()
    {
        this.colors.Clear();
        this.encodes.Clear();
    }
}

public class RenderContext
{
    private readonly CommandBuffer<Command2D> buffer2D = new();
    private readonly CommandBuffer<Command3D> buffer3D = new();

    private CommandBuffer<Command2D>? buffer2DOverride;
    private CommandBuffer<Command3D>? buffer3DOverride;

    public CommandBuffer<Command2D> Buffer2D => this.buffer2DOverride ?? this.buffer2D;
    public CommandBuffer<Command3D> Buffer3D => this.buffer3DOverride ?? this.buffer3D;


    public void ClearOverride2D() =>
        this.buffer2DOverride = null;

    public void ClearOverride3D() =>
        this.buffer3DOverride = null;

    public void Override2D(RenderContext renderContext) =>
        this.buffer2DOverride = renderContext.Buffer2D;

    public void Override3D(RenderContext renderContext) =>
        this.buffer3DOverride = renderContext.Buffer3D;

    public void Reset()
    {
        this.buffer2D.Reset();
        this.buffer3D.Reset();
    }
}
