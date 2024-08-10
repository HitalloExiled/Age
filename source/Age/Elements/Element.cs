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

public abstract class Element : ContainerNode, IEnumerable<Element>
{
    [Flags]
    private enum Dimensions
    {
        None   = 0,
        Width  = 1 << 0,
        Height = 1 << 1,
        All    = Width | Height,
    }

    public event MouseEventHandler?   Blured;
    public event MouseEventHandler?   Clicked;
    public event ContextEventHandler? Context;
    public event MouseEventHandler?   Focused;
    public event MouseEventHandler?   MouseMoved;
    public event MouseEventHandler?   MouseOut;
    public event MouseEventHandler?   MouseOver;

    private readonly List<ContainerNode> nodesToDistribute        = [];
    private readonly List<Element>       pendingChildCalculations = [];

    private Canvas?                 canvas;
    private Vector2<float>          offset;
    private Dimensions          pendingCalculation;
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
    public Element? LastElementChild { get; private set; }

    public Element? PreviousElementSibling { get; private set; }
    public Element? NextElementSibling { get; private set; }

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

    private static uint GetAbsoluteValue(in Unit unit) =>
        unit.Type == UnitType.Pixel ? (uint)unit.Value : 0;

    private static Size<uint> GetAbsoluteValue(in SizeUnit unitSize) =>
        new(GetAbsoluteValue(unitSize.Width), GetAbsoluteValue(unitSize.Height));

    private static Margin GetBorderMargin(Style? style) =>
        new(
            style?.Border?.Top.Thickness ?? 0,
            style?.Border?.Right.Thickness ?? 0,
            style?.Border?.Bottom.Thickness ?? 0,
            style?.Border?.Left.Thickness ?? 0
        );

    private static uint GetRelativeValue(in Unit unit, uint avaliable) =>
        unit.Type == UnitType.Pixel ? (uint)unit.Value : (uint)(unit.Value * avaliable);

    private static uint GetRelativeValueInRange(uint avaliable, in Unit min, in Unit max) =>
        uint.Max(uint.Min(avaliable, GetRelativeValue(min, avaliable)), GetRelativeValue(max, avaliable));

    private static Dimensions GetLazyDimensions(Style? style)
    {
        var lazyDimension = Dimensions.None;

        if (style is { Size.Width.Type: UnitType.Percentage } or { MinSize.Width.Type: UnitType.Percentage } or { MaxSize.Width.Type: UnitType.Percentage })
        {
            lazyDimension |= Dimensions.Width;
        }

        if (style is { Size.Height.Type:    UnitType.Percentage } or { MinSize.Height.Type: UnitType.Percentage } or { MaxSize.Height.Type: UnitType.Percentage })
        {
            lazyDimension |= Dimensions.Height;
        }

        return lazyDimension;
    }

    IEnumerator<Element> IEnumerable<Element>.GetEnumerator()
    {
        for (var childElement = this.FirstElementChild; childElement != null; childElement = childElement.NextElementSibling)
        {
            yield return childElement;
        }
    }

    private void CalculateLayout()
    {
        var stackMode = this.Style.Stack ?? StackType.Horizontal;

        var hightest    = 0f;
        var contentSize = new Size<uint>();

        this.pendingCalculation = GetLazyDimensions(this.Style);

        foreach (var node in this)
        {
            if (node is ContainerNode child)
            {
                if (node is TextNode textNode)
                {
                    textNode.Draw();
                }

                var childStyle = (child as Element)?.Style;
                var margin     = childStyle?.Margin ?? new(0);
                var totalSize  = new Size<uint>(child.Size.Width + margin.Horizontal, child.Size.Height + margin.Vertical);

                var lazyCalculation = GetLazyDimensions(childStyle);

                if (lazyCalculation != Dimensions.None)
                {
                    var element = (Element)child;

                    element.pendingCalculation = lazyCalculation;
                    this.pendingChildCalculations.Add(element);
                }

                if (stackMode == StackType.Horizontal)
                {
                    if (!lazyCalculation.HasFlag(Dimensions.Width))
                    {
                        contentSize.Width += totalSize.Width;

                        if (!lazyCalculation.HasFlag(Dimensions.Height))
                        {
                            UpdateBaseline(this, child, ref hightest);

                            contentSize.Height = uint.Max(contentSize.Height, totalSize.Height);
                        }
                    }
                }
                else
                {
                    if (!lazyCalculation.HasFlag(Dimensions.Height))
                    {
                        contentSize.Height += totalSize.Height;

                        if (!lazyCalculation.HasFlag(Dimensions.Width))
                        {
                            contentSize.Width = uint.Max(contentSize.Width, totalSize.Width);
                        }
                    }
                }

                this.nodesToDistribute.Add(child);
            }
        }

        this.ContentSize = contentSize;

        var size = this.Style.MinSize.HasValue && this.Style.MaxSize.HasValue
            ? contentSize.Range(GetAbsoluteValue(this.Style.MinSize.Value), GetAbsoluteValue(this.Style.MaxSize.Value))
            : this.Style.MinSize.HasValue
                ? contentSize.Max(GetAbsoluteValue(this.Style.MinSize.Value))
                : this.Style.MaxSize.HasValue
                    ? contentSize.Min(GetAbsoluteValue(this.Style.MaxSize.Value))
                    : this.Style.Size.HasValue
                        ? GetAbsoluteValue(this.Style.Size.Value)
                        : contentSize;

        if (this.pendingCalculation != Dimensions.All)
        {
            this.CalculatePendingLayouts(size);

            this.UpdateSize(size);
        }
    }

    private void CalculatePendingLayouts(in Size<uint> parentSize)
    {
        foreach (var child in this.pendingChildCalculations.ToArray())
        {
            var size = child.Size;

            if (!this.pendingCalculation.HasFlag(Dimensions.Width) && child.pendingCalculation.HasFlag(Dimensions.Width))
            {
                size.Width = child.Style switch
                {
                    { Size.Width.Type: UnitType.Percentage } =>
                        GetRelativeValue(child.Style.Size.Value.Width, parentSize.Width),

                    { MinSize.Width.Type: UnitType.Percentage, MaxSize.Width.Type: UnitType.Percentage } =>
                        GetRelativeValueInRange(parentSize.Width, child.Style.MinSize.Value.Width, child.Style.MaxSize.Value.Width),

                    { MinSize.Width.Type: UnitType.Percentage } =>
                        uint.Min(parentSize.Width, GetRelativeValue(child.Style.MinSize.Value.Width, parentSize.Width)),

                    { MaxSize.Width.Type: UnitType.Percentage } =>
                        uint.Max(parentSize.Width, GetRelativeValue(child.Style.MaxSize.Value.Width, parentSize.Width)),

                    _ => size.Width,
                };

                child.pendingCalculation &= ~Dimensions.Width;
            }

            if (!this.pendingCalculation.HasFlag(Dimensions.Height) && child.pendingCalculation.HasFlag(Dimensions.Height))
            {
                size.Height = child.Style switch
                {
                    { Size.Height.Type: UnitType.Percentage } =>
                        GetRelativeValue(child.Style.Size.Value.Height, parentSize.Height),

                    { MinSize.Height.Type: UnitType.Percentage, MaxSize.Height.Type: UnitType.Percentage } =>
                        GetRelativeValueInRange(parentSize.Height, child.Style.MinSize.Value.Height, child.Style.MaxSize.Value.Height),

                    { MinSize.Height.Type: UnitType.Percentage } =>
                        uint.Min(parentSize.Height, GetRelativeValue(child.Style.MinSize.Value.Height, parentSize.Height)),

                    { MaxSize.Height.Type: UnitType.Percentage } =>
                        uint.Max(parentSize.Height, GetRelativeValue(child.Style.MaxSize.Value.Height, parentSize.Height)),

                    _ => size.Height,
                };

                child.pendingCalculation &= ~Dimensions.Height;
            }

            child.CalculatePendingLayouts(size);

            child.UpdateSize(size);

            if (child.pendingCalculation == Dimensions.None)
            {
                child.UpdateDisposition();

                this.pendingChildCalculations.Remove(child);
            }
        }
    }

    private static void UpdateBaseline(Element parent, ContainerNode child, ref float hightest)
    {
        var style     = (child as Element)?.Style;
        var margin    = style?.Margin ?? new(0);
        var alignment = style?.Alignment ?? AlignmentType.BaseLine;
        var totalSize = new Size<uint>(child.Size.Width + margin.Horizontal, child.Size.Height + margin.Vertical);

        if (style?.Align == null && alignment == AlignmentType.BaseLine && totalSize.Height > hightest)
        {
            parent.Baseline = style?.Margin == null
                ? child.Baseline
                : (margin.Top + child.Size.Height * child.Baseline) / totalSize.Height;

            hightest = totalSize.Height;
        }
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

        if (this.Style.BoxSizing != BoxSizing.Border)
        {
            var borderMargin = GetBorderMargin(this.Style);

            size -= new Size<float>(borderMargin.Horizontal, borderMargin.Vertical);
            offset.X += borderMargin.Left;
            offset.Y += -borderMargin.Top;
        }

        ContainerNode? lastChild = null;

        for (var i = 0; i < this.nodesToDistribute.Count; i++)
        {
            var reserved = size / (this.nodesToDistribute.Count - i);

            var child = this.nodesToDistribute[i];
            var childStyle = (child as Element)?.Style;

            var borderMargin = GetBorderMargin(childStyle);
            var margin       = childStyle?.Margin ?? new();
            var hasMargin    = childStyle?.Margin != null;
            var offsetScaleX = childStyle?.Align?.X ?? getXAlignment(childStyle?.Alignment);
            var offsetScaleY = childStyle?.Align?.Y ?? getYAlignment(childStyle?.Alignment) ?? (stack == StackType.Horizontal ? this.Style.Baseline : null);

            Vector2<float> position;
            Size<float> usedSpace;

            if (stack == StackType.Horizontal)
            {
                var factorX  = normalize(offsetScaleX ?? -1);
                var factorY  = 1 - normalize(offsetScaleY ?? (hasMargin ? 0 : 1));
                var isInline = !offsetScaleY.HasValue && !hasMargin;
                var canAlign = offsetScaleX.HasValue && size.Width > contentSize.Width;

                var x = canAlign ? Math.Max(0, reserved.Width - child.Size.Width - margin.Horizontal - borderMargin.Horizontal) * factorX : 0;
                var y = isInline
                    ? size.Height - size.Height * this.Baseline - child.Size.Height * (1 - child.Baseline)
                    : (size.Height - child.Size.Height - margin.Vertical) * factorY;

                position = new(x + offset.X + margin.Left, -(y + margin.Top) + offset.Y);
                usedSpace = canAlign
                    ? new(float.Max(child.Size.Width, reserved.Width - x), child.Size.Height)
                    : child.Size.Cast<float>();

                offset.X = position.X + margin.Right + usedSpace.Width;
            }
            else
            {
                var factorX  = 1 - normalize(-offsetScaleX ?? (hasMargin ? 0 : 1));
                var factorY  = normalize(-offsetScaleY ?? -1);
                var canAlign = offsetScaleY.HasValue && size.Height > contentSize.Height;

                var x = (size.Width - child.Size.Width - margin.Horizontal) * factorX;
                var y = canAlign ? Math.Max(0, reserved.Height - child.Size.Height - margin.Vertical - borderMargin.Vertical) * factorY : 0;

                position  = new(margin.Left + x, -(y + offset.Y + margin.Top));
                usedSpace = canAlign
                    ? new(child.Size.Width, float.Max(child.Size.Height, reserved.Height - y))
                    : child.Size.Cast<float>();

                offset.Y = -position.Y + margin.Bottom + usedSpace.Height;
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
            this.Baseline = 1 - (offset.Y - lastChild.Size.Height * lastChild.Baseline) / this.Size.Height;
        }

        this.nodesToDistribute.Clear();
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

    private void UpdateSize(Size<uint> size)
    {
        var borderMargin = GetBorderMargin(this.Style);

        if (!this.pendingCalculation.HasFlag(Dimensions.Width))
        {
            size.Width += borderMargin.Horizontal;
        }

        if (!this.pendingCalculation.HasFlag(Dimensions.Height))
        {
            size.Height += borderMargin.Vertical;
        }

        this.Size = size;

        this.UpdateRect();
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

    protected override void ChildRemoved(Node child)
    {
        if (child is Element elementChild)
        {
            if (elementChild == this.FirstElementChild)
            {
                this.FirstElementChild = elementChild.NextElementSibling;
            }

            if (elementChild == this.LastElementChild)
            {
                this.LastElementChild = elementChild.PreviousElementSibling;
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
            elementChild.NextElementSibling = null;
        }
    }

    protected override void TransformChanged() =>
        this.ParentElement?.RequestUpdate();

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
            foreach (var child in this)
            {
                (child as Element)?.UpdateLayout();
            }

            this.CalculateLayout();

            if (this.pendingCalculation == Dimensions.None)
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
