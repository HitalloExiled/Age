using Age.Commands;
using Age.Numerics;
using Age.Storage;
using Age.Styling;

using static Age.Rendering.Shaders.Canvas.CanvasShader;

namespace Age.Elements;

internal class BoxModel(Element target)
{
    [Flags]
    public enum Dependency
    {
        None    = 0,
        Width   = 1 << 0,
        Height  = 1 << 1,
        Margin  = 1 << 2,
        Padding = 1 << 3,
    }

    public record struct RectEdges
    {
        public uint Top;
        public uint Right;
        public uint Bottom;
        public uint Left;

        public readonly uint Horizontal => this.Left + this.Right;
        public readonly uint Vertical   => this.Top + this.Bottom;
    }

    private readonly Element target = target;
    private readonly HashSet<BoxModel> dependents = [];

    private Size<uint>  avaliableSpace;
    private Size<uint>  contentDynamicSize;
    private Size<uint>  contentStaticSize;
    private Size<uint>  size;
    private RectEdges   border;
    private RectEdges   margin;
    private RectEdges   padding;
    private Dependency  contentDependent;
    private Dependency  parentDependent;
    private uint        hightestChild;
    private uint        renderableNodesCount;
    private Transform2D styleTransform = new();

    private Size<uint> TotalSize =>
        new(
            this.size.Width + this.padding.Horizontal + this.border.Horizontal,
            this.size.Height + this.padding.Vertical + this.border.Vertical
        );

    public bool HasPendingUpdate { get; set; }

    public BoxModel? Parent => this.target.ParentElement?.BoxModel;

    public Transform2D StyleTransform => this.styleTransform;

    private static void CalculatePendingPaddingHorizontal(BoxModel boxModel, in Size<uint> size)
    {
        if (boxModel.target.Style.Padding?.Left?.TryGetPercentage(out var left) ?? false)
        {
            boxModel.padding.Left = (uint)(size.Width * left);
        }

        if (boxModel.target.Style.Padding?.Right?.TryGetPercentage(out var right) ?? false)
        {
            boxModel.padding.Right = (uint)(size.Width * right);
        }
    }

    private static void CalculatePendingPaddingVertical(BoxModel boxModel, in Size<uint> size)
    {
        if (boxModel.target.Style.Padding?.Top?.TryGetPercentage(out var top) ?? false)
        {
            boxModel.padding.Top = (uint)(size.Width * top);
        }

        if (boxModel.target.Style.Padding?.Bottom?.TryGetPercentage(out var bottom) ?? false)
        {
            boxModel.padding.Bottom = (uint)(size.Width * bottom);
        }
    }

    private static void CalculatePendingMarginHorizontal(BoxModel boxModel, StackKind stack, in Size<uint> size, ref Size<uint> contentSize)
    {
        var horizontal = 0u;

        if (boxModel.target.Style.Margin?.Left?.TryGetPercentage(out var left) ?? false)
        {
            boxModel.margin.Left = (uint)(size.Width * left);

            horizontal += boxModel.margin.Left;
        }

        if (boxModel.target.Style.Margin?.Right?.TryGetPercentage(out var right) ?? false)
        {
            boxModel.margin.Right = (uint)(size.Width * right);

            horizontal += boxModel.margin.Right;
        }

        if (horizontal > 0)
        {
            if (stack == StackKind.Horizontal)
            {
                contentSize.Width += horizontal;
            }
            else
            {
                contentSize.Width = uint.Max(boxModel.size.Width + boxModel.border.Horizontal + horizontal, contentSize.Width);
            }
        }
    }

    private static void CalculatePendingMarginVertical(BoxModel boxModel, StackKind stack, in Size<uint> size, ref Size<uint> contentSize)
    {
        var vertical = 0u;

        if (boxModel.target.Style.Margin?.Top?.TryGetPercentage(out var top) ?? false)
        {
            boxModel.margin.Top = (uint)(size.Height * top);

            vertical += boxModel.margin.Top;
        }

        if (boxModel.target.Style.Margin?.Bottom?.TryGetPercentage(out var bottom) ?? false)
        {
            boxModel.margin.Bottom = (uint)(size.Height * bottom);

            vertical += boxModel.margin.Bottom;
        }

        if (vertical > 0)
        {
            if (stack == StackKind.Vertical)
            {
                contentSize.Height += vertical;
            }
            else
            {
                contentSize.Height = uint.Max(boxModel.size.Height + boxModel.border.Vertical + vertical, contentSize.Height);
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
        var stack       = this.target.Style.Stack ?? StackKind.Horizontal;
        var contentSize = new Size<uint>();

        this.hightestChild = 0;

        foreach (var node in this.target)
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
                    element.BoxModel.UpdateLayout();

                    childSize.Width  = element.BoxModel.margin.Horizontal;
                    childSize.Height = element.BoxModel.margin.Vertical;

                    if (this.contentDependent.HasFlag(Dependency.Width) || !element.BoxModel.parentDependent.HasFlag(Dependency.Width))
                    {
                        childSize.Width += element.BoxModel.border.Horizontal;
                    }

                    if (!element.BoxModel.parentDependent.HasFlag(Dependency.Width))
                    {
                        childSize.Width += element.BoxModel.size.Width;
                    }

                    if (this.contentDependent.HasFlag(Dependency.Height) || !element.BoxModel.parentDependent.HasFlag(Dependency.Height))
                    {
                        childSize.Height += element.BoxModel.border.Vertical;
                    }

                    if (!element.BoxModel.parentDependent.HasFlag(Dependency.Height))
                    {
                        childSize.Height += element.BoxModel.size.Height;
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

        var size = this.size;

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

        if (this.target.Style.Padding?.Top?.TryGetPixel(out var top) ?? false)
        {
            this.padding.Top = top;
        }

        if (this.target.Style.Padding?.Right?.TryGetPixel(out var right) ?? false)
        {
            this.padding.Right = right;
        }

        if (this.target.Style.Padding?.Bottom?.TryGetPixel(out var bottom) ?? false)
        {
            this.padding.Bottom = bottom;
        }

        if (this.target.Style.Padding?.Left?.TryGetPixel(out var left) ?? false)
        {
            this.padding.Left = left;
        }

        if (this.target.Style.Margin?.Top?.TryGetPixel(out top) ?? false)
        {
            this.margin.Top = top;
        }

        if (this.target.Style.Margin?.Right?.TryGetPixel(out right) ?? false)
        {
            this.margin.Right = right;
        }

        if (this.target.Style.Margin?.Bottom?.TryGetPixel(out bottom) ?? false)
        {
            this.margin.Bottom = bottom;
        }

        if (this.target.Style.Margin?.Left?.TryGetPixel(out left) ?? false)
        {
            this.margin.Left = left;
        }

        if (!this.contentDependent.HasFlag(Dependency.Width))
        {
            if (this.target.Style.Size?.Width?.TryGetPixel(out var pixel) ?? false)
            {
                size.Width = pixel;
            }
            else if ((this.target.Style.MinSize?.Width?.TryGetPixel(out var min) ?? false) && (this.target.Style.MaxSize?.Width?.TryGetPixel(out var max) ?? false))
            {
                size.Width = uint.Max(uint.Min(size.Width, min), max);
            }
            else if (this.target.Style.MinSize?.Width?.TryGetPixel(out min) ?? false)
            {
                size.Width = uint.Max(size.Width, min);
            }
            else if (this.target.Style.MaxSize?.Width?.TryGetPixel(out max) ?? false)
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
            if (this.target.Style.Size?.Height?.TryGetPixel(out var pixel) ?? false)
            {
                size.Height = pixel;
            }
            else if ((this.target.Style.MinSize?.Height?.TryGetPixel(out var min) ?? false) && (this.target.Style.MaxSize?.Height?.TryGetPixel(out var max) ?? false))
            {
                size.Height = uint.Max(uint.Min(size.Height, min), max);
            }
            else if (this.target.Style.MinSize?.Height?.TryGetPixel(out min) ?? false)
            {
                size.Height = uint.Max(size.Height, min);
            }
            else if (this.target.Style.MaxSize?.Height?.TryGetPixel(out max) ?? false)
            {
                size.Height = uint.Max(size.Height, max);
            }
            else
            {
                resolvedHeight = false;
            }
        }

        if (this.target.Style.BoxSizing == BoxSizing.Border)
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

        if (this.size != size)
        {
            this.size = size;

            if (resolvedWidth && resolvedHeight && resolvedMargin && resolvedPadding)
            {
                this.CalculatePendingLayouts();
            }
            else
            {
                this.Parent?.RequestUpdate();
            }
        }

        this.target.Size = this.TotalSize;
    }

    private void CalculatePendingMargin(ref Size<uint> size)
    {
        var contentSize = size;

        var stack = this.target.Style.Stack ?? StackKind.Horizontal;

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

        var stack = this.target.Style.Stack ?? StackKind.Horizontal;

        foreach (var dependent in this.dependents)
        {
            var size = dependent.size;

            if (!this.contentDependent.HasFlag(Dependency.Width))
            {
                CalculatePendingPaddingHorizontal(dependent, this.size);
                CalculatePendingMarginHorizontal(dependent, stack, this.size, ref contentDynamicSize);

                if (dependent.parentDependent.HasFlag(Dependency.Width))
                {
                    if (dependent.target.Style.Size?.Width?.TryGetPercentage(out var percentage) ?? false)
                    {
                        size.Width = (uint)(this.size.Width * percentage);
                    }
                    else if ((dependent.target.Style.MinSize?.Width?.TryGetPercentage(out var min) ?? false) && (dependent.target.Style.MaxSize?.Width?.TryGetPercentage(out var max) ?? false))
                    {
                        size.Width = uint.Max(uint.Min(this.size.Width, (uint)(this.size.Width * min)), (uint)(this.size.Width * max));
                    }
                    else if (dependent.target.Style.MinSize?.Width?.TryGetPercentage(out min) ?? false)
                    {
                        size.Width = uint.Min(this.size.Width, (uint)(this.size.Width * min));
                    }
                    else if (dependent.target.Style.MaxSize?.Width?.TryGetPercentage(out max) ?? false)
                    {
                        size.Width = uint.Max(this.size.Width, (uint)(this.size.Width * max));
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
                CalculatePendingPaddingVertical(dependent, this.size);
                CalculatePendingMarginVertical(dependent, stack, this.size, ref contentDynamicSize);

                if (dependent.parentDependent.HasFlag(Dependency.Height))
                {
                    if (dependent.target.Style.Size?.Height?.TryGetPercentage(out var percentage) ?? false)
                    {
                        size.Height = (uint)(this.size.Height * percentage);
                    }
                    else if ((dependent.target.Style.MinSize?.Height?.TryGetPercentage(out var min) ?? false) && (dependent.target.Style.MaxSize?.Height?.TryGetPercentage(out var max) ?? false))
                    {
                        size.Height = uint.Max(uint.Min(this.size.Height, (uint)(this.size.Height * min)), (uint)(this.size.Height * max));
                    }
                    else if (dependent.target.Style.MinSize?.Height?.TryGetPercentage(out min) ?? false)
                    {
                        size.Height = uint.Min(this.size.Height, (uint)(this.size.Height * min));
                    }
                    else if (dependent.target.Style.MaxSize?.Height?.TryGetPercentage(out max) ?? false)
                    {
                        size.Height = uint.Max(this.size.Height, (uint)(this.size.Height * max));
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

            if (size != dependent.size)
            {
                dependent.avaliableSpace.Width = size.Width > dependent.contentStaticSize.Width
                    ? size.Width - dependent.contentStaticSize.Width
                    : 0;

                dependent.avaliableSpace.Height = size.Height > dependent.contentStaticSize.Height
                    ? size.Height - dependent.contentStaticSize.Height
                    : 0;

                Console.WriteLine($"Element: {dependent.target.Name}, Size: {size}, Border: {dependent.border} :: [{this.target.Name}].Size: {this.size}");

                dependent.size = size;

                dependent.CalculatePendingLayouts();
            }

            dependent.target.Size = dependent.TotalSize;

            dependent.UpdateDisposition();
            dependent.HasPendingUpdate = false;

            this.UpdateBaseline(dependent.target);
        }

        this.contentDynamicSize += contentDynamicSize;
    }

    public void StyleChanged()
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
            margin = element.BoxModel.margin;
        }

        var alignment = style?.Alignment ?? AlignmentType.BaseLine;
        var totalSize = new Size<uint>(child.Size.Width + margin.Horizontal, child.Size.Height + margin.Vertical);

        if (style?.Align == null && alignment == AlignmentType.BaseLine && totalSize.Height > this.hightestChild)
        {
            this.target.Baseline = style?.Margin == null ? child.Baseline : (margin.Top + child.Size.Height * child.Baseline) / totalSize.Height;

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
        var size        = this.size.Cast<float>();
        var contentSize = new Size<uint>();
        var stack       = this.target.Style.Stack ?? StackKind.Horizontal;

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

        foreach (var node in this.target)
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
                margin     = element.BoxModel.margin;
                border     = element.BoxModel.border;
            }

            var hasMargin    = childStyle?.Margin != null;
            var offsetScaleX = childStyle?.Align?.X ?? GetXAlignment(childStyle?.Alignment);
            var offsetScaleY = childStyle?.Align?.Y ?? GetYAlignment(childStyle?.Alignment) ?? (stack == StackKind.Horizontal ? this.target.Style.Baseline : null);

            Vector2<float> position;
            Size<float>    usedSpace;

            if (stack == StackKind.Horizontal)
            {
                var factorX  = Normalize(offsetScaleX ?? -1);
                var factorY  = 1 - Normalize(offsetScaleY ?? (hasMargin ? 0 : 1));
                var isInline = !offsetScaleY.HasValue && !hasMargin;
                var canAlign = offsetScaleX.HasValue && size.Width > contentSize.Width;

                var x = canAlign ? (float)(Math.Max(0, reserved.Width - child.Size.Width - margin.Horizontal - border.Horizontal) * factorX) : 0;
                var y = isInline
                    ? (float)(size.Height - size.Height * this.target.Baseline - child.Size.Height * (1 - child.Baseline))
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
            Top    = this.target.Style.Border?.Top.Thickness ?? 0,
            Right  = this.target.Style.Border?.Right.Thickness ?? 0,
            Bottom = this.target.Style.Border?.Bottom.Thickness ?? 0,
            Left   = this.target.Style.Border?.Left.Thickness ?? 0,
        };

        this.contentDependent = Dependency.None;
        this.parentDependent  = Dependency.None;

        if (this.target.Style.Size?.Width == null && this.target.Style.MinSize?.Width == null && this.target.Style.MaxSize?.Width == null)
        {
            this.contentDependent |= Dependency.Width;
        }
        else if (this.target.Style.Size?.Width?.Kind == UnitKind.Percentage || this.target.Style.MinSize?.Width?.Kind == UnitKind.Percentage || this.target.Style.MaxSize?.Width?.Kind == UnitKind.Percentage)
        {
            this.parentDependent |= Dependency.Width;
        }

        if (this.target.Style.Size?.Height == null && this.target.Style.MinSize?.Height == null && this.target.Style.MaxSize?.Height == null)
        {
            this.contentDependent |= Dependency.Height;
        }
        else if (this.target.Style.Size?.Height?.Kind == UnitKind.Percentage || this.target.Style.MinSize?.Height?.Kind == UnitKind.Percentage || this.target.Style.MaxSize?.Height?.Kind == UnitKind.Percentage)
        {
            this.parentDependent |= Dependency.Height;
        }

        if (this.target.Style.Margin?.Top?.Kind == UnitKind.Percentage || this.target.Style.Margin?.Right?.Kind == UnitKind.Percentage || this.target.Style.Margin?.Bottom?.Kind == UnitKind.Percentage || this.target.Style.Margin?.Left?.Kind == UnitKind.Percentage)
        {
            this.parentDependent |= Dependency.Margin;
        }

        if (this.target.Style.Padding?.Top?.Kind == UnitKind.Percentage || this.target.Style.Padding?.Right?.Kind == UnitKind.Percentage || this.target.Style.Padding?.Bottom?.Kind == UnitKind.Percentage || this.target.Style.Padding?.Left?.Kind == UnitKind.Percentage)
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
        if (this.target.SingleCommand is not RectCommand command)
        {
            this.target.SingleCommand = command = new()
            {
                Flags = Flags.ColorAsBackground,
                SampledTexture = new(
                    TextureStorage.Singleton.DefaultTexture,
                    TextureStorage.Singleton.DefaultSampler,
                    UVRect.Normalized
                ),
            };
        }

        command.ObjectId = this.target.Style.Border.HasValue || this.target.Style.BackgroundColor.HasValue ? (uint)(this.target.Index + 1) : 0;
        command.Rect     = new(this.TotalSize.Cast<float>(), default);
        command.Border   = this.target.Style.Border ?? default;
        command.Color    = this.target.Style.BackgroundColor ?? default;
    }

    private void UpdateStyleTransform() =>
        this.styleTransform = this.styleTransform with
        {
            Position = this.target.Style.Position ?? this.styleTransform.Position,
            Rotation = this.target.Style.Rotation ?? this.styleTransform.Rotation,
        };

    internal protected virtual void RequestUpdate()
    {
        if (!this.HasPendingUpdate)
        {
            this.HasPendingUpdate = true;

            if (this.Parent != null)
            {
                this.Parent.RequestUpdate();
            }
            else if (this.target.IsConnected)
            {
                this.target.Tree.IsDirty = true;
            }
        }
    }

    public void IncreaseRenderableNodes()
    {
        this.renderableNodesCount++;
        this.RequestUpdate();
    }

    public void ChildElementAppended(Element element)
    {
        if (element.BoxModel.parentDependent != Dependency.None)
        {
            this.dependents.Add(element.BoxModel);
        }
    }

    public void ChildElementRemoved(Element element) =>
        this.dependents.Remove(element.BoxModel);

    public void DecreaseRenderableNodes()
    {
        this.renderableNodesCount--;
        this.RequestUpdate();
    }

    public void SizeChanged()
    {
        this.Parent?.RequestUpdate();
        Console.WriteLine($"Element: {this.target.Name}, Size: {this.target.Size}, BoxModel.Size: {this.size}, BoxModel.Border: {this.border},");

        this.UpdateRect();
    }

    public virtual void UpdateLayout()
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
