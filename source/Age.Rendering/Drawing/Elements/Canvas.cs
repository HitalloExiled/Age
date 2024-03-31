namespace Age.Rendering.Drawing.Elements;

public class Canvas : Node
{
    private readonly Stack<Element> updateStack = [];

    private bool updating;

    public override string NodeName { get; } = nameof(Canvas);

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

    internal void RequestUpdate(Element element)
    {
        if (!updating)
        {
            this.updateStack.Push(element);
        }
    }

    internal void UpdateLayout()
    {
        updating = true;

        while (updateStack.Count > 0)
        {
            var element = updateStack.Pop();

            element.UpdateLayout();
        }

        updating = false;
    }
}
