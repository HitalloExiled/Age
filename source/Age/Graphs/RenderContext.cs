using Age.Commands;
using Age.Core.Extensions;

namespace Age.Graphs;

public class RenderContext
{
    private readonly List<Command2D> commands2D = [];
    private readonly List<Command3D> commands3D = [];
    private readonly List<Command>   indices    = [];

    public ReadOnlySpan<Command2D> Commands2D => this.commands2D.AsSpan();
    public ReadOnlySpan<Command3D> Commands3D => this.commands3D.AsSpan();
    public ReadOnlySpan<Command>   Indices    => this.indices.AsSpan();

    public void AddCommand(Command2D command) =>
        this.commands2D.Add(command);

    public void AddCommandRange(ReadOnlySpan<Command2D> command) =>
        this.commands2D.AddRange(command);

    public void AddCommand(Command3D command) =>
        this.commands3D.Add(command);

    public void AddCommandRange(ReadOnlySpan<Command3D> command) =>
        this.commands3D.AddRange(command);

    public void AddIndexCommand(Command command) =>
        this.indices.Add(command);

    public void AddIndexCommandRange(ReadOnlySpan<Command> command) =>
        this.indices.AddRange(command);

    public void Reset()
    {
        this.commands2D.Clear();
        this.commands3D.Clear();
        this.indices.Clear();
    }
}
