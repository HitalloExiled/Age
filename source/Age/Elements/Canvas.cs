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

    private void OnWindowSizeChanged() =>
        this.Style.Size = new((Pixel)this.Tree!.Window.ClientSize.Width, (Pixel)this.Tree!.Window.ClientSize.Height);

    protected override void Connected(NodeTree tree)
    {
        base.Connected(tree);

        tree.Window.Resized += this.OnWindowSizeChanged;
        tree.Updated += this.Layout.Update;

        this.OnWindowSizeChanged();
    }

    protected override void Disconnected(NodeTree tree)
    {
        base.Disconnected(tree);

        tree.Window.Resized -= this.OnWindowSizeChanged;
        tree.Updated -= this.Layout.Update;
    }
}
