using System.Text;
using Age.Numerics;
using Age.Rendering.Commands;
using Age.Rendering.Drawing.Elements;
using Age.Rendering.Drawing.Styling;

namespace Age.Rendering.Drawing;

public abstract class Element : ContainerNode, IEnumerable<Element>
{
    private readonly List<ContainerNode> nodesToDistribute = [];

    private Canvas?        canvas;
    private Vector2<float> offset;
    private Style          style = new();
    private Transform2D    styleTransform = new();
    private string?        text;

    private Vector2<float> StylePivot
    {
        get
        {
            if (this.Size == default)
            {
                return default;
            }

            var pivot = this.Style.Pivot ?? new();

            pivot = (pivot + 1) / 2;
            pivot.Y = 1 - pivot.Y;

            return pivot * this.Size.Cast<float>() * new Size<float>(1, -1);
        }
    }

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
            if (this.canvas != value)
            {
                this.canvas = value;

                foreach (var item in this.Enumerate<Element>())
                {
                    item.Canvas = value;
                }

                this.RequestUpdate();
            }
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

                if (this.Style.Stack == StackType.Vertical)
                {
                    builder.Append('\n');
                }
            }

            return builder.ToString().TrimEnd();
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

                this.RequestUpdate();
            }
        }
    }

    public override Transform2D Transform
    {
        get => base.Transform
            * Transform2D.Translated(this.offset)
            * Transform2D.Translated(this.StylePivot)
            * this.styleTransform
            * Transform2D.Translated(-this.StylePivot);
        set => this.LocalTransform = value * this.Transform.Inverse();
    }

    public bool HasPendingUpdate { get; private set; }

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

        foreach (var child in this.Enumerate<ContainerNode>())
        {
            if (child is TextNode textNode)
            {
                textNode.Draw();
            }

            var childStyle = (child as Element)?.Style;
            var margin     = childStyle?.Margin ?? new(0);
            var alignment  = childStyle?.Alignment ?? AlignmentType.BaseLine;
            var totalSize  = new Size<uint>(child.Size.Width + margin.Horizontal, child.Size.Height + margin.Vertical);

            if (stackMode == StackType.Horizontal)
            {
                if (childStyle?.Align == null && alignment == AlignmentType.BaseLine && totalSize.Height > hightest)
                {
                    this.Baseline = childStyle?.Margin == null
                        ? child.Baseline
                        : (margin.Top + child.Size.Height * child.Baseline) / totalSize.Height;

                    hightest = totalSize.Height;
                }

                contentSize.Height  = uint.Max(contentSize.Height, totalSize.Height);
                contentSize.Width  += totalSize.Width;
            }
            else
            {
                contentSize.Height += totalSize.Height;
                contentSize.Width   = uint.Max(contentSize.Width, totalSize.Width);
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

    private void OnStyleChanged()
    {
        this.UpdateStyleTransform();
        this.RequestUpdate();
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
        var size        = this.Size.Cast<float>();
        var contentSize = this.ContentSize;
        var stack       = this.Style.Stack ?? StackType.Horizontal;

        ContainerNode? lastChild = null;

        for (var i = 0; i < this.nodesToDistribute.Count; i++)
        {
            var reserved = (size - (Size<float>)offset) / (this.nodesToDistribute.Count - i);

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
                var isInline = !offsetScaleY.HasValue && !hasMargin;
                var canAlign = offsetScaleX.HasValue && size.Width > contentSize.Width;

                var x = canAlign ? Math.Max(0, reserved.Width - child.Size.Width - margin.Horizontal) * factorX : 0;
                var y = isInline
                    ? size.Height - size.Height * this.Baseline - child.Size.Height * (1 - child.Baseline)
                    : (size.Height - child.Size.Height - margin.Vertical) * factorY;

                position  = new(x + offset.X + margin.Left, -(y + margin.Top));
                usedSpace = canAlign
                    ? new(float.Max(child.Size.Width, reserved.Width - x), child.Size.Height)
                    : child.Size.Cast<float>();
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
            }

            if (child is Element element)
            {
                element.offset = position;
            }
            else
            {
                child.LocalTransform = child.LocalTransform with { Position = position };
            }

            offset.X = position.X + margin.Right   + usedSpace.Width;
            offset.Y = -position.Y + margin.Bottom + usedSpace.Height;

            lastChild = child;
        }

        if (stack == StackType.Vertical && lastChild != null)
        {
            this.Baseline = 1 - (offset.Y - lastChild.Size.Height * lastChild.Baseline) / this.Size.Height;
        }

        this.nodesToDistribute.Clear();
    }

    private void UpdateStyleTransform() =>
        this.styleTransform = this.styleTransform with
        {
            Position = this.style.Position ?? this.styleTransform.Position,
            Rotation = this.style.Rotation ?? this.styleTransform.Rotation,
        };

    internal protected virtual void RequestUpdate()
    {
        if (!this.HasPendingUpdate)
        {
            this.HasPendingUpdate = true;
            this.ParentElement?.RequestUpdate();
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

    protected override void OnTransformChanged() =>
        this.ParentElement?.RequestUpdate();

    internal virtual void UpdateLayout()
    {
        if (this.HasPendingUpdate)
        {
            foreach (var child in this)
            {
                (child as Element)?.UpdateLayout();
            }

            this.CalculateLayout();
            this.UpdateDisposition();

            this.HasPendingUpdate = false;
        }
    }
}
