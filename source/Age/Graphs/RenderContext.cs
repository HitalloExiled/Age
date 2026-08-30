using Age.Core.Extensions;
using Age.Commands;
using Age.Scenes;

namespace Age.Graphs;

public sealed class CommandBuffer<T> : List<T> where T : Command
{
    public Span<T> Commands => this.AsSpan();
}

public class RenderContext
{
    public Canvas?  Canvas  { get; private set; }
    public World2D? World2D { get; private set; }
    public World3D? World3D { get; private set; }

    public void BindCanvas(Canvas canvas) =>
        this.Canvas = canvas;

    public void BindWorld2D(World2D world2D) =>
        this.World2D = world2D;

    public void BindWorld3D(World3D world3D) =>
        this.World3D = world3D;

    public void ClearCanvas() =>
        this.World3D = null;

    public void ClearWorld2D() =>
        this.World2D = null;

    public void ClearWorld3D() =>
        this.World3D = null;

    public void ClearBinds()
    {
        this.ClearCanvas();
        this.ClearWorld2D();
        this.ClearWorld3D();
    }
}
