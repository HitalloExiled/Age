using System.Diagnostics;
using Age.Numerics;

namespace Age.Elements;

public sealed class Canvas : Element
{
    // private const ushort PADDING = 8;

    public override string NodeName => nameof(Elements.Canvas);

    public Canvas()
    {
        this.Style = new()
        {
            // Padding = new(Unit.Px(PADDING)),
            Color           = Color.White,
            //BackgroundColor = Color.Green.WithAlpha(0.5f),
        };
    }

    private void OnViewportResized() =>
        this.Style.Size = this.Scene!.Viewport!.Size;

    private protected override void OnConnectedInternal()
    {
        base.OnConnectedInternal();

        Debug.Assert(this.Scene?.Viewport?.Window != null);

        var viewport = this.Scene?.Viewport;

        viewport!.Window!.Tree.AddDeferredUpdate(this.UpdateDirtyLayout);

        viewport.Resized += this.OnViewportResized;

        this.OnViewportResized();
    }

    private protected override void OnDisconnectingInternal()
    {
        base.OnDisconnectingInternal();

        Debug.Assert(this.Scene?.Viewport != null);

        this.Scene.Viewport.Resized -= this.OnViewportResized;
    }
}
