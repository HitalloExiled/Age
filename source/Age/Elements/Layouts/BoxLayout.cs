using System.Xml.Linq;
using Age.Commands;
using Age.Extensions;
using Age.Numerics;
using Age.Storage;
using Age.Styling;

using static Age.Rendering.Shaders.Canvas.CanvasShader;

namespace Age.Elements.Layouts;

internal partial class BoxLayout(Element target) : Layout
{
    private readonly HashSet<BoxLayout> dependents = [];

    // 24-bytes
    private Transform2D styleTransform = new();

    // 16-bytes
    private RawRectEdges border;
    private RawRectEdges margin;
    private RawRectEdges padding;

    // 8-bytes
    private Size<uint> content;
    private Size<uint> staticContent;

    // 4-bytes
    private Dependency     contentDependent;
    private ContainerNode? hightestInlineChildNode;
    private Dependency     parentDependent;
    private uint           renderableNodesCount;

    private Size<uint> Boundings =>
        new(
            this.Size.Width  + this.padding.Horizontal + this.border.Horizontal,
            this.Size.Height + this.padding.Vertical   + this.border.Vertical
        );

    private Size<uint> BoundingsWithMargin =>
        new(
            this.Size.Width  + this.padding.Horizontal + this.border.Horizontal + this.margin.Horizontal,
            this.Size.Height + this.padding.Vertical   + this.border.Vertical   + this.margin.Vertical
        );

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

    private static void CalculatePendingPaddingHorizontal(BoxLayout layout, in Size<uint> size, ref RawRectEdges padding)
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

    private static void CalculatePendingPaddingVertical(BoxLayout layout, in Size<uint> size, ref RawRectEdges padding)
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
                contentSize.Width = uint.Max(layout.Size.Width + layout.border.Horizontal + horizontal, contentSize.Width);
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
                contentSize.Height = uint.Max(layout.Size.Height + layout.border.Vertical + vertical, contentSize.Height);
            }
        }
    }

    private static Point<float> GetAlignment(StackKind stack, Layout layout, AlignmentKind alignmentKind, out bool hasHorizontalAlignment, out bool hasVerticalAlignment)
    {
        var x = -1;
        var y = -1;

        hasHorizontalAlignment = true;
        hasVerticalAlignment   = true;

        if (alignmentKind.HasFlag(AlignmentKind.Left) || stack == StackKind.Vertical && alignmentKind.HasFlag(AlignmentKind.Begin))
        {
            x = -1;
        }
        else if (alignmentKind.HasFlag(AlignmentKind.Right) || stack == StackKind.Vertical && alignmentKind.HasFlag(AlignmentKind.End))
        {
            x = 1;
        }
        else if (alignmentKind.HasFlag(AlignmentKind.Center))
        {
            x = 0;
        }
        else
        {
            hasHorizontalAlignment = false;
        }

        if (alignmentKind.HasFlag(AlignmentKind.Top) || stack == StackKind.Horizontal && alignmentKind.HasFlag(AlignmentKind.Begin))
        {
            y = -1;
        }
        else if (alignmentKind.HasFlag(AlignmentKind.Bottom) || stack == StackKind.Horizontal && alignmentKind.HasFlag(AlignmentKind.End))
        {
            y = 1;
        }
        else if (alignmentKind.HasFlag(AlignmentKind.Center))
        {
            y = 0;
        }
        else
        {
            hasVerticalAlignment = false;
        }

        return new(Normalize(x), Normalize(y));
    }

    private static float Normalize(float value) =>
        (1 + value) / 2;

    private void CalculateLayout()
    {
        var stack = this.Target.Style.Stack ?? StackKind.Horizontal;

        this.content       = new Size<uint>();
        this.staticContent = new Size<uint>();

        this.hightestInlineChildNode = null;

        foreach (var node in this.Target)
        {
            if (node is ContainerNode child)
            {
                child.Layout.Update();

                Size<uint> childSize;

                var dependencies = Dependency.None;

                if (child is Element element)
                {
                    childSize    = element.Layout.BoundingsWithMargin;
                    dependencies = element.Layout.parentDependent;
                }
                else
                {
                    childSize = child.Layout.Size;
                }

                if (stack == StackKind.Horizontal)
                {
                    if (!dependencies.HasFlag(Dependency.Width))
                    {
                        this.staticContent.Width += childSize.Width;
                        this.staticContent.Height = uint.Max(this.staticContent.Height, childSize.Height);
                    }

                    this.content.Width += childSize.Width;
                    this.content.Height = uint.Max(this.content.Height, childSize.Height);

                    this.CheckHightestInlineChild(stack, child);
                }
                else
                {
                    if (!dependencies.HasFlag(Dependency.Height))
                    {
                        this.staticContent.Width   = uint.Max(this.staticContent.Width, childSize.Width);
                        this.staticContent.Height += childSize.Height;
                    }

                    this.content.Width   = uint.Max(this.content.Width, childSize.Width);
                    this.content.Height += childSize.Height;

                    if (child == this.Target.FirstChild)
                    {
                        this.CheckHightestInlineChild(stack, child);
                    }
                }
            }
        }

        if (this.contentDependent.HasFlag(Dependency.Width) || this.contentDependent.HasFlag(Dependency.Height))
        {
            this.CalculatePendingMargin(ref this.content);
        }

        var resolvedMargin  = !this.parentDependent.HasFlag(Dependency.Margin);
        var resolvedPadding = !this.parentDependent.HasFlag(Dependency.Padding);
        var resolvedWidth   = !this.parentDependent.HasFlag(Dependency.Width);
        var resolvedHeight  = !this.parentDependent.HasFlag(Dependency.Height);

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

        var size = this.content;

        if (!this.contentDependent.HasFlag(Dependency.Width))
        {
            if (this.Target.Style.Size?.Width?.TryGetPixel(out var pixel) ?? false)
            {
                size.Width = pixel;

                resolvedWidth = true;
            }
            else if ((this.Target.Style.MinSize?.Width?.TryGetPixel(out var min) ?? false) && (this.Target.Style.MaxSize?.Width?.TryGetPixel(out var max) ?? false))
            {
                resolvedWidth = true;

                if (size.Width < min)
                {
                    size.Width = min;
                }
                else if (size.Width > max)
                {
                    size.Width = max;
                }
                else
                {
                    resolvedWidth = false;
                }
            }
            else if (this.Target.Style.MinSize?.Width?.TryGetPixel(out min) ?? false)
            {
                if (size.Width < min)
                {
                    size.Width = min;

                    resolvedWidth = true;
                }
            }
            else if (this.Target.Style.MaxSize?.Width?.TryGetPixel(out max) ?? false)
            {
                if (size.Width > max)
                {
                    size.Width = max;

                    resolvedWidth = true;
                }
            }
        }

        if (!this.contentDependent.HasFlag(Dependency.Height))
        {
            if (this.Target.Style.Size?.Height?.TryGetPixel(out var pixel) ?? false)
            {
                size.Height = pixel;

                resolvedHeight = true;
            }
            else if ((this.Target.Style.MinSize?.Height?.TryGetPixel(out var min) ?? false) && (this.Target.Style.MaxSize?.Height?.TryGetPixel(out var max) ?? false))
            {
                resolvedHeight = true;

                if (size.Height < min)
                {
                    size.Height = min;
                }
                else if (size.Height > max)
                {
                    size.Height = max;
                }
                else
                {
                    resolvedHeight = false;
                }
            }
            else if (this.Target.Style.MinSize?.Height?.TryGetPixel(out min) ?? false)
            {
                if (size.Height < min)
                {
                    size.Height = min;

                    resolvedHeight = true;
                }
            }
            else if (this.Target.Style.MaxSize?.Height?.TryGetPixel(out max) ?? false)
            {
                if (size.Height > max)
                {
                    size.Height = max;

                    resolvedHeight = true;
                }
            }
        }

        if (this.Target.Style.BoxSizing == BoxSizing.Border)
        {
            if (!this.contentDependent.HasFlag(Dependency.Width))
            {
                size.Width = size.Width.ClampSubtract(this.border.Horizontal);
            }

            if (!this.contentDependent.HasFlag(Dependency.Height))
            {
                size.Height = size.Height.ClampSubtract(this.border.Horizontal);
            }
        }

        if (this.Size != size || this.Target is Canvas)
        {
            this.Size = size;

            this.Parent?.RequestUpdate();

            if (resolvedWidth && resolvedHeight && resolvedMargin && resolvedPadding)
            {
                this.CalculatePendingLayouts();
            }
        }

        this.UpdateRect();
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
        var content        = this.content;
        var avaliableSpace = this.Size.ClampSubtract(this.staticContent);
        var stack          = this.Target.Style.Stack ?? StackKind.Horizontal;

        foreach (var dependent in this.dependents)
        {
            var size    = dependent.Size;
            var padding = dependent.padding;

            if (!this.contentDependent.HasFlag(Dependency.Width) || stack == StackKind.Vertical)
            {
                CalculatePendingPaddingHorizontal(dependent, this.Size, ref padding);
                CalculatePendingMarginHorizontal(dependent, stack, this.Size, ref content);

                if (dependent.parentDependent.HasFlag(Dependency.Width))
                {
                    var modified = false;

                    if (dependent.Target.Style.Size?.Width?.TryGetPercentage(out var percentage) ?? false)
                    {
                        size.Width = (uint)(this.Size.Width * percentage);

                        if ((dependent.Target.Style.MinSize?.Width?.TryGetPixel(out var min) ?? false) && (this.Target.Style.MaxSize?.Width?.TryGetPixel(out var max) ?? false))
                        {
                            if (size.Width < min)
                            {
                                size.Width = min;
                            }
                            else if (size.Width > max)
                            {
                                size.Width = max;
                            }
                        }
                        else if (dependent.Target.Style.MinSize?.Width?.TryGetPixel(out min) ?? false)
                        {
                            if (size.Width < min)
                            {
                                size.Width = min;
                            }
                        }
                        else if (dependent.Target.Style.MaxSize?.Width?.TryGetPixel(out max) ?? false)
                        {
                            if (size.Width > max)
                            {
                                size.Width = max;
                            }
                        }

                        modified = true;
                    }
                    else if ((dependent.Target.Style.MinSize?.Width?.TryGetPercentage(out var min) ?? false) && (dependent.Target.Style.MaxSize?.Width?.TryGetPercentage(out var max) ?? false))
                    {
                        var minValue = (uint)(this.Size.Width * min);
                        var maxValue = (uint)(this.Size.Width * max);

                        modified = true;

                        if (size.Width < minValue)
                        {
                            size.Width = minValue;
                        }
                        else if (size.Width > maxValue)
                        {
                            size.Width = maxValue;
                        }
                        else
                        {
                            modified = false;
                        }
                    }
                    else if (dependent.Target.Style.MinSize?.Width?.TryGetPercentage(out min) ?? false)
                    {
                        var minValue = (uint)(this.Size.Width * min);

                        if (size.Width < minValue)
                        {
                            size.Width = minValue;

                            modified = true;
                        }
                    }
                    else if (dependent.Target.Style.MaxSize?.Width?.TryGetPercentage(out max) ?? false)
                    {
                        var maxValue = (uint)(this.Size.Width * max);

                        if (size.Width > maxValue)
                        {
                            size.Width = maxValue;

                            modified = true;
                        }
                    }

                    if (modified)
                    {
                        content.Width -= dependent.BoundingsWithMargin.Width;

                        if (stack == StackKind.Horizontal)
                        {
                            if (size.Width < avaliableSpace.Width)
                            {
                                avaliableSpace.Width -= size.Width;
                            }
                            else
                            {
                                size.Width = avaliableSpace.Width;

                                avaliableSpace.Width = 0;
                            }

                            content.Width += size.Width;
                        }
                        else
                        {
                            content.Width = uint.Max(size.Width, content.Width);
                        }

                        size.Width = size.Width
                            .ClampSubtract(dependent.border.Horizontal)
                            .ClampSubtract(dependent.padding.Horizontal)
                            .ClampSubtract(dependent.margin.Horizontal);
                    }
                }
            }

            if (!this.contentDependent.HasFlag(Dependency.Height) || stack == StackKind.Horizontal)
            {
                CalculatePendingPaddingVertical(dependent, this.Size, ref padding);
                CalculatePendingMarginVertical(dependent, stack, this.Size, ref content);

                if (dependent.parentDependent.HasFlag(Dependency.Height))
                {
                    var modified = false;

                    if (dependent.Target.Style.Size?.Height?.TryGetPercentage(out var percentage) ?? false)
                    {
                        size.Height = (uint)(this.Size.Height * percentage);

                        if ((dependent.Target.Style.MinSize?.Height?.TryGetPixel(out var min) ?? false) && (this.Target.Style.MaxSize?.Height?.TryGetPixel(out var max) ?? false))
                        {
                            if (size.Height < min)
                            {
                                size.Height = min;
                            }
                            else if (size.Height > max)
                            {
                                size.Height = max;
                            }
                        }
                        else if (dependent.Target.Style.MinSize?.Height?.TryGetPixel(out min) ?? false)
                        {
                            if (size.Height < min)
                            {
                                size.Height = min;
                            }
                        }
                        else if (dependent.Target.Style.MaxSize?.Height?.TryGetPixel(out max) ?? false)
                        {
                            if (size.Height > max)
                            {
                                size.Height = max;
                            }
                        }

                        modified = true;
                    }
                    else if ((dependent.Target.Style.MinSize?.Height?.TryGetPercentage(out var min) ?? false) && (dependent.Target.Style.MaxSize?.Height?.TryGetPercentage(out var max) ?? false))
                    {
                        var minValue = (uint)(this.Size.Height * min);
                        var maxValue = (uint)(this.Size.Height * max);

                        modified = true;

                        if (size.Height < minValue)
                        {
                            size.Height = minValue;
                        }
                        else if (size.Height > maxValue)
                        {
                            size.Height = maxValue;
                        }
                        else
                        {
                            modified = false;
                        }
                    }
                    else if (dependent.Target.Style.MinSize?.Height?.TryGetPercentage(out min) ?? false)
                    {
                        var minValue = (uint)(this.Size.Height * min);

                        if (size.Height < minValue)
                        {
                            size.Height = minValue;

                            modified = true;
                        }
                    }
                    else if (dependent.Target.Style.MaxSize?.Height?.TryGetPercentage(out max) ?? false)
                    {
                        var maxValue = (uint)(this.Size.Height * max);

                        if (size.Height > maxValue)
                        {
                            size.Height = maxValue;

                            modified = true;
                        }
                    }
                    else
                    {
                        modified = false;
                    }

                    if (modified)
                    {
                        content.Height -= dependent.BoundingsWithMargin.Height;

                        if (stack == StackKind.Vertical)
                        {
                            if (size.Height < avaliableSpace.Height)
                            {
                                avaliableSpace.Height -= size.Height;
                            }
                            else
                            {
                                size.Height = avaliableSpace.Height;

                                avaliableSpace.Height = 0;
                            }

                            content.Height += size.Height;
                        }
                        else
                        {
                            content.Height = uint.Max(size.Height, content.Height);
                        }

                        size.Height = size.Height
                            .ClampSubtract(dependent.border.Vertical)
                            .ClampSubtract(dependent.padding.Vertical)
                            .ClampSubtract(dependent.margin.Vertical);
                    }
                }
            }

            if (size != dependent.Size || padding != dependent.padding)
            {
                dependent.padding = padding;
                dependent.Size    = size;

                dependent.CalculatePendingLayouts();
                dependent.UpdateDisposition();
            }

            dependent.UpdateRect();
            dependent.HasPendingUpdate = false;

            this.CheckHightestInlineChild(stack, dependent.Target);
        }

        this.content = content;
    }

    private void CheckHightestInlineChild(StackKind stack, ContainerNode child)
    {
        if (!child.Layout.IsInline)
        {
            return;
        }

        var hasAlignment = child is Element element
            && element.Style.Alignment.HasValue
            && (
                element.Style.Alignment.Value == AlignmentKind.Center
                || element.Style.Alignment.Value.HasFlag(AlignmentKind.Top)
                || element.Style.Alignment.Value.HasFlag(AlignmentKind.Bottom)
                || stack == StackKind.Vertical && element.Style.Alignment.Value.HasFlag(AlignmentKind.Begin)
                || stack == StackKind.Vertical && element.Style.Alignment.Value.HasFlag(AlignmentKind.Center)
                || stack == StackKind.Vertical && element.Style.Alignment.Value.HasFlag(AlignmentKind.End)
            );

        if (!hasAlignment && (this.hightestInlineChildNode == null || child.Layout.BaseLine < this.hightestInlineChildNode.Layout.BaseLine))
        {
            this.hightestInlineChildNode = child;
        }
    }

    private void UpdateBaseline(StackKind stack)
    {
        if (this.hightestInlineChildNode != null)
        {
            var offset = 0;

            if (this.hightestInlineChildNode is Element element)
            {
                offset = -(int)(element.Layout.padding.Top + element.Layout.border.Top + element.Layout.margin.Top);
            }

            this.BaseLine = offset + this.hightestInlineChildNode.Layout.BaseLine;
        }
    }

    private void UpdateDisposition()
    {
        if (this.renderableNodesCount == 0)
        {
            return;
        }

        var cursor = new Point<float>();
        var size   = this.Size.Cast<float>();
        var stack  = this.Target.Style.Stack ?? StackKind.Horizontal;

        this.UpdateBaseline(stack);

        var avaliableSpace = size - this.content.Cast<float>();

        cursor.X += this.padding.Left + this.border.Left;
        cursor.Y -= this.padding.Top  + this.border.Top;

        foreach (var node in this.Target)
        {
            if (node is not ContainerNode child)
            {
                continue;
            }

            var alignmentType  = AlignmentKind.None;
            var childBoundings = child.Layout.Size;
            var contentOffsetY = 0u;

            RawRectEdges margin  = default;

            if (child is Element element)
            {
                alignmentType  = element.Style?.Alignment ?? AlignmentKind.None;
                margin         = element.Layout.margin;
                contentOffsetY = element.Layout.padding.Top + element.Layout.border.Top + element.Layout.margin.Top;
                childBoundings = element.Layout.BoundingsWithMargin;
            }

            var alignment = GetAlignment(stack, child.Layout, alignmentType, out var hasHorizontalAlignment, out var hasVerticalAlignment);

            Vector2<float> offset;

            if (stack == StackKind.Horizontal)
            {
                avaliableSpace.Width += childBoundings.Width;

                var x = hasHorizontalAlignment ? avaliableSpace.Width.ClampSubtract(childBoundings.Width) * alignment.X : 0;
                var y = hasVerticalAlignment
                    ? size.Height.ClampSubtract(childBoundings.Height) * alignment.Y
                    : child.Layout.IsInline
                        ? -this.BaseLine - (contentOffsetY + -child.Layout.BaseLine)
                        : 0;

                var usedSpace = hasHorizontalAlignment
                    ? float.Max(childBoundings.Width, avaliableSpace.Width - x)
                    : childBoundings.Width;

                avaliableSpace.Width = avaliableSpace.Width.ClampSubtract(usedSpace);

                offset = new(float.Ceiling(cursor.X + x + margin.Left), -float.Ceiling(-cursor.Y + y + margin.Top));

                cursor.X = offset.X + usedSpace - margin.Right;
            }
            else
            {
                avaliableSpace.Height += childBoundings.Height;

                var x = size.Width.ClampSubtract(childBoundings.Width) * alignment.X;
                var y = hasVerticalAlignment ? avaliableSpace.Height.ClampSubtract(childBoundings.Height) * alignment.Y : 0;

                var usedSpace = hasVerticalAlignment
                    ? float.Max(childBoundings.Height, avaliableSpace.Height - y)
                    : childBoundings.Height;

                offset = new(float.Ceiling(cursor.X + x + margin.Left), -float.Ceiling(-cursor.Y + y + margin.Top));

                avaliableSpace.Height = avaliableSpace.Height.ClampSubtract(usedSpace);

                cursor.Y = offset.Y - usedSpace + margin.Bottom;
            }

            child.Layout.Offset = offset;
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
        command.Rect     = new(this.Boundings.Cast<float>(), default);
        command.Border   = this.Target.Style.Border ?? default;
        command.Color    = this.Target.Style.BackgroundColor ?? default;
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
}
