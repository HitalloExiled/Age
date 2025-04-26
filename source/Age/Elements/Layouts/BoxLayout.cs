using System.Numerics;
using System.Runtime.CompilerServices;
using Age.Commands;
using Age.Core.Extensions;
using Age.Extensions;
using Age.Numerics;
using Age.Resources;
using Age.Storage;
using Age.Styling;
using static Age.Services.TextStorage;
using static Age.Shaders.CanvasShader;
using StyleImage = Age.Styling.Image;

namespace Age.Elements.Layouts;

internal sealed partial class BoxLayout(Element target) : StyledLayout(target)
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

    private RectEdges     border;
    private bool          canScrollX;
    private bool          canScrollY;
    private bool          childsChanged;
    private Size<uint>    content;
    private Dependency    contentDependencies = Dependency.Size;
    private bool          dependenciesHasChanged;
    private LayoutCommand layoutCommands;
    private RectEdges     margin;
    private StencilLayer? ownStencilLayer;
    private RectEdges     padding;
    private Dependency    parentDependencies;
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
            var style   = this.ComputedStyle;

            if (this.parentDependencies.HasFlags(Dependency.Width))
            {
                size.Width = this.content.Width;
            }

            if (this.parentDependencies.HasFlags(Dependency.Height))
            {
                size.Height = this.content.Height;
            }

            checkRect(style.Margin,  ref padding);
            checkRect(style.Padding, ref padding);

            return new(
                size.Width  + padding.Horizontal + this.border.Horizontal + margin.Horizontal,
                size.Height + padding.Vertical   + this.border.Vertical   + margin.Vertical
            );

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static void check(Unit? unit, ref uint value)
            {
                if (unit?.Kind == UnitKind.Percentage)
                {
                    value = 0;
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static void checkRect(StyleRectEdges? styleRectEdges, ref RectEdges rectEdges)
            {
                check(styleRectEdges?.Left,   ref rectEdges.Left);
                check(styleRectEdges?.Right,  ref rectEdges.Right);
                check(styleRectEdges?.Top,    ref rectEdges.Top);
                check(styleRectEdges?.Bottom, ref rectEdges.Bottom);
            }
        }
    }

    private Size<uint> BoundingsWithMargin =>
        new(
            this.size.Width  + this.padding.Horizontal + this.border.Horizontal + this.margin.Horizontal,
            this.size.Height + this.padding.Vertical   + this.border.Vertical   + this.margin.Vertical
        );

    private Size<uint> SizeWithPadding =>
        new(
            this.size.Width  + this.padding.Horizontal,
            this.size.Height + this.padding.Vertical
        );

    private bool IsDrawable => this.ComputedStyle.Border != null || this.ComputedStyle.BackgroundColor.HasValue || this.ComputedStyle.BackgroundImage != null;

    protected override StencilLayer? ContentStencilLayer => this.ownStencilLayer ?? this.StencilLayer;

    public uint FontSize => this.ComputedStyle.FontSize ?? 16;

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

    public RectEdges  Border     => this.border;
    public bool       CanScrollX => this.canScrollY;
    public bool       CanScrollY => this.canScrollX;
    public Size<uint> Content    => this.content;
    public RectEdges  Margin     => this.margin;
    public RectEdges  Padding    => this.padding;

    public override StencilLayer? StencilLayer
    {
        get => base.StencilLayer;
        set
        {
            if (base.StencilLayer != value)
            {
                base.StencilLayer = this.GetLayoutCommandBox().StencilLayer = value;
            }
        }
    }

    public override bool        IsParentDependent => this.parentDependencies != Dependency.None;
    public override Transform2D Transform         => (this.ComputedStyle.Transform ?? new Transform2D()) * base.Transform;

    private static void CalculatePendingHeight(Element dependent, StackDirection direction, in uint reference, ref uint height, ref uint content, ref uint avaliableSpace)
    {
        var style = dependent.Layout.ComputedStyle;

        CalculatePendingDimension(style.Size?.Height, style.MinSize?.Height, style.MaxSize?.Height, reference, ref height, out var modified);

        if (modified)
        {
            content = content.ClampSubtract(dependent.Layout.AbsoluteBoundings.Height);

            if (direction == StackDirection.Vertical)
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

    private static void CalculatePendingMarginHorizontal(BoxLayout layout, in StyleRectEdges? styleMargin, StackDirection direction, uint reference, ref RectEdges margin, ref Size<uint> contentSize)
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
            if (direction == StackDirection.Horizontal)
            {
                contentSize.Width += horizontal;
            }
            else
            {
                contentSize.Width = uint.Max(layout.size.Width + layout.padding.Horizontal + layout.border.Horizontal + horizontal, contentSize.Width);
            }
        }
    }

    private static void CalculatePendingMarginVertical(BoxLayout layout, in StyleRectEdges? styleMargin, StackDirection direction, uint reference, ref RectEdges margin, ref Size<uint> contentSize)
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
            if (direction == StackDirection.Vertical)
            {
                contentSize.Height += vertical;
            }
            else
            {
                contentSize.Height = uint.Max(layout.size.Height + layout.padding.Vertical + layout.border.Vertical + vertical, contentSize.Height);
            }
        }
    }

    private static void CalculatePendingWidth(Element dependent, StackDirection direction, in uint reference, ref uint width, ref uint content, ref uint avaliableSpace)
    {
        var style = dependent.Layout.ComputedStyle;

        CalculatePendingDimension(style.Size?.Width, style.MinSize?.Width, style.MaxSize?.Width, reference, ref width, out var modified);

        if (modified)
        {
            content = content.ClampSubtract(dependent.Layout.AbsoluteBoundings.Width);

            if (direction == StackDirection.Horizontal)
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

    private static void PropageteStencilLayer(Element target, StencilLayer? stencilLayer)
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

    private void ResolveImageSize(Image image, in Size<float> size, out Rect<float> rect, out UVRect uv)
    {
        var imageSize = image.Size;
        var repeat    = image.Repeat;

        switch (image.Size.Kind)
        {
            case ImageSizeKind.Fit:
                {
                    var boundings = this.SizeWithPadding.Cast<float>();
                    var offset    = new Point<float>(this.border.Left, -this.border.Top);

                    rect = new(boundings, offset);
                    uv   = UVRect.Normalized;

                    break;
                }

            case ImageSizeKind.KeepAspect:
                {
                    var boundings = this.SizeWithPadding.Cast<float>();

                    uv = UVRect.Normalized;

                    if (boundings == default)
                    {
                        rect = default;

                        break;
                    }

                    var scale       = float.Min(boundings.Width, boundings.Height) / float.Max(size.Width, size.Height);
                    var correctSize = size * scale;

                    var offset = (Point<float>)(new Point<float>(this.border.Left, this.border.Top) + (boundings - correctSize) / 2);

                    rect = new(correctSize, offset.InvertedY);

                    break;
                }
            default:
                {
                    var boundings = this.SizeWithPadding.Cast<float>();

                    if (boundings == default)
                    {
                        rect = default;
                        uv   = default;

                        break;
                    }

                    var width  = ResolveUnit(imageSize.Value.Width,  (uint)boundings.Width,  this.FontSize);
                    var height = ResolveUnit(imageSize.Value.Height, (uint)boundings.Height, this.FontSize);

                    var p1x = 0f;
                    var p2x = 1f;
                    var p3x = p2x;
                    var p4x = p1x;

                    var p1y = 0f;
                    var p2y = p1y;
                    var p3y = 1f;
                    var p4y = p3y;

                    var offset = new Point<float>();

                    if (repeat.HasFlags(ImageRepeat.RepeatX))
                    {
                        var scale = boundings.Width / width;

                        p1x = (1 - scale) / 2;
                        p2x = (1 + scale) / 2;
                        p3x = p2x;
                        p4x = p1x;

                        width = (uint)boundings.Width;

                        offset.X = this.border.Left;
                    }
                    else
                    {
                        offset.X = this.border.Left + (boundings.Width - width) / 2;
                    }

                    if (repeat.HasFlags(ImageRepeat.RepeatY))
                    {
                        var scale = boundings.Height / height;

                        p1y = (1 - scale) / 2;
                        p2y = p1y;
                        p3y = (1 + scale) / 2;
                        p4y = p3y;

                        height = (uint)boundings.Height;

                        offset.Y = this.border.Top;
                    }
                    else
                    {
                        offset.Y = this.border.Top + (boundings.Height - height) / 2;
                    }

                    rect = new(new(width, height), offset.InvertedY);
                    uv   = new()
                    {
                        P1 = new(p1x, p1y),
                        P2 = new(p2x, p2y),
                        P3 = new(p3x, p3y),
                        P4 = new(p4x, p4y),
                    };

                    break;
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

    private static uint ResolveUnit(Unit? unit, uint size, uint fontSize)
    {
        if (!unit.HasValue)
        {
            return size;
        }

        if (unit.Value.TryGetPixel(out var pixel))
        {
            return pixel;
        }

        if (unit.Value.TryGetPercentage(out var percentage))
        {
            return (uint)(percentage * size);
        }

        if (unit.Value.TryGetEm(out var em))
        {
            return (uint)(em * fontSize);
        }

        return 0;
    }

    private void CalculateLayout()
    {
        var direction = this.ComputedStyle.StackDirection ?? StackDirection.Horizontal;

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

                dependencies = element.Layout.parentDependencies;
            }
            else
            {
                childSize = child.Layout.Boundings;
            }

            if (direction == StackDirection.Horizontal)
            {
                if (!dependencies.HasFlags(Dependency.Width))
                {
                    this.staticContent.Width += childSize.Width;
                    this.staticContent.Height = uint.Max(this.staticContent.Height, childSize.Height);
                }

                this.content.Width += childSize.Width;
                this.content.Height = uint.Max(this.content.Height, childSize.Height);

                this.CheckHightestInlineChild(direction, child);
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
                    this.CheckHightestInlineChild(direction, child);
                }
            }
        }

        if (this.contentDependencies.HasAnyFlag(Dependency.Width | Dependency.Height))
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

        var direction = this.ComputedStyle.StackDirection ?? StackDirection.Horizontal;

        foreach (var dependent in this.dependents)
        {
            if (dependent.Layout.parentDependencies.HasAnyFlag(Dependency.Padding | Dependency.Margin))
            {
                var margin = dependent.Layout.margin;

                if (!this.parentDependencies.HasFlags(Dependency.Width))
                {
                    CalculatePendingMarginHorizontal(dependent.Layout, dependent.Layout.ComputedStyle.Margin, direction, size.Width, ref margin, ref contentSize);
                }

                if (!this.parentDependencies.HasFlags(Dependency.Height))
                {
                    CalculatePendingMarginVertical(dependent.Layout, dependent.Layout.ComputedStyle.Margin, direction, size.Height, ref margin, ref contentSize);
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
        var direction      = this.ComputedStyle.StackDirection ?? StackDirection.Horizontal;

        foreach (var dependent in this.dependents)
        {
            var margin  = dependent.Layout.margin;
            var padding = dependent.Layout.padding;
            var size    = dependent.Layout.size;

            if (!this.contentDependencies.HasFlags(Dependency.Width) || direction == StackDirection.Vertical)
            {
                if (!this.contentDependencies.HasFlags(Dependency.Width))
                {
                    CalculatePendingPaddingHorizontal(dependent.Layout, dependent.Layout.ComputedStyle.Padding, this.size.Width, ref padding);
                    CalculatePendingMarginHorizontal(dependent.Layout, dependent.Layout.ComputedStyle.Margin, direction, this.size.Height, ref margin, ref content);
                }

                if (dependent.Layout.parentDependencies.HasFlags(Dependency.Width))
                {
                    CalculatePendingWidth(dependent, direction, this.size.Width, ref size.Width, ref content.Width, ref avaliableSpace.Width);
                }
            }

            if (!this.contentDependencies.HasFlags(Dependency.Height) || direction == StackDirection.Horizontal)
            {
                if (!this.contentDependencies.HasFlags(Dependency.Height))
                {
                    CalculatePendingPaddingVertical(dependent.Layout, dependent.Layout.ComputedStyle.Padding, this.size.Height, ref padding);
                    CalculatePendingMarginVertical(dependent.Layout, dependent.Layout.ComputedStyle.Margin, direction, this.size.Height, ref margin, ref content);
                }

                if (dependent.Layout.parentDependencies.HasFlags(Dependency.Height))
                {
                    CalculatePendingHeight(dependent, direction, this.size.Height, ref size.Height, ref content.Height, ref avaliableSpace.Height);
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

            this.CheckHightestInlineChild(direction, dependent);
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

    private void CheckHightestInlineChild(StackDirection direction, Layoutable child)
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
                    alignment.Value == Alignment.Center
                    || alignment.Value.HasAnyFlag(Alignment.Top | Alignment.Bottom)
                    || direction == StackDirection.Vertical && alignment.Value.HasAnyFlag(Alignment.Start | Alignment.Center | Alignment.End)
                );

            baseline += (int)(element.Layout.padding.Top + element.Layout.border.Top + element.Layout.margin.Top);
        }

        if (!hasAlignment && baseline > this.BaseLine)
        {
            this.BaseLine = baseline;
        }
    }

    private Point<float> GetAlignment(StackDirection direction, Alignment alignmentKind, out AlignmentAxis alignmentAxis)
    {
        var x = -1;
        var y = -1;

        var itemsAlignment = this.ComputedStyle.ItemsAlignment ?? ItemsAlignment.None;

        alignmentAxis = AlignmentAxis.Horizontal | AlignmentAxis.Vertical;

        if (alignmentKind.HasFlags(Alignment.Left) || direction == StackDirection.Horizontal && (itemsAlignment == ItemsAlignment.Begin || alignmentKind.HasFlags(Alignment.Start)))
        {
            x = -1;
        }
        else if (alignmentKind.HasFlags(Alignment.Right) || direction == StackDirection.Horizontal && (itemsAlignment == ItemsAlignment.End || alignmentKind.HasFlags(Alignment.End)))
        {
            x = 1;
        }
        else if (alignmentKind.HasFlags(Alignment.Center) || direction == StackDirection.Vertical && itemsAlignment == ItemsAlignment.Center)
        {
            x = 0;
        }
        else
        {
            alignmentAxis &= ~AlignmentAxis.Horizontal;
        }

        if (alignmentKind.HasFlags(Alignment.Top) || direction == StackDirection.Vertical && (itemsAlignment == ItemsAlignment.Begin || alignmentKind.HasFlags(Alignment.Start)))
        {
            y = -1;
        }
        else if (alignmentKind.HasFlags(Alignment.Bottom) || direction == StackDirection.Vertical && (itemsAlignment == ItemsAlignment.End || alignmentKind.HasFlags(Alignment.End)))
        {
            y = 1;
        }
        else if (alignmentKind.HasFlags(Alignment.Center) || direction == StackDirection.Horizontal && itemsAlignment == ItemsAlignment.Center)
        {
            y = 0;
        }
        else
        {
            if (itemsAlignment == ItemsAlignment.Baseline || alignmentKind.HasFlags(Alignment.Baseline))
            {
                alignmentAxis |= AlignmentAxis.Baseline;
            }

            alignmentAxis &= ~AlignmentAxis.Vertical;
        }

        static float normalize(float value) =>
            (1 + value) / 2;

        return new(normalize(x), normalize(y));
    }

    private TargetEnumerator GetComposedTargetEnumerator() =>
        new(this.Target);

    private RectCommand GetLayoutCommand(LayoutCommand layoutCommand)
    {
        var mask  = layoutCommand - 1;
        var index = BitOperations.PopCount((uint)(this.layoutCommands & mask));

        if (this.layoutCommands.HasFlags(layoutCommand))
        {
            return (RectCommand)this.Target.Commands[index];
        }

        var command = CommandPool.RectCommand.Get();

        switch (layoutCommand)
        {
            case LayoutCommand.Box:
                command.Flags           = Flags.ColorAsBackground;
                command.PipelineVariant = PipelineVariant.Color | PipelineVariant.Index;

                break;

            case LayoutCommand.Image:
                command.PipelineVariant = PipelineVariant.Color;

                break;
        }

        this.Target.Commands.Insert(index, command);

        this.layoutCommands |= layoutCommand;

        return command;
    }

    private RectCommand GetLayoutCommandBox() =>
        this.GetLayoutCommand(LayoutCommand.Box);

    private RectCommand GetLayoutCommandImage() =>
        this.GetLayoutCommand(LayoutCommand.Image);

    private void OnScroll(in MouseEvent mouseEvent)
    {
        if (!this.Target.IsHovered)
        {
            return;
        }

        if (this.ComputedStyle.Overflow is Overflow.Scroll or Overflow.ScrollX && mouseEvent.KeyStates.HasFlags(Platforms.Display.MouseKeyStates.Shift))
        {
            this.ContentOffset = this.ContentOffset with
            {
                X = (uint)(this.ContentOffset.X + 10 * -mouseEvent.Delta)
            };
        }
        else if (this.ComputedStyle.Overflow is Overflow.Scroll or Overflow.ScrollY)
        {
            this.ContentOffset = this.ContentOffset with
            {
                Y = (uint)(this.ContentOffset.Y + 10 * -mouseEvent.Delta)
            };
        }
    }

    private void ReleaseLayoutCommand(LayoutCommand layoutCommand)
    {
        if (this.layoutCommands.HasFlags(layoutCommand))
        {
            var mask  = layoutCommand - 1;
            var index = BitOperations.PopCount((uint)(this.layoutCommands & mask));

            var command = (RectCommand)this.Target.Commands[index];

            CommandPool.RectCommand.Return(command);

            this.Target.Commands.RemoveAt(index);

            this.layoutCommands &= ~layoutCommand;
        }
    }

    private void ReleaseLayoutCommand() =>
        this.ReleaseLayoutCommand(LayoutCommand.Box);

    private void ReleaseLayoutCommandImage() =>
        this.ReleaseLayoutCommand(LayoutCommand.Image);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool ResolveHeight(ref uint value)
    {
        var resolved = !this.parentDependencies.HasFlags(Dependency.Height);

        if (!this.contentDependencies.HasFlags(Dependency.Height))
        {
            var style = this.ComputedStyle;

            ResolveDimension(this.FontSize, style.Size?.Height, style.MinSize?.Height, style.MaxSize?.Height, ref value, ref resolved);

            if (this.ComputedStyle.BoxSizing == BoxSizing.Border)
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

        return !this.parentDependencies.HasFlags(Dependency.Margin);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool ResolvePadding()
    {
        ResolveRect(this.FontSize, this.ComputedStyle.Padding, ref this.padding);

        return !this.parentDependencies.HasFlags(Dependency.Padding);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool ResolveWidth(ref uint value)
    {
        var resolved = !this.parentDependencies.HasFlags(Dependency.Width);

        if (!this.contentDependencies.HasFlags(Dependency.Width))
        {
            var style = this.ComputedStyle;

            ResolveDimension(this.FontSize, style.Size?.Width, style.MinSize?.Width, style.MaxSize?.Width, ref value, ref resolved);

            if (this.ComputedStyle.BoxSizing == BoxSizing.Border)
            {
                value = value.ClampSubtract(this.border.Horizontal);
            }
        }

        return resolved;
    }

    private void SetBackgroundImage(StyleImage? image)
    {
        var command = this.GetLayoutCommandImage();

        if (image != null)
        {
            if (!TextureStorage.Singleton.TryGet(image.Uri, out var texture))
            {
                if (File.Exists(image.Uri))
                {
                    texture = Texture2D.Load(image.Uri);
                    TextureStorage.Singleton.Add(image.Uri, texture);
                }
            }

            if (texture != null)
            {
                this.ownStencilLayer = new StencilLayer(this.Target);

                command.MappedTexture   = new(texture, UVRect.Normalized);
                command.PipelineVariant = PipelineVariant.Color;
                command.StencilLayer    = new StencilLayer(this.Target);

                this.StencilLayer?.AppendChild(command.StencilLayer);

                return;
            }
        }

        if (!command.MappedTexture.IsDefault)
        {
            TextureStorage.Singleton.Release(command.MappedTexture.Texture);
        }

        if (command.StencilLayer != null)
        {
            command.StencilLayer.Dispose();
            command.StencilLayer.Detach();
        }

        this.ReleaseLayoutCommandImage();
    }

    private void UpdateDisposition()
    {
        if (this.renderableNodesCount == 0)
        {
            return;
        }

        var cursor               = new Point<float>();
        var size                 = this.size;
        var direction            = this.ComputedStyle.StackDirection ?? StackDirection.Horizontal;
        var contentJustification = this.ComputedStyle.ContentJustification ?? ContentJustification.None;

        var avaliableSpace = direction == StackDirection.Horizontal
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

            var alignmentType  = Alignment.None;
            var childBoundings = child.Layout.Boundings;
            var contentOffsetY = 0u;

            RectEdges margin = default;

            if (child is Element element)
            {
                margin         = element.Layout.margin;
                contentOffsetY = element.Layout.padding.Top + element.Layout.border.Top + element.Layout.margin.Top;
                childBoundings = element.Layout.BoundingsWithMargin;
                alignmentType  = element.Layout.ComputedStyle.Alignment ?? Alignment.None;
            }

            var alignment = this.GetAlignment(direction, alignmentType, out var alignmentAxis);

            var position  = new Vector2<float>();
            var usedSpace = new Size<float>();

            if (direction == StackDirection.Horizontal)
            {
                if (contentJustification == ContentJustification.None)
                {
                    avaliableSpace.Width += childBoundings.Width;

                    if (alignmentAxis.HasFlags(AlignmentAxis.Horizontal))
                    {
                        position.X = avaliableSpace.Width.ClampSubtract(childBoundings.Width) * alignment.X;
                    }
                }
                else
                {
                    if (contentJustification == ContentJustification.End && index == 0)
                    {
                        position.X = avaliableSpace.Width;
                    }
                    else if (contentJustification == ContentJustification.Center && index == 0)
                    {
                        position.X = avaliableSpace.Width / 2;
                    }
                    else if (contentJustification == ContentJustification.SpaceAround)
                    {
                        position.X = (index == 0 ? 1 : 2) * avaliableSpace.Width / (this.renderableNodesCount * 2);
                    }
                    else if (contentJustification == ContentJustification.SpaceBetween && index > 0)
                    {
                        position.X = avaliableSpace.Width / (this.renderableNodesCount - 1);
                    }
                    else if (contentJustification == ContentJustification.SpaceEvenly)
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

                if (contentJustification == ContentJustification.None)
                {
                    avaliableSpace.Width = avaliableSpace.Width.ClampSubtract((uint)usedSpace.Width);
                }
            }
            else
            {
                position.X = size.Width.ClampSubtract(childBoundings.Width) * alignment.X;

                if (contentJustification == ContentJustification.None)
                {
                    avaliableSpace.Height += childBoundings.Height;

                    if (alignmentAxis.HasFlags(AlignmentAxis.Vertical))
                    {
                        position.Y = (uint)(avaliableSpace.Height.ClampSubtract(childBoundings.Height) * alignment.Y);
                    }
                }
                else
                {
                    if (contentJustification == ContentJustification.End && index == 0)
                    {
                        position.Y = avaliableSpace.Height;
                    }
                    else if (contentJustification == ContentJustification.Center && index == 0)
                    {
                        position.Y = avaliableSpace.Height / 2;
                    }
                    else if (contentJustification == ContentJustification.SpaceAround)
                    {
                        position.Y = (index == 0 ? 1 : 2) * avaliableSpace.Height / (this.renderableNodesCount * 2);
                    }
                    else if (contentJustification == ContentJustification.SpaceBetween && index > 0)
                    {
                        position.Y = avaliableSpace.Height / (this.renderableNodesCount - 1);
                    }
                    else if (contentJustification == ContentJustification.SpaceEvenly)
                    {
                        position.Y = avaliableSpace.Height / (this.renderableNodesCount + 1);
                    }
                }

                usedSpace.Height = alignmentAxis.HasFlags(AlignmentAxis.Vertical)
                    ? float.Max(childBoundings.Height, avaliableSpace.Height - position.Y)
                    : childBoundings.Height;

                if (contentJustification == ContentJustification.None)
                {
                    avaliableSpace.Height = avaliableSpace.Height.ClampSubtract((uint)usedSpace.Height);
                }
            }

            child.Layout.Offset = new(float.Round(cursor.X + position.X + margin.Left), -float.Round(-cursor.Y + position.Y + margin.Top));

            if (direction == StackDirection.Horizontal)
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

        this.UpdateCommands();
    }

    private void UpdateCommands()
    {
        var layoutCommandBox = this.GetLayoutCommandBox();

        if (this.IsDrawable)
        {
            var style = this.ComputedStyle;

            layoutCommandBox.Rect            = new(this.Boundings.Cast<float>(), default);
            layoutCommandBox.Border          = style.Border ?? default(Shaders.CanvasShader.Border);
            layoutCommandBox.Color           = style.BackgroundColor ?? default;
            layoutCommandBox.PipelineVariant |= PipelineVariant.Color;

            if (style.BackgroundImage != null)
            {
                var layoutCommandImage  = this.GetLayoutCommandImage();

                this.ResolveImageSize(style.BackgroundImage, layoutCommandImage.MappedTexture.Texture.Size.Cast<float>(), out var rect, out var uv);

                layoutCommandImage.Rect          = rect;
                layoutCommandImage.MappedTexture = layoutCommandImage.MappedTexture with { UV = uv };
                layoutCommandImage.StencilLayer!.MakeDirty();
            }
        }
        else
        {
            layoutCommandBox.PipelineVariant &= ~PipelineVariant.Color;
        }

        layoutCommandBox.StencilLayer = this.StencilLayer;

        this.ownStencilLayer?.MakeDirty();
    }

    protected override void OnDisposed()
    {
        this.ownStencilLayer?.Dispose();

        var layoutCommandImage = this.GetLayoutCommandImage();

        if (!layoutCommandImage.MappedTexture.IsDefault)
        {
            TextureStorage.Singleton.Release(layoutCommandImage.MappedTexture.Texture);
        }

        foreach (var item in this.Target.Commands)
        {
            CommandPool.RectCommand.Return((RectCommand)item);
        }

        this.Target.Commands.Clear();
    }

    protected override void OnStyleChanged(StyleProperty property)
    {
        if (!this.Target.IsConnected)
        {
            return;
        }

        var style = this.ComputedStyle;

        var hidden = style.Hidden == true;

        if (property.HasFlags(StyleProperty.BackgroundImage))
        {
            this.SetBackgroundImage(style.BackgroundImage);
        }

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

        if (property.HasFlags(StyleProperty.Cursor) && this.Target.IsHovered)
        {
            this.SetCursor(style.Cursor);
        }

        var hasSizeChanges    = property.HasAnyFlag(StyleProperty.Size | StyleProperty.MinSize | StyleProperty.MaxSize);
        var hasMarginChanges  = property.HasFlags(StyleProperty.Margin);
        var hasPaddingChanges = property.HasFlags(StyleProperty.Padding);

        if (hasSizeChanges || hasMarginChanges || hasPaddingChanges)
        {
            var oldParentDependencies  = this.parentDependencies;
            var oldContentDependencies = this.contentDependencies;

            if (hasSizeChanges)
            {
                this.contentDependencies = Dependency.None;
                this.parentDependencies  = Dependency.None;

                var absWidth = style.Size?.Width;
                var minWidth = style.MinSize?.Width;
                var maxWidth = style.MaxSize?.Width;

                var absHeight = style.Size?.Height;
                var minHeight = style.MinSize?.Height;
                var maxHeight = style.MaxSize?.Height;

                if (IsAllNull(absWidth, minWidth, maxWidth))
                {
                    this.contentDependencies |= Dependency.Width;
                }
                else if (IsAnyRelative(absWidth, minWidth, maxWidth))
                {
                    this.parentDependencies |= Dependency.Width;
                }

                if (IsAllNull(absHeight, minHeight, maxHeight))
                {
                    this.contentDependencies |= Dependency.Height;
                }
                else if (IsAnyRelative(absHeight, minHeight, maxHeight))
                {
                    this.parentDependencies |= Dependency.Height;
                }
            }

            if (hasMarginChanges && HasRelativeEdges(style.Margin))
            {
                this.parentDependencies |= Dependency.Margin;
            }

            if (hasPaddingChanges && HasRelativeEdges(style.Padding))
            {
                this.parentDependencies |= Dependency.Padding;
            }

            var justHidden      = hidden && !this.Hidden;
            var justUnhidden    = !hidden && this.Hidden;
            var justUndependent = oldParentDependencies != Dependency.None && this.parentDependencies == Dependency.None;
            var justDependent   = oldParentDependencies == Dependency.None && this.parentDependencies != Dependency.None;

            if (this.Parent != null)
            {
                this.Parent.dependenciesHasChanged = oldContentDependencies != this.contentDependencies || oldParentDependencies != this.parentDependencies;

                if (justUnhidden || justDependent)
                {
                    if (justUnhidden)
                    {
                        this.Parent.renderableNodesCount++;
                    }

                    if (this.parentDependencies != Dependency.None)
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
            this.canScrollX = style.Overflow?.HasFlags(Overflow.ScrollX) == true;
            this.canScrollY = style.Overflow?.HasFlags(Overflow.ScrollY) == true;

            var currentIsScrollable = (this.canScrollX || this.canScrollY) && this.contentDependencies != (Dependency.Width | Dependency.Height);

            if (currentIsScrollable != this.IsScrollable)
            {
                if (currentIsScrollable)
                {
                    this.Target.Scrolled += this.OnScroll;
                }
                else
                {
                    this.Target.Scrolled -= this.OnScroll;
                    this.ContentOffset = default;
                }

                this.IsScrollable = currentIsScrollable;
            }

            if (style.Overflow?.HasFlags(Overflow.Clipping) == true && this.contentDependencies != (Dependency.Width | Dependency.Height))
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

            PropageteStencilLayer(this.Target, this.ContentStencilLayer);
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

            this.UpdateCommands();
            this.RequestUpdate(affectsBoundings);
        }

        this.Target.Visible = !hidden;
    }

    public void HandleElementRemoved(Element element)
    {
        if (!element.Layout.Hidden && element.Layout.parentDependencies != Dependency.None)
        {
            this.dependents.Remove(element);
        }
    }

    public void HandleLayoutableAppended(Layoutable layoutable)
    {
        if (!layoutable.Layout.Hidden)
        {
            this.childsChanged = true;
            this.renderableNodesCount++;
            this.RequestUpdate(true);
        }
    }

    public void HandleLayoutableRemoved(Layoutable layoutable)
    {
        if (layoutable is Element element)
        {
            this.HandleElementRemoved(element);
        }

        if (!layoutable.Layout.Hidden)
        {
            this.childsChanged = true;
            this.renderableNodesCount--;
            this.RequestUpdate(true);
        }
    }

    public override void HandleTargetConnected()
    {
        base.HandleTargetConnected();

        this.ComputeStyle();

        if (this.ownStencilLayer != null)
        {
            this.StencilLayer?.AppendChild(this.ownStencilLayer);
        }
    }

    public void HandleTargetIndexed()
    {
        var command = this.GetLayoutCommandBox();

        command.ObjectId = this.Target.Index == -1
            ? default
            : this.ComputedStyle.Border != null || this.ComputedStyle.BackgroundColor.HasValue ? (uint)(this.Target.Index + 1) : 0;
    }

    public void HandleTargetMouseOver() =>
        this.SetCursor(this.ComputedStyle.Cursor);

    public override void Update()
    {
        this.CalculateLayout();

        if (this.parentDependencies == Dependency.None)
        {
            this.UpdateDisposition();
            this.childsChanged          = false;
            this.dependenciesHasChanged = false;
        }
    }
}
