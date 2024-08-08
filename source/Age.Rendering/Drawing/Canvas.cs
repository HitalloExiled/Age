using Age.Rendering.Scene;

namespace Age.Rendering.Drawing;

public sealed class Canvas : Element
{
    private const ushort PADDING = 8;

    public override string NodeName { get; } = nameof(Canvas);

    public Canvas() =>
        this.Style = new()
        {
            Baseline = 1,
            Position = new(PADDING, -PADDING),
        };

    private void OnWindowSizeChanged() =>
        this.Style.Size = this.Tree!.Window.ClientSize - PADDING * 2;

    internal protected override void RequestUpdate()
    {
        base.RequestUpdate();

        if (this.IsConnected)
        {
            this.Tree.IsDirty = true;
        }
    }

    protected override void OnConnected(NodeTree tree)
    {
        tree.Window.SizeChanged += this.OnWindowSizeChanged;

        this.OnWindowSizeChanged();
    }

    protected override void OnDisconnected(NodeTree tree) =>
        tree.Window.SizeChanged -= this.OnWindowSizeChanged;

    protected override void OnChildAppended(Node child)
    {
        if (child is Element element)
        {
            element.Canvas = this;
        }
    }

    protected override void OnChildRemoved(Node child)
    {
        if (child is Element element)
        {
            element.Canvas = null;
        }
    }

    public override void LateUpdate() =>
        this.UpdateLayout();
}
