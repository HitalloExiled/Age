using Age.Core.Extensions;
using Age.Extensions;
using Age.Numerics;
using Age.Styling;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Age.Elements.Layouts;

internal abstract partial class StyledLayout : Layout
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

    public event Action<StyleProperty>? StyleChanged;

    private static readonly Style     empty     = new();
    private static readonly StylePool stylePool = new();

    private Style? computedStyle;

    [MemberNotNullWhen(true, nameof(computedStyle), nameof(StyleSheet))]
    private bool NeedsCompute => this.StyleSheet != null;

    private ElementState States
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;

                if (this.NeedsCompute)
                {
                    this.ComputeStyle(this.ComputedStyle.Data);
                }
            }
        }
    }

    private Size<uint> size;

    private RectEdges  border;
    private Size<uint> content;
    private RectEdges  margin;
    private RectEdges  padding;

    internal protected List<Styleable> Dependents { get; } = [];

    public Size<uint> AbsoluteBoundings
    {
        get
        {
            var size    = this.Size;
            var padding = this.padding;
            var margin  = this.Margin;
            var style   = this.ComputedStyle;

            if (this.ParentDependencies.HasFlags(Dependency.Width))
            {
                size.Width = this.Content.Width;
            }

            if (this.ParentDependencies.HasFlags(Dependency.Height))
            {
                size.Height = this.Content.Height;
            }

            checkRect(style.Margin,  ref padding);
            checkRect(style.Padding, ref padding);

            return new(
                size.Width  + padding.Horizontal + this.Border.Horizontal + margin.Horizontal,
                size.Height + padding.Vertical   + this.Border.Vertical   + margin.Vertical
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

    public Style ComputedStyle => this.computedStyle ?? this.UserStyle ?? empty;

    public StyleSheet? StyleSheet
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;

                var previous = this.ComputedStyle.Data;
                this.HandleComputedStyleAllocation(value != null);
                this.ComputeStyle(previous);
            }
        }
    }

    public Style? UserStyle
    {
        get;
        set
        {
            if (field != value)
            {
                if (field != null)
                {
                    field.PropertyChanged -= this.OnPropertyChanged;
                }

                if (value != null)
                {
                    value.PropertyChanged += this.OnPropertyChanged;
                }

                var previous = this.ComputedStyle.Data;

                field = value;

                this.ComputeStyle(previous);
            }
        }
    }

    public abstract override Styleable Target { get; }

    internal protected ref RectEdges  Border  => ref this.border;
    internal protected ref Size<uint> Content => ref this.content;
    internal protected ref RectEdges  Margin  => ref this.margin;
    internal protected ref RectEdges  Padding => ref this.padding;
    internal protected ref Size<uint> Size    => ref this.size;
    internal protected bool DependenciesHasChanged { get; set; }
    internal protected ushort RenderableNodesCount { get; private set; }

    internal protected Dependency ParentDependencies  { get; set; }
    internal protected Dependency ContentDependencies { get; set; } = Dependency.Size;

    public uint FontSize => GetFontSize(this.ComputedStyle);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsAnyRelative(Unit? abs, Unit? min, Unit? max) =>
        abs?.Kind == UnitKind.Percentage || min?.Kind == UnitKind.Percentage || max?.Kind == UnitKind.Percentage;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsAllNull(Unit? abs, Unit? min, Unit? max) =>
        abs == null && min == null && max == null;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool HasRelativeEdges(StyleRectEdges? edges) =>
           edges?.Top?.Kind    == UnitKind.Percentage
        || edges?.Right?.Kind  == UnitKind.Percentage
        || edges?.Bottom?.Kind == UnitKind.Percentage
        || edges?.Left?.Kind   == UnitKind.Percentage;

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



    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static uint GetFontSize(Style style) =>
        style.FontSize ?? 16;

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
    protected bool ResolveMargin(uint fontSize, StyleRectEdges? styleMargin)
    {
        ResolveRect(fontSize, styleMargin, ref this.Margin);

        return !this.ParentDependencies.HasFlags(Dependency.Margin);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected bool ResolvePadding(uint fontSize, StyleRectEdges? stylePadding)
    {
        ResolveRect(fontSize, stylePadding, ref this.Padding);

        return !this.ParentDependencies.HasFlags(Dependency.Padding);
    }

    private void CompareAndInvoke(in StyleData left, in StyleData right)
    {
        var changes = StyleData.Diff(left, right);

        if (changes != default)
        {
            this.InvokeStyleChanged(changes);
        }
    }

    private void ComputeStyle(in StyleData previous)
    {
        if (!this.Target.IsConnected)
        {
            return;
        }

        if (!this.NeedsCompute)
        {
            this.CompareAndInvoke(this.ComputedStyle.Data, previous);

            return;
        }

        if (this.StyleSheet.Base != null)
        {
            this.computedStyle.Copy(this.StyleSheet.Base);
        }
        else
        {
            this.computedStyle.Clear();
        }

        if (this.UserStyle != null)
        {
            this.computedStyle.Merge(this.UserStyle);
        }

        merge(ElementState.Focus,    this.StyleSheet.Focus);
        merge(ElementState.Hovered,  this.StyleSheet.Hovered);
        merge(ElementState.Disabled, this.StyleSheet.Disabled);
        merge(ElementState.Enabled,  this.StyleSheet.Enabled);
        merge(ElementState.Checked,  this.StyleSheet.Checked);
        merge(ElementState.Active,   this.StyleSheet.Active);

        this.CompareAndInvoke(this.computedStyle.Data, previous);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void merge(ElementState states, Style? style)
        {
            if (this.States.HasFlags(states) && style != null)
            {
                this.computedStyle.Merge(style);
            }
        }
    }

    private void HandleComputedStyleAllocation(bool allocating)
    {
        if (allocating)
        {
            this.computedStyle ??= stylePool.Get();
        }
        else
        {
            stylePool.Return(this.computedStyle!);
            this.computedStyle = null;
        }
    }

    private void InvokeStyleChanged(StyleProperty property)
    {
        if (!this.Target.IsConnected)
        {
            return;
        }

        var style  = this.ComputedStyle;
        var hidden = style.Hidden == true;

        if (property.HasFlags(StyleProperty.Border))
        {
            this.Border = new()
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
            var oldParentDependencies  = this.ParentDependencies;
            var oldContentDependencies = this.ContentDependencies;

            if (hasSizeChanges)
            {
                this.ContentDependencies = Dependency.None;
                this.ParentDependencies  = Dependency.None;

                var absWidth = style.Size?.Width;
                var minWidth = style.MinSize?.Width;
                var maxWidth = style.MaxSize?.Width;

                var absHeight = style.Size?.Height;
                var minHeight = style.MinSize?.Height;
                var maxHeight = style.MaxSize?.Height;

                if (IsAllNull(absWidth, minWidth, maxWidth))
                {
                    this.ContentDependencies |= Dependency.Width;
                }
                else if (IsAnyRelative(absWidth, minWidth, maxWidth))
                {
                    this.ParentDependencies |= Dependency.Width;
                }

                if (IsAllNull(absHeight, minHeight, maxHeight))
                {
                    this.ContentDependencies |= Dependency.Height;
                }
                else if (IsAnyRelative(absHeight, minHeight, maxHeight))
                {
                    this.ParentDependencies |= Dependency.Height;
                }
            }

            if (hasMarginChanges && HasRelativeEdges(style.Margin))
            {
                this.ParentDependencies |= Dependency.Margin;
            }

            if (hasPaddingChanges && HasRelativeEdges(style.Padding))
            {
                this.ParentDependencies |= Dependency.Padding;
            }

            var justHidden      = hidden && !this.Hidden;
            var justUnhidden    = !hidden && this.Hidden;
            var justUndependent = oldParentDependencies != Dependency.None && this.ParentDependencies == Dependency.None;
            var justDependent   = oldParentDependencies == Dependency.None && this.ParentDependencies != Dependency.None;

            if (this.Parent != null)
            {
                this.Parent.DependenciesHasChanged = oldContentDependencies != this.ContentDependencies || oldParentDependencies != this.ParentDependencies;

                if (justUnhidden || justDependent)
                {
                    if (justUnhidden)
                    {
                        this.Parent.RenderableNodesCount++;
                    }

                    if (this.ParentDependencies != Dependency.None)
                    {
                        var dependents = this.Target.AssignedSlot?.Layout.Dependents ?? this.Parent.Dependents;

                        dependents.Add(this.Target);
                        dependents.Sort();
                    }
                }
                else if (justHidden || justUndependent)
                {
                    if (justHidden)
                    {
                        this.Parent.RenderableNodesCount--;
                    }

                    var dependents = this.Target.AssignedSlot?.Layout.Dependents ?? this.Parent.Dependents;

                    dependents.Remove(this.Target);
                }
            }
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
            this.RequestUpdate(affectsBoundings);
        }

        this.Target.Visible = !hidden;

        this.OnStyleChanged(property);
        StyleChanged?.Invoke(property);
    }

    private void OnPropertyChanged(StyleProperty property)
    {
        this.computedStyle?.Copy(this.UserStyle!, property);
        this.InvokeStyleChanged(property);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool ResolveHeight(Style style, uint fontSize, ref uint value)
    {
        var resolved = !this.ParentDependencies.HasFlags(Dependency.Height);

        if (!this.ContentDependencies.HasFlags(Dependency.Height))
        {
            ResolveDimension(fontSize, style.Size?.Height, style.MinSize?.Height, style.MaxSize?.Height, ref value, ref resolved);

            if (style.BoxSizing == BoxSizing.Border)
            {
                value = value.ClampSubtract(this.Border.Vertical);
            }
        }

        return resolved;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool ResolveWidth(Style style, uint fontSize, ref uint value)
    {
        var resolved = !this.ParentDependencies.HasFlags(Dependency.Width);

        if (!this.ContentDependencies.HasFlags(Dependency.Width))
        {
            ResolveDimension(fontSize, style.Size?.Width, style.MinSize?.Width, style.MaxSize?.Width, ref value, ref resolved);

            if (style.BoxSizing == BoxSizing.Border)
            {
                value = value.ClampSubtract(this.Border.Horizontal);
            }
        }

        return resolved;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected bool ResolveSize(Style style, uint fontSize, ref Size<uint> size)
    {
        var resolvedWidth  = this.ResolveWidth(style, fontSize, ref size.Width);
        var resolvedHeight = this.ResolveHeight(style, fontSize, ref size.Height);

        return resolvedWidth && resolvedHeight;
    }

    protected abstract void OnStyleChanged(StyleProperty property);

    public void AddState(ElementState state) =>
        this.States |= state;

    public void ComputeStyle() =>
        this.ComputeStyle(default);

    public void HandleTargetMouseOver() =>
        this.SetCursor(this.ComputedStyle.Cursor);

    public override void HandleTargetConnected()
    {
        base.HandleTargetConnected();

        this.ComputeStyle();
    }

    public void IncreaseRenderableNodes() =>
        this.RenderableNodesCount++;

    public void DecreaseRenderableNodes() =>
        this.RenderableNodesCount++;

    public void RemoveState(ElementState state) =>
        this.States &= ~state;

    public override string ToString() =>
        $"{{ Target: {this.Target} }}";

    public void UpdateBoundings() =>
        this.Boundings = new(
            this.size.Width + this.padding.Horizontal + this.border.Horizontal,
            this.size.Height + this.padding.Vertical + this.border.Vertical
        );
}
