using Age.Commands;

namespace Age.Scene;

public abstract class Renderable : Node
{
    internal List<Command> Commands { get; init; } = [];

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

    public RenderTree? RenderTree => this.Tree as RenderTree;

    protected override void OnConnected(NodeTree tree)
    {
        base.OnConnected(tree);

        if (tree is RenderTree renderTree)
        {
            this.OnConnected(renderTree);
        }
    }

    protected override void OnDisconnected(NodeTree tree)
    {
        base.OnDisconnected(tree);

        if (tree is RenderTree renderTree)
        {
            this.OnDisconnected(renderTree);
        }
    }

    protected virtual void OnConnected(RenderTree renderTree) { }
    protected virtual void OnDisconnected(RenderTree renderTree) { }
}
