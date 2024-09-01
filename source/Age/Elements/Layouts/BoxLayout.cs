using Age.Commands;
using Age.Numerics;
using Age.Storage;
using Age.Styling;

using static Age.Rendering.Shaders.Canvas.CanvasShader;

namespace Age.Elements.Layouts;

internal partial class BoxLayout(Element target) : Layout
{
    private readonly HashSet<BoxLayout> dependents = [];

    private Transform2D    styleTransform = new();
    private Size<uint>     avaliableSpace;
    private Size<uint>     content;
    private Size<uint>     contentDynamicSize;
    private Size<uint>     contentStaticSize;
    private Vector2<float> offset;
    private RectEdges      border;
    private RectEdges      margin;
    private RectEdges      padding;
    private Dependency     contentDependent;
    private Dependency     parentDependent;
    private uint           hightestChild;
    private uint           renderableNodesCount;

    public override BoxLayout? Parent => target.ParentElement?.Layout;
    public override Element    Target => target;

    public Transform2D Transform
    {
        get
        {
            var pivot = new Vector2<float>();

            if (this.Size != default)
            {
                var stylePivot = this.Target.Style.Pivot ?? new();

                stylePivot = (stylePivot + 1) / 2;
                stylePivot.Y = 1 - stylePivot.Y;

                pivot = stylePivot * this.Size.Cast<float>() * new Size<float>(1, -1);
            }

            return Transform2D.Translated(this.offset)
                * Transform2D.Translated(pivot)
                * this.styleTransform
                * Transform2D.Translated(-pivot);
        }
    }

    private static void CalculatePendingPaddingHorizontal(BoxLayout layout, in Size<uint> size)
    {
        if (layout.Target.Style.Padding?.Left?.TryGetPercentage(out var left) ?? false)
        {
            layout.padding.Left = (uint)(size.Width * left);
        }

        if (layout.Target.Style.Padding?.Right?.TryGetPercentage(out var right) ?? false)
        {
            layout.padding.Right = (uint)(size.Width * right);
        }
    }

    private static void CalculatePendingPaddingVertical(BoxLayout layout, in Size<uint> size)
    {
        if (layout.Target.Style.Padding?.Top?.TryGetPercentage(out var top) ?? false)
        {
            layout.padding.Top = (uint)(size.Width * top);
        }

        if (layout.Target.Style.Padding?.Bottom?.TryGetPercentage(out var bottom) ?? false)
        {
            layout.padding.Bottom = (uint)(size.Width * bottom);
        }
    }

    private static void CalculatePendingMarginHorizontal(BoxLayout layout, StackKind stack, in Size<uint> size, ref Size<uint> contentSize)
    {
        var horizontal = 0u;

        if (layout.Target.Style.Margin?.Left?.TryGetPercentage(out var left) ?? false)
        {
            layout.margin.Left = (uint)(size.Width * left);

            horizontal += layout.margin.Left;
        }

        if (layout.Target.Style.Margin?.Right?.TryGetPercentage(out var right) ?? false)
        {
            layout.margin.Right = (uint)(size.Width * right);

            horizontal += layout.margin.Right;
        }

        if (horizontal > 0)
        {
            if (stack == StackKind.Horizontal)
            {
                contentSize.Width += horizontal;
            }
            else
            {
                contentSize.Width = uint.Max(layout.content.Width + layout.border.Horizontal + horizontal, contentSize.Width);
            }
        }
    }

    private static void CalculatePendingMarginVertical(BoxLayout layout, StackKind stack, in Size<uint> size, ref Size<uint> contentSize)
    {
        var vertical = 0u;

        if (layout.Target.Style.Margin?.Top?.TryGetPercentage(out var top) ?? false)
        {
            layout.margin.Top = (uint)(size.Height * top);

            vertical += layout.margin.Top;
        }

        if (layout.Target.Style.Margin?.Bottom?.TryGetPercentage(out var bottom) ?? false)
        {
            layout.margin.Bottom = (uint)(size.Height * bottom);

            vertical += layout.margin.Bottom;
        }

        if (vertical > 0)
        {
            if (stack == StackKind.Vertical)
            {
                contentSize.Height += vertical;
            }
            else
            {
                contentSize.Height = uint.Max(layout.content.Height + layout.border.Vertical + vertical, contentSize.Height);
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

    private void CalculateLayout()
    {
        var stack       = this.Target.Style.Stack ?? StackKind.Horizontal;
        var contentSize = new Size<uint>();

        this.hightestChild = 0;

        foreach (var node in this.Target)
        {
            if (node is ContainerNode child)
            {
                child.Layout.Update();

                var childSize = new Size<uint>();

                if (node is TextNode textNode)
                {
                    childSize = textNode.Layout.Size;
                }
                else if (child is Element element)
                {
                    childSize.Width  = element.Layout.margin.Horizontal;
                    childSize.Height = element.Layout.margin.Vertical;

                    if (this.contentDependent.HasFlag(Dependency.Width) || !element.Layout.parentDependent.HasFlag(Dependency.Width))
                    {
                        childSize.Width += element.Layout.border.Horizontal;
                    }

                    if (!element.Layout.parentDependent.HasFlag(Dependency.Width))
                    {
                        childSize.Width += element.Layout.content.Width;
                    }

                    if (this.contentDependent.HasFlag(Dependency.Height) || !element.Layout.parentDependent.HasFlag(Dependency.Height))
                    {
                        childSize.Height += element.Layout.border.Vertical;
                    }

                    if (!element.Layout.parentDependent.HasFlag(Dependency.Height))
                    {
                        childSize.Height += element.Layout.content.Height;
                    }
                }

                if (stack == StackKind.Horizontal)
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

        if (this.contentDependent.HasFlag(Dependency.Width) || this.contentDependent.HasFlag(Dependency.Height))
        {
            this.CalculatePendingMargin(ref contentSize);
        }

        this.contentStaticSize = contentSize;

        var size = this.content;

        if (!this.parentDependent.HasFlag(Dependency.Width))
        {
            size.Width = contentSize.Width;
        }

        if (!this.parentDependent.HasFlag(Dependency.Height))
        {
            size.Height = contentSize.Height;
        }

        var resolvedMargin  = !this.parentDependent.HasFlag(Dependency.Margin);
        var resolvedPadding = !this.parentDependent.HasFlag(Dependency.Padding);
        var resolvedWidth   = true;
        var resolvedHeight  = true;

        if (this.Target.Style.Padding?.Top?.TryGetPixel(out var top) ?? false)
        {
            this.padding.Top = top;
        }

        if (this.Target.Style.Padding?.Right?.TryGetPixel(out var right) ?? false)
        {
            this.padding.Right = right;
        }

        if (this.Target.Style.Padding?.Bottom?.TryGetPixel(out var bottom) ?? false)
        {
            this.padding.Bottom = bottom;
        }

        if (this.Target.Style.Padding?.Left?.TryGetPixel(out var left) ?? false)
        {
            this.padding.Left = left;
        }

        if (this.Target.Style.Margin?.Top?.TryGetPixel(out top) ?? false)
        {
            this.margin.Top = top;
        }

        if (this.Target.Style.Margin?.Right?.TryGetPixel(out right) ?? false)
        {
            this.margin.Right = right;
        }

        if (this.Target.Style.Margin?.Bottom?.TryGetPixel(out bottom) ?? false)
        {
            this.margin.Bottom = bottom;
        }

        if (this.Target.Style.Margin?.Left?.TryGetPixel(out left) ?? false)
        {
            this.margin.Left = left;
        }

        if (!this.contentDependent.HasFlag(Dependency.Width))
        {
            if (this.Target.Style.Size?.Width?.TryGetPixel(out var pixel) ?? false)
            {
                size.Width = pixel;
            }
            else if ((this.Target.Style.MinSize?.Width?.TryGetPixel(out var min) ?? false) && (this.Target.Style.MaxSize?.Width?.TryGetPixel(out var max) ?? false))
            {
                size.Width = uint.Max(uint.Min(size.Width, min), max);
            }
            else if (this.Target.Style.MinSize?.Width?.TryGetPixel(out min) ?? false)
            {
                size.Width = uint.Max(size.Width, min);
            }
            else if (this.Target.Style.MaxSize?.Width?.TryGetPixel(out max) ?? false)
            {
                size.Width = uint.Max(size.Width, max);
            }
            else
            {
                resolvedWidth = false;
            }
        }

        if (!this.contentDependent.HasFlag(Dependency.Height))
        {
            if (this.Target.Style.Size?.Height?.TryGetPixel(out var pixel) ?? false)
            {
                size.Height = pixel;
            }
            else if ((this.Target.Style.MinSize?.Height?.TryGetPixel(out var min) ?? false) && (this.Target.Style.MaxSize?.Height?.TryGetPixel(out var max) ?? false))
            {
                size.Height = uint.Max(uint.Min(size.Height, min), max);
            }
            else if (this.Target.Style.MinSize?.Height?.TryGetPixel(out min) ?? false)
            {
                size.Height = uint.Max(size.Height, min);
            }
            else if (this.Target.Style.MaxSize?.Height?.TryGetPixel(out max) ?? false)
            {
                size.Height = uint.Max(size.Height, max);
            }
            else
            {
                resolvedHeight = false;
            }
        }

        if (this.Target.Style.BoxSizing == BoxSizing.Border)
        {
            if (!this.contentDependent.HasFlag(Dependency.Width))
            {
                size.Width -= this.border.Horizontal;
            }

            if (!this.contentDependent.HasFlag(Dependency.Height))
            {
                size.Height -= this.border.Vertical;
            }
        }

        this.avaliableSpace = size - contentSize;

        if (this.content != size)
        {
            this.Parent?.RequestUpdate();

            this.content = size;

            if (resolvedWidth && resolvedHeight && resolvedMargin && resolvedPadding)
            {
                this.CalculatePendingLayouts();
            }
        }

        this.UpdateLayoutSize();
    }

    private void CalculatePendingMargin(ref Size<uint> size)
    {
        var contentSize = size;

        var stack = this.Target.Style.Stack ?? StackKind.Horizontal;

        foreach (var dependent in this.dependents)
        {
            if (dependent.parentDependent.HasFlag(Dependency.Padding) || dependent.parentDependent.HasFlag(Dependency.Margin))
            {
                if (!this.parentDependent.HasFlag(Dependency.Width))
                {
                    CalculatePendingMarginHorizontal(dependent, stack, size, ref contentSize);
                }

                if (!this.parentDependent.HasFlag(Dependency.Height))
                {
                    CalculatePendingMarginVertical(dependent, stack, size, ref contentSize);
                }
            }
        }

        size = contentSize;
    }

    private void CalculatePendingLayouts()
    {
        var contentDynamicSize = new Size<uint>();

        var stack = this.Target.Style.Stack ?? StackKind.Horizontal;

        foreach (var dependent in this.dependents)
        {
            var size = dependent.content;

            if (!this.contentDependent.HasFlag(Dependency.Width))
            {
                CalculatePendingPaddingHorizontal(dependent, this.content);
                CalculatePendingMarginHorizontal(dependent, stack, this.content, ref contentDynamicSize);

                if (dependent.parentDependent.HasFlag(Dependency.Width))
                {
                    if (dependent.Target.Style.Size?.Width?.TryGetPercentage(out var percentage) ?? false)
                    {
                        size.Width = (uint)(this.content.Width * percentage);
                    }
                    else if ((dependent.Target.Style.MinSize?.Width?.TryGetPercentage(out var min) ?? false) && (dependent.Target.Style.MaxSize?.Width?.TryGetPercentage(out var max) ?? false))
                    {
                        size.Width = uint.Max(uint.Min(this.content.Width, (uint)(this.content.Width * min)), (uint)(this.content.Width * max));
                    }
                    else if (dependent.Target.Style.MinSize?.Width?.TryGetPercentage(out min) ?? false)
                    {
                        size.Width = uint.Min(this.content.Width, (uint)(this.content.Width * min));
                    }
                    else if (dependent.Target.Style.MaxSize?.Width?.TryGetPercentage(out max) ?? false)
                    {
                        size.Width = uint.Max(this.content.Width, (uint)(this.content.Width * max));
                    }

                    size.Width -= dependent.padding.Horizontal;

                    if (stack == StackKind.Horizontal)
                    {
                        if (size.Width < this.avaliableSpace.Width)
                        {
                            this.avaliableSpace.Width -= size.Width;
                        }
                        else
                        {
                            size.Width = this.avaliableSpace.Width;

                            this.avaliableSpace.Width = 0;
                        }

                        contentDynamicSize.Width += size.Width;
                    }
                    else
                    {
                        contentDynamicSize.Width = uint.Max(size.Width, contentDynamicSize.Width);
                    }

                    size.Width -= dependent.border.Horizontal;
                }
            }

            if (!this.contentDependent.HasFlag(Dependency.Height))
            {
                CalculatePendingPaddingVertical(dependent, this.content);
                CalculatePendingMarginVertical(dependent, stack, this.content, ref contentDynamicSize);

                if (dependent.parentDependent.HasFlag(Dependency.Height))
                {
                    if (dependent.Target.Style.Size?.Height?.TryGetPercentage(out var percentage) ?? false)
                    {
                        size.Height = (uint)(this.content.Height * percentage);
                    }
                    else if ((dependent.Target.Style.MinSize?.Height?.TryGetPercentage(out var min) ?? false) && (dependent.Target.Style.MaxSize?.Height?.TryGetPercentage(out var max) ?? false))
                    {
                        size.Height = uint.Max(uint.Min(this.content.Height, (uint)(this.content.Height * min)), (uint)(this.content.Height * max));
                    }
                    else if (dependent.Target.Style.MinSize?.Height?.TryGetPercentage(out min) ?? false)
                    {
                        size.Height = uint.Min(this.content.Height, (uint)(this.content.Height * min));
                    }
                    else if (dependent.Target.Style.MaxSize?.Height?.TryGetPercentage(out max) ?? false)
                    {
                        size.Height = uint.Max(this.content.Height, (uint)(this.content.Height * max));
                    }

                    size.Height -= dependent.padding.Vertical;

                    if (stack == StackKind.Vertical)
                    {
                        if (size.Height < this.avaliableSpace.Height)
                        {
                            this.avaliableSpace.Height -= size.Height;
                        }
                        else
                        {
                            size.Height = this.avaliableSpace.Height;

                            this.avaliableSpace.Height = 0;
                        }

                        contentDynamicSize.Height += size.Height;
                    }
                    else
                    {
                        contentDynamicSize.Height = uint.Max(size.Height, contentDynamicSize.Height);
                    }

                    size.Height -= dependent.border.Vertical;
                }
            }

            if (size != dependent.content)
            {
                dependent.avaliableSpace.Width = size.Width > dependent.contentStaticSize.Width
                    ? size.Width - dependent.contentStaticSize.Width
                    : 0;

                dependent.avaliableSpace.Height = size.Height > dependent.contentStaticSize.Height
                    ? size.Height - dependent.contentStaticSize.Height
                    : 0;

                dependent.content = size;
                dependent.CalculatePendingLayouts();
            }

            dependent.UpdateLayoutSize();
            dependent.UpdateDisposition();

            dependent.HasPendingUpdate = false;

            this.UpdateBaseline(dependent.Target);
        }

        this.contentDynamicSize += contentDynamicSize;
    }

    private Size<uint> GetTotalSize() =>
        new(
            this.content.Width + this.padding.Horizontal + this.border.Horizontal,
            this.content.Height + this.padding.Vertical + this.border.Vertical
        );

    private void UpdateBaseline(ContainerNode child)
    {
        Style?    style  = null;
        RectEdges margin = default;

        if (child is Element element)
        {
            style  = element.Style;
            margin = element.Layout.margin;
        }

        var alignment = style?.Alignment ?? AlignmentType.BaseLine;
        var totalSize = new Size<uint>(child.Layout.Size.Width + margin.Horizontal, child.Layout.Size.Height + margin.Vertical);

        if (style?.Align == null && alignment == AlignmentType.BaseLine && totalSize.Height > this.hightestChild)
        {
            this.BaseLine = style?.Margin == null ? child.Layout.BaseLine : (margin.Top + child.Layout.Size.Height * child.Layout.BaseLine) / totalSize.Height;

            this.hightestChild = totalSize.Height;
        }
    }

    private void UpdateDisposition()
    {
        if (this.renderableNodesCount == 0)
        {
            return;
        }

        var offset      = new Point<float>();
        var size        = this.content.Cast<float>();
        var contentSize = new Size<uint>();
        var stack       = this.Target.Style.Stack ?? StackKind.Horizontal;

        if (stack == StackKind.Horizontal)
        {
            contentSize.Width  = this.contentStaticSize.Width + this.contentDynamicSize.Width;
            contentSize.Height = uint.Max(this.contentStaticSize.Height, this.contentDynamicSize.Height);
        }
        else
        {
            contentSize.Width  = uint.Max(this.contentStaticSize.Width, this.contentDynamicSize.Width);
            contentSize.Height = this.contentStaticSize.Height + this.contentDynamicSize.Height;
        }

        offset.X += this.border.Left + this.padding.Left;
        offset.Y -= this.border.Top  + this.padding.Top;

        ContainerNode? lastChild = null;

        var i = 0;

        foreach (var node in this.Target)
        {
            if (node is not ContainerNode child)
            {
                continue;
            }

            var reserved   = size / (this.renderableNodesCount - i);

            RectEdges border     = default;
            Style?    childStyle = null;
            RectEdges margin     = default;

            if (child is Element element)
            {
                childStyle = element.Style;
                margin     = element.Layout.margin;
                border     = element.Layout.border;
            }

            var hasMargin    = childStyle?.Margin != null;
            var offsetScaleX = childStyle?.Align?.X ?? GetXAlignment(childStyle?.Alignment);
            var offsetScaleY = childStyle?.Align?.Y ?? GetYAlignment(childStyle?.Alignment) ?? (stack == StackKind.Horizontal ? this.Target.Style.Baseline : null);

            Vector2<float> position;
            Size<float>    usedSpace;

            if (stack == StackKind.Horizontal)
            {
                var factorX  = Normalize(offsetScaleX ?? -1);
                var factorY  = 1 - Normalize(offsetScaleY ?? (hasMargin ? 0 : 1));
                var isInline = !offsetScaleY.HasValue && !hasMargin;
                var canAlign = offsetScaleX.HasValue && size.Width > contentSize.Width;

                var x = canAlign ? (float)(Math.Max(0, reserved.Width - child.Layout.Size.Width - margin.Horizontal - border.Horizontal) * factorX) : 0;
                var y = isInline
                    ? (float)(size.Height - size.Height * this.BaseLine - child.Layout.Size.Height * (1 - child.Layout.BaseLine))
                    : (float)((size.Height - child.Layout.Size.Height - margin.Vertical) * factorY);

                usedSpace = canAlign ? new(float.Max(child.Layout.Size.Width, reserved.Width - x), child.Layout.Size.Height) : child.Layout.Size.Cast<float>();

                position = new(float.Ceiling(x + offset.X + margin.Left), -float.Ceiling(y - offset.Y - -margin.Top));

                offset.X = position.X + margin.Right + usedSpace.Width;
            }
            else
            {
                var factorX  = 1 - Normalize(-(offsetScaleX ?? (hasMargin ? 0 : -1)));
                var factorY  = Normalize(-(offsetScaleY ?? 1));
                var canAlign = offsetScaleY.HasValue && size.Height > contentSize.Height;

                var x = (size.Width - child.Layout.Size.Width - margin.Horizontal) * factorX;
                var y = 0f;

                if (canAlign)
                {
                    y = (float)(Math.Max(0, reserved.Height - child.Layout.Size.Height - margin.Vertical - border.Vertical) * factorY);
                }

                usedSpace = canAlign ? new(child.Layout.Size.Width, float.Max(child.Layout.Size.Height, reserved.Height - y)) : child.Layout.Size.Cast<float>();

                position  = new(float.Ceiling(x + offset.X + margin.Left), -float.Ceiling(y - offset.Y - -margin.Top));

                offset.Y = position.Y + -(margin.Bottom + usedSpace.Height);
            }

            if (child is Element element1) // TODO - Removes multiples castas
            {
                element1.Layout.offset = position;
            }
            else
            {
                child.LocalTransform = child.LocalTransform with { Position = position };
            }

            lastChild = child;
        }

        if (stack == StackKind.Vertical && lastChild != null)
        {
            // TODO - Analyse use case
            // this.Baseline = 1 - (offset.Y - lastChild.Size.Height * lastChild.Baseline) / target.Size.Height;
        }
    }

    private void UpdateLayoutInfo()
    {
        this.border = new()
        {
            Top    = this.Target.Style.Border?.Top.Thickness ?? 0,
            Right  = this.Target.Style.Border?.Right.Thickness ?? 0,
            Bottom = this.Target.Style.Border?.Bottom.Thickness ?? 0,
            Left   = this.Target.Style.Border?.Left.Thickness ?? 0,
        };

        this.contentDependent = Dependency.None;
        this.parentDependent  = Dependency.None;

        if (this.Target.Style.Size?.Width == null && this.Target.Style.MinSize?.Width == null && this.Target.Style.MaxSize?.Width == null)
        {
            this.contentDependent |= Dependency.Width;
        }
        else if (this.Target.Style.Size?.Width?.Kind == UnitKind.Percentage || this.Target.Style.MinSize?.Width?.Kind == UnitKind.Percentage || this.Target.Style.MaxSize?.Width?.Kind == UnitKind.Percentage)
        {
            this.parentDependent |= Dependency.Width;
        }

        if (this.Target.Style.Size?.Height == null && this.Target.Style.MinSize?.Height == null && this.Target.Style.MaxSize?.Height == null)
        {
            this.contentDependent |= Dependency.Height;
        }
        else if (this.Target.Style.Size?.Height?.Kind == UnitKind.Percentage || this.Target.Style.MinSize?.Height?.Kind == UnitKind.Percentage || this.Target.Style.MaxSize?.Height?.Kind == UnitKind.Percentage)
        {
            this.parentDependent |= Dependency.Height;
        }

        if (this.Target.Style.Margin?.Top?.Kind == UnitKind.Percentage || this.Target.Style.Margin?.Right?.Kind == UnitKind.Percentage || this.Target.Style.Margin?.Bottom?.Kind == UnitKind.Percentage || this.Target.Style.Margin?.Left?.Kind == UnitKind.Percentage)
        {
            this.parentDependent |= Dependency.Margin;
        }

        if (this.Target.Style.Padding?.Top?.Kind == UnitKind.Percentage || this.Target.Style.Padding?.Right?.Kind == UnitKind.Percentage || this.Target.Style.Padding?.Bottom?.Kind == UnitKind.Percentage || this.Target.Style.Padding?.Left?.Kind == UnitKind.Percentage)
        {
            this.parentDependent |= Dependency.Padding;
        }

        if (this.Parent != null)
        {
            if (this.parentDependent != Dependency.None)
            {
                this.Parent.dependents.Add(this);
            }
            else
            {
                this.Parent.dependents.Remove(this);
            }
        }
    }

    private void UpdateRect()
    {
        if (this.Target.SingleCommand is not RectCommand command)
        {
            this.Target.SingleCommand = command = new()
            {
                Flags = Flags.ColorAsBackground,
                SampledTexture = new(
                    TextureStorage.Singleton.DefaultTexture,
                    TextureStorage.Singleton.DefaultSampler,
                    UVRect.Normalized
                ),
            };
        }

        command.ObjectId = this.Target.Style.Border.HasValue || this.Target.Style.BackgroundColor.HasValue ? (uint)(this.Target.Index + 1) : 0;
        command.Rect     = new(this.Size.Cast<float>(), default);
        command.Border   = this.Target.Style.Border ?? default;
        command.Color    = this.Target.Style.BackgroundColor ?? default;
    }

    private void UpdateLayoutSize()
    {
        var size = this.GetTotalSize();

        if (size != this.Size)
        {
            this.Size = size;
            this.UpdateRect();
        }
    }

    private void UpdateStyleTransform() =>
        this.styleTransform = this.styleTransform with
        {
            Position = this.Target.Style.Position ?? this.styleTransform.Position,
            Rotation = this.Target.Style.Rotation ?? this.styleTransform.Rotation,
        };

    public void AddDependent(Element element)
    {
        if (element.Layout.parentDependent != Dependency.None)
        {
            this.dependents.Add(element.Layout);
        }
    }

    public void DecreaseRenderableNodes()
    {
        this.renderableNodesCount--;
        this.RequestUpdate();
    }

    public void IncreaseRenderableNodes()
    {
        this.renderableNodesCount++;
        this.RequestUpdate();
    }

    public void RemoveDependent(Element element) =>
        this.dependents.Remove(element.Layout);

    public void StyleChanged()
    {
        this.UpdateStyleTransform();
        this.UpdateLayoutInfo();
        this.RequestUpdate();
    }

    public override void Update()
    {
        if (this.HasPendingUpdate)
        {
            this.CalculateLayout();

            if (this.parentDependent == Dependency.None)
            {
                this.UpdateDisposition();
            }

            this.HasPendingUpdate = false;
        }
    }
}
