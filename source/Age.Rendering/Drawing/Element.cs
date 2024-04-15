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

        var hightest    = 0f;
        var contentSize = new Size<uint>();

        foreach (var child in this.Enumerate<Node2D>())
        {
            var childStyle = (child as Element)?.Style;
            var margin     = childStyle?.Margin ?? new(0);
            var alignment  = childStyle?.Alignment ?? AlignmentType.BaseLine;

            if (stackMode == StackType.Horizontal)
            {
                if (child is TextNode textNode)
                {
                    textNode.Draw();
                }

                if (childStyle?.Align == null && alignment == AlignmentType.BaseLine && child.Size.Height > hightest)
                {
                    this.Baseline = child.Baseline;

                    hightest = child.Size.Height;
                }

                contentSize.Height  = uint.Max(contentSize.Height, margin.Top + child.Size.Height + margin.Bottom);
                contentSize.Width  += margin.Left + child.Size.Width + margin.Right;
            }
            else
            {
                contentSize.Height += margin.Top + child.Size.Height + margin.Bottom;
                contentSize.Width   = uint.Max(contentSize.Width, margin.Left + child.Size.Width + margin.Right);
            }

            this.nodesToDistribute.Add(child);
        }

        this.ContentSize = contentSize;
        this.Size        = this.Style.MinSize.HasValue && this.Style.MaxSize.HasValue
            ? contentSize.Range(this.Style.MinSize.Value, this.Style.MaxSize.Value)
            : this.Style.MinSize.HasValue
                ? contentSize.Max(this.Style.MinSize.Value)
                : this.Style.MaxSize.HasValue
                    ? contentSize.Min(this.Style.MaxSize.Value)
                    :  this.Style.Size ?? contentSize;

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

        var offset      = new Point<float>();
        var size        = this.Size;
        var contentSize = this.ContentSize;
        var stack       = this.Style.Stack ?? StackType.Horizontal;

        var reserved = size.Cast<float>() / this.nodesToDistribute.Count;

        for (var i = 0; i < this.nodesToDistribute.Count; i++)
        {
            var child = this.nodesToDistribute[i];
            var childStyle = (child as Element)?.Style;

            var margin       = childStyle?.Margin ?? new();
            var hasMargin    = childStyle?.Margin != null;
            var offsetScaleX = childStyle?.Align?.X ?? getXAlignment(childStyle?.Alignment);
            var offsetScaleY = childStyle?.Align?.Y ?? getYAlignment(childStyle?.Alignment) ?? (stack == StackType.Horizontal ? this.Style.Baseline : null);

            Vector2<float> position;
            Size<float>    usedSpace;

            if (stack == StackType.Horizontal)
            {
                var factorX  = normalize(offsetScaleX ?? -1);
                var factorY  = 1 - normalize(offsetScaleY ?? (hasMargin ? 0 : 1));
                var isInline = !offsetScaleY.HasValue && !hasMargin && (childStyle?.Stack ?? StackType.Horizontal) == StackType.Horizontal;
                var canAlign = offsetScaleX.HasValue && size.Width > contentSize.Width;

                var x = canAlign ? Math.Max(0, reserved.Width - child.Size.Width - margin.Horizontal) * factorX : 0;
                var y = isInline
                    ? size.Height - child.Size.Height * child.Baseline - size.Height * (1 - this.Baseline)
                    : (size.Height - child.Size.Height - margin.Vertical) * factorY;

                position  = new(x + offset.X + margin.Left, -(y + margin.Top));
                usedSpace = canAlign
                    ? new(float.Max(child.Size.Width, reserved.Width - x), child.Size.Height)
                    : child.Size.Cast<float>();

                if (usedSpace.Width > reserved.Width)
                {
                    reserved.Width = (size.Width - usedSpace.Width - offset.X) / (this.nodesToDistribute.Count - (i + 1));
                }
            }
            else
            {
                var factorX  = 1 - normalize(-offsetScaleX ?? (hasMargin ? 0 : 1));
                var factorY  = normalize(-offsetScaleY ?? -1);
                var canAlign = offsetScaleY.HasValue && size.Height > contentSize.Height;

                var x = (size.Width - child.Size.Width - margin.Horizontal) * factorX;
                var y = canAlign ? Math.Max(0, reserved.Height - child.Size.Height - margin.Vertical) * factorY : 0;

                position  = new(margin.Left + x, -(y + offset.Y + margin.Top));
                usedSpace = canAlign
                    ? new(child.Size.Width, float.Max(child.Size.Height, reserved.Height - y))
                    : child.Size.Cast<float>();

                if (usedSpace.Height > reserved.Height)
                {
                    reserved.Height = (size.Height - usedSpace.Height - offset.Y) / (this.nodesToDistribute.Count - (i + 1));
                }
            }

            if (child is Element element)
            {
                element.dispositionTransform = element.dispositionTransform with { Position = position };
            }
            else
            {
                child.LocalTransform = child.LocalTransform with { Position = position };
            }

            offset.X = position.X + margin.Right   + usedSpace.Width;
            offset.Y = -position.Y + margin.Bottom + usedSpace.Height;
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
