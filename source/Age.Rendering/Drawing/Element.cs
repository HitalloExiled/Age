using System.Text;
using Age.Numerics;
using Age.Rendering.Commands;
using Age.Rendering.Drawing.Elements;

namespace Age.Rendering.Drawing;

public abstract class Element : Node2D, IEnumerable<Element>
{
    private bool           calculated;
    private float          calculatedBaseLine;
    private Size<float>    calculatedSize;
    private Transform2D    dispositionTransform = new();
    private readonly Style style                = new();
    private Transform2D    styleTransform       = new();

    private Canvas? canvas;
    private bool    hasPendingUpdate;
    private string? text;

    public Element? ParentElement => this.Parent as Element;

    public Element? FirstElementChild { get; private set; }
    public Element? LastElementChild  { get; private set; }

    public Element? PreviousElementSibling { get; private set; }
    public Element? NextElementSibling     { get; private set; }

    public Canvas? Canvas
    {
        get => this.canvas;
        internal set
        {
            this.canvas = value;

            foreach (var item in this.Enumerate<Element>())
            {
                item.Canvas = value;
            }

            this.RequestUpdate();
        }
    }

    public Style Style
    {
        get => this.style;
        set => this.style.Update(value);
    }

    public string? Text
    {
        get
        {
            var builder = new StringBuilder();

            foreach (var item in this.Traverse<TextNode>(true))
            {
                builder.Append(item.Value);
            }

            return builder.ToString();
        }
        set
        {
            if (value != this.text)
            {
                if (this.FirstChild is TextNode textNode)
                {
                    if (textNode != this.LastChild)
                    {
                        if (textNode.NextSibling != null && this.LastChild != null)
                        {
                            this.RemoveChildrenInRange(textNode.NextSibling, this.LastChild);
                        }
                    }

                    textNode.Value = value;
                }
                else
                {
                    this.RemoveChildren();

                    this.AppendChild(new TextNode() { Value = value });
                }

                this.text = value;

                if (this.IsConnected)
                {
                    this.RequestUpdate();
                }
            }
        }
    }

    public override Transform2D Transform
    {
        get => this.styleTransform * this.dispositionTransform * base.Transform;
        set => this.LocalTransform = value * this.Transform.Inverse();
    }

    public Element() =>
        this.style.Changed += this.OnStyleChanged;

    IEnumerator<Element> IEnumerable<Element>.GetEnumerator()
    {
        for (var childElement = this.FirstElementChild; childElement != null; childElement = childElement.NextElementSibling)
        {
            yield return childElement;
        }
    }

    private void ApplyStyle() => this.DrawBorder();

    private void DrawBorder()
    {
        RectDrawCommand command;

        if (this.Commands.Count == 0)
        {
            command = new()
            {
                SampledTexture = new(
                    Container.Singleton.TextureStorage.DefaultTexture,
                    Container.Singleton.TextureStorage.DefaultSampler,
                    UVRect.Normalized
                ),
            };

            this.Commands.Add(command);
        }
        else
        {
            command = (RectDrawCommand)this.Commands[0];
        }

        command.Rect  = new(this.Size, default);
        command.Color = Color.Cyan;
    }

    private void RequestUpdate()
    {
        if (this.canvas != null && !this.hasPendingUpdate)
        {
            this.hasPendingUpdate = true;
            this.canvas.RequestUpdate(this);
            Container.Singleton.RenderingService.RequestDraw();
        }
    }

    private void OnStyleChanged()
    {
        this.UpdateStyleTransform();

        if (this.IsConnected)
        {
            this.RequestUpdate();
        }
    }

    private void UpdateStyleTransform() =>
        this.styleTransform = this.styleTransform with { Position = this.style.Position };

    protected override void OnAdopted()
    {
        if (this.Parent is Element parentElement)
        {
            this.style.Parent = parentElement.Style;
        }
    }

    protected override void OnChildAppended(Node child)
    {
        if (child is Element childElement)
        {
            if (this.LastElementChild != null)
            {
                this.LastElementChild.NextElementSibling = childElement;
                childElement.PreviousElementSibling = this.LastElementChild;

                this.LastElementChild = childElement;
            }
            else
            {
                this.FirstElementChild = this.LastElementChild = childElement;
            }

            childElement.Canvas = this.Canvas;
            this.RequestUpdate();
        }
    }

    protected override void OnChildRemoved(Node child)
    {
        if (child is Element elementChild)
        {
            if (elementChild == this.FirstElementChild)
            {
                this.FirstElementChild = elementChild.NextElementSibling;
            }

            if (elementChild == this.LastElementChild)
            {
                this.FirstElementChild = elementChild.PreviousElementSibling;
            }

            if (elementChild.PreviousElementSibling != null)
            {
                elementChild.PreviousElementSibling.NextElementSibling = elementChild.NextElementSibling;

                if (elementChild.NextElementSibling != null)
                {
                    elementChild.NextElementSibling.PreviousElementSibling = elementChild.PreviousElementSibling.NextElementSibling;
                }
            }
            else if (elementChild.NextElementSibling != null)
            {
                elementChild.NextElementSibling.PreviousElementSibling = null;
            }

            elementChild.PreviousElementSibling = null;
            elementChild.NextElementSibling     = null;
        }
    }

    protected override void OnBoundsChanged() =>
        this.ParentElement?.RequestUpdate();

    private void CalculateLayout()
    {
        if (this.calculated)
        {
            return;
        }

        var stackMode = this.Style.Stack;

        this.calculatedBaseLine = default;
        this.calculatedSize     = default;

        foreach (var child in this.Enumerate<Node2D>())
        {
            if (child is TextNode textNode)
            {
                textNode.Redraw();

                if (stackMode == StackMode.Horizontal)
                {
                    this.calculatedBaseLine = float.Min(this.calculatedBaseLine, textNode.BaseLine);
                }
            }

            if (child is Element element)
            {
                element.CalculateLayout();
                this.calculatedBaseLine = float.Min(this.calculatedBaseLine, element.calculatedBaseLine);
            }

            if (stackMode == StackMode.Horizontal)
            {
                this.calculatedSize.Height  = float.Max(this.calculatedSize.Height, child.Size.Height);
                this.calculatedSize.Width  += child.Size.Width;
            }
            else
            {
                this.calculatedSize.Height += child.Size.Height;
                this.calculatedSize.Width   = float.Max(this.calculatedSize.Width, child.Size.Width);
            }
        }
    }

    internal void UpdateLayout()
    {
        var previous = new Rect<float>();

        this.CalculateLayout();

        ref readonly var size    = ref this.calculatedSize;
        var proportionalBaseline = -this.calculatedBaseLine / this.calculatedSize.Height;

        foreach (var child in this.Enumerate<Node2D>())
        {
            if (child is TextNode textNode)
            {
                // textNode.Redraw();
                textNode.LocalTransform = textNode.LocalTransform with
                {
                    Position = new Vector2<float>(previous.Size.Width + previous.Position.X, -(size.Height - textNode.Size.Height) * proportionalBaseline)
                };

                previous = new(child.Size, child.LocalTransform.Position);
            }
            else if (child is Element element)
            {
                element.dispositionTransform = element.dispositionTransform with
                {
                    Position = new Vector2<float>(previous.Size.Width + previous.Position.X, -(size.Height - element.Size.Height) * proportionalBaseline)
                };

                element.ApplyStyle();

                previous = new(child.Size, element.dispositionTransform.Position);
            }
        }

        this.Size = size;

        this.ApplyStyle();

        this.hasPendingUpdate = false;
        this.calculated       = false;
    }
}
