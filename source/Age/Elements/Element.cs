using System.Text;
using Age.Commands;
using Age.Core;
using Age.Numerics;
using Age.Scene;
using Age.Storage;
using Age.Styling;

using static Age.Rendering.Shaders.Canvas.CanvasShader;

namespace Age.Elements;

public delegate void ContextEventHandler(in ContextEvent mouseEvent);
public delegate void MouseEventHandler(in MouseEvent mouseEvent);

public abstract partial class Element : ContainerNode, IEnumerable<Element>
{
    public event MouseEventHandler?   Blured;
    public event MouseEventHandler?   Clicked;
    public event ContextEventHandler? Context;
    public event MouseEventHandler?   Focused;
    public event MouseEventHandler?   MouseMoved;
    public event MouseEventHandler?   MouseOut;
    public event MouseEventHandler?   MouseOver;

    private LayoutInfo layoutInfo = new();

    private Canvas?                 canvas;
    private Vector2<float>          offset;
    private Style                   style = new();
    private Transform2D             styleTransform = new();
    private string?                 text;
    private CacheValue<Transform2D> transformCache;

    private Transform2D PivotedTransform =>
        Transform2D.Translated(this.offset)
            * Transform2D.Translated(this.StylePivot)
            * this.styleTransform
            * Transform2D.Translated(-this.StylePivot);

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

    internal protected override Transform2D TransformCache
    {
        get
        {
            if (this.transformCache.Version != CacheVersion)
            {
                this.transformCache = new()
                {
                    Value = base.TransformCache * this.PivotedTransform,
                    Version = CacheVersion
                };
            }

            return this.transformCache.Value;
        }
    }

    internal bool HasPendingUpdate { get; set; }

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

                foreach (var node in this.Traverse())
                {
                    if (node is Element element)
                    {
                        element.Canvas = value;
                    }
                }

                this.RequestUpdate();
            }
        }
    }

    public bool IsFocused { get; internal set; }

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

            foreach (var node in this.Traverse())
            {
                if (node is TextNode textNode)
                {
                    builder.Append(textNode.Value);

                    if (this.Style.Stack == StackType.Vertical)
                    {
                        builder.Append('\n');
                    }
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
        get => base.Transform * this.PivotedTransform;
        set => this.LocalTransform = value * this.Transform.Inverse();
    }

    public Element() =>
        this.style.Changed += this.OnStyleChanged;

    ~Element() =>
        this.style.Changed -= this.OnStyleChanged;

    private static void CalculatePendingPaddingHorizontal(Element element, in Size<uint> size)
    {
        if (element.Style.Padding?.Left?.TryGetPercentage(out var left) ?? false)
        {
            element.layoutInfo.Padding.Left = (uint)(size.Width * left);
        }

        if (element.Style.Padding?.Right?.TryGetPercentage(out var right) ?? false)
        {
            element.layoutInfo.Padding.Right = (uint)(size.Width * right);
        }
    }

    private static void CalculatePendingPaddingVertical(Element element, in Size<uint> size)
    {
        if (element.Style.Padding?.Top?.TryGetPercentage(out var top) ?? false)
        {
            element.layoutInfo.Padding.Top = (uint)(size.Width * top);
        }

        if (element.Style.Padding?.Bottom?.TryGetPercentage(out var bottom) ?? false)
        {
            element.layoutInfo.Padding.Bottom = (uint)(size.Width * bottom);
        }
    }

    private static void CalculatePendingMarginHorizontal(Element element, StackType stackType, in Size<uint> size, ref Size<uint> contentSize)
    {
        var horizontal = 0u;

        if (element.Style.Margin?.Left?.TryGetPercentage(out var left) ?? false)
        {
            element.layoutInfo.Margin.Left = (uint)(size.Width * left);

            horizontal += element.layoutInfo.Margin.Left;
        }

        if (element.Style.Margin?.Right?.TryGetPercentage(out var right) ?? false)
        {
            element.layoutInfo.Margin.Right = (uint)(size.Width * right);

            horizontal += element.layoutInfo.Margin.Right;
        }

        if (horizontal > 0)
        {
            if (stackType == StackType.Horizontal)
            {
                contentSize.Width += horizontal;
            }
            else
            {
                contentSize.Width = uint.Max(element.layoutInfo.Size.Width + element.layoutInfo.Border.Horizontal + horizontal, contentSize.Width);
            }
        }
    }

    private static void CalculatePendingMarginVertical(Element element, StackType stackType, in Size<uint> size, ref Size<uint> contentSize)
    {
        var vertical = 0u;

        if (element.Style.Margin?.Top?.TryGetPercentage(out var top) ?? false)
        {
            element.layoutInfo.Margin.Top = (uint)(size.Height * top);

            vertical += element.layoutInfo.Margin.Top;
        }

        if (element.Style.Margin?.Bottom?.TryGetPercentage(out var bottom) ?? false)
        {
            element.layoutInfo.Margin.Bottom = (uint)(size.Height * bottom);

            vertical += element.layoutInfo.Margin.Bottom;
        }

        if (vertical > 0)
        {
            if (stackType == StackType.Vertical)
            {
                contentSize.Height += vertical;
            }
            else
            {
                contentSize.Height = uint.Max(element.layoutInfo.Size.Height + element.layoutInfo.Border.Vertical + vertical, contentSize.Height);
            }
        }
    }

    private static int? GetYAlignment(AlignmentType? alignmentType) =>
        !alignmentType.HasValue
            ? null
            : alignmentType.Value.HasFlag(AlignmentType.Bottom)
                ? -1
                : alignmentType.Value.HasFlag(AlignmentType.Top)
                    ? 1
                    : alignmentType.Value.HasFlag(AlignmentType.Center)
                        ? 0
                        : null;

    private static int? GetXAlignment(AlignmentType? alignmentType) =>
        !alignmentType.HasValue
            ? null
            : alignmentType.Value.HasFlag(AlignmentType.Left)
                ? -1
                : alignmentType.Value.HasFlag(AlignmentType.Right)
                    ? 1
                    : alignmentType.Value.HasFlag(AlignmentType.Center)
                        ? 0
                        : null;

    private static float Normalize(float value) =>
        (1 + value) / 2;

    IEnumerator<Element> IEnumerable<Element>.GetEnumerator()
    {
        for (var childElement = this.FirstElementChild; childElement != null; childElement = childElement.NextElementSibling)
        {
            yield return childElement;
        }
    }

    private void CalculateLayout()
    {
        var stackMode   = this.Style.Stack ?? StackType.Horizontal;
        var contentSize = new Size<uint>();

        this.layoutInfo.HightestChild = 0;

        foreach (var node in this)
        {
            if (node is ContainerNode child)
            {
                var childSize = new Size<uint>();

                if (node is TextNode textNode)
                {
                    textNode.Draw();

                    childSize = textNode.Size;
                }
                else if (child is Element element)
                {
                    element.UpdateLayout();

                    childSize.Width  = element.layoutInfo.Margin.Horizontal;
                    childSize.Height = element.layoutInfo.Margin.Vertical;

                    if (this.layoutInfo.ContentDependent.HasFlag(Dependency.Width) || !element.layoutInfo.ParentDependent.HasFlag(Dependency.Width))
                    {
                        childSize.Width += element.layoutInfo.Border.Horizontal;
                    }

                    if (!element.layoutInfo.ParentDependent.HasFlag(Dependency.Width))
                    {
                        childSize.Width += element.layoutInfo.Size.Width;
                    }

                    if (this.layoutInfo.ContentDependent.HasFlag(Dependency.Height) || !element.layoutInfo.ParentDependent.HasFlag(Dependency.Height))
                    {
                        childSize.Height += element.layoutInfo.Border.Vertical;
                    }

                    if (!element.layoutInfo.ParentDependent.HasFlag(Dependency.Height))
                    {
                        childSize.Height += element.layoutInfo.Size.Height;
                    }
                }

                if (stackMode == StackType.Horizontal)
                {
                    contentSize.Width += childSize.Width;
                    contentSize.Height = uint.Max(contentSize.Height, childSize.Height);

                    this.UpdateBaseline(child);
                }
                else
                {
                    contentSize.Width = uint.Max(contentSize.Width, childSize.Width);
                    contentSize.Height += childSize.Height;
                }
            }
        }

        if (this.layoutInfo.ContentDependent.HasFlag(Dependency.Width) || this.layoutInfo.ContentDependent.HasFlag(Dependency.Height))
        {
            this.CalculatePendingMargin(ref contentSize);
        }

        this.layoutInfo.ContentStaticSize = contentSize;

        var size = this.layoutInfo.Size;

        if (!this.layoutInfo.ParentDependent.HasFlag(Dependency.Width))
        {
            size.Width = contentSize.Width;
        }

        if (!this.layoutInfo.ParentDependent.HasFlag(Dependency.Height))
        {
            size.Height = contentSize.Height;
        }

        var resolvedMargin  = !this.layoutInfo.ParentDependent.HasFlag(Dependency.Margin);
        var resolvedPadding = !this.layoutInfo.ParentDependent.HasFlag(Dependency.Padding);
        var resolvedWidth   = true;
        var resolvedHeight  = true;

        if (this.Style.Padding?.Top?.TryGetPixel(out var top) ?? false)
        {
            this.layoutInfo.Padding.Top = top;
        }

        if (this.Style.Padding?.Right?.TryGetPixel(out var right) ?? false)
        {
            this.layoutInfo.Padding.Right = right;
        }

        if (this.Style.Padding?.Bottom?.TryGetPixel(out var bottom) ?? false)
        {
            this.layoutInfo.Padding.Bottom = bottom;
        }

        if (this.Style.Padding?.Left?.TryGetPixel(out var left) ?? false)
        {
            this.layoutInfo.Padding.Left = left;
        }

        if (this.Style.Margin?.Top?.TryGetPixel(out top) ?? false)
        {
            this.layoutInfo.Margin.Top = top;
        }

        if (this.Style.Margin?.Right?.TryGetPixel(out right) ?? false)
        {
            this.layoutInfo.Margin.Right = right;
        }

        if (this.Style.Margin?.Bottom?.TryGetPixel(out bottom) ?? false)
        {
            this.layoutInfo.Margin.Bottom = bottom;
        }

        if (this.Style.Margin?.Left?.TryGetPixel(out left) ?? false)
        {
            this.layoutInfo.Margin.Left = left;
        }

        if (!this.layoutInfo.ContentDependent.HasFlag(Dependency.Width))
        {
            if (this.Style.Size?.Width?.TryGetPixel(out var pixel) ?? false)
            {
                size.Width = pixel;
            }
            else if ((this.Style.MinSize?.Width?.TryGetPixel(out var min) ?? false) && (this.Style.MaxSize?.Width?.TryGetPixel(out var max) ?? false))
            {
                size.Width = uint.Max(uint.Min(size.Width, min), max);
            }
            else if (this.Style.MinSize?.Width?.TryGetPixel(out min) ?? false)
            {
                size.Width = uint.Max(size.Width, min);
            }
            else if (this.Style.MaxSize?.Width?.TryGetPixel(out max) ?? false)
            {
                size.Width = uint.Max(size.Width, max);
            }
            else
            {
                resolvedWidth = false;
            }
        }

        if (!this.layoutInfo.ContentDependent.HasFlag(Dependency.Height))
        {
            if (this.Style.Size?.Height?.TryGetPixel(out var pixel) ?? false)
            {
                size.Height = pixel;
            }
            else if ((this.Style.MinSize?.Height?.TryGetPixel(out var min) ?? false) && (this.Style.MaxSize?.Height?.TryGetPixel(out var max) ?? false))
            {
                size.Height = uint.Max(uint.Min(size.Height, min), max);
            }
            else if (this.Style.MinSize?.Height?.TryGetPixel(out min) ?? false)
            {
                size.Height = uint.Max(size.Height, min);
            }
            else if (this.Style.MaxSize?.Height?.TryGetPixel(out max) ?? false)
            {
                size.Height = uint.Max(size.Height, max);
            }
            else
            {
                resolvedHeight = false;
            }
        }

        if (this.Style.BoxSizing == BoxSizing.Border)
        {
            if (!this.layoutInfo.ContentDependent.HasFlag(Dependency.Width))
            {
                size.Width -= this.layoutInfo.Border.Horizontal;
            }

            if (!this.layoutInfo.ContentDependent.HasFlag(Dependency.Height))
            {
                size.Height -= this.layoutInfo.Border.Vertical;
            }
        }

        this.layoutInfo.AvaliableSpace = size - contentSize;

        if (this.layoutInfo.Size != size)
        {
            this.layoutInfo.Size = size;

            if (resolvedWidth && resolvedHeight && resolvedMargin && resolvedPadding)
            {
                this.CalculatePendingLayouts();
            }
            else
            {
                this.ParentElement?.RequestUpdate();
            }
        }

        this.Size = this.layoutInfo.Size
            + new Size<uint>(this.layoutInfo.Padding.Horizontal, this.layoutInfo.Padding.Vertical)
            + new Size<uint>(this.layoutInfo.Border.Horizontal, this.layoutInfo.Border.Vertical);
    }

    private void CalculatePendingMargin(ref Size<uint> size)
    {
        var contentSize = size;

        var stackType = this.Style.Stack ?? StackType.Horizontal;

        foreach (var child in this.layoutInfo.Dependents)
        {
            if (child.layoutInfo.ParentDependent.HasFlag(Dependency.Padding) || child.layoutInfo.ParentDependent.HasFlag(Dependency.Margin))
            {
                if (!this.layoutInfo.ParentDependent.HasFlag(Dependency.Width))
                {
                    CalculatePendingMarginHorizontal(child, stackType, size, ref contentSize);
                }

                if (!this.layoutInfo.ParentDependent.HasFlag(Dependency.Height))
                {
                    CalculatePendingMarginVertical(child, stackType, size, ref contentSize);
                }
            }
        }

        size = contentSize;
    }

    private void CalculatePendingPadding(in Size<uint> size)
    {
        foreach (var child in this.layoutInfo.Dependents)
        {
            if (child.layoutInfo.ParentDependent.HasFlag(Dependency.Padding) || child.layoutInfo.ParentDependent.HasFlag(Dependency.Margin))
            {
                if (!this.layoutInfo.ParentDependent.HasFlag(Dependency.Width))
                {
                    CalculatePendingPaddingHorizontal(child, size);
                }

                if (!this.layoutInfo.ParentDependent.HasFlag(Dependency.Height))
                {
                    CalculatePendingPaddingVertical(child, size);
                }
            }
        }
    }

    private void CalculatePendingLayouts()
    {
        var contentDynamicSize = new Size<uint>();

        var stackType = this.Style.Stack ?? StackType.Horizontal;

        foreach (var child in this.layoutInfo.Dependents)
        {
            var size = child.layoutInfo.Size;

            if (!this.layoutInfo.ContentDependent.HasFlag(Dependency.Width))
            {
                CalculatePendingPaddingHorizontal(child, this.layoutInfo.Size);
                CalculatePendingMarginHorizontal(child, stackType, this.layoutInfo.Size, ref contentDynamicSize);

                if (child.layoutInfo.ParentDependent.HasFlag(Dependency.Width))
                {
                    if (child.Style.Size?.Width?.TryGetPercentage(out var percentage) ?? false)
                    {
                        size.Width = (uint)(this.layoutInfo.Size.Width * percentage);
                    }
                    else if ((child.Style.MinSize?.Width?.TryGetPercentage(out var min) ?? false) && (child.Style.MaxSize?.Width?.TryGetPercentage(out var max) ?? false))
                    {
                        size.Width = uint.Max(uint.Min(this.layoutInfo.Size.Width, (uint)(this.layoutInfo.Size.Width * min)), (uint)(this.layoutInfo.Size.Width * max));
                    }
                    else if (child.Style.MinSize?.Width?.TryGetPercentage(out min) ?? false)
                    {
                        size.Width = uint.Min(this.layoutInfo.Size.Width, (uint)(this.layoutInfo.Size.Width * min));
                    }
                    else if (child.Style.MaxSize?.Width?.TryGetPercentage(out max) ?? false)
                    {
                        size.Width = uint.Max(this.layoutInfo.Size.Width, (uint)(this.layoutInfo.Size.Width * max));
                    }

                    size.Width -= child.layoutInfo.Padding.Horizontal;

                    if (stackType == StackType.Horizontal)
                    {
                        if (size.Width < this.layoutInfo.AvaliableSpace.Width)
                        {
                            this.layoutInfo.AvaliableSpace.Width -= size.Width;
                        }
                        else
                        {
                            size.Width = this.layoutInfo.AvaliableSpace.Width;

                            this.layoutInfo.AvaliableSpace.Width = 0;
                        }

                        contentDynamicSize.Width += size.Width;
                    }
                    else
                    {
                        contentDynamicSize.Width = uint.Max(size.Width, contentDynamicSize.Width);
                    }

                    size.Width -= child.layoutInfo.Border.Horizontal;
                }
            }

            if (!this.layoutInfo.ContentDependent.HasFlag(Dependency.Height))
            {
                CalculatePendingPaddingVertical(child, this.layoutInfo.Size);
                CalculatePendingMarginVertical(child, stackType, this.layoutInfo.Size, ref contentDynamicSize);

                if (child.layoutInfo.ParentDependent.HasFlag(Dependency.Height))
                {
                    if (child.Style.Size?.Height?.TryGetPercentage(out var percentage) ?? false)
                    {
                        size.Height = (uint)(this.layoutInfo.Size.Height * percentage);
                    }
                    else if ((child.Style.MinSize?.Height?.TryGetPercentage(out var min) ?? false) && (child.Style.MaxSize?.Height?.TryGetPercentage(out var max) ?? false))
                    {
                        size.Height = uint.Max(uint.Min(this.layoutInfo.Size.Height, (uint)(this.layoutInfo.Size.Height * min)), (uint)(this.layoutInfo.Size.Height * max));
                    }
                    else if (child.Style.MinSize?.Height?.TryGetPercentage(out min) ?? false)
                    {
                        size.Height = uint.Min(this.layoutInfo.Size.Height, (uint)(this.layoutInfo.Size.Height * min));
                    }
                    else if (child.Style.MaxSize?.Height?.TryGetPercentage(out max) ?? false)
                    {
                        size.Height = uint.Max(this.layoutInfo.Size.Height, (uint)(this.layoutInfo.Size.Height * max));
                    }

                    size.Height -= child.layoutInfo.Padding.Vertical;

                    if (stackType == StackType.Vertical)
                    {
                        if (size.Height < this.layoutInfo.AvaliableSpace.Height)
                        {
                            this.layoutInfo.AvaliableSpace.Height -= size.Height;
                        }
                        else
                        {
                            size.Height = this.layoutInfo.AvaliableSpace.Height;

                            this.layoutInfo.AvaliableSpace.Height = 0;
                        }

                        contentDynamicSize.Height += size.Height;
                    }
                    else
                    {
                        contentDynamicSize.Height = uint.Max(size.Height, contentDynamicSize.Height);
                    }

                    size.Height -= child.layoutInfo.Border.Vertical;
                }
            }

            if (size != child.layoutInfo.Size)
            {
                child.layoutInfo.AvaliableSpace.Width = size.Width > child.layoutInfo.ContentStaticSize.Width
                    ? size.Width - child.layoutInfo.ContentStaticSize.Width
                    : 0;

                child.layoutInfo.AvaliableSpace.Height = size.Height > child.layoutInfo.ContentStaticSize.Height
                    ? size.Height - child.layoutInfo.ContentStaticSize.Height
                    : 0;

                Console.WriteLine($"Element: {child.Name}, Size: {size}, Border: {child.layoutInfo.Border} :: [{this.Name}].Size: {this.layoutInfo.Size}");

                child.layoutInfo.Size = size;

                child.CalculatePendingLayouts();
            }

            child.Size = child.layoutInfo.TotalSize;

            child.UpdateDisposition();
            child.HasPendingUpdate = false;

            this.UpdateBaseline(child);
        }

        this.layoutInfo.ContentDynamicSize += contentDynamicSize;
    }

    private void OnStyleChanged()
    {
        this.UpdateStyleTransform();
        this.RequestUpdate();
        this.UpdateLayoutInfo();


    }

    private void UpdateBaseline(ContainerNode child)
    {
        Style?    style  = null;
        RectEdges margin = default;

        if (child is Element element)
        {
            style  = element.Style;
            margin = element.layoutInfo.Margin;
        }

        var alignment = style?.Alignment ?? AlignmentType.BaseLine;
        var totalSize = new Size<uint>(child.Size.Width + margin.Horizontal, child.Size.Height + margin.Vertical);

        if (style?.Align == null && alignment == AlignmentType.BaseLine && totalSize.Height > this.layoutInfo.HightestChild)
        {
            this.Baseline = style?.Margin == null ? child.Baseline : (margin.Top + child.Size.Height * child.Baseline) / totalSize.Height;

            this.layoutInfo.HightestChild = totalSize.Height;
        }
    }

    private void UpdateDisposition()
    {
        if (this.layoutInfo.RenderableNodesCount == 0)
        {
            return;
        }

        var offset      = new Point<float>();
        var size        = this.layoutInfo.Size.Cast<float>();
        var contentSize = new Size<uint>();
        var stack       = this.Style.Stack ?? StackType.Horizontal;

        if (stack == StackType.Horizontal)
        {
            contentSize.Width  = this.layoutInfo.ContentStaticSize.Width + this.layoutInfo.ContentDynamicSize.Width;
            contentSize.Height = uint.Max(this.layoutInfo.ContentStaticSize.Height, this.layoutInfo.ContentDynamicSize.Height);
        }
        else
        {
            contentSize.Width  = uint.Max(this.layoutInfo.ContentStaticSize.Width, this.layoutInfo.ContentDynamicSize.Width);
            contentSize.Height = this.layoutInfo.ContentStaticSize.Height + this.layoutInfo.ContentDynamicSize.Height;
        }

        offset.X += this.layoutInfo.Border.Left + this.layoutInfo.Padding.Left;
        offset.Y -= this.layoutInfo.Border.Top  + this.layoutInfo.Padding.Top;

        ContainerNode? lastChild = null;

        var i = 0;

        foreach (var node in this)
        {
            if (node is not ContainerNode child)
            {
                continue;
            }

            var reserved   = size / (this.layoutInfo.RenderableNodesCount - i);

            RectEdges border     = default;
            Style?    childStyle = null;
            RectEdges margin     = default;

            if (child is Element element)
            {
                childStyle = element.Style;
                margin     = element.layoutInfo.Margin;
                border     = element.layoutInfo.Border;
            }

            var hasMargin    = childStyle?.Margin != null;
            var offsetScaleX = childStyle?.Align?.X ?? GetXAlignment(childStyle?.Alignment);
            var offsetScaleY = childStyle?.Align?.Y ?? GetYAlignment(childStyle?.Alignment) ?? (stack == StackType.Horizontal ? this.Style.Baseline : null);

            Vector2<float> position;
            Size<float>    usedSpace;

            if (stack == StackType.Horizontal)
            {
                var factorX  = Normalize(offsetScaleX ?? -1);
                var factorY  = 1 - Normalize(offsetScaleY ?? (hasMargin ? 0 : 1));
                var isInline = !offsetScaleY.HasValue && !hasMargin;
                var canAlign = offsetScaleX.HasValue && size.Width > contentSize.Width;

                var x = canAlign ? (float)(Math.Max(0, reserved.Width - child.Size.Width - margin.Horizontal - border.Horizontal) * factorX) : 0;
                var y = isInline
                    ? (float)(size.Height - size.Height * this.Baseline - child.Size.Height * (1 - child.Baseline))
                    : (float)((size.Height - child.Size.Height - margin.Vertical) * factorY);

                usedSpace = canAlign ? new(float.Max(child.Size.Width, reserved.Width - x), child.Size.Height) : child.Size.Cast<float>();

                position = new(float.Ceiling(x + offset.X + margin.Left), -float.Ceiling(y - offset.Y - -margin.Top));

                offset.X = position.X + margin.Right + usedSpace.Width;
            }
            else
            {
                var factorX  = 1 - Normalize(-(offsetScaleX ?? (hasMargin ? 0 : -1)));
                var factorY  = Normalize(-(offsetScaleY ?? 1));
                var canAlign = offsetScaleY.HasValue && size.Height > contentSize.Height;

                var x = (size.Width - child.Size.Width - margin.Horizontal) * factorX;
                var y = 0f;

                if (canAlign)
                {
                    y = (float)(Math.Max(0, reserved.Height - child.Size.Height - margin.Vertical - border.Vertical) * factorY);
                }

                usedSpace = canAlign ? new(child.Size.Width, float.Max(child.Size.Height, reserved.Height - y)) : child.Size.Cast<float>();

                position  = new(float.Ceiling(x + offset.X + margin.Left), -float.Ceiling(y - offset.Y - -margin.Top));

                offset.Y = position.Y + -(margin.Bottom + usedSpace.Height);
            }

            if (child is Element element1) // TODO - Removes multiples castas
            {
                element1.offset = position;
            }
            else
            {
                child.LocalTransform = child.LocalTransform with { Position = position };
            }

            lastChild = child;
        }

        if (stack == StackType.Vertical && lastChild != null)
        {
            // TODO - Analyse use case
            // this.Baseline = 1 - (offset.Y - lastChild.Size.Height * lastChild.Baseline) / this.Size.Height;
        }
    }

    private void UpdateLayoutInfo()
    {
        this.layoutInfo.Border = new ()
        {
            Top    = this.Style.Border?.Top.Thickness ?? 0,
            Right  = this.Style.Border?.Right.Thickness ?? 0,
            Bottom = this.Style.Border?.Bottom.Thickness ?? 0,
            Left   = this.Style.Border?.Left.Thickness ?? 0,
        };

        this.layoutInfo.ContentDependent = Dependency.None;
        this.layoutInfo.ParentDependent  = Dependency.None;

        if (this.Style.Size?.Width == null && this.Style.MinSize?.Width == null && this.Style.MaxSize?.Width == null)
        {
            this.layoutInfo.ContentDependent |= Dependency.Width;
        }
        else if (this.Style.Size?.Width?.Kind == UnitKind.Percentage || this.Style.MinSize?.Width?.Kind == UnitKind.Percentage || this.Style.MaxSize?.Width?.Kind == UnitKind.Percentage)
        {
            this.layoutInfo.ParentDependent |= Dependency.Width;
        }

        if (this.Style.Size?.Height == null && this.Style.MinSize?.Height == null && this.Style.MaxSize?.Height == null)
        {
            this.layoutInfo.ContentDependent |= Dependency.Height;
        }
        else if (this.Style.Size?.Height?.Kind == UnitKind.Percentage || this.Style.MinSize?.Height?.Kind == UnitKind.Percentage || this.Style.MaxSize?.Height?.Kind == UnitKind.Percentage)
        {
            this.layoutInfo.ParentDependent |= Dependency.Height;
        }

        if (this.Style.Margin?.Top?.Kind == UnitKind.Percentage || this.Style.Margin?.Right?.Kind == UnitKind.Percentage || this.Style.Margin?.Bottom?.Kind == UnitKind.Percentage || this.Style.Margin?.Left?.Kind == UnitKind.Percentage)
        {
            this.layoutInfo.ParentDependent |= Dependency.Margin;
        }

        if (this.Style.Padding?.Top?.Kind == UnitKind.Percentage || this.Style.Padding?.Right?.Kind == UnitKind.Percentage || this.Style.Padding?.Bottom?.Kind == UnitKind.Percentage || this.Style.Padding?.Left?.Kind == UnitKind.Percentage)
        {
            this.layoutInfo.ParentDependent |= Dependency.Padding;
        }

        if (this.ParentElement != null)
        {
            if (this.layoutInfo.ParentDependent != Dependency.None)
            {
                this.ParentElement.layoutInfo.Dependents.Add(this);
            }
            else
            {
                this.ParentElement.layoutInfo.Dependents.Remove(this);
            }
        }
    }

    private void UpdateRect()
    {
        if (this.SingleCommand is not RectCommand command)
        {
            this.SingleCommand = command = new()
            {
                Flags = Flags.ColorAsBackground,
                SampledTexture = new(
                    TextureStorage.Singleton.DefaultTexture,
                    TextureStorage.Singleton.DefaultSampler,
                    UVRect.Normalized
                ),
            };
        }

        command.ObjectId = this.Style.Border.HasValue || this.Style.BackgroundColor.HasValue ? (uint)(this.Index + 1) : 0;
        command.Rect     = new(this.Size.Cast<float>(), default);
        command.Border   = this.Style.Border ?? default;
        command.Color    = this.Style.BackgroundColor ?? default;
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

    protected override void ChildAppended(Node child)
    {
        if (child is ContainerNode containerNode)
        {
            if (containerNode is Element childElement)
            {
                childElement.Canvas = this is Canvas canvas ? canvas : this.Canvas;

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

                if (childElement.layoutInfo.ParentDependent != Dependency.None)
                {
                    this.layoutInfo.Dependents.Add(childElement);
                }
            }

            this.layoutInfo.RenderableNodesCount++;
            this.RequestUpdate();
        }
    }

    protected override void ChildRemoved(Node child)
    {
        if (child is ContainerNode containerNode)
        {
            if (containerNode is Element childElement)
            {
                childElement.Canvas = null;

                if (childElement == this.FirstElementChild)
                {
                    this.FirstElementChild = childElement.NextElementSibling;
                }

                if (childElement == this.LastElementChild)
                {
                    this.LastElementChild = childElement.PreviousElementSibling;
                }

                if (childElement.PreviousElementSibling != null)
                {
                    childElement.PreviousElementSibling.NextElementSibling = childElement.NextElementSibling;

                    if (childElement.NextElementSibling != null)
                    {
                        childElement.NextElementSibling.PreviousElementSibling = childElement.PreviousElementSibling.NextElementSibling;
                    }
                }
                else if (childElement.NextElementSibling != null)
                {
                    childElement.NextElementSibling.PreviousElementSibling = null;
                }

                childElement.PreviousElementSibling = null;
                childElement.NextElementSibling = null;

                this.layoutInfo.Dependents.Remove(childElement);
            }

            this.layoutInfo.RenderableNodesCount--;
            this.RequestUpdate();

        }
    }

    protected override void SizeChanged()
    {
        this.ParentElement?.RequestUpdate();
        // Console.WriteLine($"Element: {this.Name}, Size: {this.Size}, LayoutInfo.Size: {this.layoutInfo.Size}, LayoutInfo.Border: {this.layoutInfo.Border},");

        this.UpdateRect();
    }

    internal void InvokeBlur(in MouseEvent mouseEvent)
    {
        this.IsFocused = false;
        this.Blured?.Invoke(mouseEvent);
    }

    internal void InvokeClick(in MouseEvent mouseEvent) =>
        this.Clicked?.Invoke(mouseEvent);

    internal void InvokeContext(in ContextEvent contextEvent) =>
        this.Context?.Invoke(contextEvent);

    internal void InvokeFocus(in MouseEvent mouseEvent)
    {
        this.IsFocused = true;
        this.Focused?.Invoke(mouseEvent);
    }

    internal void InvokeMouseMoved(in MouseEvent mouseEvent) =>
        this.MouseMoved?.Invoke(mouseEvent);

    internal void InvokeMouseOut(in MouseEvent mouseEvent) =>
        this.MouseOut?.Invoke(mouseEvent);

    internal void InvokeMouseOver(in MouseEvent mouseEvent) =>
        this.MouseOver?.Invoke(mouseEvent);

    internal virtual void UpdateLayout()
    {
        if (this.HasPendingUpdate)
        {
            this.CalculateLayout();

            if (this.layoutInfo.ParentDependent == Dependency.None)
            {
                this.UpdateDisposition();
            }

            this.HasPendingUpdate = false;
        }
    }

    public void Click() =>
        this.Clicked?.Invoke(new() { Target = this });

    public void Focus()
    {
        this.IsFocused = true;
        this.Focused?.Invoke(new() { Target = this });
    }
}
