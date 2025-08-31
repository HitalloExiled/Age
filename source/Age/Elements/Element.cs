using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Age.Commands;
using Age.Core.Collections;
using Age.Core.Extensions;
using Age.Elements.Enumerators;
using Age.Elements.Events;
using Age.Extensions;
using Age.Numerics;
using Age.Resources;
using Age.Scene;
using Age.Storage;
using Age.Styling;
using static Age.Shaders.CanvasShader;

using AgeInput             = Age.Input;
using Key                  = Age.Platforms.Display.Key;
using PlatformContextEvent = Age.Platforms.Display.ContextEvent;
using PlatformMouseEvent   = Age.Platforms.Display.MouseEvent;

namespace Age.Elements;

public abstract partial class Element : Layoutable, IComparable<Element>, IEnumerable<Element>
{
    internal event Action<StyleProperty>? StyleChanged;

    public event Action? Activated
    {
        add => this.AddEvent(EventProperty.Activated, value);
        remove => this.RemoveEvent(EventProperty.Activated, value);
    }

    public event MouseEventHandler? Blured
    {
        add => this.AddEvent(EventProperty.Blured, value);
        remove => this.RemoveEvent(EventProperty.Blured, value);
    }

    public event MouseEventHandler? Clicked
    {
        add => this.AddEvent(EventProperty.Clicked, value);
        remove => this.RemoveEvent(EventProperty.Clicked, value);
    }

    public event ContextEventHandler? Context
    {
        add => this.AddEvent(EventProperty.Context, value);
        remove => this.RemoveEvent(EventProperty.Context, value);
    }

    public event Action? Deactivated
    {
        add => this.AddEvent(EventProperty.Deactivated, value);
        remove => this.RemoveEvent(EventProperty.Deactivated, value);
    }

    public event MouseEventHandler? DoubleClicked
    {
        add => this.AddEvent(EventProperty.DoubleClicked, value);
        remove => this.RemoveEvent(EventProperty.DoubleClicked, value);
    }

    public event MouseEventHandler? Focused
    {
        add => this.AddEvent(EventProperty.Focused, value);
        remove => this.RemoveEvent(EventProperty.Focused, value);
    }

    public event InputEventHandler? Input
    {
        add
        {
            lock (this.elementLock)
            {
                this.AddEvent(EventProperty.Input, value, out var added);

                if (this.Tree is RenderTree renderTree && added)
                {
                    renderTree.Window.Input += this.OnInput;
                }
            }
        }
        remove
        {
            lock (this.elementLock)
            {
                this.RemoveEvent(EventProperty.Input, value, out var removed);

                if (this.Tree is RenderTree renderTree && removed)
                {
                    renderTree.Window.Input -= this.OnInput;
                }
            }
        }
    }

    public event KeyEventHandler? KeyDown
    {
        add
        {
            lock (this.elementLock)
            {
                this.AddEvent(EventProperty.KeyDown, value, out var added);

                if (this.Tree is RenderTree renderTree && added)
                {
                    renderTree.Window.KeyDown += this.OnKeyDown;
                }
            }
        }
        remove
        {
            lock (this.elementLock)
            {
                this.RemoveEvent(EventProperty.KeyDown, value, out var removed);

                if (this.Tree is RenderTree renderTree && removed)
                {
                    renderTree.Window.KeyDown -= this.OnKeyDown;
                }
            }
        }
    }

    public event KeyEventHandler? KeyUp
    {
        add
        {
            lock (this.elementLock)
            {
                this.AddEvent(EventProperty.KeyUp, value, out var added);

                if (this.Tree is RenderTree renderTree && added)
                {
                    renderTree.Window.KeyUp += this.OnKeyUp;
                }
            }
        }
        remove
        {
            lock (this.elementLock)
            {
                this.RemoveEvent(EventProperty.KeyUp, value, out var removed);

                if (this.Tree is RenderTree renderTree && removed)
                {
                    renderTree.Window.KeyUp -= this.OnKeyUp;
                }
            }
        }
    }

    public event MouseEventHandler? MouseDown
    {
        add => this.AddEvent(EventProperty.MouseDown, value);
        remove => this.RemoveEvent(EventProperty.MouseDown, value);
    }

    public event MouseEventHandler? MouseEnter
    {
        add => this.AddEvent(EventProperty.MouseEnter, value);
        remove => this.RemoveEvent(EventProperty.MouseEnter, value);
    }

    public event MouseEventHandler? MouseLeave
    {
        add => this.AddEvent(EventProperty.MouseLeave, value);
        remove => this.RemoveEvent(EventProperty.MouseLeave, value);
    }

    public event MouseEventHandler? MouseMoved
    {
        add => this.AddEvent(EventProperty.MouseMoved, value);
        remove => this.RemoveEvent(EventProperty.MouseMoved, value);
    }

    public event MouseEventHandler? MouseOut
    {
        add => this.AddEvent(EventProperty.MouseOut, value);
        remove => this.RemoveEvent(EventProperty.MouseOut, value);
    }

    public event MouseEventHandler? MouseOver
    {
        add => this.AddEvent(EventProperty.MouseOver, value);
        remove => this.RemoveEvent(EventProperty.MouseOver, value);
    }

    public event MouseEventHandler? MouseUp
    {
        add => this.AddEvent(EventProperty.MouseUp, value);
        remove => this.RemoveEvent(EventProperty.MouseUp, value);
    }

    public event MouseEventHandler? MouseWheel
    {
        add
        {
            lock (this.elementLock)
            {
                this.AddEvent(EventProperty.MouseWheel, value, out var added);

                if (this.Tree is RenderTree renderTree && added)
                {
                    renderTree.Window.MouseWheel += this.OnScroll;
                }
            }
        }
        remove
        {
            lock (this.elementLock)
            {
                this.RemoveEvent(EventProperty.MouseWheel, value, out var removed);

                if (this.Tree is RenderTree renderTree && removed)
                {
                    renderTree.Window.MouseWheel -= this.OnScroll;
                }
            }
        }
    }

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

    private const uint SCROLL_DEFAULT_BORDER_RADIUS = 3;
    private const uint SCROLL_DEFAULT_MARGIN        = 9;
    private const uint SCROLL_DEFAULT_SIZE          = 6;
    private const uint SCROLL_HOVER_BORDER_RADIUS   = 6;
    private const uint SCROLL_HOVER_MARGIN          = 6;
    private const uint SCROLL_HOVER_SIZE            = 12;
    private const uint SCROLL_MARGIN                = 6;

    private static readonly StylePool stylePool = new();
    private static readonly Color     scrollActiveColor  = (Color.White * 0.8f).WithAlpha(1);
    private static readonly Color     scrollHoverColor   = (Color.White * 0.6f).WithAlpha(1);
    private static readonly Color     scrollDefaultColor = (Color.White * 0.4f).WithAlpha(0.75f);

    private readonly List<Element>                      dependents  = [];
    private readonly Lock                               elementLock = new();
    private readonly KeyedList<EventProperty, Delegate> events      = [];

    private RectEdges     border;
    private bool          childsChanged;
    private byte          commandsSeparator;
    private Size<uint>    content;
    private Dependency    contentDependencies = Dependency.Size;
    private Point<uint>   contentOffset;
    private bool          dependenciesHasChanged;
    private LayoutCommand layoutCommands;
    private RectEdges     margin;
    private StencilLayer? ownStencilLayer;
    private RectEdges     padding;
    private Dependency    parentDependencies;
    private ushort        renderableNodes;
    private Size<uint>    size;
    private Size<uint>    staticContent;

    #region Events
    private Action?              ActivatedEvent     => this.GetEvent<Action>(EventProperty.Activated);
    private MouseEventHandler?   BluredEvent        => this.GetEvent<MouseEventHandler>(EventProperty.Blured);
    private MouseEventHandler?   ClickedEvent       => this.GetEvent<MouseEventHandler>(EventProperty.Clicked);
    private ContextEventHandler? ContextEvent       => this.GetEvent<ContextEventHandler>(EventProperty.Context);
    private Action?              DeactivatedEvent   => this.GetEvent<Action>(EventProperty.Deactivated);
    private MouseEventHandler?   DoubleClickedEvent => this.GetEvent<MouseEventHandler>(EventProperty.DoubleClicked);
    private MouseEventHandler?   FocusedEvent       => this.GetEvent<MouseEventHandler>(EventProperty.Focused);
    private InputEventHandler?   InputEvent         => this.GetEvent<InputEventHandler>(EventProperty.Input);
    private KeyEventHandler?     KeyDownEvent       => this.GetEvent<KeyEventHandler>(EventProperty.KeyDown);
    private KeyEventHandler?     KeyUpEvent         => this.GetEvent<KeyEventHandler>(EventProperty.KeyUp);
    private MouseEventHandler?   MouseDownEvent     => this.GetEvent<MouseEventHandler>(EventProperty.MouseDown);
    private MouseEventHandler?   MouseEnterEvent    => this.GetEvent<MouseEventHandler>(EventProperty.MouseEnter);
    private MouseEventHandler?   MouseLeaveEvent    => this.GetEvent<MouseEventHandler>(EventProperty.MouseLeave);
    private MouseEventHandler?   MouseMovedEvent    => this.GetEvent<MouseEventHandler>(EventProperty.MouseMoved);
    private MouseEventHandler?   MouseOutEvent      => this.GetEvent<MouseEventHandler>(EventProperty.MouseOut);
    private MouseEventHandler?   MouseOverEvent     => this.GetEvent<MouseEventHandler>(EventProperty.MouseOver);
    private MouseEventHandler?   MouseUpEvent       => this.GetEvent<MouseEventHandler>(EventProperty.MouseUp);
    private MouseEventHandler?   ScrolledEvent      => this.GetEvent<MouseEventHandler>(EventProperty.MouseWheel);
    #endregion Events

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

    private ElementState States
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;

                if (this.StyleSheet != null)
                {
                    this.ComputeStyle(this.ComputedStyle.Data);
                }
            }
        }
    }

    private Style? UserStyle
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

                field = value;

                this.ComputeStyle(this.ComputedStyle.Data);
            }
        }
    }

    protected bool IsFocusable { get; set; }

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

    internal protected ShadowTree? ShadowTree { get; set; }

    internal bool        CanScroll     => this.CanScrollX || this.CanScrollY;
    internal bool        CanScrollX    => this.ComputedStyle.Overflow?.HasFlags(Overflow.ScrollX) == true;
    internal bool        CanScrollY    => this.ComputedStyle.Overflow?.HasFlags(Overflow.ScrollY) == true;
    internal Point<uint> ContentOffset => this.contentOffset;

    internal uint FontSize     => this.ComputedStyle.FontSize ?? DEFAULT_FONT_SIZE;
    internal bool IsScrollable { get; set; }

    internal override bool IsParentDependent => this.parentDependencies != Dependency.None;

    internal Span<Command> PreCommands  => this.Commands.AsSpan(..this.commandsSeparator);
    internal Span<Command> PostCommands => this.Commands.AsSpan(this.commandsSeparator..);

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

    public Style   ComputedStyle { get; } = stylePool.Get();
    public Canvas? Canvas        { get; private set; }
    public bool    IsFocused     => this.States.HasFlags(ElementState.Focused);
    public bool    IsHovered     => this.States.HasFlags(ElementState.Hovered);

    public string? InnerText
    {
        get
        {
            var builder = new StringBuilder();

            foreach (var node in this.GetComposedTreeTraversalEnumerator())
            {
                if (node is Text text)
                {
                    builder.Append(text.Buffer);

                    if (this.ComputedStyle.StackDirection == StackDirection.Vertical)
                    {
                        builder.Append('\n');
                    }
                }
            }

            return builder.ToString().TrimEnd();
        }
        set
        {
            if (this.FirstChild is Text text)
            {
                if (text != this.LastChild && text.NextSibling != null && this.LastChild != null)
                {
                    this.RemoveChildrenInRange(text.NextSibling, this.LastChild);
                }

                text.Value = value;
            }
            else
            {
                this.RemoveChildren();

                this.AppendChild(new Text(value));
            }

            this.RequestUpdate(true);
        }
    }

    public Point<uint> Scroll
    {
        get => this.contentOffset;
        set
        {
            value.X = uint.Clamp(value.X, 0, this.content.Width.ClampSubtract(this.size.Width));
            value.Y = uint.Clamp(value.Y, 0, this.content.Height.ClampSubtract(this.size.Height));

            if (this.contentOffset != value)
            {
                this.contentOffset = value;

                if (this.CanScrollX)
                {
                    this.UpdateScrollXControl();
                }

                if (this.CanScrollY)
                {
                    this.UpdateScrollYControl();
                }

                this.ownStencilLayer?.MakeChildrenDirty();
                this.RequestUpdate(false);
            }
        }
    }

    public Style Style
    {
        get => this.UserStyle ??= new();
        set => this.UserStyle = value;
    }

    public StyleSheet? StyleSheet
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;

                this.ComputeStyle(this.ComputedStyle.Data);
            }
        }
    }

    public Element? FirstElementChild
    {
        get
        {
            for (var node = this.FirstChild; node != null; node = node?.NextSibling)
            {
                if (node is Element element)
                {
                    return element;
                }
            }

            return null;
        }
    }

    public Element? LastElementChild
    {
        get
        {
            for (var node = this.LastChild; node != null; node = node?.PreviousSibling)
            {
                if (node is Element element)
                {
                    return element;
                }
            }

            return null;
        }
    }

    protected Element() => this.NodeFlags = NodeFlags.IgnoreUpdates;

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

    private static void PropageteStencilLayer(Element target, StencilLayer? stencilLayer)
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

    private void AddEvent(EventProperty key, Delegate? handler, out bool added)
    {
        if (handler == null)
        {
            added = false;
            return;
        }

        added = !this.events.TryGet(key, out var @delegate);

        @delegate = Delegate.Combine(@delegate, handler);

        this.events[key] = @delegate;
    }

    private void AddEvent(EventProperty key, Delegate? handler) =>
        this.AddEvent(key, handler, out _);

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
        if (!this.IsConnected)
        {
            return;
        }

        var inheritedProperties = this.GetInheritedProperties();

        this.ComputedStyle.Copy(inheritedProperties);

        if (this.StyleSheet?.Base != null)
        {
            this.ComputedStyle.Merge(this.StyleSheet.Base);
        }

        if (this.UserStyle != null)
        {
            this.ComputedStyle.Merge(this.UserStyle);
        }

        if (this.StyleSheet != null)
        {
            merge(ElementState.Focused,  this.StyleSheet.Focused);
            merge(ElementState.Hovered,  this.StyleSheet.Hovered);
            merge(ElementState.Disabled, this.StyleSheet.Disabled);
            merge(ElementState.Enabled,  this.StyleSheet.Enabled);
            merge(ElementState.Checked,  this.StyleSheet.Checked);
            merge(ElementState.Active,   this.StyleSheet.Active);
        }

        this.CompareAndInvoke(this.ComputedStyle.Data, previous);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void merge(ElementState states, Style? style)
        {
            if (this.States.HasFlags(states) && style != null)
            {
                this.ComputedStyle.Merge(style);
            }
        }
    }

    private MouseEvent CreateEvent(in PlatformMouseEvent mouseEvent, bool indirect) =>
        new()
        {
            Target        = this,
            Button        = mouseEvent.Button,
            Delta         = mouseEvent.Delta,
            KeyStates     = mouseEvent.KeyStates,
            PrimaryButton = mouseEvent.PrimaryButton,
            X             = mouseEvent.X,
            Y             = mouseEvent.Y,
            Indirect      = indirect,
        };

    private ContextEvent CreateEvent(in PlatformContextEvent platformContextEvent) =>
        new()
        {
            Target  = this,
            X       = platformContextEvent.X,
            Y       = platformContextEvent.Y,
            ScreenX = platformContextEvent.ScreenX,
            ScreenY = platformContextEvent.ScreenY,
        };

    private void DrawScrollControls()
    {
        var canDrawScrollX = this.CanScrollX && this.size.Width < this.content.Width;
        var canDrawScrollY = this.CanScrollY && this.size.Height < this.content.Height;

        if (canDrawScrollX)
        {
            this.DrawScrollXControl();
        }

        if (canDrawScrollY)
        {
            this.DrawScrollYControl();
        }

        if (canDrawScrollX || canDrawScrollY)
        {
            this.RequestUpdate(false);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DrawScrollXControl()
    {
        var offset         = this.CanScrollY ? SCROLL_HOVER_SIZE : 0;
        var scale          = 1 - ((float)this.Boundings.Width / this.content.Width);
        var scrollBarWidth = this.Boundings.Width - this.border.Horizontal - offset - (SCROLL_MARGIN * 2);

        var scrollCommand = this.AllocateLayoutCommandScrollX();

        scrollCommand.Border    = new(0, SCROLL_DEFAULT_BORDER_RADIUS, default);
        scrollCommand.Color     = scrollDefaultColor;
        scrollCommand.Flags     = Flags.ColorAsBackground;
        scrollCommand.Metadata  = scrollBarWidth;
        scrollCommand.ObjectId  = CombineIds(this.Index + 1, (int)LayoutCommand.ScrollX);
        scrollCommand.Size      = new(float.Max(scrollBarWidth * scale, SCROLL_DEFAULT_SIZE * 2), SCROLL_DEFAULT_SIZE);
        scrollCommand.Transform = Transform2D.CreateTranslated(this.border.Left + SCROLL_MARGIN, this.GetScrollXPositionY());

        this.UpdateScrollXControl(scrollCommand);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DrawScrollYControl()
    {
        var offset          = this.CanScrollX ? SCROLL_HOVER_SIZE : 0;
        var scrollBarHeight = this.Boundings.Height - this.border.Vertical - offset - (SCROLL_MARGIN * 2);
        var scale           = 1 - ((float)this.Boundings.Height / this.content.Height);

        var scrollCommand = this.AllocateLayoutCommandScrollY();

        scrollCommand.Border    = new(0, SCROLL_DEFAULT_BORDER_RADIUS, default);
        scrollCommand.Color     = scrollDefaultColor;
        scrollCommand.Flags     = Flags.ColorAsBackground;
        scrollCommand.Metadata  = scrollBarHeight;
        scrollCommand.ObjectId  = CombineIds(this.Index + 1, (int)LayoutCommand.ScrollY);
        scrollCommand.Size      = new(SCROLL_DEFAULT_SIZE, float.Max(scrollBarHeight * scale, SCROLL_DEFAULT_SIZE * 2));
        scrollCommand.Transform = Transform2D.CreateTranslated(this.GetScrollYPositionX(), -(this.border.Top + SCROLL_MARGIN));

        this.UpdateScrollYControl(scrollCommand);
    }

    private Point<float> GetAlignment(StackDirection direction, Alignment alignmentKind, out AlignmentAxis alignmentAxis)
    {
        var x = -1;
        var y = -1;

        var itemsAlignment = this.ComputedStyle.ItemsAlignment ?? ItemsAlignment.None;

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

    private ComposedElementEnumerator GetComposedElementEnumerator() =>
        new(this);

    private T? GetEvent<T>(EventProperty key) where T : Delegate =>
        this.events.TryGet(key, out var @delegate) ? (T)@delegate : null;

    private int GetIndex(LayoutCommand layoutCommand)
    {
        var mask = layoutCommand - 1;

        return BitOperations.PopCount((uint)(this.layoutCommands & mask));
    }

    private StyleData GetInheritedProperties() =>
        GetStyleSource(this.Parent)?.ComputedStyle is Style parentStyle
            ? new StyleData
            {
                Color         = parentStyle.Color,
                FontFamily    = parentStyle.FontFamily,
                FontSize      = parentStyle.FontSize,
                FontWeight    = parentStyle.FontWeight,
                TextSelection = parentStyle.TextSelection
            }
            : default;

    private RectCommand AllocateLayoutCommand(LayoutCommand layoutCommand)
    {
        var index = this.GetIndex(layoutCommand);

        if (this.layoutCommands.HasFlags(layoutCommand))
        {
            return (RectCommand)this.Commands[index];
        }

        var command = CommandPool.RectCommand.Get();

        switch (layoutCommand)
        {
            case LayoutCommand.Box:
                command.Flags           = Flags.ColorAsBackground;
                command.PipelineVariant = PipelineVariant.Color | PipelineVariant.Index;

                break;

            default:
                command.PipelineVariant = PipelineVariant.Color | PipelineVariant.Index;

                break;
        }

        this.Commands.Insert(index, command);

        this.layoutCommands |= layoutCommand;

        this.UpdateCommandsSeparator();

        return command;
    }

    private RectCommand AllocateLayoutCommandBox() =>
        this.AllocateLayoutCommand(LayoutCommand.Box);

    private RectCommand AllocateLayoutCommandImage() =>
        this.AllocateLayoutCommand(LayoutCommand.Image);

    private RectCommand AllocateLayoutCommandScrollX() =>
        this.AllocateLayoutCommand(LayoutCommand.ScrollX);

    private RectCommand AllocateLayoutCommandScrollY() =>
        this.AllocateLayoutCommand(LayoutCommand.ScrollY);

    private RectCommand GetLayoutCommand(LayoutCommand layoutCommand) =>
        this.TryGetLayoutCommand(layoutCommand, out var rectCommand)
            ? rectCommand
            : throw new InvalidOperationException($"{this} - {layoutCommand} not allocated.");

    private RectCommand GetLayoutCommandBox() =>
        this.GetLayoutCommand(LayoutCommand.Box);

    private RectCommand GetLayoutCommandImage() =>
        this.GetLayoutCommand(LayoutCommand.Image);

    private RectCommand GetLayoutCommandScrollX() =>
        this.GetLayoutCommand(LayoutCommand.ScrollX);

    private RectCommand GetLayoutCommandScrollY() =>
        this.GetLayoutCommand(LayoutCommand.ScrollY);

    private float GetScrollXPositionY() =>
        -(this.Boundings.Height - this.border.Top  - SCROLL_DEFAULT_SIZE - SCROLL_DEFAULT_MARGIN);

    private float GetScrollYPositionX() =>
        this.Boundings.Width - this.border.Left - SCROLL_DEFAULT_SIZE - SCROLL_DEFAULT_MARGIN;

    private void InvokeStyleChanged(StyleProperty property)
    {
        this.OnStyleChanged(property);
        StyleChanged?.Invoke(property);
    }

    private bool HasLayoutCommand(LayoutCommand layoutCommand) =>
        this.layoutCommands.HasFlags(layoutCommand);

    private void HideScrollControls()
    {
        Console.WriteLine($"{this}.HideScrollControls");
        if (this.layoutCommands.HasAnyFlag(LayoutCommand.ScrollX | LayoutCommand.ScrollY))
        {
            this.ReleaseLayoutCommandScrollX();
            this.ReleaseLayoutCommandScrollY();
            this.RequestUpdate(false);
        }
    }

    private void OnInput(char character)
    {
        if (this.IsFocused)
        {
            this.InputEvent?.Invoke(character);
        }
    }

    private void OnKeyDown(Key key)
    {
        if (this.IsFocused)
        {
            var keyEvent = new KeyEvent
            {
                Key     = key,
                Holding = !AgeInput.IsKeyJustPressed(key),
                Modifiers = AgeInput.GetModifiers(),
            };

            this.KeyDownEvent?.Invoke(keyEvent);
        }
    }

    private void OnKeyUp(Key key)
    {
        if (this.IsFocused)
        {
            var keyEvent = new KeyEvent
            {
                Key     = key,
                Holding = !AgeInput.IsKeyJustPressed(key),
                Modifiers = AgeInput.GetModifiers(),
            };

            this.KeyUpEvent?.Invoke(keyEvent);
        }
    }

    private void HandleScrollMouseDown(in PlatformMouseEvent platformMouseEvent, LayoutCommand layoutCommand)
    {
        if (platformMouseEvent.IsHoldingPrimaryButton && layoutCommand is LayoutCommand.ScrollX or LayoutCommand.ScrollY)
        {
            if (layoutCommand == LayoutCommand.ScrollX)
            {
                IsScrollingX = true;

                this.SetScrollXActiveStyle();
            }

            if (layoutCommand == LayoutCommand.ScrollY)
            {
                IsScrollingY = true;

                this.SetScrollYActiveStyle();
            }
        }
    }

    private void HandleScrollMouseEnter()
    {
        if (IsScrolling)
        {
            this.RenderTree!.Window.MouseMove -= this.OnMouseMoved;
            this.HandleScrollMouseMoved(default, false);
        }
        else if (this.CanScroll)
        {
            this.DrawScrollControls();
        }
        else
        {
            for (var element = this; element != null; element = element.ComposedParentElement)
            {
                if (element.CanScroll)
                {
                    element.DrawScrollXControl();

                    break;
                }
            }
        }
    }

    private void HandleScrollMouseLeave()
    {
        if (IsScrolling)
        {
            this.RenderTree!.Window.MouseMove += this.OnMouseMoved;
            this.HandleScrollMouseMoved(default, false);
        }
        else
        {

            this.HideScrollControls();
        }
    }

    private void OnMouseMoved(in PlatformMouseEvent mouseEvent)
    {
        if (mouseEvent.IsHoldingPrimaryButton)
        {
            this.HandleScrollMouseMoved(default, false);
        }
        else
        {
            IsScrollingX = IsScrollingY = false;

            this.HideScrollControls();

            this.RenderTree!.Window.MouseMove -= this.OnMouseMoved;
        }
    }

    private void HandleScrollMouseMoved(uint virtualChildIndex, bool indirect)
    {
        var layoutCommand = (LayoutCommand)virtualChildIndex;

        if (IsScrolling)
        {
            if (indirect)
            {
                return;
            }

            var globalDelta = AgeInput.GetMouseDeltaPosition();
            var delta       = (this.Transform.Matrix.ExtractRotation() * new Vector2<float>(globalDelta.X, globalDelta.Y)).ToPoint();

            if (IsScrollingX)
            {
                this.UpdateScrollXOffset(delta);
            }
            else if (IsScrollingY)
            {
                this.UpdateScrollYOffset(delta);
            }
        }
        else if (layoutCommand is LayoutCommand.ScrollX or LayoutCommand.ScrollY)
        {
            this.SetCursor(Platforms.Display.Cursor.Arrow);

            if (!IsHoveringScrollX && (LayoutCommand)virtualChildIndex is LayoutCommand.ScrollX)
            {
                IsHoveringScrollX = true;

                this.SetScrollXHoverStyle();
            }

            if (!IsHoveringScrollY && (LayoutCommand)virtualChildIndex is LayoutCommand.ScrollY)
            {
                IsHoveringScrollY = true;

                this.SetScrollYHoverStyle();
            }
        }
        else
        {
            if (IsHoveringScrollX)
            {
                IsHoveringScrollX = false;

                this.SetScrollXDefaultStyle();
            }

            if (IsHoveringScrollY)
            {
                IsHoveringScrollY = false;

                this.SetScrollYDefaultStyle();
            }
        }
    }

    private void HandleScrollMouseUp(uint virtualChildIndex, bool indirect)
    {
        if (IsScrolling)
        {
            var layoutCommand = (LayoutCommand)virtualChildIndex;

            if (IsScrollingX)
            {
                if (layoutCommand == LayoutCommand.ScrollX)
                {
                    IsHoveringScrollX = true;

                    this.SetScrollXHoverStyle();
                }
                else
                {
                    this.SetScrollXDefaultStyle();
                }
            }

            if (IsScrollingY)
            {
                if (layoutCommand == LayoutCommand.ScrollY)
                {
                    IsHoveringScrollY = true;

                    this.SetScrollYHoverStyle();
                }
                else
                {
                    this.SetScrollYDefaultStyle();
                }
            }

            if (!IsHoveringScrollX && !IsHoveringScrollY)
            {
                this.SetCursor(this.ComputedStyle.Cursor);
            }

            IsScrollingX = IsScrollingY = false;
        }
    }

    private void OnParentStyleChanged(StyleProperty property)
    {
        var inheritedProperties = this.GetInheritedProperties();

        this.ComputedStyle.Merge(inheritedProperties);
    }

    private void OnPropertyChanged(StyleProperty property)
    {
        this.ComputedStyle?.Copy(this.UserStyle!, property);
        this.InvokeStyleChanged(property);
    }

    private void OnScroll(in PlatformMouseEvent platformMouseEvent)
    {
        if (this.IsScrollable)
        {
            var mouseEvent = this.CreateEvent(platformMouseEvent, false);

            this.ScrolledEvent?.Invoke(mouseEvent);
        }
    }

    private void HandleMouseWheel(in PlatformMouseEvent mouseEvent)
    {
        if (!this.IsHovered)
        {
            return;
        }

        if (!this.IsScrollable)
        {
            for (var element = this; element != null; element = element.ComposedParentElement)
            {
                if (element.IsScrollable)
                {
                    element.HandleMouseWheel(mouseEvent);

                    break;
                }
            }

            return;
        }

        var overflow = this.ComputedStyle.Overflow ?? default;

        if (overflow.HasFlags(Overflow.ScrollX) && mouseEvent.KeyStates.HasFlags(Platforms.Display.MouseKeyStates.Shift))
        {
            this.Scroll = this.Scroll with
            {
                X = (uint)(this.contentOffset.X + (10 * -mouseEvent.Delta))
            };
        }
        else if (overflow.HasFlags(Overflow.ScrollY))
        {
            this.Scroll = this.Scroll with
            {
                Y = (uint)(this.Scroll.Y + (10 * -mouseEvent.Delta))
            };
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

            PropageteStencilLayer(this, this.ContentStencilLayer);
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

            if (property.HasFlags(StyleProperty.Color) && TryGetLayoutCommandBox(out var command))
            {
                command.Color = ComputedStyle.BackgroundColor ?? default;
            }

            this.RequestUpdate(affectsBoundings);
        }

        this.Visible = !hidden;
    }

    private void ReleaseLayoutCommand(LayoutCommand layoutCommand)
    {
        if (this.HasLayoutCommand(layoutCommand))
        {
            var mask = layoutCommand - 1;
            var index = BitOperations.PopCount((uint)(this.layoutCommands & mask));

            var command = (RectCommand)this.Commands[index];

            CommandPool.RectCommand.Return(command);

            this.Commands.RemoveAt(index);

            this.layoutCommands &= ~layoutCommand;

            this.UpdateCommandsSeparator();
        }
    }

    private void ReleaseLayoutCommandBox() =>
        this.ReleaseLayoutCommand(LayoutCommand.Box);

    private void ReleaseLayoutCommandImage() =>
        this.ReleaseLayoutCommand(LayoutCommand.Image);

    private void ReleaseLayoutCommandScrollX() =>
        this.ReleaseLayoutCommand(LayoutCommand.ScrollX);

    private void ReleaseLayoutCommandScrollY() =>
        this.ReleaseLayoutCommand(LayoutCommand.ScrollY);

    private void RemoveEvent(EventProperty key, Delegate? handler) =>
        this.RemoveEvent(key, handler, out _);

    private void RemoveEvent(EventProperty key, Delegate? handler, out bool removed)
    {
        if (handler == null)
        {
            removed = false;
            return;
        }

        this.events.TryGet(key, out var @delegate);

        @delegate = Delegate.Remove(@delegate, handler);

        if (removed = @delegate == null)
        {
            this.events.Remove(key);
        }
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

    private void SetScrollXActiveStyle()
    {
        var commandScrollX = this.GetLayoutCommandScrollX();

        commandScrollX.Color = scrollActiveColor;

        this.RequestUpdate(false);
    }

    private void SetScrollXDefaultStyle()
    {
        var commandScrollX = this.GetLayoutCommandScrollX();

        commandScrollX.Color     = scrollDefaultColor;
        commandScrollX.Size      = commandScrollX.Size with { Height = SCROLL_DEFAULT_SIZE };
        commandScrollX.Transform = Transform2D.CreateTranslated(commandScrollX.Transform.Position.X, this.GetScrollXPositionY());
        commandScrollX.Border    = new(0, SCROLL_DEFAULT_BORDER_RADIUS, default);

        this.RequestUpdate(false);
    }

    private void SetScrollXHoverStyle()
    {
        var commandScrollX = this.GetLayoutCommandScrollX();

        commandScrollX.Color     = scrollHoverColor;
        commandScrollX.Size      = commandScrollX.Size with { Height = SCROLL_HOVER_SIZE };
        commandScrollX.Transform = Transform2D.CreateTranslated(commandScrollX.Transform.Position.X, -(this.Boundings.Height - this.border.Top - SCROLL_HOVER_SIZE - SCROLL_HOVER_MARGIN));
        commandScrollX.Border    = new(0, SCROLL_HOVER_BORDER_RADIUS, default);

        this.RequestUpdate(false);
    }

    private void SetScrollYActiveStyle()
    {
        var commandScrollY = this.GetLayoutCommandScrollY();

        commandScrollY.Color = scrollActiveColor;

        this.RequestUpdate(false);
    }

    private void SetScrollYDefaultStyle()
    {
        var commandScrollY = this.GetLayoutCommandScrollY();

        commandScrollY.Color     = scrollDefaultColor;
        commandScrollY.Size      = commandScrollY.Size with { Width = SCROLL_DEFAULT_SIZE };
        commandScrollY.Transform = Transform2D.CreateTranslated(this.GetScrollYPositionX(), commandScrollY.Transform.Position.Y);
        commandScrollY.Border    = new(0, SCROLL_DEFAULT_BORDER_RADIUS, default);

        this.RequestUpdate(false);
    }

    private void SetScrollYHoverStyle()
    {
        var commandScrollY = this.GetLayoutCommandScrollY();

        commandScrollY.Color     = scrollHoverColor;
        commandScrollY.Size      = commandScrollY.Size with { Width = SCROLL_HOVER_SIZE };
        commandScrollY.Transform = Transform2D.CreateTranslated(this.Boundings.Width - this.border.Left - SCROLL_HOVER_SIZE - SCROLL_HOVER_MARGIN, commandScrollY.Transform.Position.Y);
        commandScrollY.Border    = new(0, SCROLL_HOVER_BORDER_RADIUS, default);

        this.RequestUpdate(false);
    }

    private bool TryGetLayoutCommand(LayoutCommand layoutCommand, [NotNullWhen(true)] out RectCommand? rectCommand)
    {
        var index = this.GetIndex(layoutCommand);

        if (this.layoutCommands.HasFlags(layoutCommand))
        {
            rectCommand = (RectCommand)this.Commands[index];
            return true;
        }

        rectCommand = null;
        return false;
    }

    private bool TryGetLayoutCommandBox([NotNullWhen(true)] out RectCommand? rectCommand) =>
        this.TryGetLayoutCommand(LayoutCommand.Box, out rectCommand);

    private bool TryGetLayoutCommandImage([NotNullWhen(true)] out RectCommand? rectCommand) =>
        this.TryGetLayoutCommand(LayoutCommand.Image, out rectCommand);

    private bool TryGetLayoutCommandScrollX([NotNullWhen(true)] out RectCommand? rectCommand) =>
        this.TryGetLayoutCommand(LayoutCommand.ScrollX, out rectCommand);

    private bool TryGetLayoutCommandScrollY([NotNullWhen(true)] out RectCommand? rectCommand) =>
        this.TryGetLayoutCommand(LayoutCommand.ScrollY, out rectCommand);

    private void UpdateBoundings()
    {
        this.Boundings = new(
            this.size.Width  + this.padding.Horizontal + this.border.Horizontal,
            this.size.Height + this.padding.Vertical   + this.border.Vertical
        );

        this.UpdateCommands();
    }

    private void UpdateCommands()
    {
        var layoutCommandBox = this.AllocateLayoutCommandBox();

        if (this.Boundings.Area > 0)
        {
            layoutCommandBox.Size            = this.Boundings.Cast<float>();
            layoutCommandBox.Border          = this.ComputedStyle.Border ?? default(Shaders.CanvasShader.Border);
            layoutCommandBox.Color           = this.ComputedStyle.BackgroundColor ?? default;
            layoutCommandBox.PipelineVariant = PipelineVariant.Color | PipelineVariant.Index;
            layoutCommandBox.ObjectId        = (uint)(this.Index + 1);

            if (this.ComputedStyle.BackgroundImage != null)
            {
                var layoutCommandImage = this.GetLayoutCommandImage();

                this.ResolveImageSize(this.ComputedStyle.BackgroundImage, layoutCommandImage.TextureMap.Texture.Size.Cast<float>(), out var size, out var transform, out var uv);

                layoutCommandImage.Size       = size;
                layoutCommandImage.Transform  = transform;
                layoutCommandImage.TextureMap = layoutCommandImage.TextureMap with { UV = uv };

                layoutCommandImage.StencilLayer!.MakeDirty();
            }
        }
        else
        {
            layoutCommandBox.PipelineVariant = PipelineVariant.None;
        }

        layoutCommandBox.StencilLayer = this.StencilLayer;

        this.ownStencilLayer?.MakeDirty();
    }

    private void UpdateCommandsSeparator()
    {
        LayoutCommand? layoutCommand = null;

        if (this.layoutCommands.HasFlags(LayoutCommand.Image))
        {
            layoutCommand = LayoutCommand.Image;
        }
        else if (this.layoutCommands.HasFlags(LayoutCommand.Box))
        {
            layoutCommand = LayoutCommand.Box;
        }

        this.commandsSeparator = (byte)(layoutCommand.HasValue ? this.GetIndex(layoutCommand.Value) + 1 : 0);
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
                alignmentType  = element.ComputedStyle.Alignment ?? Alignment.None;
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

    private void UpdateScrollXOffset(in Point<float> delta)
    {
        var scrollCommand = this.GetLayoutCommandScrollX();
        var controlOffset = this.border.Left + SCROLL_MARGIN;
        var localPosition = scrollCommand.Transform.Position.X - controlOffset;
        var position      = float.Clamp(localPosition + delta.X, 0, scrollCommand.Metadata - scrollCommand.Size.Width);
        var ratio         = position / (scrollCommand.Metadata - scrollCommand.Size.Width);
        var offset        = this.content.Width.ClampSubtract(this.size.Width);

        scrollCommand.Transform = Transform2D.CreateTranslated(position + controlOffset, scrollCommand.Transform.Position.Y);

        this.contentOffset.X = (uint)float.Round(offset * ratio);

        this.RequestUpdate(false);
    }

    private void UpdateScrollXControl(RectCommand scrollCommand)
    {
        var ratio  = (float)this.contentOffset.X / this.content.Width.ClampSubtract(this.size.Width);
        var offset = scrollCommand.Metadata - scrollCommand.Size.Width;

        scrollCommand.Transform = Transform2D.CreateTranslated(this.border.Left + SCROLL_MARGIN + (offset * ratio), scrollCommand.Transform.Position.Y);
    }

    private void UpdateScrollXControl() =>
        this.UpdateScrollXControl(this.GetLayoutCommandScrollX());

    private void UpdateScrollYOffset(in Point<float> delta)
    {
        var scrollCommand = this.GetLayoutCommandScrollY();
        var controlOffset = this.border.Top                     + SCROLL_MARGIN;
        var localPosition = -scrollCommand.Transform.Position.Y - controlOffset;
        var position      = float.Clamp(localPosition           + delta.Y, 0, scrollCommand.Metadata - scrollCommand.Size.Height);
        var ratio         = position                            / (scrollCommand.Metadata            - scrollCommand.Size.Height);
        var offset        = this.content.Height.ClampSubtract(this.size.Height);

        scrollCommand.Transform = Transform2D.CreateTranslated(scrollCommand.Transform.Position.X, -(position + controlOffset));

        this.contentOffset.Y = (uint)float.Round(offset * ratio);

        this.RequestUpdate(false);
    }

    private void UpdateScrollYControl(RectCommand scrollCommand)
    {
        var ratio  = (float)this.contentOffset.Y / this.content.Height.ClampSubtract(this.size.Height);
        var offset = scrollCommand.Metadata - scrollCommand.Size.Height;

        scrollCommand.Transform = Transform2D.CreateTranslated(scrollCommand.Transform.Position.X, -(this.border.Top + SCROLL_MARGIN + (offset * ratio)));
    }

    private void UpdateScrollYControl() =>
        this.UpdateScrollYControl(this.GetLayoutCommandScrollY());

    [MemberNotNull(nameof(ShadowTree))]
    protected void AttachShadowTree(bool? inheritsHostStyle = null) => this.ShadowTree = new(this, inheritsHostStyle == true);

    protected override void OnChildAppended(Node child)
    {
        if (this.ShadowTree == null && child is Layoutable layoutable)
        {
            this.HandleLayoutableAppended(layoutable);
        }
    }

    protected override void OnChildRemoved(Node child)
    {
        if (this.ShadowTree == null && child is Layoutable layoutable)
        {
            this.HandleLayoutableRemoved(layoutable);
        }
    }

    protected override void OnConnected(NodeTree tree)
    {
        base.OnConnected(tree);

        this.ShadowTree?.Tree = tree;
    }

    protected override void OnConnected(RenderTree renderTree)
    {
        base.OnConnected(renderTree);

        if (this.events.ContainsKey(EventProperty.Input))
        {
            renderTree.Window.Input += this.OnInput;
        }

        if (this.events.ContainsKey(EventProperty.KeyDown))
        {
            renderTree.Window.KeyDown += this.OnKeyDown;
        }

        if (this.events.ContainsKey(EventProperty.KeyUp))
        {
            renderTree.Window.KeyUp += this.OnKeyUp;
        }

        if (this.events.ContainsKey(EventProperty.MouseWheel))
        {
            renderTree.Window.MouseWheel += this.OnScroll;
        }

        if (!renderTree.IsDirty && !this.Hidden)
        {
            renderTree.MakeDirty();
        }

        this.Canvas = this.ComposedParentElement?.Canvas ?? this.Parent as Canvas;

        this.ComputeStyle(default);

        GetStyleSource(this.Parent)?.StyleChanged += this.OnParentStyleChanged;
    }

    protected override void OnDisconnected(NodeTree tree)
    {
        base.OnDisconnected(tree);

        this.ShadowTree?.Tree = null;
    }

    protected override void OnDisconnected(RenderTree renderTree)
    {
        base.OnDisconnected(renderTree);

        this.Canvas = null;

        renderTree.Window.Input      -= this.OnInput;
        renderTree.Window.KeyDown    -= this.OnKeyDown;
        renderTree.Window.KeyUp      -= this.OnKeyUp;
        renderTree.Window.MouseWheel -= this.OnScroll;

        if (!renderTree.IsDirty && !this.Hidden)
        {
            renderTree.MakeDirty();
        }
    }

    protected override void OnDisposed()
    {
        this.ownStencilLayer?.Dispose();

        if (this.TryGetLayoutCommandImage(out var layoutCommandImage) && !layoutCommandImage.TextureMap.IsDefault)
        {
            TextureStorage.Singleton.Release(layoutCommandImage.TextureMap.Texture);
        }

        foreach (var item in this.Commands)
        {
            CommandPool.RectCommand.Return((RectCommand)item);
        }

        this.Commands.Clear();

        this.ShadowTree?.Dispose();

        stylePool.Return(this.ComputedStyle);
    }

    protected override void OnIndexed()
    {
        var zIndex = this.ComputedStyle.ZIndex ?? this.ComposedParentElement?.ComputedStyle.ZIndex ?? 0;

        if (this.TryGetLayoutCommandBox(out var boxCommand))
        {
            boxCommand.ObjectId = this.Boundings.Area > 0 ? (uint)(this.Index + 1) : 0;
            boxCommand.ZIndex   = zIndex;
        }

        if (this.TryGetLayoutCommandScrollX(out var scrollXCommand))
        {
            scrollXCommand.ObjectId = CombineIds(this.Index + 1, (int)LayoutCommand.ScrollX);
            scrollXCommand.ZIndex   = zIndex;
        }

        if (this.TryGetLayoutCommandScrollY(out var scrollYCommand))
        {
            scrollYCommand.ObjectId = CombineIds(this.Index + 1, (int)LayoutCommand.ScrollY);
            scrollYCommand.ZIndex   = zIndex;
        }
    }

    protected override void OnRemoved(Node parent)
    {
        base.OnRemoved(parent);

        GetStyleSource(parent)?.StyleChanged -= this.OnParentStyleChanged;
    }

    private protected void AddState(ElementState state) =>
        this.States |= state;

    private protected void RemoveState(ElementState state) =>
        this.States &= ~state;

    internal static Element? GetCommonComposedAncestor(Element left, Element right)
    {
        var leftComposedParentElement  = left.ComposedParentElement;
        var rightComposedParentElement = right.ComposedParentElement;

        if (leftComposedParentElement == rightComposedParentElement)
        {
            return leftComposedParentElement;
        }
        else if (left == rightComposedParentElement)
        {
            return left;
        }
        else if (leftComposedParentElement == right)
        {
            return right;
        }
        else
        {
            var leftDepth  = 0;
            var rightDepth = 0;

            var currentLeft  = leftComposedParentElement;
            var currentRight = rightComposedParentElement;

            while (currentLeft != null)
            {
                leftDepth++;
                currentLeft  = currentLeft.ComposedParentElement;
            }

            while (currentRight != null)
            {
                rightDepth++;
                currentRight  = currentRight.ComposedParentElement;
            }

            currentLeft  = left;
            currentRight = right;

            while (leftDepth > rightDepth)
            {
                currentLeft = currentLeft.ComposedParentElement!;
                leftDepth--;
            }

            while (leftDepth < rightDepth)
            {
                currentRight = currentRight.ComposedParentElement!;
                rightDepth--;
            }

            while (currentLeft != currentRight)
            {
                currentLeft  = currentLeft.ComposedParentElement;
                currentRight = currentRight.ComposedParentElement;

                if (currentLeft == null || currentRight == null)
                {
                    return null;
                }
            }

            return currentLeft;
        }
    }

    internal static ComposedPath GetComposedPathBetween(Element left, Element right)
    {
        var leftToAncestor  = new List<Element>();
        var rightToAncestor = new List<Element>();

        GetComposedPathBetween(leftToAncestor, rightToAncestor, left, right);

        return new(leftToAncestor, rightToAncestor);
    }

    internal static void GetComposedPathBetween(List<Element> leftToAncestor, List<Element> rightToAncestor, Element left, Element right)
    {
        const string ERROR_MESSAGE = "The specified elements do not share a common ancestor in the composed tree.";

        leftToAncestor.Clear();
        rightToAncestor.Clear();

        leftToAncestor.Add(left);
        rightToAncestor.Add(right);

        var leftComposedParentElement  = left.ComposedParentElement;
        var rightComposedParentElement = right.ComposedParentElement;

        if (leftComposedParentElement == rightComposedParentElement)
        {
            if (leftComposedParentElement == null)
            {
                throw new InvalidOperationException(ERROR_MESSAGE);
            }

            leftToAncestor.Add(leftComposedParentElement);
            rightToAncestor.Add(leftComposedParentElement);
        }
        else if (left == rightComposedParentElement)
        {
            rightToAncestor.Add(left);
        }
        else if (leftComposedParentElement == right)
        {
            leftToAncestor.Add(right);
        }
        else
        {
            var leftDepth  = 0;
            var rightDepth = 0;

            var currentLeft  = leftComposedParentElement;
            var currentRight = rightComposedParentElement;

            while (currentLeft != null)
            {
                leftDepth++;
                currentLeft = currentLeft.ComposedParentElement;
            }

            while (currentRight != null)
            {
                rightDepth++;
                currentRight  = currentRight.ComposedParentElement;
            }

            currentLeft  = left;
            currentRight = right;

            while (leftDepth > rightDepth)
            {
                currentLeft = currentLeft.ComposedParentElement!;
                leftDepth--;

                leftToAncestor.Add(currentLeft);
            }

            while (leftDepth < rightDepth)
            {
                currentRight = currentRight.ComposedParentElement!;
                rightDepth--;

                rightToAncestor.Add(currentRight);
            }

            while (currentLeft != currentRight)
            {
                currentLeft  = currentLeft.ComposedParentElement;
                currentRight = currentRight.ComposedParentElement;

                if (currentLeft == null || currentRight == null)
                {
                    leftToAncestor.Clear();
                    rightToAncestor.Clear();

                    throw new InvalidOperationException(ERROR_MESSAGE);
                }

                leftToAncestor.Add(currentLeft);
                rightToAncestor.Add(currentRight);
            }
        }
    }

    internal ComposedTreeEnumerator GetComposedTreeEnumerator() =>
        new(this);

    internal ComposedTreeTraversalEnumerator GetComposedTreeTraversalEnumerator(Stack<(Slot, int)>? stack = null, Action<Element>? parentCallback = null) =>
        new(this, stack, parentCallback);

    internal int GetEffectiveDepth()
    {
        var depth = 0;

        var node = this.EffectiveParentElement;

        while (node != null)
        {
            depth++;
            node = node.EffectiveParentElement;
        }

        return depth;
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

    internal void ApplyCursor() =>
        this.SetCursor(this.ComputedStyle.Cursor);

    internal void InvokeActivate()
    {
        this.AddState(ElementState.Active);
        this.ActivatedEvent?.Invoke();
    }

    internal void InvokeBlur(in PlatformMouseEvent platformMouseEvent)
    {
        if (this.IsFocusable)
        {
            this.RemoveState(ElementState.Focused);

            this.BluredEvent?.Invoke(this.CreateEvent(platformMouseEvent, false));
        }
    }

    internal void InvokeClick(in PlatformMouseEvent platformMouseEvent, uint virtualChildIndex, bool indirect)
    {
        if ((LayoutCommand)virtualChildIndex is not LayoutCommand.ScrollX and not LayoutCommand.ScrollY)
        {
            this.ClickedEvent?.Invoke(this.CreateEvent(platformMouseEvent, indirect));
        }
    }

    internal void InvokeContext(in PlatformContextEvent platformContextEvent, uint virtualChildIndex)
    {
        if ((LayoutCommand)virtualChildIndex is not LayoutCommand.ScrollX and not LayoutCommand.ScrollY)
        {
            this.ContextEvent?.Invoke(this.CreateEvent(platformContextEvent));
        }
    }

    internal void InvokeDeactivate()
    {
        this.RemoveState(ElementState.Active);
        this.DeactivatedEvent?.Invoke();
    }

    internal void InvokeDoubleClick(in PlatformMouseEvent platformMouseEvent, uint virtualChildIndex, bool indirect)
    {
        if ((LayoutCommand)virtualChildIndex is not LayoutCommand.ScrollX and not LayoutCommand.ScrollY)
        {
            this.DoubleClickedEvent?.Invoke(this.CreateEvent(platformMouseEvent, indirect));
        }
    }

    internal void InvokeFocus(in PlatformMouseEvent platformMouseEvent)
    {
        if (this.IsFocusable)
        {
            this.AddState(ElementState.Focused);

            this.FocusedEvent?.Invoke(this.CreateEvent(platformMouseEvent, false));
        }
    }

    internal void InvokeMouseDown(in PlatformMouseEvent platformMouseEvent, uint virtualChildIndex, bool indirect)
    {
        if ((LayoutCommand)virtualChildIndex is LayoutCommand.ScrollX or LayoutCommand.ScrollY)
        {
            this.HandleScrollMouseDown(platformMouseEvent, (LayoutCommand)virtualChildIndex);
        }
        else
        {
            this.MouseDownEvent?.Invoke(this.CreateEvent(platformMouseEvent, indirect));
        }
    }

    internal void InvokeMouseEnter(in PlatformMouseEvent platformMouseEvent)
    {
        this.MouseEnterEvent?.Invoke(this.CreateEvent(platformMouseEvent, false));

        if (this.CanScroll)
        {
            this.HandleScrollMouseEnter();
        }
    }

    internal void InvokeMouseLeave(in PlatformMouseEvent platformMouseEvent)
    {
        this.MouseLeaveEvent?.Invoke(this.CreateEvent(platformMouseEvent, false));

        if (this.CanScroll)
        {
            this.HandleScrollMouseLeave();
        }
    }

    internal void InvokeMouseMoved(in PlatformMouseEvent platformMouseEvent, uint virtualChildIndex, bool indirect)
    {
        this.MouseMovedEvent?.Invoke(this.CreateEvent(platformMouseEvent, indirect));

        if (this.CanScroll)
        {
            this.HandleScrollMouseMoved(virtualChildIndex, indirect);
        }
    }

    internal void InvokeMouseOut(in PlatformMouseEvent platformMouseEvent)
    {
        if (!IsSelectingText)
        {
            this.RemoveState(ElementState.Hovered);
        }

        this.MouseOutEvent?.Invoke(this.CreateEvent(platformMouseEvent, false));
    }

    internal void InvokeMouseOver(in PlatformMouseEvent platformMouseEvent)
    {
        if (!IsSelectingText)
        {
            this.AddState(ElementState.Hovered);
            this.ApplyCursor();
        }

        this.MouseOverEvent?.Invoke(this.CreateEvent(platformMouseEvent, false));
    }

    internal void InvokeMouseUp(in PlatformMouseEvent platformMouseEvent, uint virtualChildIndex, bool indirect)
    {
        if (this.CanScroll)
        {
            this.HandleScrollMouseUp(virtualChildIndex, indirect);
        }

        this.MouseUpEvent?.Invoke(this.CreateEvent(platformMouseEvent, indirect));
    }

    internal void InvokeMouseWheel(in PlatformMouseEvent mouseEvent)
    {
        this.ScrolledEvent?.Invoke(this.CreateEvent(mouseEvent, false));
        this.HandleMouseWheel(mouseEvent);
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

    public void Blur()
    {
        this.RemoveState(ElementState.Focused);
        this.BluredEvent?.Invoke(new() { Target = this });
    }

    public void Click()
    {
        this.AddState(ElementState.Active);
        this.ClickedEvent?.Invoke(new() { Target = this });
    }

    public int CompareTo(Element? other)
    {
        if (other == null)
        {
            return 1;
        }
        else if (this == other)
        {
            return 0;
        }

        var left  = this;
        var right = other;

        var leftParent  = left.EffectiveParentElement;
        var rightParent = right.EffectiveParentElement;

        if (leftParent != rightParent)
        {
            var leftDepth  = getDepth(leftParent);
            var rightDepth = getDepth(rightParent);

            while (leftDepth > rightDepth)
            {
                leftParent = left.EffectiveParentElement;

                if (leftParent == right)
                {
                    return 1;
                }

                left = leftParent!;
                leftDepth--;
            }

            while (leftDepth < rightDepth)
            {
                rightParent = right.EffectiveParentElement;

                if (rightParent == left)
                {
                    return -1;
                }

                right = rightParent!;
                rightDepth--;
            }

            leftParent  = left.EffectiveParentElement;
            rightParent = right.EffectiveParentElement;

            while (leftParent != rightParent)
            {
                left  = leftParent!;
                right = rightParent!;

                leftParent  = left.EffectiveParentElement;
                rightParent = right.EffectiveParentElement;
            }
        }

        if (leftParent == rightParent)
        {
            if (leftParent == null)
            {
                throw new InvalidOperationException("Can't compare an root node to another");
            }

            if (left.Parent == right.Parent)
            {
                if (left == right.NextSibling)
                {
                    return 1;
                }

                if (left != right.PreviousSibling)
                {
                    for (var node = left!.PreviousSibling; node != null; node = node?.PreviousSibling)
                    {
                        if (node == right)
                        {
                            return 1;
                        }
                    }
                }
            }
            else if (right.Parent is ShadowTree)
            {
                return 1;
            }
        }

        return -1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int getDepth(Element? parentElement) =>
            parentElement == null ? 0 : parentElement.GetEffectiveDepth() + 1;
    }

    public void Focus()
    {
        this.AddState(ElementState.Focused);
        this.FocusedEvent?.Invoke(new() { Target = this });
    }

    public BoxModel GetBoxModel()
    {
        var boundings = this.GetUpdatedBoundings();

        var padding = this.padding;
        var border  = this.border;
        var content = this.content;
        var margin  = this.margin;

        return new()
        {
            Margin    = margin,
            Boundings = boundings,
            Border    = border,
            Padding   = padding,
            Content   = content,
        };
    }

    public void ScrollTo(in Rect<int> boundings)
    {
        if (!this.CanScrollX || !this.CanScrollY)
        {
            return;
        }

        var boxModel = this.GetBoxModel();

        var boundsLeft   = boxModel.Boundings.Left   + boxModel.Border.Left   + boxModel.Padding.Left;
        var boundsRight  = boxModel.Boundings.Right  - boxModel.Border.Right  - boxModel.Padding.Right;
        var boundsTop    = boxModel.Boundings.Top    + boxModel.Border.Top    + boxModel.Padding.Top;
        var boundsBottom = boxModel.Boundings.Bottom - boxModel.Border.Bottom - boxModel.Padding.Bottom;

        var scroll = this.Scroll;

        if (this.CanScrollX)
        {
            if (boundings.Left < boundsLeft)
            {
                var characterLeft = boundings.Left + scroll.X;

                scroll.X = (uint)(characterLeft - boundsLeft);
            }
            else if (boundings.Right > boundsRight)
            {
                var characterRight = boundings.Right + scroll.X;

                scroll.X = (uint)(characterRight - boundsRight);
            }
        }

        if (this.CanScrollY)
        {
            if (boundings.Top < boundsTop)
            {
                var characterTop = boundings.Top + scroll.Y;

                scroll.Y = (uint)(characterTop - boundsTop);
            }
            else if (boundings.Bottom > boundsBottom)
            {
                var characterBottom = boundings.Bottom + scroll.Y;

                scroll.Y = (uint)(characterBottom - boundsBottom);
            }
        }

        this.Scroll = scroll;
    }

    public void ScrollTo(Element element) =>
        this.ScrollTo(element.GetUpdatedBoundings());

    IEnumerator<Element> IEnumerable<Element>.GetEnumerator()
    {
        foreach (var node in this)
        {
            if (node is Element element)
            {
                yield return element;
            }
        }
    }

    internal static bool IsScrollControl(uint virtualChildIndex) =>
        (LayoutCommand)virtualChildIndex is LayoutCommand.ScrollX or LayoutCommand.ScrollY;
}
