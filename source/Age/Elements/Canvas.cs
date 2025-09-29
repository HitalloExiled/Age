using System.Diagnostics;
using Age.Numerics;
using Age.Scenes;

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

    private void OnViewportResized()
    {
        this.Style.Size = this.Scene!.Viewport!.Size;
    }

    private void OnViewportChanged(Viewport? @new, Viewport? old)
    {
        old?.Resized  -= this.OnViewportResized;
        @new?.Resized += this.OnViewportResized;

        this.OnViewportResized();
    }

    private protected override void OnConnectedInternal()
    {
        base.OnConnectedInternal();

        Debug.Assert(this.Scene?.Viewport?.Window != null);

        this.Scene.Viewport.Window.Tree.AddDeferredUpdate(this.UpdateDirtyLayout);
        this.Scene.ViewportChanged += this.OnViewportChanged;

        this.Scene.Viewport.Resized += this.OnViewportResized;

        this.OnViewportResized();
    }

    private protected override void OnDisconnectingInternal()
    {
        base.OnDisconnectingInternal();

        Debug.Assert(this.Scene?.Viewport?.Window != null);

        this.Scene.ViewportChanged  -= this.OnViewportChanged;
        this.Scene.Viewport.Resized -= this.OnViewportResized;
    }
}
