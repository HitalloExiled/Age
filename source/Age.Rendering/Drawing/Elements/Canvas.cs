namespace Age.Rendering.Drawing.Elements;

public class Canvas : Element
{
    private const ushort PADDING = 8;
    private readonly Stack<Element> updateStack = [];

    public override string NodeName { get; } = nameof(Canvas);

    public Canvas() =>
        this.Style = new()
        {
            Baseline = 1,
            Position = new(PADDING, -PADDING),
        };

    private void OnWindowSizeChanged() =>
        this.Style.Size = this.Tree!.Window.ClientSize - PADDING * 2;

    protected override void OnConnected()
    {
        this.Tree!.Window.SizeChanged += this.OnWindowSizeChanged;
        this.OnWindowSizeChanged();
    }

    protected override void OnDisconnected() =>
        this.Tree!.Window.SizeChanged -= this.OnWindowSizeChanged;

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

    internal void RequestUpdate(Element element) =>
        this.updateStack.Push(element);

    internal override void UpdateLayout()
    {
        while (this.updateStack.Count > 0)
        {
            var element = this.updateStack.Pop();

            element.UpdateLayout();
        }

        base.UpdateLayout();
    }
}
