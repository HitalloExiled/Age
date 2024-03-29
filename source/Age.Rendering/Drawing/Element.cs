using Age.Numerics;
using Age.Rendering.Commands;

namespace Age.Rendering.Drawing;

public class Element : Node
{
    private Transform2D transform;
    private readonly List<Element> childrenElements = [];

    private int elementIndex = -1;

    internal List<DrawCommand> Commands { get; set; } = [];

    public Element? ParentElement => this.Parent as Element;

    public Element? PreviousElementSibling => this.ParentElement?.GetElement(this.elementIndex - 1);
    public Element? NextElementSibling     => this.ParentElement?.GetElement(this.elementIndex + 1);


    public Transform2D Transform
    {
        get => this.transform;
        set
        {
            var hasSizeChanged = this.transform.Size != value.Size;

            this.transform = value;

            if (hasSizeChanged)
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
            element.Transform = element.Transform with { Position = element.Transform.Position - this.Transform.Position };

            this.childrenElements.Remove(element);
        }
    }

    private void ApplyRepositioning()
    {
        var size      = new Size<float>();
        var stackMode = StackMode.Vertical;

        if (this.ParentElement != null)
        {
            stackMode = this.ParentElement.Style.Stack;

            foreach (var item in this.ParentElement.childrenElements)
            {
                if (stackMode == StackMode.Horizontal)
                {
                    size.Height  = float.Max(size.Height, item.Transform.Size.Height);
                    size.Width  += item.Transform.Size.Width;
                }
                else
                {
                    size.Height += item.Transform.Size.Height;
                    size.Width   = float.Max(size.Width, item.Transform.Size.Width);
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
                    ? new Vector2<float>(previous.Transform.Position.X + previous.Transform.Size.Width, -(size.Height - next.Transform.Size.Height))
                    : new Vector2<float>(0, previous.Transform.Position.Y + -previous.Transform.Size.Height);

                next.Transform = next.Transform with { Position = position };
            }
            else
            {
                var position = stackMode == StackMode.Horizontal
                    ? new Vector2<float>(0, -(size.Height - next.Transform.Size.Height))
                    : new Vector2<float>(0, 0);

                next.Transform = next.Transform with { Position = position };
            }

            previous = next;
            next     = next.NextElementSibling;
        }

        if (this.ParentElement != null && this.ParentElement.Transform.Size != size)
        {
            this.ParentElement.Transform = this.ParentElement.Transform with { Size = size };
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
