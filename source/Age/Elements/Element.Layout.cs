using Age.Extensions;
using Age.Core.Extensions;
using Age.Numerics;
using Age.Styling;
using System.Runtime.CompilerServices;
using Age.Storage;
using Age.Resources;
using Age.Commands;

namespace Age.Elements;

public abstract partial class Element
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

    private readonly List<Element> dependents  = [];

    private RectEdges     border;
    private bool          childsChanged;
    private Size<uint>    content;
    private Dependency    contentDependencies = Dependency.Size;
    private Point<uint>   contentOffset;
    private bool          dependenciesHasChanged;
    private RectEdges     margin;
    private StencilLayer? ownStencilLayer;
    private RectEdges     padding;
    private Dependency    parentDependencies;
    private ushort        renderableNodes;
    private Size<uint>    size;
    private Size<uint>    staticContent;

    private Size<uint> AbsoluteBoundings
    {
        get
        {
            var size    = this.size;
            var padding = this.padding;
            var margin  = this.margin;

            if (this.parentDependencies.HasFlags(Dependency.Width))
            {
                size.Width = this.content.Width;
            }

            if (this.parentDependencies.HasFlags(Dependency.Height))
            {
                size.Height = this.content.Height;
            }

            checkRect(this.ComputedStyle.Margin,  ref padding);
            checkRect(this.ComputedStyle.Padding, ref padding);

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

    private protected override StencilLayer? ContentStencilLayer => this.ownStencilLayer ?? this.StencilLayer;

    private protected override Transform2D LayoutTransform
    {
        get
        {
            if (this.ComputedStyle.Transforms?.Length > 0)
            {
                var boundings       = this.Boundings;
                var fontSize        = this.FontSize;
                var transformOrigin = this.ComputedStyle.TransformOrigin ?? new(Unit.Pc(50));

                var x = Unit.Resolve(transformOrigin.X, boundings.Width,  fontSize);
                var y = Unit.Resolve(transformOrigin.Y, boundings.Height, fontSize);

                var origin = Transform2D.CreateTranslated(-x, y);

                var transform = Transform2D.Identity;

                for (var i = this.ComputedStyle.Transforms.Length - 1; i >= 0; i--)
                {
                    transform *= TransformOp.Resolve(in this.ComputedStyle.Transforms[i], boundings, fontSize);
                }

                return origin * transform * origin.Inverse() * base.LayoutTransform;
            }

            return base.LayoutTransform;
        }
    }

    internal Point<uint> ContentOffset => this.contentOffset;
    internal uint        FontSize      => this.ComputedStyle.FontSize ?? DEFAULT_FONT_SIZE;

    internal override bool IsParentDependent => this.parentDependencies != Dependency.None;

    internal override StencilLayer? StencilLayer
    {
        get => base.StencilLayer;
        set
        {
            if (base.StencilLayer != value)
            {
                base.StencilLayer = value;

                if (this.TryGetLayoutCommandBox(out var boxCommand))
                {
                    boxCommand.StencilLayer = value;
                }

                if (this.TryGetLayoutCommandScrollX(out var scrollXCommand))
                {
                    scrollXCommand.StencilLayer = value;
                }

                if (this.TryGetLayoutCommandScrollY(out var scrollYCommand))
                {
                    scrollYCommand.StencilLayer = value;
                }
            }
        }
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

    private static void CalculatePendingHeight(Element dependent, StackDirection direction, in uint reference, ref uint height, ref uint content, ref uint avaliableSpace)
    {
        CalculatePendingDimension(dependent.ComputedStyle.Size?.Height, dependent.ComputedStyle.MinSize?.Height, dependent.ComputedStyle.MaxSize?.Height, reference, ref height, out var modified);

        if (modified)
        {
            content = content.ClampSubtract(dependent.AbsoluteBoundings.Height);

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
                .ClampSubtract(dependent.border.Vertical)
                .ClampSubtract(dependent.padding.Vertical)
                .ClampSubtract(dependent.margin.Vertical);
        }
    }

    private static void CalculatePendingMarginHorizontal(Element element, in StyleRectEdges? styleMargin, StackDirection direction, uint reference, ref RectEdges margin, ref Size<uint> contentSize)
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
                contentSize.Width = uint.Max(element.size.Width + element.padding.Horizontal + element.border.Horizontal + horizontal, contentSize.Width);
            }
        }
    }

    private static void CalculatePendingMarginVertical(Element element, in StyleRectEdges? styleMargin, StackDirection direction, uint reference, ref RectEdges margin, ref Size<uint> contentSize)
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
                contentSize.Height = uint.Max(element.size.Height + element.padding.Vertical + element.border.Vertical + vertical, contentSize.Height);
            }
        }
    }

    private static void CalculatePendingPaddingHorizontal(in StyleRectEdges? stylePadding, uint reference, ref RectEdges padding)
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

    private static void CalculatePendingPaddingVertical(in StyleRectEdges? stylePadding, uint reference, ref RectEdges padding)
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

    private static void CalculatePendingWidth(Element dependent, StackDirection direction, in uint reference, ref uint width, ref uint content, ref uint avaliableSpace)
    {
        CalculatePendingDimension(dependent.ComputedStyle.Size?.Width, dependent.ComputedStyle.MinSize?.Width, dependent.ComputedStyle.MaxSize?.Width, reference, ref width, out var modified);

        if (modified)
        {
            content = content.ClampSubtract(dependent.AbsoluteBoundings.Width);

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
                .ClampSubtract(dependent.border.Horizontal)
                .ClampSubtract(dependent.padding.Horizontal)
                .ClampSubtract(dependent.margin.Horizontal);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool HasRelativeEdges(StyleRectEdges? edges) =>
           edges?.Top?.Kind    == UnitKind.Percentage
        || edges?.Right?.Kind  == UnitKind.Percentage
        || edges?.Bottom?.Kind == UnitKind.Percentage
        || edges?.Left?.Kind   == UnitKind.Percentage;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsAllNull(Unit? abs, Unit? min, Unit? max) =>
        abs == null && min == null && max == null;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsAnyRelative(Unit? abs, Unit? min, Unit? max) =>
        abs?.Kind == UnitKind.Percentage || min?.Kind == UnitKind.Percentage || max?.Kind == UnitKind.Percentage;

    private static void PropagateStencilLayer(Element target, StencilLayer? stencilLayer)
    {
        var enumerator = target.GetComposedTreeTraversalEnumerator();

        while (enumerator.MoveNext())
        {
            var current = enumerator.Current;

            if (current.StencilLayer == stencilLayer)
            {
                enumerator.SkipToNextSibling();
            }
            else if (current is Element element && element.ownStencilLayer != null)
            {
                if (stencilLayer != null)
                {
                    stencilLayer.AppendChild(element.ownStencilLayer);
                }
                else
                {
                    element.ownStencilLayer.Detach();
                }

                element.StencilLayer = stencilLayer;

                enumerator.SkipToNextSibling();
            }
            else
            {
                current.StencilLayer = stencilLayer;
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

    private void CalculateLayout()
    {
        var direction = this.ComputedStyle.StackDirection ?? StackDirection.Horizontal;

        this.content       = new Size<uint>();
        this.staticContent = new Size<uint>();
        this.BaseLine      = -1;

        var enumerator = this.GetComposedElementEnumerator();

        while (enumerator.MoveNext())
        {
            var child = enumerator.Current;

            if (child.Hidden)
            {
                continue;
            }

            child.UpdateDirtyLayout();

            Size<uint> childSize;

            var dependencies = Dependency.None;

            if (child is Element element)
            {
                var boudings = element.AbsoluteBoundings;

                childSize.Width  = boudings.Width;
                childSize.Height = boudings.Height;

                dependencies = element.parentDependencies;
            }
            else
            {
                childSize = child.Boundings;
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

                if (child == this.FirstChild)
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

        var fontSize = this.FontSize;

        var resolvedMargin  = this.ResolveMargin(fontSize, this.ComputedStyle.Margin);
        var resolvedPadding = this.ResolvePadding(fontSize, this.ComputedStyle.Padding);
        var resolvedSize    = this.ResolveSize(fontSize, ref size);

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

    private void CalculatePendingLayouts()
    {
        var content        = this.content;
        var avaliableSpace = this.size.ClampSubtract(this.staticContent);
        var direction      = this.ComputedStyle.StackDirection ?? StackDirection.Horizontal;

        foreach (var dependent in this.dependents)
        {
            var margin         = dependent.margin;
            var padding        = dependent.padding;
            var size           = dependent.size;
            var dependentStyle = dependent.ComputedStyle;

            if (!this.contentDependencies.HasFlags(Dependency.Width) || direction == StackDirection.Vertical)
            {
                if (!this.contentDependencies.HasFlags(Dependency.Width))
                {
                    CalculatePendingPaddingHorizontal(dependentStyle.Padding, this.size.Width, ref padding);
                    CalculatePendingMarginHorizontal(dependent, dependentStyle.Margin, direction, this.size.Height, ref margin, ref content);
                }

                if (dependent.parentDependencies.HasFlags(Dependency.Width))
                {
                    CalculatePendingWidth(dependent, direction, this.size.Width, ref size.Width, ref content.Width, ref avaliableSpace.Width);
                }
            }

            if (!this.contentDependencies.HasFlags(Dependency.Height) || direction == StackDirection.Horizontal)
            {
                if (!this.contentDependencies.HasFlags(Dependency.Height))
                {
                    CalculatePendingPaddingVertical(dependentStyle.Padding, this.size.Height, ref padding);
                    CalculatePendingMarginVertical(dependent, dependentStyle.Margin, direction, this.size.Height, ref margin, ref content);
                }

                if (dependent.parentDependencies.HasFlags(Dependency.Height))
                {
                    CalculatePendingHeight(dependent, direction, this.size.Height, ref size.Height, ref content.Height, ref avaliableSpace.Height);
                }
            }

            if (dependent.dependenciesHasChanged || dependent.childsChanged || size != dependent.size || padding != dependent.padding || margin != dependent.margin)
            {
                dependent.childsChanged          = false;
                dependent.dependenciesHasChanged = false;
                dependent.size                   = size;
                dependent.padding                = padding;
                dependent.margin                 = margin;

                if (dependent.dependents.Count > 0)
                {
                    dependent.CalculatePendingLayouts();
                }

                dependent.UpdateDisposition();
            }

            dependent.UpdateBoundings();
            dependent.MakePristine();

            this.CheckHightestInlineChild(direction, dependent);
        }

        this.content = content;
    }

    private void CalculatePendingMargin(ref Size<uint> size)
    {
        var contentSize = size;

        var direction = this.ComputedStyle.StackDirection ?? StackDirection.Horizontal;

        foreach (var dependent in this.dependents)
        {
            if (dependent.parentDependencies.HasAnyFlag(Dependency.Padding | Dependency.Margin))
            {
                var margin = dependent.margin;

                var dependentStyle = dependent.ComputedStyle;

                if (!this.parentDependencies.HasFlags(Dependency.Width))
                {
                    CalculatePendingMarginHorizontal(dependent, dependentStyle.Margin, direction, size.Width, ref margin, ref contentSize);
                }

                if (!this.parentDependencies.HasFlags(Dependency.Height))
                {
                    CalculatePendingMarginVertical(dependent, dependentStyle.Margin, direction, size.Height, ref margin, ref contentSize);
                }

                dependent.margin = margin;
            }
        }

        size = contentSize;
    }

    private void CheckHightestInlineChild(StackDirection direction, Layoutable child)
    {
        if (child.BaseLine == -1)
        {
            return;
        }

        var baseline     = child.BaseLine;
        var hasAlignment = false;

        if (child is Element element)
        {
            var alignment = element.ComputedStyle.Alignment;

            hasAlignment = alignment.HasValue
                && (
                    alignment.Value == Alignment.Center
                    || alignment.Value.HasAnyFlag(Alignment.Top | Alignment.Bottom)
                    || (direction == StackDirection.Vertical && alignment.Value.HasAnyFlag(Alignment.Start | Alignment.Center | Alignment.End))
                );

            baseline += element.padding.Top + element.border.Top + element.margin.Top;
        }

        if (!hasAlignment && baseline > this.BaseLine)
        {
            this.BaseLine = baseline;
        }
    }



    private void OnStyleChanged(StyleProperty property)
    {
        if (!this.IsConnected)
        {
            return;
        }

        var hidden = this.ComputedStyle.Hidden == true;

        if (property.HasFlags(StyleProperty.BackgroundImage))
        {
            this.SetBackgroundImage(this.ComputedStyle.BackgroundImage);
        }

        if (property.HasFlags(StyleProperty.Border))
        {
            this.border = new()
            {
                Top    = this.ComputedStyle.Border?.Top.Thickness ?? 0,
                Right  = this.ComputedStyle.Border?.Right.Thickness ?? 0,
                Bottom = this.ComputedStyle.Border?.Bottom.Thickness ?? 0,
                Left   = this.ComputedStyle.Border?.Left.Thickness ?? 0,
            };
        }

        if (property.HasFlags(StyleProperty.Cursor) && this.IsHovered)
        {
            this.SetCursor(this.ComputedStyle.Cursor);
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
                this.parentDependencies = Dependency.None;

                var absWidth = this.ComputedStyle.Size?.Width;
                var minWidth = this.ComputedStyle.MinSize?.Width;
                var maxWidth = this.ComputedStyle.MaxSize?.Width;

                var absHeight = this.ComputedStyle.Size?.Height;
                var minHeight = this.ComputedStyle.MinSize?.Height;
                var maxHeight = this.ComputedStyle.MaxSize?.Height;

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

            if (hasMarginChanges && HasRelativeEdges(this.ComputedStyle.Margin))
            {
                this.parentDependencies |= Dependency.Margin;
            }

            if (hasPaddingChanges && HasRelativeEdges(this.ComputedStyle.Padding))
            {
                this.parentDependencies |= Dependency.Padding;
            }

            var justHidden      = hidden && !this.Hidden;
            var justUnhidden    = !hidden && this.Hidden;
            var justUndependent = oldParentDependencies != Dependency.None && this.parentDependencies == Dependency.None;
            var justDependent   = oldParentDependencies == Dependency.None && this.parentDependencies != Dependency.None;

            if (this.ComposedParentElement is Element parent)
            {
                parent.dependenciesHasChanged = oldContentDependencies != this.contentDependencies || oldParentDependencies != this.parentDependencies;

                if (justUnhidden || justDependent)
                {
                    if (justUnhidden)
                    {
                        parent.renderableNodes++;
                    }

                    if (this.parentDependencies != Dependency.None)
                    {
                        var dependents = this.AssignedSlot?.dependents ?? parent.dependents;

                        dependents.Add(this);
                        dependents.Sort();
                    }
                }
                else if (justHidden || justUndependent)
                {
                    if (justHidden)
                    {
                        parent.renderableNodes--;
                    }

                    var dependents = this.AssignedSlot?.dependents ?? parent.dependents;

                    dependents.Remove(this);
                }
            }
        }

        if (property.HasFlags(StyleProperty.Overflow))
        {
            this.IsScrollable = this.ComputedStyle.Overflow?.HasAnyFlag(Overflow.Scroll) == true && this.contentDependencies != (Dependency.Width | Dependency.Height);

            if (!this.IsScrollable)
            {
                this.Scroll = default;
            }

            if (this.ComputedStyle.Overflow?.HasFlags(Overflow.Clipping) == true && this.contentDependencies != (Dependency.Width | Dependency.Height))
            {
                if (this.ownStencilLayer == null)
                {
                    this.ownStencilLayer = new StencilLayer(this);

                    this.StencilLayer?.AppendChild(this.ownStencilLayer);
                }
            }
            else if (this.ownStencilLayer != null)
            {
                this.ownStencilLayer.Detach();
                this.ownStencilLayer.Dispose();

                this.ownStencilLayer = null;
            }

            PropagateStencilLayer(this, this.ContentStencilLayer);
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

            if (property.HasFlags(StyleProperty.Color) && this.TryGetLayoutCommandBox(out var command))
            {
                command.Color = this.ComputedStyle.BackgroundColor ?? default;
            }

            this.RequestUpdate(affectsBoundings);
        }

        this.Visible = !hidden;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool ResolveHeight(uint fontSize, ref uint value)
    {
        var resolved = !this.parentDependencies.HasFlags(Dependency.Height);

        if (!this.contentDependencies.HasFlags(Dependency.Height))
        {
            ResolveDimension(fontSize, this.ComputedStyle.Size?.Height, this.ComputedStyle.MinSize?.Height, this.ComputedStyle.MaxSize?.Height, ref value, ref resolved);

            if (this.ComputedStyle.BoxSizing == BoxSizing.Border)
            {
                value = value.ClampSubtract(this.border.Vertical);
            }
        }

        return resolved;
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

                    size      = boundings;
                    transform = Transform2D.CreateTranslated(offset);
                    uv        = UVRect.Normalized;

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

                    var width = Unit.Resolve(imageSize.Value.Width, (uint)boundings.Width, fontSize);
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
    private bool ResolveSize(uint fontSize, ref Size<uint> size)
    {
        var resolvedWidth  = this.ResolveWidth(fontSize, ref size.Width);
        var resolvedHeight = this.ResolveHeight(fontSize, ref size.Height);

        return resolvedWidth && resolvedHeight;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool ResolveWidth(uint fontSize, ref uint value)
    {
        var resolved = !this.parentDependencies.HasFlags(Dependency.Width);

        if (!this.contentDependencies.HasFlags(Dependency.Width))
        {
            ResolveDimension(fontSize, this.ComputedStyle.Size?.Width, this.ComputedStyle.MinSize?.Width, this.ComputedStyle.MaxSize?.Width, ref value, ref resolved);

            if (this.ComputedStyle.BoxSizing == BoxSizing.Border)
            {
                value = value.ClampSubtract(this.border.Horizontal);
            }
        }

        return resolved;
    }

    private void SetBackgroundImage(Image? image)
    {
        var command = this.AllocateLayoutCommandImage();

        if (image != null)
        {
            if (!TextureStorage.Singleton.TryGet(image.Uri, out var texture) && File.Exists(image.Uri))
            {
                texture = Texture2D.Load(image.Uri);
                TextureStorage.Singleton.Add(image.Uri, texture);
            }

            if (texture != null)
            {
                command.TextureMap      = new(texture, UVRect.Normalized);
                command.PipelineVariant = PipelineVariant.Color;
                command.StencilLayer    = new StencilLayer(this);

                this.StencilLayer?.AppendChild(command.StencilLayer);

                return;
            }
        }

        DisposeLayoutCommandImage(command);

        this.ReleaseLayoutCommandImage();
    }

    private void UpdateDisposition()
    {
        if (this.renderableNodes == 0)
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
        cursor.Y -= this.padding.Top + this.border.Top;

        var index = 0;

        var enumerator = this.GetComposedElementEnumerator();

        while (enumerator.MoveNext())
        {
            var node = enumerator.Current;

            if (node is not Layoutable child || child.Hidden)
            {
                continue;
            }

            var alignmentType  = Alignment.None;
            var childBoundings = child.Boundings;
            var contentOffsetY = 0u;

            RectEdges margin = default;

            if (child is Element element)
            {
                margin         = element.margin;
                contentOffsetY = (uint)(element.padding.Top + element.border.Top + element.margin.Top);
                childBoundings = element.BoundingsWithMargin;
                alignmentType  = element.ComputedStyle.Alignment.GetValueOrDefault();
            }

            var alignment = GetAlignment(this.ComputedStyle.ItemsAlignment.GetValueOrDefault(), direction, alignmentType, out var alignmentAxis);

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
                        position.X = (index == 0 ? 1 : 2) * avaliableSpace.Width / (this.renderableNodes * 2);
                    }
                    else if (contentJustification == ContentJustification.SpaceBetween && index > 0)
                    {
                        position.X = avaliableSpace.Width / (this.renderableNodes - 1);
                    }
                    else if (contentJustification == ContentJustification.SpaceEvenly)
                    {
                        position.X = avaliableSpace.Width / (this.renderableNodes + 1);
                    }
                }

                if (alignmentAxis.HasFlags(AlignmentAxis.Vertical))
                {
                    position.Y = size.Height.ClampSubtract(childBoundings.Height) * alignment.Y;
                }
                else if (alignmentAxis.HasFlags(AlignmentAxis.Baseline) && child.BaseLine > -1)
                {
                    position.Y = this.BaseLine - (contentOffsetY + child.BaseLine);
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
                        position.Y = (index == 0 ? 1 : 2) * avaliableSpace.Height / (this.renderableNodes * 2);
                    }
                    else if (contentJustification == ContentJustification.SpaceBetween && index > 0)
                    {
                        position.Y = avaliableSpace.Height / (this.renderableNodes - 1);
                    }
                    else if (contentJustification == ContentJustification.SpaceEvenly)
                    {
                        position.Y = avaliableSpace.Height / (this.renderableNodes + 1);
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

            child.Offset = new(float.Round(cursor.X + position.X + margin.Left), -float.Round(-cursor.Y + position.Y + margin.Top));

            if (direction == StackDirection.Horizontal)
            {
                cursor.X = child.Offset.X + usedSpace.Width - margin.Right;
            }
            else
            {
                cursor.Y = child.Offset.Y - usedSpace.Height + margin.Bottom;
            }

            index++;
        }
    }

    private void UpdateBoundings()
    {
        this.Boundings = new(
            this.size.Width  + this.padding.Horizontal + this.border.Horizontal,
            this.size.Height + this.padding.Vertical   + this.border.Vertical
        );

        this.UpdateCommands();
    }

    internal void HandleElementRemoved(Element element)
    {
        if (!element.Hidden && element.parentDependencies != Dependency.None)
        {
            this.dependents.Remove(element);
        }
    }

    internal void HandleLayoutableAppended(Layoutable layoutable)
    {
        if (!layoutable.Hidden)
        {
            this.childsChanged = true;
            this.renderableNodes++;
            this.RequestUpdate(true);
        }
    }

    internal void HandleLayoutableRemoved(Layoutable layoutable)
    {
        if (layoutable is Element element)
        {
            this.HandleElementRemoved(element);
        }

        if (!layoutable.Hidden)
        {
            this.childsChanged = true;
            this.renderableNodes--;
            this.RequestUpdate(true);
        }
    }

    internal override void UpdateLayout()
    {
        this.CalculateLayout();

        if (this.parentDependencies == Dependency.None)
        {
            this.UpdateDisposition();
            this.childsChanged = false;
            this.dependenciesHasChanged = false;
        }
    }
}
