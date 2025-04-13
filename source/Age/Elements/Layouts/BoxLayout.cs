using System.Runtime.CompilerServices;
using Age.Commands;
using Age.Core.Extensions;
using Age.Extensions;
using Age.Numerics;
using Age.Styling;

using static Age.Shaders.CanvasShader;

namespace Age.Elements.Layouts;

internal sealed partial class BoxLayout : Layout
{
    private const StyleProperty LAYOUT_AFFECTED_PROPERTIES =
        StyleProperty.Border
        | StyleProperty.BoxSizing
        | StyleProperty.FontFamily
        | StyleProperty.FontSize
        | StyleProperty.FontWeight
        | StyleProperty.Hidden
        | StyleProperty.Margin
        | StyleProperty.MaxSize
        | StyleProperty.MinSize
        | StyleProperty.Padding
        | StyleProperty.Size
        | StyleProperty.Stack;

    private readonly List<Element> dependents = [];
    private readonly Element       target;

    private RectEdges     border;
    private bool          canScrollX;
    private bool          canScrollY;
    private bool          childsChanged;
    private Size<uint>    content;
    private Dependency    contentDependent = Dependency.Size;
    private bool          dependenciesHasChanged;
    private RectEdges     margin;
    private StencilLayer? ownStencilLayer;
    private RectEdges     padding;
    private Dependency    parentDependent;
    private uint          renderableNodesCount;
    private Size<uint>    size;
    private Size<uint>    staticContent;

    private Size<uint> AbsoluteBoundings
    {
        get
        {
            var size    = this.size;
            var padding = this.padding;
            var margin  = this.margin;
            var style   = this.State.Style;

            if (this.parentDependent.HasFlags(Dependency.Width))
            {
                size.Width = this.content.Width;
            }

            if (this.parentDependent.HasFlags(Dependency.Height))
            {
                size.Height = this.content.Height;
            }

            if (style.Margin?.Left?.Kind == UnitKind.Percentage)
            {
                margin.Left = 0;
            }

            if (style.Margin?.Right?.Kind == UnitKind.Percentage)
            {
                margin.Right = 0;
            }

            if (style.Margin?.Top?.Kind == UnitKind.Percentage)
            {
                margin.Top = 0;
            }

            if (style.Margin?.Bottom?.Kind == UnitKind.Percentage)
            {
                margin.Bottom = 0;
            }

            if (style.Padding?.Left?.Kind == UnitKind.Percentage)
            {
                padding.Left = 0;
            }

            if (style.Padding?.Right?.Kind == UnitKind.Percentage)
            {
                padding.Right = 0;
            }

            if (style.Padding?.Top?.Kind == UnitKind.Percentage)
            {
                padding.Top = 0;
            }

            if (style.Padding?.Bottom?.Kind == UnitKind.Percentage)
            {
                padding.Bottom = 0;
            }

            return new(
                size.Width  + padding.Horizontal + this.border.Horizontal + margin.Horizontal,
                size.Height + padding.Vertical   + this.border.Vertical   + margin.Vertical
            );
        }
    }

    private Size<uint> BoundingsWithMargin =>
        new(
            this.size.Width  + this.padding.Horizontal + this.border.Horizontal + this.margin.Horizontal,
            this.size.Height + this.padding.Vertical   + this.border.Vertical   + this.margin.Vertical
        );

    protected override StencilLayer? ContentStencilLayer => this.ownStencilLayer ?? this.StencilLayer;

    public StyledStateManager State { get; }

    public uint FontSize => this.State.Style.FontSize ?? 16;

    public bool IsScrollable { get; internal set; }

    public Point<uint> ContentOffset
    {
        get;
        set
        {
            value.X = Math<uint>.MinMax(0, this.content.Width.ClampSubtract(this.size.Width),   value.X);
            value.Y = Math<uint>.MinMax(0, this.content.Height.ClampSubtract(this.size.Height), value.Y);

            if (field != value)
            {
                field = value;

                this.ownStencilLayer?.MakeChildrenDirty();
                this.RequestUpdate(false);
            }
        }
    }

    public RectEdges  Border        => this.border;
    public bool       CanScrollX    => this.canScrollY;
    public bool       CanScrollY    => this.canScrollX;
    public Style      ComputedStyle => this.State.Style;
    public Size<uint> Content       => this.content;
    public RectEdges  Margin        => this.margin;
    public RectEdges  Padding       => this.padding;

    public override StencilLayer? StencilLayer
    {
        get => base.StencilLayer;
        set
        {
            if (base.StencilLayer != value)
            {
                var command = this.GetRectCommand();

                base.StencilLayer = command.StencilLayer = value;
            }
        }
    }

    public override bool        IsParentDependent => this.parentDependent != Dependency.None;
    public override Element     Target            => this.target;
    public override Transform2D Transform         => (this.State.Style.Transform ?? new Transform2D()) * base.Transform;



    public BoxLayout(Element target)
    {
        this.target = target;

        this.State = new(target);
        this.State.Changed += this.StyleChanged;
    }

    private static void CalculatePendingHeight(Element dependent, StackKind stack, in uint reference, ref uint height, ref uint content, ref uint avaliableSpace)
    {
        var style = dependent.Layout.ComputedStyle;

        CalculatePendingDimension(style.Size?.Height, style.MinSize?.Height, style.MaxSize?.Height, reference, ref height, out var modified);

        if (modified)
        {
            content = content.ClampSubtract(dependent.Layout.AbsoluteBoundings.Height);

            if (stack == StackKind.Vertical)
            {
                if (height < avaliableSpace)
                {
                    avaliableSpace -= height;
                }
                else
                {
                    height = avaliableSpace;

                    avaliableSpace = 0;
                }

                content += height;
            }
            else
            {
                content = uint.Max(height, content);
            }

            height = height
                .ClampSubtract(dependent.Layout.border.Vertical)
                .ClampSubtract(dependent.Layout.padding.Vertical)
                .ClampSubtract(dependent.Layout.margin.Vertical);
        }
    }

    private static void CalculatePendingPaddingHorizontal(BoxLayout layout, in StyleRectEdges? stylePadding, uint reference, ref RectEdges padding)
    {
        if (stylePadding?.Left?.TryGetPercentage(out var left) == true)
        {
            padding.Left = (uint)(reference * left);
        }

        if (stylePadding?.Right?.TryGetPercentage(out var right) == true)
        {
            padding.Right = (uint)(reference * right);
        }
    }

    private static void CalculatePendingPaddingVertical(BoxLayout layout, in StyleRectEdges? stylePadding, uint reference, ref RectEdges padding)
    {
        if (stylePadding?.Top?.TryGetPercentage(out var top) == true)
        {
            padding.Top = (uint)(reference * top);
        }

        if (stylePadding?.Bottom?.TryGetPercentage(out var bottom) == true)
        {
            padding.Bottom = (uint)(reference * bottom);
        }
    }

    private static void CalculatePendingMarginHorizontal(BoxLayout layout, in StyleRectEdges? styleMargin, StackKind stack, uint reference, ref RectEdges margin, ref Size<uint> contentSize)
    {
        var horizontal = 0u;

        if (styleMargin?.Left?.TryGetPercentage(out var left) == true)
        {
            horizontal += margin.Left = (uint)(reference * left);
        }

        if (styleMargin?.Right?.TryGetPercentage(out var right) == true)
        {
            horizontal += margin.Right = (uint)(reference * right);
        }

        if (horizontal > 0)
        {
            if (stack == StackKind.Horizontal)
            {
                contentSize.Width += horizontal;
            }
            else
            {
                contentSize.Width = uint.Max(layout.size.Width + layout.padding.Horizontal + layout.border.Horizontal + horizontal, contentSize.Width);
            }
        }
    }

    private static void CalculatePendingMarginVertical(BoxLayout layout, in StyleRectEdges? styleMargin, StackKind stack, uint reference, ref RectEdges margin, ref Size<uint> contentSize)
    {
        var vertical = 0u;

        if (styleMargin?.Top?.TryGetPercentage(out var top) == true)
        {
            vertical += margin.Top = (uint)(reference * top);
        }

        if (styleMargin?.Bottom?.TryGetPercentage(out var bottom) == true)
        {
            vertical += margin.Bottom = (uint)(reference * bottom);
        }

        if (vertical > 0)
        {
            if (stack == StackKind.Vertical)
            {
                contentSize.Height += vertical;
            }
            else
            {
                contentSize.Height = uint.Max(layout.size.Height + layout.padding.Vertical + layout.border.Vertical + vertical, contentSize.Height);
            }
        }
    }

    private static void CalculatePendingWidth(Element dependent, StackKind stack, in uint reference, ref uint width, ref uint content, ref uint avaliableSpace)
    {
        var style = dependent.Layout.ComputedStyle;

        CalculatePendingDimension(style.Size?.Width, style.MinSize?.Width, style.MaxSize?.Width, reference, ref width, out var modified);

        if (modified)
        {
            content = content.ClampSubtract(dependent.Layout.AbsoluteBoundings.Width);

            if (stack == StackKind.Horizontal)
            {
                if (width < avaliableSpace)
                {
                    avaliableSpace -= width;
                }
                else
                {
                    width = avaliableSpace;

                    avaliableSpace = 0;
                }

                content += width;
            }
            else
            {
                content = uint.Max(width, content);
            }

            width = width
                .ClampSubtract(dependent.Layout.border.Horizontal)
                .ClampSubtract(dependent.Layout.padding.Horizontal)
                .ClampSubtract(dependent.Layout.margin.Horizontal);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsAnyRelative(Unit? abs, Unit? min, Unit? max) =>
        abs?.Kind == UnitKind.Percentage || min?.Kind == UnitKind.Percentage || max?.Kind == UnitKind.Percentage;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsAllNull(Unit? abs, Unit? min, Unit? max) =>
        abs == null && min == null && max == null;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool HasRelativeEdges(StyleRectEdges? edges) =>
            edges?.Top?.Kind   == UnitKind.Percentage
        || edges?.Right?.Kind  == UnitKind.Percentage
        || edges?.Bottom?.Kind == UnitKind.Percentage
        || edges?.Left?.Kind   == UnitKind.Percentage;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ResolveDimension(uint fontSize, Unit? absUnit, Unit? minUnit, Unit? maxUnit, ref uint value, ref bool resolved)
    {
        if (absUnit?.TryGetPixel(out var pixel) == true)
        {
            value = pixel;

            resolved = true;
        }
        else if (absUnit?.TryGetEm(out var em) == true)
        {
            value = (uint)(em.Value * fontSize);

            resolved = true;
        }
        else
        {
            var min = value;
            var max = value;

            if (minUnit?.TryGetPixel(out var minPixel) == true)
            {
                min = minPixel;
            }
            else if (minUnit?.TryGetEm(out var minEm) == true)
            {
                min = (uint)(minEm.Value * fontSize);
            }

            if (maxUnit?.TryGetPixel(out var maxPixel) == true)
            {
                max = maxPixel;
            }
            else if (maxUnit?.TryGetEm(out var maxEm) == true)
            {
                max = (uint)(maxEm.Value * fontSize);
            }

            if (value < min)
            {
                value = min;

                resolved = true;
            }
            else if (value > max)
            {
                value = max;

                resolved = true;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ResolveRect(uint fontSize, in StyleRectEdges? styleRectEdges, ref RectEdges rectEdges)
    {
        var left   = styleRectEdges?.Left;
        var right  = styleRectEdges?.Right;
        var top    = styleRectEdges?.Top;
        var bottom = styleRectEdges?.Bottom;

        if (left?.TryGetPixel(out var leftPixel) == true)
        {
            rectEdges.Left = leftPixel;
        }
        else if (left?.TryGetEm(out var leftEm) == true)
        {
            rectEdges.Left = (uint)(leftEm * fontSize);
        }

        if (right?.TryGetPixel(out var rightPixel) == true)
        {
            rectEdges.Right = rightPixel;
        }
        else if (right?.TryGetEm(out var rightEm) == true)
        {
            rectEdges.Right = (uint)(rightEm * fontSize);
        }

        if (top?.TryGetPixel(out var topPixel) == true)
        {
            rectEdges.Top = topPixel;
        }
        else if (top?.TryGetEm(out var topEm) == true)
        {
            rectEdges.Top = (uint)(topEm * fontSize);
        }

        if (bottom?.TryGetPixel(out var bottomPixel) == true)
        {
            rectEdges.Bottom = bottomPixel;
        }
        else if (bottom?.TryGetEm(out var bottomEm) == true)
        {
            rectEdges.Bottom = (uint)(bottomEm * fontSize);
        }
    }

    private static void SetStencilLayer(Element target, StencilLayer? stencilLayer)
    {
        var enumerator = target.GetComposedTreeTraversalEnumerator();

        while (enumerator.MoveNext())
        {
            var current = enumerator.Current!;

            if (current.Layout.StencilLayer == stencilLayer)
            {
                enumerator.SkipToNextSibling();
            }
            else if (current is Element element && element.Layout.ownStencilLayer != null)
            {
                if (stencilLayer != null)
                {
                    stencilLayer.AppendChild(element.Layout.ownStencilLayer);
                }
                else
                {
                    element.Layout.ownStencilLayer.Detach();
                }

                element.Layout.StencilLayer = stencilLayer;

                enumerator.SkipToNextSibling();
            }
            else
            {
                current.Layout.StencilLayer = stencilLayer;
            }
        }
    }

    private Point<float> GetAlignment(StackKind stack, AlignmentKind alignmentKind, out AlignmentAxis alignmentAxis)
    {
        var x = -1;
        var y = -1;

        var itemsAlignment = this.State.Style.ItemsAlignment ?? ItemsAlignmentKind.None;

        alignmentAxis = AlignmentAxis.Horizontal | AlignmentAxis.Vertical;

        if (alignmentKind.HasFlags(AlignmentKind.Left) || stack == StackKind.Horizontal && (itemsAlignment == ItemsAlignmentKind.Begin || alignmentKind.HasFlags(AlignmentKind.Start)))
        {
            x = -1;
        }
        else if (alignmentKind.HasFlags(AlignmentKind.Right) || stack == StackKind.Horizontal && (itemsAlignment == ItemsAlignmentKind.End || alignmentKind.HasFlags(AlignmentKind.End)))
        {
            x = 1;
        }
        else if (alignmentKind.HasFlags(AlignmentKind.Center) || stack == StackKind.Vertical && itemsAlignment == ItemsAlignmentKind.Center)
        {
            x = 0;
        }
        else
        {
            alignmentAxis &= ~AlignmentAxis.Horizontal;
        }

        if (alignmentKind.HasFlags(AlignmentKind.Top) || stack == StackKind.Vertical && (itemsAlignment == ItemsAlignmentKind.Begin || alignmentKind.HasFlags(AlignmentKind.Start)))
        {
            y = -1;
        }
        else if (alignmentKind.HasFlags(AlignmentKind.Bottom) || stack == StackKind.Vertical && (itemsAlignment == ItemsAlignmentKind.End || alignmentKind.HasFlags(AlignmentKind.End)))
        {
            y = 1;
        }
        else if (alignmentKind.HasFlags(AlignmentKind.Center) || stack == StackKind.Horizontal && itemsAlignment == ItemsAlignmentKind.Center)
        {
            y = 0;
        }
        else
        {
            if (itemsAlignment == ItemsAlignmentKind.Baseline || alignmentKind.HasFlags(AlignmentKind.Baseline))
            {
                alignmentAxis |= AlignmentAxis.Baseline;
            }

            alignmentAxis &= ~AlignmentAxis.Vertical;
        }

        static float normalize(float value) =>
            (1 + value) / 2;

        return new(normalize(x), normalize(y));
    }

    private RectCommand GetRectCommand()
    {
        if (this.Target.SingleCommand is not RectCommand command)
        {
            this.Target.SingleCommand = command = CommandPool.RectCommand.Get();

            command.Flags           = Flags.ColorAsBackground;
            command.PipelineVariant = PipelineVariant.Color | PipelineVariant.Index;
        }

        return command;
    }

    private void CalculateLayout()
    {
        var stack = this.State.Style.Stack ?? StackKind.Horizontal;

        this.content       = new Size<uint>();
        this.staticContent = new Size<uint>();
        this.BaseLine      = -1;

        var enumerator = this.GetComposedTargetEnumerator();

        while (enumerator.MoveNext())
        {
            var child = enumerator.Current;

            if (child.Layout.Hidden)
            {
                continue;
            }

            child.Layout.UpdateDirtyLayout();

            Size<uint> childSize;

            var dependencies = Dependency.None;

            if (child is Element element)
            {
                var boudings = element.Layout.AbsoluteBoundings;

                childSize.Width  = boudings.Width;
                childSize.Height = boudings.Height;

                dependencies = element.Layout.parentDependent;
            }
            else
            {
                childSize = child.Layout.Boundings;
            }

            if (stack == StackKind.Horizontal)
            {
                if (!dependencies.HasFlags(Dependency.Width))
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
                if (!dependencies.HasFlags(Dependency.Height))
                {
                    this.staticContent.Width = uint.Max(this.staticContent.Width, childSize.Width);
                    this.staticContent.Height += childSize.Height;
                }

                this.content.Width = uint.Max(this.content.Width, childSize.Width);
                this.content.Height += childSize.Height;

                if (child == this.Target.FirstChild)
                {
                    this.CheckHightestInlineChild(stack, child);
                }
            }
        }

        if (this.contentDependent.HasAnyFlag(Dependency.Width | Dependency.Height))
        {
            this.CalculatePendingMargin(ref this.content);
        }

        var size = this.content;

        var resolvedMargin  = this.ResolveMargin();
        var resolvedPadding = this.ResolvePadding();
        var resolvedSize    = this.ResolveSize(ref size);

        var sizeHasChanged = this.size != size;

        this.size = size;

        if (resolvedSize && resolvedMargin && resolvedPadding)
        {
            if (this.dependents.Count > 0 && (sizeHasChanged || this.childsChanged || this.dependenciesHasChanged))
            {
                this.CalculatePendingLayouts();
            }

            this.UpdateBoundings();
        }
    }

    private void CalculatePendingMargin(ref Size<uint> size)
    {
        var contentSize = size;

        var stack = this.State.Style.Stack ?? StackKind.Horizontal;

        foreach (var dependent in this.dependents)
        {
            if (dependent.Layout.parentDependent.HasAnyFlag(Dependency.Padding | Dependency.Margin))
            {
                var margin = dependent.Layout.margin;

                if (!this.parentDependent.HasFlags(Dependency.Width))
                {
                    CalculatePendingMarginHorizontal(dependent.Layout, dependent.Layout.ComputedStyle.Margin, stack, size.Width, ref margin, ref contentSize);
                }

                if (!this.parentDependent.HasFlags(Dependency.Height))
                {
                    CalculatePendingMarginVertical(dependent.Layout, dependent.Layout.ComputedStyle.Margin, stack, size.Height, ref margin, ref contentSize);
                }

                dependent.Layout.margin = margin;
            }
        }

        size = contentSize;
    }

    private void CalculatePendingLayouts()
    {
        var content        = this.content;
        var avaliableSpace = this.size.ClampSubtract(this.staticContent);
        var stack          = this.State.Style.Stack ?? StackKind.Horizontal;

        foreach (var dependent in this.dependents)
        {
            var margin  = dependent.Layout.margin;
            var padding = dependent.Layout.padding;
            var size    = dependent.Layout.size;

            if (!this.contentDependent.HasFlags(Dependency.Width) || stack == StackKind.Vertical)
            {
                if (!this.contentDependent.HasFlags(Dependency.Width))
                {
                    CalculatePendingPaddingHorizontal(dependent.Layout, dependent.Layout.ComputedStyle.Padding, this.size.Width, ref padding);
                    CalculatePendingMarginHorizontal(dependent.Layout, dependent.Layout.ComputedStyle.Margin, stack, this.size.Height, ref margin, ref content);
                }

                if (dependent.Layout.parentDependent.HasFlags(Dependency.Width))
                {
                    CalculatePendingWidth(dependent, stack, this.size.Width, ref size.Width, ref content.Width, ref avaliableSpace.Width);
                }
            }

            if (!this.contentDependent.HasFlags(Dependency.Height) || stack == StackKind.Horizontal)
            {
                if (!this.contentDependent.HasFlags(Dependency.Height))
                {
                    CalculatePendingPaddingVertical(dependent.Layout, dependent.Layout.ComputedStyle.Padding, this.size.Height, ref padding);
                    CalculatePendingMarginVertical(dependent.Layout, dependent.Layout.ComputedStyle.Margin, stack, this.size.Height, ref margin, ref content);
                }

                if (dependent.Layout.parentDependent.HasFlags(Dependency.Height))
                {
                    CalculatePendingHeight(dependent, stack, this.size.Height, ref size.Height, ref content.Height, ref avaliableSpace.Height);
                }
            }

            if (dependent.Layout.dependenciesHasChanged || dependent.Layout.childsChanged || size != dependent.Layout.size || padding != dependent.Layout.padding || margin != dependent.Layout.margin)
            {
                dependent.Layout.childsChanged          = false;
                dependent.Layout.dependenciesHasChanged = false;
                dependent.Layout.size                   = size;
                dependent.Layout.padding                = padding;
                dependent.Layout.margin                 = margin;

                if (dependent.Layout.dependents.Count > 0)
                {
                    dependent.Layout.CalculatePendingLayouts();
                }

                dependent.Layout.UpdateDisposition();
            }

            dependent.Layout.UpdateBoundings();
            dependent.Layout.MakePristine();

            this.CheckHightestInlineChild(stack, dependent);
        }

        this.content = content;
    }

    private static void CalculatePendingDimension(in Unit? absUnit, Unit? minUnit, Unit? maxUnit, uint reference, ref uint dimension, out bool modified)
    {
        modified = false;

        if (absUnit?.TryGetPercentage(out var percentage) == true)
        {
            dimension = (uint)(reference * percentage);

            Pixel min = default;
            Pixel max = default;

            var hasMin = minUnit?.TryGetPixel(out min) == true;
            var hasMax = minUnit?.TryGetPixel(out min) == true;

            if (hasMin && hasMax)
            {
                if (dimension < min)
                {
                    dimension = min;
                }
                else if (dimension > max)
                {
                    dimension = max;
                }
            }
            else if (hasMin)
            {
                if (dimension < min)
                {
                    dimension = min;
                }
            }
            else if (hasMax)
            {
                if (dimension > max)
                {
                    dimension = max;
                }
            }

            modified = true;
        }
        else
        {
            Percentage min = default;
            Percentage max = default;

            var hasMin = minUnit?.TryGetPercentage(out min) == true;
            var hasMax = minUnit?.TryGetPercentage(out min) == true;

            if (hasMin && hasMax)
            {
                var minValue = (uint)(reference * min);
                var maxValue = (uint)(reference * max);

                modified = true;

                if (dimension < minValue)
                {
                    dimension = minValue;
                }
                else if (dimension > maxValue)
                {
                    dimension = maxValue;
                }
                else
                {
                    modified = false;
                }
            }
            else if (hasMin)
            {
                var minValue = (uint)(reference * min);

                if (dimension < minValue)
                {
                    dimension = minValue;

                    modified = true;
                }
            }
            else if (hasMax)
            {
                var maxValue = (uint)(reference * max);

                if (dimension > maxValue)
                {
                    dimension = maxValue;

                    modified = true;
                }
            }
        }
    }

    private void CheckHightestInlineChild(StackKind stack, Layoutable child)
    {
        if (child.Layout.BaseLine == -1)
        {
            return;
        }

        var baseline     = child.Layout.BaseLine;
        var hasAlignment = false;

        if (child is Element element)
        {
            var alignment = element.Layout.ComputedStyle.Alignment;

            hasAlignment = alignment.HasValue
                && (
                    alignment.Value == AlignmentKind.Center
                    || alignment.Value.HasAnyFlag(AlignmentKind.Top | AlignmentKind.Bottom)
                    || stack == StackKind.Vertical && alignment.Value.HasAnyFlag(AlignmentKind.Start | AlignmentKind.Center | AlignmentKind.End)
                );

            baseline += (int)(element.Layout.padding.Top + element.Layout.border.Top + element.Layout.margin.Top);
        }

        if (!hasAlignment && baseline > this.BaseLine)
        {
            this.BaseLine = baseline;
        }
    }

    private TargetEnumerator GetComposedTargetEnumerator() =>
        new(this.Target);

    private void OnScroll(in MouseEvent mouseEvent)
    {
        if (!this.target.IsHovered)
        {
            return;
        }

        if (this.State.Style.Overflow is OverflowKind.Scroll or OverflowKind.ScrollX && mouseEvent.KeyStates.HasFlags(Platforms.Display.MouseKeyStates.Shift))
        {
            this.ContentOffset = this.ContentOffset with
            {
                X = (uint)(this.ContentOffset.X + 10 * -mouseEvent.Delta)
            };
        }
        else if (this.State.Style.Overflow is OverflowKind.Scroll or OverflowKind.ScrollY)
        {
            this.ContentOffset = this.ContentOffset with
            {
                Y = (uint)(this.ContentOffset.Y + 10 * -mouseEvent.Delta)
            };
        }
    }

    private void StyleChanged(StyleProperty property)
    {
        if (!this.Target.IsConnected)
        {
            return;
        }

        var style = this.State.Style;

        var hidden = style.Hidden == true;

        if (property.HasFlags(StyleProperty.Border))
        {
            this.border = new()
            {
                Top    = style.Border?.Top.Thickness ?? 0,
                Right  = style.Border?.Right.Thickness ?? 0,
                Bottom = style.Border?.Bottom.Thickness ?? 0,
                Left   = style.Border?.Left.Thickness ?? 0,
            };
        }

        if (property.HasFlags(StyleProperty.Cursor) && this.target.IsHovered)
        {
            this.SetCursor(style.Cursor);
        }

        var hasSizeChanges    = property.HasAnyFlag(StyleProperty.Size | StyleProperty.MinSize | StyleProperty.MaxSize);
        var hasMarginChanges  = property.HasFlags(StyleProperty.Margin);
        var hasPaddingChanges = property.HasFlags(StyleProperty.Padding);

        if (hasSizeChanges || hasMarginChanges || hasPaddingChanges)
        {
            var oldParentDependent  = this.parentDependent;
            var oldContentDependent = this.contentDependent;

            if (hasSizeChanges)
            {
                this.contentDependent = Dependency.None;
                this.parentDependent  = Dependency.None;

                var absWidth = style.Size?.Width;
                var minWidth = style.MinSize?.Width;
                var maxWidth = style.MaxSize?.Width;

                var absHeight = style.Size?.Height;
                var minHeight = style.MinSize?.Height;
                var maxHeight = style.MaxSize?.Height;

                if (IsAllNull(absWidth, minWidth, maxWidth))
                {
                    this.contentDependent |= Dependency.Width;
                }
                else if (IsAnyRelative(absWidth, minWidth, maxWidth))
                {
                    this.parentDependent |= Dependency.Width;
                }

                if (IsAllNull(absHeight, minHeight, maxHeight))
                {
                    this.contentDependent |= Dependency.Height;
                }
                else if (IsAnyRelative(absHeight, minHeight, maxHeight))
                {
                    this.parentDependent |= Dependency.Height;
                }
            }

            if (hasMarginChanges && HasRelativeEdges(style.Margin))
            {
                this.parentDependent |= Dependency.Margin;
            }

            if (hasPaddingChanges && HasRelativeEdges(style.Padding))
            {
                this.parentDependent |= Dependency.Padding;
            }

            var justHidden      = hidden && !this.Hidden;
            var justUnhidden    = !hidden && this.Hidden;
            var justUndependent = oldParentDependent != Dependency.None && this.parentDependent == Dependency.None;
            var justDependent   = oldParentDependent == Dependency.None && this.parentDependent != Dependency.None;

            if (this.Parent != null)
            {
                this.Parent.dependenciesHasChanged = oldContentDependent != this.contentDependent || oldParentDependent != this.parentDependent;

                if (justUnhidden || justDependent)
                {
                    if (justUnhidden)
                    {
                        this.Parent.renderableNodesCount++;
                    }

                    if (this.parentDependent != Dependency.None)
                    {
                        var dependents = this.Target.AssignedSlot?.Layout.dependents ?? this.Parent.dependents;

                        dependents.Add(this.Target);
                        dependents.Sort();
                    }
                }
                else if (justHidden || justUndependent)
                {
                    if (justHidden)
                    {
                        this.Parent.renderableNodesCount--;
                    }

                    var dependents = this.Target.AssignedSlot?.Layout.dependents ?? this.Parent.dependents;

                    dependents.Remove(this.Target);
                }
            }
        }

        if (property.HasFlags(StyleProperty.Overflow))
        {
            this.canScrollX = style.Overflow is OverflowKind.Scroll or OverflowKind.ScrollX;
            this.canScrollY = style.Overflow is OverflowKind.Scroll or OverflowKind.ScrollY;

            var currentIsScrollable = style.Overflow is not null and not OverflowKind.None and not OverflowKind.Clipping && this.contentDependent != (Dependency.Width | Dependency.Height);

            if (currentIsScrollable != this.IsScrollable)
            {
                if (currentIsScrollable)
                {
                    this.target.Scrolled += this.OnScroll;
                }
                else
                {
                    this.target.Scrolled -= this.OnScroll;
                    this.ContentOffset = default;
                }

                this.IsScrollable = currentIsScrollable;
            }

            if (style.Overflow is not (null or OverflowKind.None) && this.contentDependent != (Dependency.Width | Dependency.Height))
            {
                if (this.ownStencilLayer == null)
                {
                    this.ownStencilLayer = new StencilLayer(this.Target);

                    this.StencilLayer?.AppendChild(this.ownStencilLayer);
                }
            }
            else if (this.ownStencilLayer != null)
            {
                this.ownStencilLayer.Detach();
                this.ownStencilLayer.Dispose();

                this.ownStencilLayer = null;
            }

            SetStencilLayer(this.Target, this.ContentStencilLayer);
        }

        var affectsBoundings = property.HasAnyFlag(LAYOUT_AFFECTED_PROPERTIES);

        if (hidden)
        {
            this.RequestUpdate(affectsBoundings);

            this.Hidden = hidden;
        }
        else
        {
            this.Hidden = hidden;

            this.UpdateRect();
            this.RequestUpdate(affectsBoundings);
        }

        this.Target.Visible = !hidden;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool ResolveHeight(ref uint value)
    {
        var resolved = !this.parentDependent.HasFlags(Dependency.Height);

        if (!this.contentDependent.HasFlags(Dependency.Height))
        {
            var style = this.ComputedStyle;

            ResolveDimension(this.FontSize, style.Size?.Height, style.MinSize?.Height, style.MaxSize?.Height, ref value, ref resolved);

            if (this.State.Style.BoxSizing == BoxSizing.Border)
            {
                value = value.ClampSubtract(this.border.Vertical);
            }
        }

        return resolved;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool ResolveSize(ref Size<uint> size)
    {
        var resolvedWidth  = this.ResolveWidth(ref size.Width);
        var resolvedHeight = this.ResolveHeight(ref size.Height);

        return resolvedWidth && resolvedHeight;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool ResolveMargin()
    {
        ResolveRect(this.FontSize, this.ComputedStyle.Margin, ref this.margin);

        return !this.parentDependent.HasFlags(Dependency.Margin);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool ResolvePadding()
    {
        ResolveRect(this.FontSize, this.ComputedStyle.Padding, ref this.padding);

        return !this.parentDependent.HasFlags(Dependency.Padding);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool ResolveWidth(ref uint value)
    {
        var resolved = !this.parentDependent.HasFlags(Dependency.Width);

        if (!this.contentDependent.HasFlags(Dependency.Width))
        {
            var style = this.ComputedStyle;

            ResolveDimension(this.FontSize, style.Size?.Width, style.MinSize?.Width, style.MaxSize?.Width, ref value, ref resolved);

            if (this.State.Style.BoxSizing == BoxSizing.Border)
            {
                value = value.ClampSubtract(this.border.Horizontal);
            }
        }

        return resolved;
    }

    private void UpdateDisposition()
    {
        if (this.renderableNodesCount == 0)
        {
            return;
        }

        var cursor               = new Point<float>();
        var size                 = this.size;
        var stack                = this.State.Style.Stack ?? StackKind.Horizontal;
        var contentJustification = this.State.Style.ContentJustification ?? ContentJustificationKind.None;

        var avaliableSpace = stack == StackKind.Horizontal
            ? new Size<float>(size.Width.ClampSubtract(this.content.Width), size.Height)
            : new Size<float>(size.Width, size.Height.ClampSubtract(this.content.Height));

        cursor.X += this.padding.Left + this.border.Left;
        cursor.Y -= this.padding.Top  + this.border.Top;

        var index = 0;

        var enumerator = this.GetComposedTargetEnumerator();

        while (enumerator.MoveNext())
        {
            var node = enumerator.Current;

            if (node is not Layoutable child || child.Layout.Hidden)
            {
                continue;
            }

            var alignmentType  = AlignmentKind.None;
            var childBoundings = child.Layout.Boundings;
            var contentOffsetY = 0u;

            RectEdges margin = default;

            if (child is Element element)
            {
                margin         = element.Layout.margin;
                contentOffsetY = element.Layout.padding.Top + element.Layout.border.Top + element.Layout.margin.Top;
                childBoundings = element.Layout.BoundingsWithMargin;
                alignmentType  = element.Layout.ComputedStyle.Alignment ?? AlignmentKind.None;
            }

            var alignment = this.GetAlignment(stack, alignmentType, out var alignmentAxis);

            var position  = new Vector2<float>();
            var usedSpace = new Size<float>();

            if (stack == StackKind.Horizontal)
            {
                if (contentJustification == ContentJustificationKind.None)
                {
                    avaliableSpace.Width += childBoundings.Width;

                    if (alignmentAxis.HasFlags(AlignmentAxis.Horizontal))
                    {
                        position.X = avaliableSpace.Width.ClampSubtract(childBoundings.Width) * alignment.X;
                    }
                }
                else
                {
                    if (contentJustification == ContentJustificationKind.End && index == 0)
                    {
                        position.X = avaliableSpace.Width;
                    }
                    else if (contentJustification == ContentJustificationKind.Center && index == 0)
                    {
                        position.X = avaliableSpace.Width / 2;
                    }
                    else if (contentJustification == ContentJustificationKind.SpaceAround)
                    {
                        position.X = (index == 0 ? 1 : 2) * avaliableSpace.Width / (this.renderableNodesCount * 2);
                    }
                    else if (contentJustification == ContentJustificationKind.SpaceBetween && index > 0)
                    {
                        position.X = avaliableSpace.Width / (this.renderableNodesCount - 1);
                    }
                    else if (contentJustification == ContentJustificationKind.SpaceEvenly)
                    {
                        position.X = avaliableSpace.Width / (this.renderableNodesCount + 1);
                    }
                }

                if (alignmentAxis.HasFlags(AlignmentAxis.Vertical))
                {
                    position.Y = size.Height.ClampSubtract(childBoundings.Height) * alignment.Y;
                }
                else if (alignmentAxis.HasFlags(AlignmentAxis.Baseline) && child.Layout.BaseLine > -1)
                {
                    position.Y = this.BaseLine - (contentOffsetY + child.Layout.BaseLine);
                }

                usedSpace.Width = alignmentAxis.HasFlags(AlignmentAxis.Horizontal)
                    ? float.Max(childBoundings.Width, avaliableSpace.Width - position.X)
                    : childBoundings.Width;

                if (contentJustification == ContentJustificationKind.None)
                {
                    avaliableSpace.Width = avaliableSpace.Width.ClampSubtract((uint)usedSpace.Width);
                }
            }
            else
            {
                position.X = size.Width.ClampSubtract(childBoundings.Width) * alignment.X;

                if (contentJustification == ContentJustificationKind.None)
                {
                    avaliableSpace.Height += childBoundings.Height;

                    if (alignmentAxis.HasFlags(AlignmentAxis.Vertical))
                    {
                        position.Y = (uint)(avaliableSpace.Height.ClampSubtract(childBoundings.Height) * alignment.Y);
                    }
                }
                else
                {
                    if (contentJustification == ContentJustificationKind.End && index == 0)
                    {
                        position.Y = avaliableSpace.Height;
                    }
                    else if (contentJustification == ContentJustificationKind.Center && index == 0)
                    {
                        position.Y = avaliableSpace.Height / 2;
                    }
                    else if (contentJustification == ContentJustificationKind.SpaceAround)
                    {
                        position.Y = (index == 0 ? 1 : 2) * avaliableSpace.Height / (this.renderableNodesCount * 2);
                    }
                    else if (contentJustification == ContentJustificationKind.SpaceBetween && index > 0)
                    {
                        position.Y = avaliableSpace.Height / (this.renderableNodesCount - 1);
                    }
                    else if (contentJustification == ContentJustificationKind.SpaceEvenly)
                    {
                        position.Y = avaliableSpace.Height / (this.renderableNodesCount + 1);
                    }
                }

                usedSpace.Height = alignmentAxis.HasFlags(AlignmentAxis.Vertical)
                    ? float.Max(childBoundings.Height, avaliableSpace.Height - position.Y)
                    : childBoundings.Height;

                if (contentJustification == ContentJustificationKind.None)
                {
                    avaliableSpace.Height = avaliableSpace.Height.ClampSubtract((uint)usedSpace.Height);
                }
            }

            child.Layout.Offset = new(float.Round(cursor.X + position.X + margin.Left), -float.Round(-cursor.Y + position.Y + margin.Top));

            if (stack == StackKind.Horizontal)
            {
                cursor.X = child.Layout.Offset.X + usedSpace.Width - margin.Right;
            }
            else
            {
                cursor.Y = child.Layout.Offset.Y - usedSpace.Height + margin.Bottom;
            }

            index++;
        }
    }

    private void UpdateBoundings()
    {
        this.Boundings = new(
            this.size.Width + this.padding.Horizontal + this.border.Horizontal,
            this.size.Height + this.padding.Vertical + this.border.Vertical
        );

        this.UpdateRect();
    }

    private void UpdateRect()
    {
        var command = this.GetRectCommand();

        var isDrawable = this.State.Style.Border.HasValue || this.State.Style.BackgroundColor.HasValue;

        if (isDrawable)
        {
            command.Rect            = new(this.Boundings.Cast<float>(), default);
            command.Border          = this.State.Style.Border ?? default;
            command.Color           = this.State.Style.BackgroundColor ?? default;
            command.PipelineVariant |= PipelineVariant.Color;
        }
        else
        {
            command.PipelineVariant &= ~PipelineVariant.Color;
        }

        command.StencilLayer = this.StencilLayer;

        this.ownStencilLayer?.MakeDirty();
    }

    protected override void Disposed()
    {
        this.ownStencilLayer?.Dispose();

        foreach (var item in this.target.Commands)
        {
            CommandPool.RectCommand.Return((RectCommand)item);
        }

        this.target.Commands.Clear();
    }

    public void LayoutableAppended(Layoutable layoutable)
    {
        if (!layoutable.Layout.Hidden)
        {
            this.childsChanged = true;
            this.renderableNodesCount++;
            this.RequestUpdate(true);
        }
    }

    public void LayoutableRemoved(Layoutable layoutable)
    {
        if (layoutable is Element element)
        {
            this.ElementRemoved(element);
        }

        if (!layoutable.Layout.Hidden)
        {
            this.childsChanged = true;
            this.renderableNodesCount--;
            this.RequestUpdate(true);
        }
    }

    public void ElementRemoved(Element element)
    {
        if (!element.Layout.Hidden && element.Layout.parentDependent != Dependency.None)
        {
            this.dependents.Remove(element);
        }
    }

    public override void TargetConnected()
    {
        base.TargetConnected();

        this.State.Update();

        if (this.ownStencilLayer != null)
        {
            this.StencilLayer?.AppendChild(this.ownStencilLayer);
        }
    }

    public void TargetIndexed()
    {
        var command = this.GetRectCommand();

        command.ObjectId = this.Target.Index == -1
            ? default
            : this.State.Style.Border.HasValue || this.State.Style.BackgroundColor.HasValue ? (uint)(this.Target.Index + 1) : 0;
    }

    public void TargetMouseOut() { }

    public void TargetMouseOver() =>
        this.SetCursor(this.State.Style.Cursor);

    public override void Update()
    {
        this.CalculateLayout();

        if (this.parentDependent == Dependency.None)
        {
            this.UpdateDisposition();
            this.childsChanged          = false;
            this.dependenciesHasChanged = false;
        }
    }
}
