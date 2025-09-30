using Age.Commands;
using Age.Core.Extensions;

namespace Age.Graphs;

public class CommandBuffer<T> where T : Command
{
    private readonly List<T>       commands = [];
    private readonly List<Command> indices    = [];

    public ReadOnlySpan<T> Commands => this.commands.AsSpan();
    public ReadOnlySpan<Command>   Indices  => this.indices.AsSpan();

    public void AddCommand(T command) =>
        this.commands.Add(command);

    public void AddCommandRange(ReadOnlySpan<T> command) =>
        this.commands.AddRange(command);

    public void AddIndexCommand(Command command) =>
        this.indices.Add(command);

    public void AddIndexCommandRange(ReadOnlySpan<Command> command) =>
        this.indices.AddRange(command);

     public void Reset()
    {
        this.commands.Clear();
        this.indices.Clear();
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
