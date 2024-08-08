using Age.Scene;

namespace Age.Elements;

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

    protected override void Connected(NodeTree tree)
    {
        tree.Window.Resized += this.OnWindowSizeChanged;

        this.OnWindowSizeChanged();
    }

    protected override void Disconnected(NodeTree tree) =>
        tree.Window.Resized -= this.OnWindowSizeChanged;

    protected override void ChildAppended(Node child)
    {
        if (child is Element element)
        {
            element.Canvas = this;
        }
    }

    protected override void ChildRemoved(Node child)
    {
        if (child is Element element)
        {
            element.Canvas = null;
        }
    }

    public override void LateUpdate() =>
        this.UpdateLayout();
}
