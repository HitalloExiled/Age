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
    private Size<uint>     contentDynamicSize;
    private Size<uint>     contentStaticSize;
    private RectEdges      border;
    private RectEdges      margin;
    private RectEdges      padding;
    private Dependency     contentDependent;
    private Dependency     parentDependent;
    private uint           hightestChildHeight;
    private ContainerNode? hightestChildNode;
    private uint           renderableNodesCount;

    public override BoxLayout? Parent => target.ParentElement?.Layout;
    public override Element    Target => target;

    public override Transform2D Transform
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

            return base.Transform
                * Transform2D.Translated(pivot)
                * this.styleTransform
                * Transform2D.Translated(-pivot);
        }
    }

    private static void CalculatePendingPaddingHorizontal(BoxLayout layout, in Size<uint> size, ref RectEdges padding)
    {
        if (layout.Target.Style.Padding?.Left?.TryGetPercentage(out var left) ?? false)
        {
            padding.Left = (uint)(size.Width * left);
        }

        if (layout.Target.Style.Padding?.Right?.TryGetPercentage(out var right) ?? false)
        {
            padding.Right = (uint)(size.Width * right);
        }
    }

    private static void CalculatePendingPaddingVertical(BoxLayout layout, in Size<uint> size, ref RectEdges padding)
    {
        if (layout.Target.Style.Padding?.Top?.TryGetPercentage(out var top) ?? false)
        {
            padding.Top = (uint)(size.Width * top);
        }

        if (layout.Target.Style.Padding?.Bottom?.TryGetPercentage(out var bottom) ?? false)
        {
            padding.Bottom = (uint)(size.Width * bottom);
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
                contentSize.Width = uint.Max(layout.Content.Width + layout.border.Horizontal + horizontal, contentSize.Width);
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
                contentSize.Height = uint.Max(layout.Content.Height + layout.border.Vertical + vertical, contentSize.Height);
            }
        }
    }

    private static Point<float> GetAlignment(StackKind stack, Layout layout, AlignmentKind alignmentKind)
    {
        var x = alignmentKind.HasFlag(AlignmentKind.Left) || stack == StackKind.Vertical && alignmentKind.HasFlag(AlignmentKind.Begin)
            ? -1
            : alignmentKind.HasFlag(AlignmentKind.Right) || stack == StackKind.Vertical && alignmentKind.HasFlag(AlignmentKind.End)
                ? 1
                : alignmentKind.HasFlag(AlignmentKind.Center)
                    ? 0
                    : -1;

        var y = alignmentKind.HasFlag(AlignmentKind.Top) || stack == StackKind.Horizontal && alignmentKind.HasFlag(AlignmentKind.Begin)
            ? -1
            : alignmentKind.HasFlag(AlignmentKind.Bottom) || stack == StackKind.Horizontal && alignmentKind.HasFlag(AlignmentKind.End)
                ? 1
                : alignmentKind.HasFlag(AlignmentKind.Center)
                    ? 0
                    : -1;

        return new(Normalize(x), Normalize(y));
    }

    private static float Normalize(float value) =>
        (1 + value) / 2;

    private void CalculateLayout()
    {
        var stack = this.Target.Style.Stack ?? StackKind.Horizontal;

        this.contentStaticSize   = new Size<uint>();
        this.hightestChildHeight = 0;

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
                    childSize.Width  = element.Layout.padding.Horizontal + element.Layout.margin.Horizontal;
                    childSize.Height = element.Layout.padding.Vertical   + element.Layout.margin.Vertical;

                    if (this.contentDependent.HasFlag(Dependency.Width) || !element.Layout.parentDependent.HasFlag(Dependency.Width))
                    {
                        childSize.Width += element.Layout.border.Horizontal;
                    }

                    if (!element.Layout.parentDependent.HasFlag(Dependency.Width))
                    {
                        childSize.Width += element.Layout.Content.Width;
                    }

                    if (this.contentDependent.HasFlag(Dependency.Height) || !element.Layout.parentDependent.HasFlag(Dependency.Height))
                    {
                        childSize.Height += element.Layout.border.Vertical;
                    }

                    if (!element.Layout.parentDependent.HasFlag(Dependency.Height))
                    {
                        childSize.Height += element.Layout.Content.Height;
                    }
                }

                if (stack == StackKind.Horizontal)
                {
                    contentStaticSize.Width += childSize.Width;
                    contentStaticSize.Height = uint.Max(contentStaticSize.Height, childSize.Height);

                    this.CheckHightestChild(stack, child);
                }
                else
                {
                    contentStaticSize.Width = uint.Max(contentStaticSize.Width, childSize.Width);
                    contentStaticSize.Height += childSize.Height;

                    if (child == this.Target.FirstChild)
                    {
                        this.CheckHightestChild(stack, child);

                    }
                }
            }
        }

        if (this.contentDependent.HasFlag(Dependency.Width) || this.contentDependent.HasFlag(Dependency.Height))
        {
            this.CalculatePendingMargin(ref contentStaticSize);
        }

        var content = this.Content;

        if (!this.parentDependent.HasFlag(Dependency.Width))
        {
            content.Width = contentStaticSize.Width;
        }

        if (!this.parentDependent.HasFlag(Dependency.Height))
        {
            content.Height = contentStaticSize.Height;
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
                content.Width = pixel;
            }
            else if ((this.Target.Style.MinSize?.Width?.TryGetPixel(out var min) ?? false) && (this.Target.Style.MaxSize?.Width?.TryGetPixel(out var max) ?? false))
            {
                content.Width = uint.Max(uint.Min(content.Width, min), max);
            }
            else if (this.Target.Style.MinSize?.Width?.TryGetPixel(out min) ?? false)
            {
                content.Width = uint.Max(content.Width, min);
            }
            else if (this.Target.Style.MaxSize?.Width?.TryGetPixel(out max) ?? false)
            {
                content.Width = uint.Max(content.Width, max);
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
                content.Height = pixel;
            }
            else if ((this.Target.Style.MinSize?.Height?.TryGetPixel(out var min) ?? false) && (this.Target.Style.MaxSize?.Height?.TryGetPixel(out var max) ?? false))
            {
                content.Height = uint.Max(uint.Min(content.Height, min), max);
            }
            else if (this.Target.Style.MinSize?.Height?.TryGetPixel(out min) ?? false)
            {
                content.Height = uint.Max(content.Height, min);
            }
            else if (this.Target.Style.MaxSize?.Height?.TryGetPixel(out max) ?? false)
            {
                content.Height = uint.Max(content.Height, max);
            }
            else
            {
                resolvedHeight = false;
            }
        }

        if (this.Target.Style.BoxSizing == BoxSizing.Border)
        {
            if (!this.contentDependent.HasFlag(Dependency.Width) && content.Width >= this.border.Horizontal)
            {
                content.Width -= this.border.Horizontal;
            }

            if (!this.contentDependent.HasFlag(Dependency.Height) && content.Width >= this.border.Vertical)
            {
                content.Height -= this.border.Vertical;
            }
        }

        this.avaliableSpace = content - contentStaticSize;

        if (this.Content != content || this.Target is Canvas)
        {
            this.Parent?.RequestUpdate();

            this.Content = content;

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
            var content = dependent.Content;
            var padding = dependent.padding;

            if (!this.contentDependent.HasFlag(Dependency.Width))
            {
                CalculatePendingPaddingHorizontal(dependent, this.Content, ref padding);
                CalculatePendingMarginHorizontal(dependent, stack, this.Content, ref contentDynamicSize);

                if (dependent.parentDependent.HasFlag(Dependency.Width))
                {
                    if (dependent.Target.Style.Size?.Width?.TryGetPercentage(out var percentage) ?? false)
                    {
                        content.Width = (uint)(this.Content.Width * percentage);
                    }
                    else if ((dependent.Target.Style.MinSize?.Width?.TryGetPercentage(out var min) ?? false) && (dependent.Target.Style.MaxSize?.Width?.TryGetPercentage(out var max) ?? false))
                    {
                        content.Width = uint.Max(uint.Min(this.Content.Width, (uint)(this.Content.Width * min)), (uint)(this.Content.Width * max));
                    }
                    else if (dependent.Target.Style.MinSize?.Width?.TryGetPercentage(out min) ?? false)
                    {
                        content.Width = uint.Min(this.Content.Width, (uint)(this.Content.Width * min));
                    }
                    else if (dependent.Target.Style.MaxSize?.Width?.TryGetPercentage(out max) ?? false)
                    {
                        content.Width = uint.Max(this.Content.Width, (uint)(this.Content.Width * max));
                    }

                    content.Width -= dependent.padding.Horizontal;

                    if (stack == StackKind.Horizontal)
                    {
                        if (content.Width < this.avaliableSpace.Width)
                        {
                            this.avaliableSpace.Width -= content.Width;
                        }
                        else
                        {
                            content.Width = this.avaliableSpace.Width;

                            this.avaliableSpace.Width = 0;
                        }

                        contentDynamicSize.Width += content.Width;
                    }
                    else
                    {
                        contentDynamicSize.Width = uint.Max(content.Width, contentDynamicSize.Width);
                    }

                    content.Width -= dependent.border.Horizontal;
                }
            }

            if (!this.contentDependent.HasFlag(Dependency.Height))
            {
                CalculatePendingPaddingVertical(dependent, this.Content, ref padding);
                CalculatePendingMarginVertical(dependent, stack, this.Content, ref contentDynamicSize);

                if (dependent.parentDependent.HasFlag(Dependency.Height))
                {
                    if (dependent.Target.Style.Size?.Height?.TryGetPercentage(out var percentage) ?? false)
                    {
                        content.Height = (uint)(this.Content.Height * percentage);
                    }
                    else if ((dependent.Target.Style.MinSize?.Height?.TryGetPercentage(out var min) ?? false) && (dependent.Target.Style.MaxSize?.Height?.TryGetPercentage(out var max) ?? false))
                    {
                        content.Height = uint.Max(uint.Min(this.Content.Height, (uint)(this.Content.Height * min)), (uint)(this.Content.Height * max));
                    }
                    else if (dependent.Target.Style.MinSize?.Height?.TryGetPercentage(out min) ?? false)
                    {
                        content.Height = uint.Min(this.Content.Height, (uint)(this.Content.Height * min));
                    }
                    else if (dependent.Target.Style.MaxSize?.Height?.TryGetPercentage(out max) ?? false)
                    {
                        content.Height = uint.Max(this.Content.Height, (uint)(this.Content.Height * max));
                    }

                    content.Height -= dependent.padding.Vertical;

                    if (stack == StackKind.Vertical)
                    {
                        if (content.Height < this.avaliableSpace.Height)
                        {
                            this.avaliableSpace.Height -= content.Height;
                        }
                        else
                        {
                            content.Height = this.avaliableSpace.Height;

                            this.avaliableSpace.Height = 0;
                        }

                        contentDynamicSize.Height += content.Height;
                    }
                    else
                    {
                        contentDynamicSize.Height = uint.Max(content.Height, contentDynamicSize.Height);
                    }

                    content.Height -= dependent.border.Vertical;
                }
            }

            if (content != dependent.Content || padding != dependent.padding)
            {
                dependent.avaliableSpace.Width = content.Width > dependent.contentStaticSize.Width
                    ? content.Width - dependent.contentStaticSize.Width
                    : 0;

                dependent.avaliableSpace.Height = content.Height > dependent.contentStaticSize.Height
                    ? content.Height - dependent.contentStaticSize.Height
                    : 0;

                dependent.padding = padding;
                dependent.Content = content;

                dependent.CalculatePendingLayouts();
                dependent.UpdateDisposition();
            }

            dependent.HasPendingUpdate = false;

            dependent.UpdateLayoutSize();

            this.CheckHightestChild(stack, dependent.Target);
        }

        this.contentDynamicSize += contentDynamicSize;
    }

    private Size<uint> GetTotalSize() =>
        new(
            this.Content.Width + this.padding.Horizontal + this.border.Horizontal,
            this.Content.Height + this.padding.Vertical + this.border.Vertical
        );

    private void CheckHightestChild(StackKind stack, ContainerNode child)
    {
        if (child.Layout.BaseLine < 0)
        {
            return;
        }

        if (child is Element element)
        {
            var hasAlignment = element.Style.Alignment.HasValue
                && !element.Style.Alignment.Value.HasFlag(AlignmentKind.Top)
                && !element.Style.Alignment.Value.HasFlag(AlignmentKind.Bottom)
                && (stack == StackKind.Horizontal || !element.Style.Alignment.Value.HasFlag(AlignmentKind.Center));

            var totalSize = new Size<uint>(element.Layout.Size.Width + element.Layout.margin.Horizontal, element.Layout.Size.Height + element.Layout.margin.Vertical);

            if (!hasAlignment && totalSize.Height > this.hightestChildHeight)
            {
                this.hightestChildHeight = totalSize.Height;
                this.hightestChildNode   = element;
            }
        }
        else if (child.Layout.Size.Height > this.hightestChildHeight)
        {
            this.hightestChildHeight = child.Layout.Size.Height;
            this.hightestChildNode   = child;
        }
    }

    private void UpdateBaseline(StackKind stack)
    {
        if (this.hightestChildNode != null)
        {
            if (this.hightestChildNode is Element element)
            {
                if (stack == StackKind.Horizontal)
                {
                    var offset = element.Layout.margin.Top + element.Layout.border.Top + element.Layout.padding.Top;

                    this.BaseLine = offset == 0
                        ? element.Layout.BaseLine
                        : (offset + element.Layout.Content.Height * element.Layout.BaseLine) / this.hightestChildHeight;
                }
                else
                {
                    var offset = -this.hightestChildNode.Layout.Offset.Y - this.border.Top + element.Layout.border.Top + element.Layout.padding.Top;

                    this.BaseLine = 1 - (float)(offset + this.hightestChildNode.Layout.Content.Height * (1 - this.hightestChildNode.Layout.BaseLine)) / this.Content.Height;
                }
            }
            else
            {
                if (stack == StackKind.Horizontal)
                {
                    this.BaseLine = (float)this.hightestChildNode.Layout.BaseLine;
                }
                else
                {
                    var offset = -this.hightestChildNode.Layout.Offset.Y - this.border.Top;

                    this.BaseLine = 1 - (float)(offset + this.hightestChildNode.Layout.Content.Height * (1 - this.hightestChildNode.Layout.BaseLine)) / this.Content.Height;
                }
            }
        }
    }

    private void UpdateDisposition()
    {
        if (this.renderableNodesCount == 0)
        {
            return;
        }

        var cursor      = new Point<float>();
        var size        = this.Content.Cast<float>();
        var contentSize = new Size<uint>();
        var stack       = this.Target.Style.Stack ?? StackKind.Horizontal;

        var lineHeight = size.Height;

        if (stack == StackKind.Horizontal)
        {
            this.UpdateBaseline(stack);

            if (this.BaseLine > 0)
            {
                lineHeight = size.Height * (1 - this.BaseLine);
            }

            contentSize.Width  = this.contentStaticSize.Width + this.contentDynamicSize.Width;
            contentSize.Height = uint.Max(this.contentStaticSize.Height, this.contentDynamicSize.Height);
        }
        else
        {
            contentSize.Width  = uint.Max(this.contentStaticSize.Width, this.contentDynamicSize.Width);
            contentSize.Height = this.contentStaticSize.Height + this.contentDynamicSize.Height;
        }

        var freeSpace = size - contentSize.Cast<float>();

        var initialOffet = new Point<uint>(this.border.Left + this.padding.Left, this.border.Top  + this.padding.Top);

        cursor.X += initialOffet.X;
        cursor.Y -= initialOffet.Y;

        var i = 0;

        foreach (var node in this.Target)
        {
            if (node is not ContainerNode child)
            {
                continue;
            }

            var alignmentType = AlignmentKind.None;

            RectEdges border = default;
            RectEdges margin = default;

            var childSize      = child.Layout.Size;
            var contentOffsetY = 0u;

            if (child is Element element)
            {
                alignmentType  = element.Style?.Alignment ?? AlignmentKind.None;
                margin         = element.Layout.margin;
                border         = element.Layout.border;
                contentOffsetY = element.Layout.margin.Top + element.Layout.border.Top + element.Layout.padding.Top;
                childSize = new(
                    element.Layout.Content.Width  + element.Layout.margin.Horizontal + element.Layout.border.Horizontal + element.Layout.padding.Horizontal,
                    element.Layout.Content.Height + element.Layout.margin.Vertical   + element.Layout.border.Vertical   + element.Layout.padding.Vertical
                );
            }

            var alignment = GetAlignment(stack, child.Layout, alignmentType);

            Vector2<float> offset;

            if (stack == StackKind.Horizontal)
            {
                var isInline = alignmentType == AlignmentKind.None && child.Layout.BaseLine > 0;

                var hasHorizontalAlignment =
                    alignmentType.HasFlag(AlignmentKind.Begin)
                    || alignmentType.HasFlag(AlignmentKind.Left)
                    || alignmentType.HasFlag(AlignmentKind.Center)
                    || alignmentType.HasFlag(AlignmentKind.Right)
                    || alignmentType.HasFlag(AlignmentKind.End);

                var x = hasHorizontalAlignment
                    ? float.Max(0, freeSpace.Width - childSize.Width) * alignment.X
                    : 0;

                var y = isInline
                    ? (lineHeight - (contentOffsetY + child.Layout.Content.Height * (1 - child.Layout.BaseLine)))
                    : ((size.Height - childSize.Height) * alignment.Y);

                offset = new(float.Ceiling(cursor.X + margin.Left + x), -float.Ceiling(-cursor.Y + margin.Top + y));

                var usedSpace = hasHorizontalAlignment
                    ? float.Max(child.Layout.Size.Width, x - freeSpace.Width - margin.Horizontal)
                    : child.Layout.Size.Width;

                cursor.X = offset.X + usedSpace + margin.Right;
            }
            else
            {
                var hasVerticalAlignment =
                    alignmentType.HasFlag(AlignmentKind.Begin)
                    || alignmentType.HasFlag(AlignmentKind.Top)
                    || alignmentType.HasFlag(AlignmentKind.Center)
                    || alignmentType.HasFlag(AlignmentKind.Bottom)
                    || alignmentType.HasFlag(AlignmentKind.End);

                var x = (size.Width - childSize.Width) * alignment.X;
                var y = hasVerticalAlignment
                    ? float.Max(0, freeSpace.Height - childSize.Height) * alignment.Y
                    : 0;

                offset = new(float.Ceiling(cursor.X + margin.Left + x), -float.Ceiling(-cursor.Y + margin.Top + y));

                var usedSpace = hasVerticalAlignment
                    ? float.Max(child.Layout.Size.Height, y - freeSpace.Height - margin.Vertical)
                    : child.Layout.Size.Height;

                cursor.Y = offset.Y - usedSpace - margin.Bottom;
            }

            child.Layout.Offset = offset;

            i++;
        }

        if (stack == StackKind.Vertical)
        {
            this.UpdateBaseline(stack);
        }
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

    public void UpdateState()
    {
        this.border = new()
        {
            Top    = this.Target.Style.Border?.Top.Thickness ?? 0,
            Right  = this.Target.Style.Border?.Right.Thickness ?? 0,
            Bottom = this.Target.Style.Border?.Bottom.Thickness ?? 0,
            Left   = this.Target.Style.Border?.Left.Thickness ?? 0,
        };

        this.styleTransform = this.styleTransform with
        {
            Position = this.Target.Style.Position ?? this.styleTransform.Position,
            Rotation = this.Target.Style.Rotation ?? this.styleTransform.Rotation,
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

        this.RequestUpdate();
    }

    
        

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
