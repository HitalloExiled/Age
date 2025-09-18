using System.Diagnostics.CodeAnalysis;
using Age.Commands;

namespace Age.Scene;

public abstract class Renderable : Node
{
    internal List<Command> Commands { get; init; } = [];

    public Viewport? Viewport { get; set; }
    public Window?   Window   { get; set; }

    [MemberNotNullWhen(true, nameof(Viewport), nameof(Window))]
    public new bool IsConnected => base.IsConnected;

    internal Command? SingleCommand
    {
        get => this.Commands.Count == 1 ? this.Commands[0] : null;
        set
        {
            if (value == null)
            {
                this.Commands.Clear();
            }
            else if (this.Commands.Count == 1)
            {
                this.Commands[0] = value;
            }
            else
            {
                this.Commands.Clear();
                this.Commands.Add(value);
            }
        }
    }

    public bool Visible { get; set; } = true;

    protected override void OnConnected()
    {
        base.OnConnected();

        if (this.Parent is Window window)
        {
            this.Viewport = window;
            this.Window   = window;
        }
        else if (this.Parent is Viewport viewport)
        {
            this.Viewport = viewport;
            this.Window   = viewport.Window;
        }
        else if (this.Parent is Renderable renderable)
        {
            this.Viewport = renderable.Viewport;
            this.Window   = renderable.Window;
        }
    }

    protected override void OnDisconnected()
    {
        base.OnDisconnected();

        this.Viewport = null;
        this.Window   = null;
    }
}
