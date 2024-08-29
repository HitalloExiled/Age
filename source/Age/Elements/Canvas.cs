using Age.Scene;
using Age.Styling;

namespace Age.Elements;

public sealed class Canvas : Element
{
    private const ushort PADDING = 8;

    public override string NodeName { get; } = nameof(Canvas);

    public Canvas() =>
        this.Style = new()
        {
            Baseline = 1,
            // Position = new(PADDING, -PADDING),
        };

    private void OnWindowSizeChanged() =>
        // this.Style.Size = SizeUnit.Pixel(this.Tree!.Window.ClientSize - PADDING * 2);
        this.Style.Size = new((Pixel)this.Tree!.Window.ClientSize.Width, (Pixel)this.Tree!.Window.ClientSize.Height);

    internal protected override void RequestUpdate()
    {
        base.RequestUpdate();

        if (this.IsConnected)
        {
            this.Tree.IsDirty = true;
        }
    }

    protected override void Connected(NodeTree tree)
    {
        tree.Window.Resized += this.OnWindowSizeChanged;

        this.OnWindowSizeChanged();
    }

    protected override void Disconnected(NodeTree tree) =>
        tree.Window.Resized -= this.OnWindowSizeChanged;

    public override void LateUpdate() =>
        this.UpdateLayout();
}
