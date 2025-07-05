using System.Numerics;
using System.Runtime.CompilerServices;
using Age.Commands;
using Age.Core.Extensions;
using Age.Extensions;
using Age.Numerics;
using Age.Resources;
using Age.Storage;
using Age.Styling;

using static Age.Shaders.CanvasShader;

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
    private bool          childsChanged;
    private Size<uint>    content;
    private Dependency    contentDependencies = Dependency.Size;
    private bool          dependenciesHasChanged;
    private LayoutCommand layoutCommands;
    private RectEdges     margin;
    private StencilLayer? ownStencilLayer;
    private RectEdges     padding;
    private Dependency    parentDependencies;
    private ushort        renderableNodesCount;
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
            static void check(Unit? unit, ref ushort value)
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

    protected override StencilLayer? ContentStencilLayer => this.ownStencilLayer ?? this.StencilLayer;

    public uint FontSize => GetFontSize(this.ComputedStyle);

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
    public bool       CanScrollX => this.ComputedStyle.Overflow?.HasFlags(Overflow.ScrollX) == true;
    public bool       CanScrollY => this.ComputedStyle.Overflow?.HasFlags(Overflow.ScrollY) == true;
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
    public override Transform2D Transform
    {
        get
        {
            var style = this.ComputedStyle;

            if (style.Transforms?.Length > 0)
            {
                var boundings       = this.Boundings;
                var fontSize        = GetFontSize(style);
                var transformOrigin = style.TransformOrigin ?? new(Unit.Pc(50));

                var x = Unit.Resolve(transformOrigin.X, boundings.Width, fontSize);
                var y = Unit.Resolve(transformOrigin.Y, boundings.Height, fontSize);

                var origin = Transform2D.CreateTranslated(-x, y);

                var transform = Transform2D.Identity;

                for (var i = style.Transforms.Length - 1; i >= 0; i--)
                {
                    transform *= TransformOp.Resolve(in style.Transforms[i], boundings, fontSize);
                }

                return origin * transform * origin.Inverse() * base.Transform;
            }

            return base.Transform;
        }
    }

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
            padding.Left = (ushort)(reference * left);
        }

        if (stylePadding?.Right?.TryGetPercentage(out var right) == true)
        {
            padding.Right = (ushort)(reference * right);
        }
    }

    private static void CalculatePendingPaddingVertical(BoxLayout layout, in StyleRectEdges? stylePadding, uint reference, ref RectEdges padding)
    {
        if (stylePadding?.Top?.TryGetPercentage(out var top) == true)
        {
            padding.Top = (ushort)(reference * top);
        }

        if (stylePadding?.Bottom?.TryGetPercentage(out var bottom) == true)
        {
            padding.Bottom = (ushort)(reference * bottom);
        }
    }

    private static void CalculatePendingMarginHorizontal(BoxLayout layout, in StyleRectEdges? styleMargin, StackDirection direction, uint reference, ref RectEdges margin, ref Size<uint> contentSize)
    {
        var horizontal = 0u;

        if (styleMargin?.Left?.TryGetPercentage(out var left) == true)
        {
            horizontal += margin.Left = (ushort)(reference * left);
        }

        if (styleMargin?.Right?.TryGetPercentage(out var right) == true)
        {
            horizontal += margin.Right = (ushort)(reference * right);
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
            vertical += margin.Top = (ushort)(reference * top);
        }

        if (styleMargin?.Bottom?.TryGetPercentage(out var bottom) == true)
        {
            vertical += margin.Bottom = (ushort)(reference * bottom);
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
    private static uint GetFontSize(Style style) =>
        style.FontSize ?? DEFAULT_FONT_SIZE;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool HasRelativeEdges(StyleRectEdges? edges) =>
           edges?.Top?.Kind    == UnitKind.Percentage
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
            value = (uint)pixel;

            resolved = true;
        }
        else if (absUnit?.TryGetEm(out var em) == true)
        {
            value = (uint)(em * fontSize);

            resolved = true;
        }
        else
        {
            var min = value;
            var max = value;

            if (minUnit?.TryGetPixel(out var minPixel) == true)
            {
                min = (uint)minPixel;
            }
            else if (minUnit?.TryGetEm(out var minEm) == true)
            {
                min = (uint)(minEm * fontSize);
            }

            if (maxUnit?.TryGetPixel(out var maxPixel) == true)
            {
                max = (uint)maxPixel;
            }
            else if (maxUnit?.TryGetEm(out var maxEm) == true)
            {
                max = (uint)(maxEm * fontSize);
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

    private void ResolveImageSize(Image image, in Size<float> textureSize, out Size<float> size, out Transform2D transform, out UVRect uv)
    {
        var fontSize      = this.FontSize;
        var imageSize     = image.Size;
        var imageRepeat   = image.Repeat;
        var imagePosition = image.Position;

        switch (image.Size.Kind)
        {
            case ImageSizeKind.Fit:
                {
                    var boundings = this.SizeWithPadding.Cast<float>();

                    var x = Unit.Resolve(imagePosition.X, (uint)boundings.Width, fontSize);
                    var y = Unit.Resolve(imagePosition.Y, (uint)boundings.Height, fontSize);

                    var offset = new Vector2<float>(x + this.border.Left, -y + -this.border.Top);

                    size       = boundings;
                    transform  = Transform2D.CreateTranslated(offset);
                    uv         = UVRect.Normalized;

                    break;
                }

            case ImageSizeKind.KeepAspect:
                {
                    var boundings = this.SizeWithPadding.Cast<float>();

                    uv = UVRect.Normalized;

                    if (boundings == default)
                    {
                        size      = default;
                        transform = Transform2D.Identity;

                        break;
                    }

                    var scale = float.Min(boundings.Width, boundings.Height) / float.Max(textureSize.Width, textureSize.Height);

                    size = textureSize * scale;

                    var x = Unit.Resolve(imagePosition.X, (uint)size.Width, fontSize);
                    var y = Unit.Resolve(imagePosition.Y, (uint)size.Height, fontSize);

                    var offset = new Vector2<float>(x + this.border.Left, y + this.border.Top);

                    transform = Transform2D.CreateTranslated(offset.InvertedY);

                    break;
                }
            default:
                {
                    var boundings = this.SizeWithPadding.Cast<float>();

                    if (boundings == default)
                    {
                        size      = default;
                        transform = Transform2D.Identity;
                        uv        = default;

                        break;
                    }

                    var width  = Unit.Resolve(imageSize.Value.Width,  (uint)boundings.Width,  fontSize);
                    var height = Unit.Resolve(imageSize.Value.Height, (uint)boundings.Height, fontSize);

                    var x = Unit.Resolve(imagePosition.X, (uint)(boundings.Width - width), fontSize);
                    var y = Unit.Resolve(imagePosition.Y, (uint)(boundings.Height - height), fontSize);

                    var offsetX = 0f;
                    var offsetY = 0f;

                    var repeatX = 1f;
                    var repeatY = 1f;

                    var offset = new Vector2<float>();

                    if (imageRepeat.HasFlags(ImageRepeat.RepeatX))
                    {
                        repeatX = boundings.Width / width;
                        offsetX = x / width;

                        width = boundings.Width;

                        offset.X = this.border.Left;
                    }
                    else
                    {
                        offset.X = this.border.Left + x;
                    }

                    if (imageRepeat.HasFlags(ImageRepeat.RepeatY))
                    {
                        repeatY = boundings.Height / height;
                        offsetY = -(y / height);

                        height = boundings.Height;

                        offset.Y = this.border.Top;
                    }
                    else
                    {
                        offset.Y = this.border.Top + y;
                    }

                    size      = new(width, height);
                    transform = Transform2D.CreateTranslated(offset.InvertedY);
                    uv        = new()
                    {
                        P1 = new(offsetX, offsetY),
                        P2 = new(offsetX + repeatX, offsetY),
                        P3 = new(offsetX + repeatX, offsetY + repeatY),
                        P4 = new(offsetX, offsetY + repeatY),
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
            rectEdges.Left = (ushort)leftPixel;
        }
        else if (left?.TryGetEm(out var leftEm) == true)
        {
            rectEdges.Left = (ushort)(leftEm * fontSize);
        }

        if (right?.TryGetPixel(out var rightPixel) == true)
        {
            rectEdges.Right = (ushort)rightPixel;
        }
        else if (right?.TryGetEm(out var rightEm) == true)
        {
            rectEdges.Right = (ushort)(rightEm * fontSize);
        }

        if (top?.TryGetPixel(out var topPixel) == true)
        {
            rectEdges.Top = (ushort)topPixel;
        }
        else if (top?.TryGetEm(out var topEm) == true)
        {
            rectEdges.Top = (ushort)(topEm * fontSize);
        }

        if (bottom?.TryGetPixel(out var bottomPixel) == true)
        {
            rectEdges.Bottom = (ushort)bottomPixel;
        }
        else if (bottom?.TryGetEm(out var bottomEm) == true)
        {
            rectEdges.Bottom = (ushort)(bottomEm * fontSize);
        }
    }

    private void CalculateLayout(Style style)
    {
        var direction = style.StackDirection ?? StackDirection.Horizontal;

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
            this.CalculatePendingMargin(style, ref this.content);
        }

        var size = this.content;

        var fontSize = GetFontSize(style);

        var resolvedMargin  = this.ResolveMargin(fontSize, style.Margin);
        var resolvedPadding = this.ResolvePadding(fontSize, style.Padding);
        var resolvedSize    = this.ResolveSize(style, fontSize, ref size);

        var sizeHasChanged = this.size != size;

        this.size = size;

        if (resolvedSize && resolvedMargin && resolvedPadding)
        {
            if (this.dependents.Count > 0 && (sizeHasChanged || this.childsChanged || this.dependenciesHasChanged))
            {
                this.CalculatePendingLayouts(style);
            }

            this.UpdateBoundings(style);
        }
    }

    private void CalculatePendingMargin(Style style, ref Size<uint> size)
    {
        var contentSize = size;

        var direction = style.StackDirection ?? StackDirection.Horizontal;

        foreach (var dependent in this.dependents)
        {
            if (dependent.Layout.parentDependencies.HasAnyFlag(Dependency.Padding | Dependency.Margin))
            {
                var margin = dependent.Layout.margin;

                var dependentStyle = dependent.Layout.ComputedStyle;

                if (!this.parentDependencies.HasFlags(Dependency.Width))
                {
                    CalculatePendingMarginHorizontal(dependent.Layout, dependentStyle.Margin, direction, size.Width, ref margin, ref contentSize);
                }

                if (!this.parentDependencies.HasFlags(Dependency.Height))
                {
                    CalculatePendingMarginVertical(dependent.Layout, dependentStyle.Margin, direction, size.Height, ref margin, ref contentSize);
                }

                dependent.Layout.margin = margin;
            }
        }

        size = contentSize;
    }

    private void CalculatePendingLayouts(Style style)
    {
        var content        = this.content;
        var avaliableSpace = this.size.ClampSubtract(this.staticContent);
        var direction      = style.StackDirection ?? StackDirection.Horizontal;

        foreach (var dependent in this.dependents)
        {
            var margin         = dependent.Layout.margin;
            var padding        = dependent.Layout.padding;
            var size           = dependent.Layout.size;
            var dependentStyle = dependent.Layout.ComputedStyle;

            if (!this.contentDependencies.HasFlags(Dependency.Width) || direction == StackDirection.Vertical)
            {
                if (!this.contentDependencies.HasFlags(Dependency.Width))
                {
                    CalculatePendingPaddingHorizontal(dependent.Layout, dependentStyle.Padding, this.size.Width, ref padding);
                    CalculatePendingMarginHorizontal(dependent.Layout, dependentStyle.Margin, direction, this.size.Height, ref margin, ref content);
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
                    CalculatePendingPaddingVertical(dependent.Layout, dependentStyle.Padding, this.size.Height, ref padding);
                    CalculatePendingMarginVertical(dependent.Layout, dependentStyle.Margin, direction, this.size.Height, ref margin, ref content);
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
                    dependent.Layout.CalculatePendingLayouts(dependentStyle);
                }

                dependent.Layout.UpdateDisposition(dependentStyle);
            }

            dependent.Layout.UpdateBoundings(dependentStyle);
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

            var min = 0;
            var max = 0;

            var hasMin = minUnit?.TryGetPixel(out min) == true;
            var hasMax = maxUnit?.TryGetPixel(out max) == true;

            if (hasMin && hasMax)
            {
                if (dimension < min)
                {
                    dimension = (uint)min;
                }
                else if (dimension > max)
                {
                    dimension = (uint)max;
                }
            }
            else if (hasMin)
            {
                if (dimension < min)
                {
                    dimension = (uint)min;
                }
            }
            else if (hasMax)
            {
                if (dimension > max)
                {
                    dimension = (uint)max;
                }
            }

            modified = true;
        }
        else
        {
            var min = 0f;
            var max = 0f;

            var hasMin = minUnit?.TryGetPercentage(out min) == true;
            var hasMax = maxUnit?.TryGetPercentage(out max) == true;

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
                    || (direction == StackDirection.Vertical && alignment.Value.HasAnyFlag(Alignment.Start | Alignment.Center | Alignment.End))
                );

            baseline += element.Layout.padding.Top + element.Layout.border.Top + element.Layout.margin.Top;
        }

        if (!hasAlignment && baseline > this.BaseLine)
        {
            this.BaseLine = baseline;
        }
    }

    private Point<float> GetAlignment(Style style, StackDirection direction, Alignment alignmentKind, out AlignmentAxis alignmentAxis)
    {
        var x = -1;
        var y = -1;

        var itemsAlignment = style.ItemsAlignment ?? ItemsAlignment.None;

        alignmentAxis = AlignmentAxis.Horizontal | AlignmentAxis.Vertical;

        if (alignmentKind.HasFlags(Alignment.Left) || (direction == StackDirection.Horizontal && (itemsAlignment == ItemsAlignment.Begin || alignmentKind.HasFlags(Alignment.Start))))
        {
            x = -1;
        }
        else if (alignmentKind.HasFlags(Alignment.Right) || (direction == StackDirection.Horizontal && (itemsAlignment == ItemsAlignment.End || alignmentKind.HasFlags(Alignment.End))))
        {
            x = 1;
        }
        else if (alignmentKind.HasFlags(Alignment.Center) || (direction == StackDirection.Vertical && itemsAlignment == ItemsAlignment.Center))
        {
            x = 0;
        }
        else
        {
            alignmentAxis &= ~AlignmentAxis.Horizontal;
        }

        if (alignmentKind.HasFlags(Alignment.Top) || (direction == StackDirection.Vertical && (itemsAlignment == ItemsAlignment.Begin || alignmentKind.HasFlags(Alignment.Start))))
        {
            y = -1;
        }
        else if (alignmentKind.HasFlags(Alignment.Bottom) || (direction == StackDirection.Vertical && (itemsAlignment == ItemsAlignment.End || alignmentKind.HasFlags(Alignment.End))))
        {
            y = 1;
        }
        else if (alignmentKind.HasFlags(Alignment.Center) || (direction == StackDirection.Horizontal && itemsAlignment == ItemsAlignment.Center))
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

        var overflow = this.ComputedStyle.Overflow ?? default;

        if (overflow.HasFlags(Overflow.ScrollX) && mouseEvent.KeyStates.HasFlags(Platforms.Display.MouseKeyStates.Shift))
        {
            this.ContentOffset = this.ContentOffset with
            {
                X = (uint)(this.ContentOffset.X + (10 * -mouseEvent.Delta))
            };
        }
        else if (overflow.HasFlags(Overflow.ScrollY))
        {
            this.ContentOffset = this.ContentOffset with
            {
                Y = (uint)(this.ContentOffset.Y + (10 * -mouseEvent.Delta))
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
    private bool ResolveHeight(Style style, uint fontSize, ref uint value)
    {
        var resolved = !this.parentDependencies.HasFlags(Dependency.Height);

        if (!this.contentDependencies.HasFlags(Dependency.Height))
        {
            ResolveDimension(fontSize, style.Size?.Height, style.MinSize?.Height, style.MaxSize?.Height, ref value, ref resolved);

            if (style.BoxSizing == BoxSizing.Border)
            {
                value = value.ClampSubtract(this.border.Vertical);
            }
        }

        return resolved;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool ResolveSize(Style style, uint fontSize, ref Size<uint> size)
    {
        var resolvedWidth  = this.ResolveWidth(style, fontSize, ref size.Width);
        var resolvedHeight = this.ResolveHeight(style, fontSize, ref size.Height);

        return resolvedWidth && resolvedHeight;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool ResolveMargin(uint fontSize, StyleRectEdges? styleMargin)
    {
        ResolveRect(fontSize, styleMargin, ref this.margin);

        return !this.parentDependencies.HasFlags(Dependency.Margin);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool ResolvePadding(uint fontSize, StyleRectEdges? stylePadding)
    {
        ResolveRect(fontSize, stylePadding, ref this.padding);

        return !this.parentDependencies.HasFlags(Dependency.Padding);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool ResolveWidth(Style style, uint fontSize, ref uint value)
    {
        var resolved = !this.parentDependencies.HasFlags(Dependency.Width);

        if (!this.contentDependencies.HasFlags(Dependency.Width))
        {
            ResolveDimension(fontSize, style.Size?.Width, style.MinSize?.Width, style.MaxSize?.Width, ref value, ref resolved);

            if (style.BoxSizing == BoxSizing.Border)
            {
                value = value.ClampSubtract(this.border.Horizontal);
            }
        }

        return resolved;
    }

    private void SetBackgroundImage(Image? image)
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
                command.TextureMap      = new(texture, UVRect.Normalized);
                command.PipelineVariant = PipelineVariant.Color;
                command.StencilLayer    = new StencilLayer(this.Target);

                this.StencilLayer?.AppendChild(command.StencilLayer);

                return;
            }
        }

        if (!command.TextureMap.IsDefault)
        {
            TextureStorage.Singleton.Release(command.TextureMap.Texture);
        }

        if (command.StencilLayer != null)
        {
            command.StencilLayer.Dispose();
            command.StencilLayer.Detach();
        }

        this.ReleaseLayoutCommandImage();
    }

    private void UpdateDisposition(Style style)
    {
        if (this.renderableNodesCount == 0)
        {
            return;
        }

        var cursor               = new Point<float>();
        var size                 = this.size;
        var direction            = style.StackDirection ?? StackDirection.Horizontal;
        var contentJustification = style.ContentJustification ?? ContentJustification.None;

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
                contentOffsetY = (uint)(element.Layout.padding.Top + element.Layout.border.Top + element.Layout.margin.Top);
                childBoundings = element.Layout.BoundingsWithMargin;
                alignmentType  = element.Layout.ComputedStyle.Alignment ?? Alignment.None;
            }

            var alignment = this.GetAlignment(style, direction, alignmentType, out var alignmentAxis);

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

    private void UpdateBoundings(Style style)
    {
        this.Boundings = new(
            this.size.Width + this.padding.Horizontal + this.border.Horizontal,
            this.size.Height + this.padding.Vertical + this.border.Vertical
        );

        this.UpdateCommands(style);
    }

    private void UpdateCommands(Style style)
    {
        var layoutCommandBox = this.GetLayoutCommandBox();

        if (style.Border != null || style.BackgroundColor != null || style.BackgroundImage != null)
        {
            layoutCommandBox.Size            = this.Boundings.Cast<float>();
            layoutCommandBox.Border          = style.Border ?? default(Shaders.CanvasShader.Border);
            layoutCommandBox.Color           = style.BackgroundColor ?? default;
            layoutCommandBox.PipelineVariant |= PipelineVariant.Color;

            if (style.BackgroundImage != null)
            {
                var layoutCommandImage  = this.GetLayoutCommandImage();

                this.ResolveImageSize(style.BackgroundImage, layoutCommandImage.TextureMap.Texture.Size.Cast<float>(), out var size, out var transform, out var uv);

                layoutCommandImage.Size          = size;
                layoutCommandImage.Transform     = transform;
                layoutCommandImage.TextureMap = layoutCommandImage.TextureMap with { UV = uv };
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

        if (!layoutCommandImage.TextureMap.IsDefault)
        {
            TextureStorage.Singleton.Release(layoutCommandImage.TextureMap.Texture);
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
            var currentIsScrollable = (style.Overflow?.HasFlags(Overflow.ScrollX) == true || style.Overflow?.HasFlags(Overflow.ScrollY) == true) && this.contentDependencies != (Dependency.Width | Dependency.Height);

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

            this.UpdateCommands(style);
            this.RequestUpdate(affectsBoundings);
        }

        this.Target.Visible = !hidden;
    }

    public RectCommand GetLayoutLayer(LayoutLayer layer) =>
        this.GetLayoutCommand((LayoutCommand)layer);

    public void ReleaseLayoutLayer(LayoutLayer layer) =>
        this.ReleaseLayoutCommand((LayoutCommand)layer);

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

        var style = this.ComputedStyle;

        command.ObjectId = this.Target.Index == -1
            ? default
            : style.Border != null || style.BackgroundColor.HasValue ? (uint)(this.Target.Index + 1) : 0;
    }

    public void HandleTargetMouseOver() =>
        this.SetCursor(this.ComputedStyle.Cursor);

    public override void Update()
    {
        var style = this.ComputedStyle;

        this.CalculateLayout(style);

        if (this.parentDependencies == Dependency.None)
        {
            this.UpdateDisposition(style);
            this.childsChanged          = false;
            this.dependenciesHasChanged = false;
        }
    }
}
