using System.Text;
using Age.Numerics;
using Age.Rendering.Commands;
using Age.Rendering.Drawing.Elements;
using Age.Rendering.Drawing.Styling;

namespace Age.Rendering.Drawing;

public abstract class Element : Node2D, IEnumerable<Element>
{
    private readonly List<Node2D> nodesToDistribute = [];

    private Canvas?     canvas;
    private Transform2D dispositionTransform = new();
    private bool        hasPendingUpdate;
    private Style       style = new();
    private Transform2D styleTransform       = new();
    private string?     text;

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
        set
        {
            if (this.style != value)
            {
                this.style.Changed -= this.OnStyleChanged;
                this.style = value;
                this.style.Changed += this.OnStyleChanged;
                this.OnStyleChanged();
            }
        }
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

    private void CalculateLayout()
    {
        var stackMode = this.Style.Stack ?? StackType.Horizontal;

        var hightest = 0f;
        var size     = this.Style.Size ?? new Size<uint>();


        foreach (var child in this.Enumerate<Node2D>())
        {
            var margin = (child as Element)?.Style?.Margin ?? new(0);

            if (stackMode == StackType.Horizontal)
            {
                if (child is TextNode textNode)
                {
                    textNode.Draw();
                }

                if (child.Size.Height > hightest)
                {
                    this.Baseline = child.Baseline;

                    hightest = child.Size.Height;
                }

                if (!this.Style.Size.HasValue)
                {
                    size.Height  = uint.Max(size.Height, margin.Top + child.Size.Height + margin.Bottom);
                    size.Width  += margin.Left + child.Size.Width + margin.Right;
                }
            }
            else
            {
                if (!this.Style.Size.HasValue)
                {
                    size.Height += margin.Top + child.Size.Height + margin.Bottom;
                    size.Width   = uint.Max(size.Width, margin.Left + child.Size.Width + margin.Right);
                }
            }

            this.nodesToDistribute.Add(child);
        }

        this.Size = this.Style.MinSize.HasValue && this.Style.MaxSize.HasValue
            ? size.Range(this.Style.MinSize.Value, this.Style.MaxSize.Value)
            : this.Style.MinSize.HasValue
                ? size.Max(this.Style.MinSize.Value)
                : this.Style.MaxSize.HasValue
                    ? size.Min(this.Style.MaxSize.Value)
                    : size;

        this.ApplyStyle();
    }

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

        command.Rect  = new(this.Size.Cast<float>(), default);
        command.Color = this.Style.BorderColor ?? new(0.75f, 0.75f, 0.75f, 1);
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

    private void UpdateDisposition()
    {
        if (this.nodesToDistribute.Count == 0)
        {
            return;
        }

        static int? getXAlignment(AlignmentType? alignmentType) =>
            !alignmentType.HasValue
                ? null
                : alignmentType.Value.HasFlag(AlignmentType.Left)
                    ? -1
                    : alignmentType.Value.HasFlag(AlignmentType.Right)
                        ? 1
                        : alignmentType.Value.HasFlag(AlignmentType.Center)
                            ? 0
                            : null;

        static int? getYAlignment(AlignmentType? alignmentType) =>
            !alignmentType.HasValue
                ? null
                : alignmentType.Value.HasFlag(AlignmentType.Bottom)
                    ? -1
                    : alignmentType.Value.HasFlag(AlignmentType.Top)
                        ? 1
                        : alignmentType.Value.HasFlag(AlignmentType.Center)
                            ? 0
                            : null;

        static float normalize(float value) =>
            (1 + value) / 2;

        var previous = new Rect<int>();
        var size     = this.Size;
        var stack    = this.Style.Stack ?? StackType.Horizontal;

        foreach (var child in this.nodesToDistribute)
        {
            var childStyle = (child as Element)?.Style;

            var margin       = childStyle?.Margin ?? new();
            var offsetScaleX = childStyle?.Align?.X ?? getXAlignment(childStyle?.Alignment);
            var offsetScaleY = childStyle?.Align?.Y ?? getYAlignment(childStyle?.Alignment) ?? (stack == StackType.Horizontal ? this.Style.Baseline : null);

            Vector2<float> position;

            if (stack == StackType.Horizontal)
            {
                var offsetX  = previous.Position.X + previous.Size.Width;
                var isInline = !offsetScaleY.HasValue && childStyle?.Margin == null;

                var x = offsetX + (size.Width - child.Size.Width - offsetX - margin.Horizontal) * normalize(offsetScaleX ?? -1);
                var y = isInline
                    ? -(size.Height - child.Size.Height * child.Baseline - size.Height * (1 - this.Baseline))
                    : -((size.Height - child.Size.Height - margin.Vertical) * (1 - normalize(offsetScaleY ?? 1)));

                position = new(margin.Left + x, -margin.Top + y);
            }
            else
            {
                var offsetY = -previous.Position.Y + previous.Size.Height;

                var x = (size.Width - child.Size.Width - margin.Horizontal) * (1 - normalize(-offsetScaleX ?? 1));
                var y = -(offsetY + (size.Height - child.Size.Height - offsetY - margin.Vertical) * normalize(-offsetScaleY ?? -1));

                position = new(margin.Left + x, -margin.Top + y);
            }
            if (child is Element element)
            {
                element.dispositionTransform = element.dispositionTransform with { Position = position };
            }
            else
            {
                child.LocalTransform = child.LocalTransform with { Position = position };
            }

            previous = new(child.Size.Cast<int>(), position + new Vector2<float>(margin.Right, -margin.Bottom));
            // previous = new(child.Size.Cast<int>(), position);
        }

        this.nodesToDistribute.Clear();
    }

    private void UpdateStyleTransform() =>
        this.styleTransform = this.styleTransform with { Position = this.style.Position ?? default };

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

    internal virtual void UpdateLayout()
    {
        this.CalculateLayout();
        this.UpdateDisposition();

        this.hasPendingUpdate = false;
    }
}
