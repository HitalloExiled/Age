using Age.Scene;
using Age.Styling;

namespace Age.Elements;

public sealed class Canvas : Element
{
    // private const ushort PADDING = 8;

    public override string NodeName { get; } = nameof(Canvas);

    public Canvas() =>
        this.Style = new()
        {
            // Padding = new((Pixel)PADDING),
        };

    private void OnWindowSizeChanged()
    {
        if (this.Tree is RenderTree renderTree)
        {
            this.Style.Size = new((Pixel)renderTree.Window.ClientSize.Width, (Pixel)renderTree.Window.ClientSize.Height);
        }
    }

    private void OnUpdate() => this.Layout.Update();

    protected override void Connected(RenderTree renderTree)
    {
        base.Connected(renderTree);

        renderTree.Window.Resized += this.OnWindowSizeChanged;
        renderTree.Updated        += this.OnUpdate;

        this.OnWindowSizeChanged();
    }

    protected override void Disconnected(RenderTree renderTree)
    {
        base.Disconnected(renderTree);

        renderTree.Window.Resized -= this.OnWindowSizeChanged;
        renderTree.Updated        -= this.OnUpdate;
    }
}
