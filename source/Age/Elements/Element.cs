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

    private static Size<uint> GetBorderSize(Style? style) =>
        new(
            (style?.Border?.Right.Thickness ?? 0) + (style?.Border?.Left.Thickness ?? 0),
            (style?.Border?.Top.Thickness ?? 0) + (style?.Border?.Bottom.Thickness ?? 0)
        );

    private static Dependency GetDependencies(Style? style)
    {
        var dependency = Dependency.None;

        if (style?.Size?.Width == null && style?.MinSize?.Width == null && style?.MaxSize?.Width == null)
        {
            dependency |= Dependency.ChildWidth;
        }
        else if (style?.Size?.Width?.Type == UnitType.Percentage || style?.MinSize?.Width?.Type == UnitType.Percentage || style?.MaxSize?.Width?.Type == UnitType.Percentage)
        {
            dependency |= Dependency.ParentWidth;
        }

        if (style?.Size?.Height == null && style?.MinSize?.Height == null && style?.MaxSize?.Height == null)
        {
            dependency |= Dependency.ChildHeight;
        }
        else if (style?.Size?.Height?.Type == UnitType.Percentage || style?.MinSize?.Height?.Type == UnitType.Percentage || style?.MaxSize?.Height?.Type == UnitType.Percentage)
        {
            dependency |= Dependency.ParentHeight;
        }

        return dependency;
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

                    var margin = element.Style.Margin ?? new(0);

                    childSize.Width  = margin.Horizontal;
                    childSize.Height = margin.Vertical;

                    if (this.layoutInfo.Dependencies.HasFlag(Dependency.ChildWidth))
                    {
                        childSize.Width += element.layoutInfo.Border.Width;
                    }
                    else if (!element.layoutInfo.Dependencies.HasFlag(Dependency.ParentWidth))
                    {
                        childSize.Width += element.Size.Width;
                    }

                    if (this.layoutInfo.Dependencies.HasFlag(Dependency.ChildHeight))
                    {
                        childSize.Height += element.layoutInfo.Border.Height;
                    }
                    else if (!element.layoutInfo.Dependencies.HasFlag(Dependency.ParentHeight))
                    {
                        childSize.Height += element.Size.Height;
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

        this.layoutInfo.ContentStaticSize = contentSize;

        var size = this.layoutInfo.Size;

        if (!this.layoutInfo.Dependencies.HasFlag(Dependency.ParentWidth))
        {
            size.Width = contentSize.Width;
        }

        if (!this.layoutInfo.Dependencies.HasFlag(Dependency.ParentHeight))
        {
            size.Height = contentSize.Height;
        }

        var resolvedWidth  = true;
        var resolvedHeight = true;

        if (!this.layoutInfo.Dependencies.HasFlag(Dependency.ChildWidth))
        {
            if (this.Style.Size?.Width?.Type == UnitType.Pixel)
            {
                size.Width = (uint)this.Style.Size.Value.Width.Value.Value;
            }
            else if (this.Style.MinSize?.Width?.Type == UnitType.Pixel && this.Style.MaxSize?.Width?.Type == UnitType.Pixel)
            {
                size.Width = uint.Max(uint.Min(size.Width, (uint)this.Style.MinSize.Value.Width.Value.Value), (uint)this.Style.MaxSize.Value.Width.Value.Value);
            }
            else if (this.Style.MinSize?.Width?.Type == UnitType.Pixel)
            {
                size.Width = uint.Max(size.Width, (uint)this.Style.MinSize.Value.Width.Value.Value);
            }
            else if (this.Style.MaxSize?.Width?.Type == UnitType.Pixel)
            {
                size.Width = uint.Max(size.Width, (uint)this.Style.MaxSize.Value.Width.Value.Value);
            }
            else
            {
                resolvedWidth = false;
            }
        }

        if (!this.layoutInfo.Dependencies.HasFlag(Dependency.ChildHeight))
        {
            if (this.Style.Size?.Height?.Type == UnitType.Pixel)
            {
                size.Height = (uint)this.Style.Size.Value.Height.Value.Value;
            }
            else if (this.Style.MinSize?.Height?.Type == UnitType.Pixel && this.Style.MaxSize?.Height?.Type == UnitType.Pixel)
            {
                size.Height = uint.Max(uint.Min(size.Height, (uint)this.Style.MinSize.Value.Height.Value.Value), (uint)this.Style.MaxSize.Value.Height.Value.Value);
            }
            else if (this.Style.MinSize?.Height?.Type == UnitType.Pixel)
            {
                size.Height = uint.Max(size.Height, (uint)this.Style.MinSize.Value.Height.Value.Value);
            }
            else if (this.Style.MaxSize?.Height?.Type == UnitType.Pixel)
            {
                size.Height = uint.Max(size.Height, (uint)this.Style.MaxSize.Value.Height.Value.Value);
            }
            else
            {
                resolvedHeight = true;
            }
        }

        if (this.Style.BoxSizing == BoxSizing.Border)
        {
            if (!this.layoutInfo.Dependencies.HasFlag(Dependency.ChildWidth))
            {
                size.Width -= this.layoutInfo.Border.Width;
            }

            if (!this.layoutInfo.Dependencies.HasFlag(Dependency.ChildHeight))
            {
                size.Height -= this.layoutInfo.Border.Height;
            }
        }

        this.layoutInfo.AvaliableSpace = size - contentSize;

        if (this.layoutInfo.Size != size)
        {
            this.layoutInfo.Size = size;

            if (resolvedWidth && resolvedHeight)
            {
                this.CalculatePendingLayouts();
            }
            else
            {
                this.ParentElement?.RequestUpdate();
            }
        }

        this.Size = this.layoutInfo.Size + this.layoutInfo.Border;
    }

    private void CalculatePendingLayouts()
    {
        var contentDynamicSize = new Size<uint>();

        foreach (var child in this.layoutInfo.Dependents)
        {
            var size = child.layoutInfo.Size;

            if (!this.layoutInfo.Dependencies.HasFlag(Dependency.ChildWidth) && child.layoutInfo.Dependencies.HasFlag(Dependency.ParentWidth))
            {
                if (child.Style.Size?.Width?.Type == UnitType.Percentage)
                {
                    size.Width = (uint)(child.Style.Size.Value.Width.Value.Value * this.layoutInfo.Size.Width);
                }
                else if (child.Style.MinSize?.Width?.Type == UnitType.Percentage && child.Style.MaxSize?.Width?.Type == UnitType.Percentage)
                {
                    size.Width = uint.Max(uint.Min(this.layoutInfo.Size.Width, (uint)(child.Style.MinSize.Value.Width.Value.Value * this.layoutInfo.Size.Width)), (uint)(child.Style.MaxSize.Value.Width.Value.Value * this.layoutInfo.Size.Width));
                }
                else if (child.Style.MinSize?.Width?.Type == UnitType.Percentage)
                {
                    size.Width = uint.Min(this.layoutInfo.Size.Width, (uint)(child.Style.MinSize.Value.Width.Value.Value * this.layoutInfo.Size.Width));
                }
                else if (child.Style.MaxSize?.Width?.Type == UnitType.Percentage)
                {
                    size.Width = uint.Max(this.layoutInfo.Size.Width, (uint)(child.Style.MaxSize.Value.Width.Value.Value * this.layoutInfo.Size.Width));
                }

                if (this.Style.Stack != StackType.Vertical)
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

                size.Width -= child.layoutInfo.Border.Width;
            }

            if (!this.layoutInfo.Dependencies.HasFlag(Dependency.ChildHeight) && child.layoutInfo.Dependencies.HasFlag(Dependency.ParentHeight))
            {
                if (child.Style.Size?.Height?.Type == UnitType.Percentage)
                {
                    size.Height = (uint)(child.Style.Size.Value.Height.Value.Value * this.layoutInfo.Size.Height);
                }
                else if (child.Style.MinSize?.Height?.Type == UnitType.Percentage && child.Style.MaxSize?.Height?.Type == UnitType.Percentage)
                {
                    size.Height = uint.Max(uint.Min(this.layoutInfo.Size.Height, (uint)(child.Style.MinSize.Value.Height.Value.Value * this.layoutInfo.Size.Height)), (uint)(child.Style.MaxSize.Value.Height.Value.Value * this.layoutInfo.Size.Height));
                }
                else if (child.Style.MinSize?.Height?.Type == UnitType.Percentage)
                {
                    size.Height = uint.Min(this.layoutInfo.Size.Height, (uint)(child.Style.MinSize.Value.Height.Value.Value * this.layoutInfo.Size.Height));
                }
                else if (child.Style.MaxSize?.Height?.Type == UnitType.Percentage)
                {
                    size.Height = uint.Max(this.layoutInfo.Size.Height, (uint)(child.Style.MaxSize.Value.Height.Value.Value * this.layoutInfo.Size.Height));
                }

                if (this.Style.Stack == StackType.Vertical)
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

                size.Height -= child.layoutInfo.Border.Height;
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

            child.Size = child.layoutInfo.Size + child.layoutInfo.Border;
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

        this.layoutInfo.Border          = GetBorderSize(this.Style);
        this.layoutInfo.Dependencies = GetDependencies(this.Style);

        if (this.ParentElement != null)
        {
            if (this.layoutInfo.Dependencies != Dependency.None)
            {
                this.ParentElement.layoutInfo.Dependents.Add(this);
            }
            else
            {
                this.ParentElement.layoutInfo.Dependents.Remove(this);
            }
        }
    }

    private void UpdateBaseline(ContainerNode child)
    {
        var style     = (child as Element)?.Style;
        var margin    = style?.Margin ?? new(0);
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

        offset.X += this.Style.Border?.Left.Thickness ?? 0;
        offset.Y -= this.Style.Border?.Top.Thickness ?? 0;

        ContainerNode? lastChild = null;

        var i = 0;

        foreach (var node in this)
        {
            if (node is not ContainerNode child)
            {
                continue;
            }

            var reserved   = size / (this.layoutInfo.RenderableNodesCount - i);
            var childStyle = (child as Element)?.Style;

            var border       = GetBorderSize(childStyle);
            var margin       = childStyle?.Margin ?? new();
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

                var x = canAlign ? (float)(Math.Max(0, reserved.Width - child.Size.Width - margin.Horizontal - border.Width) * factorX) : 0;
                var y = isInline
                    ? (float)(size.Height - size.Height * this.Baseline - child.Size.Height * (1 - child.Baseline))
                    : (float)((size.Height - child.Size.Height - margin.Vertical) * factorY);

                usedSpace = canAlign ? new(float.Max(child.Size.Width, reserved.Width - x), child.Size.Height) : child.Size.Cast<float>();

                position = new(float.Ceiling(x + offset.X + margin.Left), -float.Ceiling(y - offset.Y - margin.Top));

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
                    y = (float)(Math.Max(0, reserved.Height - child.Size.Height - margin.Vertical - border.Height) * factorY);
                }

                usedSpace = canAlign ? new(child.Size.Width, float.Max(child.Size.Height, reserved.Height - y)) : child.Size.Cast<float>();

                position  = new(float.Ceiling(x + offset.X + margin.Left), -float.Ceiling(y - offset.Y - margin.Top));

                offset.Y = position.Y + -(margin.Bottom + usedSpace.Height);
            }

            if (child is Element element)
            {
                element.offset = position;
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

                if (childElement.layoutInfo.Dependencies != Dependency.None)
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

            if (this.layoutInfo.Dependencies == Dependency.None)
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
