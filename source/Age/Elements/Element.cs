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
    private bool hasPendingUpdate;

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

    private static uint GetAbsoluteValue(in Unit unit) =>
        unit.Type == UnitType.Pixel ? (uint)unit.Value : 0;

    private static uint GetAbsoluteValueInRange(uint avaliable, in Unit min, in Unit max) =>
        uint.Max(uint.Min(avaliable, GetAbsoluteValue(min)), GetAbsoluteValue(max));

    private static Size<uint> GetBorderSize(Style? style) =>
        new(
            (style?.Border?.Right.Thickness ?? 0) + (style?.Border?.Left.Thickness ?? 0),
            (style?.Border?.Top.Thickness ?? 0) + (style?.Border?.Bottom.Thickness ?? 0)
        );

    private static uint GetRelativeValue(in Unit unit, uint avaliable) =>
        unit.Type == UnitType.Pixel ? (uint)unit.Value : (uint)(unit.Value * avaliable);

    private static uint GetRelativeValueInRange(uint avaliable, in Unit min, in Unit max) =>
        uint.Max(uint.Min(avaliable, GetRelativeValue(min, avaliable)), GetRelativeValue(max, avaliable));

    private static PendingCalculation GetLazyDimensions(Style? style)
    {
        var lazyDimension = PendingCalculation.None;

        if (style is { Size.Width.Type: UnitType.Percentage } or { MinSize.Width.Type: UnitType.Percentage } or { MaxSize.Width.Type: UnitType.Percentage })
        {
            lazyDimension |= PendingCalculation.Width;
        }

        if (style is { Size.Height.Type: UnitType.Percentage } or { MinSize.Height.Type: UnitType.Percentage } or { MaxSize.Height.Type: UnitType.Percentage })
        {
            lazyDimension |= PendingCalculation.Height;
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
        var stackMode   = this.Style.Stack ?? StackType.Horizontal;
        var contentSize = new Size<uint>();

        this.layoutInfo.HightestChild      = 0;
        this.layoutInfo.PendingCalculation = GetLazyDimensions(this.Style);

        foreach (var node in this)
        {
            if (node is ContainerNode child)
            {
                // if (node is TextNode textNode)
                // {
                //     textNode.Draw();
                // }

                var childStyle = (child as Element)?.Style;
                var margin     = childStyle?.Margin ?? new(0);
                var totalSize  = new Size<uint>(child.Size.Width + margin.Horizontal, child.Size.Height + margin.Vertical);

                var lazyCalculation = GetLazyDimensions(childStyle);

                if (lazyCalculation != PendingCalculation.None)
                {
                    var element = (Element)child;

                    element.layoutInfo.PendingCalculation = lazyCalculation;
                    this.layoutInfo.PendingChildCalculations.Add(element);
                }

                if (stackMode == StackType.Horizontal)
                {
                    if (!lazyCalculation.HasFlag(PendingCalculation.Width))
                    {
                        contentSize.Width += totalSize.Width;

                        if (!lazyCalculation.HasFlag(PendingCalculation.Height))
                        {
                            UpdateBaseline(this, child);

                            contentSize.Height = uint.Max(contentSize.Height, totalSize.Height);
                        }
                    }
                }
                else
                {
                    if (!lazyCalculation.HasFlag(PendingCalculation.Height))
                    {
                        contentSize.Height += totalSize.Height;

                        if (!lazyCalculation.HasFlag(PendingCalculation.Width))
                        {
                            contentSize.Width = uint.Max(contentSize.Width, totalSize.Width);
                        }
                    }
                }

                this.layoutInfo.NodesToDistribute.Add(child);
            }
        }

        var size = this.ContentSize = contentSize;

        if (this.Style.Size?.Width.HasValue ?? false)
        {
            size.Width = GetAbsoluteValue(this.Style.Size.Value.Width.Value);
        }
        else if ((this.Style.MinSize?.Width.HasValue ?? false) && (this.Style.MaxSize?.Width.HasValue ?? false))
        {
            size.Width = GetAbsoluteValueInRange(size.Width, this.Style.MinSize.Value.Width.Value, this.Style.MaxSize.Value.Width.Value);
        }
        else if (this.Style.MinSize?.Width.HasValue ?? false)
        {
            size.Width = uint.Max(size.Width, GetAbsoluteValue(this.Style.MinSize.Value.Width.Value));
        }
        else if (this.Style.MaxSize?.Width.HasValue ?? false)
        {
            size.Width = uint.Min(size.Width, GetAbsoluteValue(this.Style.MaxSize.Value.Width.Value));
        }

        if (this.Style.Size?.Height.HasValue ?? false)
        {
            size.Height = GetAbsoluteValue(this.Style.Size.Value.Height.Value);
        }
        else if ((this.Style.MinSize?.Height.HasValue ?? false) && (this.Style.MaxSize?.Height.HasValue ?? false))
        {
            size.Height = GetAbsoluteValueInRange(size.Height, this.Style.MinSize.Value.Height.Value, this.Style.MaxSize.Value.Height.Value);
        }
        else if (this.Style.MinSize?.Height.HasValue ?? false)
        {
            size.Height = uint.Max(size.Height, GetAbsoluteValue(this.Style.MinSize.Value.Height.Value));
        }
        else if (this.Style.MaxSize?.Height.HasValue ?? false)
        {
            size.Height = uint.Min(size.Height, GetAbsoluteValue(this.Style.MaxSize.Value.Height.Value));
        }

        var border = GetBorderSize(this.Style);

        this.layoutInfo.Size   = size;
        this.layoutInfo.Border = border;

        if (this.Style.BoxSizing != BoxSizing.Border)
        {
            if (!this.layoutInfo.PendingCalculation.HasFlag(PendingCalculation.Width))
            {
                size.Width += border.Width;
            }

            if (!this.layoutInfo.PendingCalculation.HasFlag(PendingCalculation.Height))
            {
                size.Height += border.Height;
            }
        }

        this.layoutInfo.AvaliableSpace = (size - border - contentSize).Max(default);

        if (this.layoutInfo.PendingCalculation != PendingCalculation.All)
        {
            this.CalculatePendingLayouts();
        }

        this.Size = size;

        this.UpdateRect();
    }

    private void CalculatePendingLayouts()
    {
        foreach (var child in this.layoutInfo.PendingChildCalculations.ToArray())
        {
            if (!this.layoutInfo.PendingCalculation.HasFlag(PendingCalculation.Width) && child.layoutInfo.PendingCalculation.HasFlag(PendingCalculation.Width))
            {
                if (child.Style.Size?.Width?.Type == UnitType.Percentage)
                {
                    child.layoutInfo.Size.Width = GetRelativeValue(child.Style.Size.Value.Width.Value, this.layoutInfo.Size.Width);
                }
                else if (child.Style.MinSize?.Width?.Type == UnitType.Percentage && child.Style.MaxSize?.Width?.Type == UnitType.Percentage)
                {
                    child.layoutInfo.Size.Width = GetRelativeValueInRange(this.layoutInfo.Size.Width, child.Style.MinSize.Value.Width.Value, child.Style.MaxSize.Value.Width.Value);
                }
                else if (child.Style.MinSize?.Width?.Type == UnitType.Percentage)
                {
                    child.layoutInfo.Size.Width = uint.Min(this.layoutInfo.Size.Width, GetRelativeValue(child.Style.MinSize.Value.Width.Value, this.layoutInfo.Size.Width));
                }
                else if (child.Style.MaxSize?.Width?.Type == UnitType.Percentage)
                {
                    child.layoutInfo.Size.Width = uint.Max(this.layoutInfo.Size.Width, GetRelativeValue(child.Style.MaxSize.Value.Width.Value, this.layoutInfo.Size.Width));
                }

                if (this.Style.Stack != StackType.Vertical)
                {
                    if (child.layoutInfo.Size.Width < this.layoutInfo.AvaliableSpace.Width)
                    {
                        this.layoutInfo.AvaliableSpace.Width -= child.layoutInfo.Size.Width;
                    }
                    else
                    {
                        child.layoutInfo.Size.Width = this.layoutInfo.AvaliableSpace.Width;

                        this.layoutInfo.AvaliableSpace.Width = 0;
                    }
                }

                child.layoutInfo.PendingCalculation &= ~PendingCalculation.Width;

                this.ContentSize = this.ContentSize with
                {
                    Width = this.ContentSize.Width + child.layoutInfo.Size.Width
                };
            }

            if (!this.layoutInfo.PendingCalculation.HasFlag(PendingCalculation.Height) && child.layoutInfo.PendingCalculation.HasFlag(PendingCalculation.Height))
            {
                if (child.Style.Size?.Height?.Type == UnitType.Percentage)
                {
                    child.layoutInfo.Size.Height = GetRelativeValue(child.Style.Size.Value.Height.Value, this.layoutInfo.Size.Height);
                }
                else if (child.Style.MinSize?.Height?.Type == UnitType.Percentage && child.Style.MaxSize?.Height?.Type == UnitType.Percentage)
                {
                    child.layoutInfo.Size.Height = GetRelativeValueInRange(this.layoutInfo.Size.Height, child.Style.MinSize.Value.Height.Value, child.Style.MaxSize.Value.Height.Value);
                }
                else if (child.Style.MinSize?.Height?.Type == UnitType.Percentage)
                {
                    child.layoutInfo.Size.Height = uint.Min(this.layoutInfo.Size.Height, GetRelativeValue(child.Style.MinSize.Value.Height.Value, this.layoutInfo.Size.Height));
                }
                else if (child.Style.MaxSize?.Height?.Type == UnitType.Percentage)
                {
                    child.layoutInfo.Size.Height = uint.Max(this.layoutInfo.Size.Height, GetRelativeValue(child.Style.MaxSize.Value.Height.Value, this.layoutInfo.Size.Height));
                }

                if (this.Style.Stack == StackType.Vertical)
                {
                    if (child.layoutInfo.Size.Height < this.layoutInfo.AvaliableSpace.Height)
                    {
                        this.layoutInfo.AvaliableSpace.Height -= child.layoutInfo.Size.Height;
                    }
                    else
                    {
                        child.layoutInfo.Size.Height = this.layoutInfo.AvaliableSpace.Height;

                        this.layoutInfo.AvaliableSpace.Height = 0;
                    }
                }

                UpdateBaseline(this, child);

                child.layoutInfo.PendingCalculation &= ~PendingCalculation.Height;

                this.ContentSize = this.ContentSize with
                {
                    Height = this.ContentSize.Height + child.layoutInfo.Size.Height
                };
            }

            child.layoutInfo.AvaliableSpace = (child.layoutInfo.Size - child.layoutInfo.Border - child.ContentSize).Max(default);

            // Console.WriteLine($"Element: {child.Name}, Size: {size}, Border: {border}");

            child.CalculatePendingLayouts();

            if (child.layoutInfo.PendingCalculation == PendingCalculation.None)
            {
                child.Size = child.layoutInfo.Size + (this.Style.BoxSizing != BoxSizing.Border ? child.layoutInfo.Border : default);

                child.UpdateRect();
                child.UpdateDisposition();
                child.HasPendingUpdate = false;

                this.layoutInfo.PendingChildCalculations.Remove(child);
            }
        }
    }

    private static void UpdateBaseline(Element parent, ContainerNode child)
    {
        var style     = (child as Element)?.Style;
        var margin    = style?.Margin ?? new(0);
        var alignment = style?.Alignment ?? AlignmentType.BaseLine;
        var totalSize = new Size<uint>(child.Size.Width + margin.Horizontal, child.Size.Height + margin.Vertical);

        if (style?.Align == null && alignment == AlignmentType.BaseLine && totalSize.Height > parent.layoutInfo.HightestChild)
        {
            parent.Baseline = style?.Margin == null
                ? child.Baseline
                : (margin.Top + child.Size.Height * child.Baseline) / totalSize.Height;

            parent.layoutInfo.HightestChild = totalSize.Height;
        }
    }

    private void OnStyleChanged()
    {
        this.UpdateStyleTransform();
        this.RequestUpdate();
    }

    private void UpdateDisposition()
    {
        if (this.layoutInfo.NodesToDistribute.Count == 0)
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
        var size        = this.layoutInfo.Size.Cast<float>();
        var contentSize = this.ContentSize;
        var stack       = this.Style.Stack ?? StackType.Horizontal;

        offset.X += this.Style.Border?.Left.Thickness ?? 0;
        offset.Y -= this.Style.Border?.Top.Thickness ?? 0;

        ContainerNode? lastChild = null;

        for (var i = 0; i < this.layoutInfo.NodesToDistribute.Count; i++)
        {
            var reserved = size / (this.layoutInfo.NodesToDistribute.Count - i);

            var child      = this.layoutInfo.NodesToDistribute[i];
            var childStyle = (child as Element)?.Style;

            var border       = GetBorderSize(childStyle);
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

                var x = canAlign ? (float)(Math.Max(0, reserved.Width - child.Size.Width - margin.Horizontal - border.Width) * factorX) : 0;
                var y = isInline
                    ? (float)(size.Height - size.Height * this.Baseline - child.Size.Height * (1 - child.Baseline))
                    : (float)((size.Height - child.Size.Height - margin.Vertical) * factorY);

                usedSpace = canAlign ? new(float.Max(child.Size.Width, reserved.Width - x), child.Size.Height) : child.Size.Cast<float>();

                position = new(x + offset.X + margin.Left, -(y - offset.Y - margin.Top));

                offset.X = position.X + margin.Right + usedSpace.Width;
            }
            else
            {
                var factorX  = 1 - normalize(-(offsetScaleX ?? (hasMargin ? 0 : -1)));
                var factorY  = normalize(-(offsetScaleY ?? 1));
                var canAlign = offsetScaleY.HasValue && size.Height > contentSize.Height;

                var x = (size.Width - child.Size.Width - margin.Horizontal) * factorX;
                var y = 0f;

                if (canAlign)
                {
                    y = (float)(Math.Max(0, reserved.Height - child.Size.Height - margin.Vertical - border.Height) * factorY);
                }

                usedSpace = canAlign ? new(child.Size.Width, float.Max(child.Size.Height, reserved.Height - y)) : child.Size.Cast<float>();

                position  = new(x + offset.X + margin.Left, -(y - offset.Y - margin.Top));

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

        this.layoutInfo.NodesToDistribute.Clear();
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

            if (this.layoutInfo.PendingCalculation == PendingCalculation.None)
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
