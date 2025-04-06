using Age.Numerics;
using Age.Scene;
using Age.Styling;

namespace Age.Elements;

public sealed class Canvas : Element
{
    // private const ushort PADDING = 8;

    public override string NodeName { get; } = nameof(Canvas);

    public Canvas()
    {
        this.Flags = NodeFlags.IgnoreUpdates;
        this.Style = new()
        {
            // Padding = new((Pixel)PADDING),
            Color = Color.White,
        };
    }

    private void OnWindowSizeChanged()
    {
        if (this.Tree is RenderTree renderTree)
        {
            this.Style.Size = new((Pixel)renderTree.Window.ClientSize.Width, (Pixel)renderTree.Window.ClientSize.Height);
        }
    }

    protected override void Connected(RenderTree renderTree)
    {
        base.Connected(renderTree);

        renderTree.Window.Resized += this.OnWindowSizeChanged;

        this.OnWindowSizeChanged();
        this.Layout.UpdateDirtyLayout();
    }

    protected override void Disconnected(RenderTree renderTree)
    {
        base.Disconnected(renderTree);

        renderTree.Window.Resized -= this.OnWindowSizeChanged;
    }
}
