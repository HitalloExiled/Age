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

    private protected override void OnConnectedInternal()
    {
        base.OnConnectedInternal();

        Debug.Assert(this.Window != null);
        Debug.Assert(this.Viewport != null);

        this.Viewport.Resized += this.OnViewportResized;

        this.OnViewportResized();

        this.Window.Tree.AddDeferredUpdate(this.UpdateDirtyLayout);
    }

    private protected override void OnDisconnectingInternal()
    {
        base.OnDisconnectingInternal();

        Debug.Assert(this.Viewport != null);

        this.Viewport.Resized -= this.OnViewportResized;
    }
}
