using Age.Numerics;
using Age.Scene;

namespace Age.Elements;

public sealed class Canvas : Element
{
    // private const ushort PADDING = 8;

    public override string NodeName => nameof(Elements.Canvas);

    public Canvas()
    {
        this.NodeFlags = NodeFlags.IgnoreUpdates;
        this.Style = new()
        {
            // Padding = new(Unit.Px(PADDING)),
            Color = Color.White,
        };
    }

    private void OnWindowResized()
    {
        if (this.Tree is RenderTree renderTree)
        {
            this.Style.Size = new(Unit.Px(renderTree.Window.ClientSize.Width), Unit.Px(renderTree.Window.ClientSize.Height));
        }
    }

    protected override void OnConnected(RenderTree renderTree)
    {
        base.OnConnected(renderTree);

        renderTree.Window.Resized += this.OnWindowResized;

        this.OnWindowResized();

        renderTree.AddDeferredUpdate(this.UpdateDirtyLayout);
    }

    protected override void OnDisconnected(RenderTree renderTree)
    {
        base.OnDisconnected(renderTree);

        renderTree.Window.Resized -= this.OnWindowResized;
    }
}
