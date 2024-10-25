namespace Age.Scene;

public abstract class RenderNode : Node
{
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
