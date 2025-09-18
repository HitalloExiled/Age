using System.Diagnostics;
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
        this.Style     = new()
        {
            // Padding = new(Unit.Px(PADDING)),
            Color           = Color.White,
            //BackgroundColor = Color.Green.WithAlpha(0.5f),
        };
    }

    private void OnViewportResized()
    {
        this.Style.Size = new(this.Viewport!.Size.Width, this.Viewport!.Size.Height);
    }

    protected override void OnConnected()
    {
        base.OnConnected();

        Debug.Assert(this.Window != null);
        Debug.Assert(this.Viewport != null);

        this.Viewport.Resized += this.OnViewportResized;

        this.OnViewportResized();

        this.Window.Tree.AddDeferredUpdate(this.UpdateDirtyLayout);
    }

    protected override void OnDisconnected()
    {
        base.OnDisconnected();

        Debug.Assert(this.Viewport != null);

        this.Viewport.Resized -= this.OnViewportResized;
    }
}
