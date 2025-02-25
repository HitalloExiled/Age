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

    #region 1-byte
    public bool Visible { get; set; } = true;
    #endregion

    public RenderTree? RenderTree => this.Tree as RenderTree;

    protected override void Connected(NodeTree tree)
    {
        base.Connected(tree);

        if (tree is RenderTree renderTree)
        {
            this.Connected(renderTree);
        }
    }

    protected override void Disconnected(NodeTree tree)
    {
        base.Disconnected(tree);

        if (tree is RenderTree renderTree)
        {
            this.Disconnected(renderTree);
        }
    }

    protected virtual void Connected(RenderTree renderTree) { }
    protected virtual void Disconnected(RenderTree renderTree) { }
}
