using Age.Numerics;
using Age.Rendering.Commands;

namespace Age.Rendering.Drawing;

public class Element : Node
{
    private readonly List<Element> childrenElements = [];

    private Rect<int> bounds;
    private int       elementIndex = -1;

    internal List<DrawCommand> Commands { get; set; } = [];

    public Element? ParentElement => this.Parent as Element;

    public Element? PreviousElementSibling => this.ParentElement?.GetElement(this.elementIndex - 1);
    public Element? NextElementSibling     => this.ParentElement?.GetElement(this.elementIndex + 1);

    public Rect<int> Bounds
    {
        get => this.bounds;
        set
        {
            var hasChanged = this.bounds != value;

            this.bounds = value;

            if (hasChanged)
            {
                this.ApplyRepositioning();
            }
        }
    }

    public Style Style { get; set; } = new();

    private Element? GetElement(int index) =>
        index > -1 && index < this.childrenElements.Count ? this.childrenElements[index] : null;

    protected override void OnChildAdded(Node child)
    {
        if (child is Element element)
        {
            element.elementIndex = this.childrenElements.Count;

            this.childrenElements.Add(element);

            element.ApplyRepositioning();
        }
    }

    protected override void OnChildRemoved(Node child)
    {
        if (child is Element element)
        {
            element.elementIndex = -1;
            element.Bounds = element.Bounds with { Position = element.Bounds.Position - this.Bounds.Position };

            this.childrenElements.Remove(element);
        }
    }

    private void ApplyRepositioning()
    {
        var size      = new Size<int>();
        var stackMode = StackMode.Vertical;

        if (this.ParentElement != null)
        {
            stackMode = this.ParentElement.Style.Stack;

            foreach (var item in this.ParentElement.childrenElements)
            {
                if (stackMode == StackMode.Horizontal)
                {
                    size.Height  = int.Max(size.Height, item.Bounds.Size.Height);
                    size.Width  += item.Bounds.Size.Width;
                }
                else
                {
                    size.Height += item.Bounds.Size.Height;
                    size.Width   = int.Max(size.Width, item.Bounds.Size.Width);
                }
            }
        }

        var previous = this.PreviousElementSibling;
        var next     = this;

        while (next != null)
        {
            if (previous != null)
            {
                var position = stackMode == StackMode.Horizontal
                    ? new Point<int>(previous.Bounds.Position.X + previous.Bounds.Size.Width, -(size.Height - next.Bounds.Size.Height))
                    : new Point<int>(0, previous.Bounds.Position.Y + -previous.Bounds.Size.Height);

                next.bounds = next.Bounds with { Position = position };
            }
            else
            {
                var position = stackMode == StackMode.Horizontal
                    ? new Point<int>(0, -(size.Height - next.Bounds.Size.Height))
                    : new Point<int>(0, 0);

                next.bounds = next.Bounds with { Position = position };
            }

            previous = next;
            next     = next.NextElementSibling;
        }

        if (this.ParentElement != null && this.ParentElement.Bounds.Size != size)
        {
            this.ParentElement.Bounds = this.ParentElement.Bounds with { Size = size };
            this.ParentElement.Commands.Clear();

            var command = new RectDrawCommand()
            {
                Rect   = new(size.Width, size.Height, 0, 0),
                Border = new(),
            };

            this.ParentElement.Commands.Add(command);
        }
    }
}
