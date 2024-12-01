using System.Runtime.CompilerServices;
using Age.Commands;
using Age.Extensions;
using Age.Numerics;
using Age.Scene;
using Age.Styling;

using static Age.Shaders.CanvasShader;

namespace Age.Elements.Layouts;

internal sealed partial class BoxLayout : Layout
{
    #region 8-bytes
    private readonly List<Element> dependents = [];
    private readonly Element       target;

    private StencilLayer? ownStencilLayer;

    protected override StencilLayer? ContentStencilLayer => this.ownStencilLayer ?? this.StencilLayer;

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
    #endregion

    #region 4-bytes
    private RawRectEdges border;
    private Size<uint>   content;
    private Dependency   contentDependent;
    private RawRectEdges margin;
    private RawRectEdges padding;
    private Dependency   parentDependent;
    private uint         renderableNodesCount;
    private Size<uint>   staticContent;

    public Point<int> ScrollOffset
    {
        get;
        set
        {
            value.X = Math<int>.MinMax(-(int)this.content.Width.ClampSubtract(this.Size.Width), 0, value.X);
            value.Y = Math<int>.MinMax(0, (int)this.content.Height.ClampSubtract(this.Size.Height), value.Y);

            if (field != value)
            {
                field = value;

                this.ownStencilLayer?.MakeDirty();
                this.RequestUpdate();
            }
        }
    }

    public uint FontSize { get; private set; }
    #endregion

    #region 1-byte
    private bool childsChanged;
    private bool dependenciesHasChanged;

    public bool IsScrollable { get; internal set; }
    public bool IsHoveringText { get; set; }
    #endregion

    public Size<uint> Boundings =>
        new(
            this.Size.Width  + this.padding.Horizontal + this.border.Horizontal,
            this.Size.Height + this.padding.Vertical   + this.border.Vertical
        );
    public Size<uint> BoundingsWithMargin =>
        new(
            this.Size.Width  + this.padding.Horizontal + this.border.Horizontal + this.margin.Horizontal,
            this.Size.Height + this.padding.Vertical   + this.border.Vertical   + this.margin.Vertical
        );

    public RawRectEdges Border => this.border;

    public Size<uint> InnerBoundings =>
        new(
            this.Size.Width  + this.padding.Horizontal,
            this.Size.Height + this.padding.Vertical
        );

    public StyledStateManager State { get; } = new();

    public override BoxLayout?  Parent    => this.target.ParentElement?.Layout;
    public override Element     Target    => this.target;
    public override Transform2D Transform => (this.State.Style.Transform ?? new Transform2D()) * base.Transform;

    public BoxLayout(Element target)
    {
        this.target = target;

        this.State.Changed += this.StyleChanged;
    }

    private static void CalculatePendingPaddingHorizontal(BoxLayout layout, in Size<uint> size, ref RawRectEdges padding)
    {
        if (layout.State.Style.Padding?.Left?.TryGetPercentage(out var left) == true)
        {
            padding.Left = (uint)(size.Width * left);
        }

        if (layout.State.Style.Padding?.Right?.TryGetPercentage(out var right) == true)
        {
            padding.Right = (uint)(size.Width * right);
        }
    }

    private static void CalculatePendingPaddingVertical(BoxLayout layout, in Size<uint> size, ref RawRectEdges padding)
    {
        if (layout.State.Style.Padding?.Top?.TryGetPercentage(out var top) == true)
        {
            padding.Top = (uint)(size.Width * top);
        }

        if (layout.State.Style.Padding?.Bottom?.TryGetPercentage(out var bottom) == true)
        {
            padding.Bottom = (uint)(size.Width * bottom);
        }
    }

    private static void CalculatePendingMarginHorizontal(BoxLayout layout, StackKind stack, in Size<uint> size, ref RawRectEdges margin, ref Size<uint> contentSize)
    {
        var horizontal = 0u;

        if (layout.State.Style.Margin?.Left?.TryGetPercentage(out var left) == true)
        {
            horizontal += margin.Left = (uint)(size.Width * left);
        }

        if (layout.State.Style.Margin?.Right?.TryGetPercentage(out var right) == true)
        {
            horizontal += margin.Right = (uint)(size.Width * right);
        }

        if (horizontal > 0)
        {
            if (stack == StackKind.Horizontal)
            {
                contentSize.Width += horizontal;
            }
            else
            {
                contentSize.Width = uint.Max(layout.Size.Width + layout.padding.Horizontal + layout.border.Horizontal + horizontal, contentSize.Width);
            }
        }
    }

    private static void CalculatePendingMarginVertical(BoxLayout layout, StackKind stack, in Size<uint> size, ref RawRectEdges margin, ref Size<uint> contentSize)
    {
        var vertical = 0u;

        if (layout.State.Style.Margin?.Top?.TryGetPercentage(out var top) == true)
        {
            vertical += margin.Top = (uint)(size.Height * top);
        }

        if (layout.State.Style.Margin?.Bottom?.TryGetPercentage(out var bottom) == true)
        {
            vertical += margin.Bottom = (uint)(size.Height * bottom);
        }

        if (vertical > 0)
        {
            if (stack == StackKind.Vertical)
            {
                contentSize.Height += vertical;
            }
            else
            {
                contentSize.Height = uint.Max(layout.Size.Height + layout.padding.Vertical + layout.border.Vertical + vertical, contentSize.Height);
            }
        }
    }

    private static void SetStencilLayer(BoxLayout layout, StencilLayer? stencilLayer)
    {
        var enumerator = layout.target.GetTraverseEnumerator();

        while (enumerator.MoveNext())
        {
            var current = enumerator.Current!;

            if (current is ContainerNode containerNode)
            {
                if (containerNode.Layout.StencilLayer == stencilLayer)
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
                    containerNode.Layout.StencilLayer = stencilLayer;
                }
            }
        }
    }

    private Point<float> GetAlignment(StackKind stack, AlignmentKind alignmentKind, out AlignmentAxis alignmentAxis)
    {
        var x = -1;
        var y = -1;

        var itemsAlignment = this.State.Style.ItemsAlignment ?? ItemsAlignmentKind.None;

        alignmentAxis = AlignmentAxis.Horizontal | AlignmentAxis.Vertical;

        if (alignmentKind.HasFlag(AlignmentKind.Left) || stack == StackKind.Vertical && (itemsAlignment == ItemsAlignmentKind.Begin || alignmentKind.HasFlag(AlignmentKind.Start)))
        {
            x = -1;
        }
        else if (alignmentKind.HasFlag(AlignmentKind.Right) || stack == StackKind.Vertical && (itemsAlignment == ItemsAlignmentKind.End || alignmentKind.HasFlag(AlignmentKind.End)))
        {
            x = 1;
        }
        else if (alignmentKind.HasFlag(AlignmentKind.Center) || stack == StackKind.Vertical && itemsAlignment == ItemsAlignmentKind.Center)
        {
            x = 0;
        }
        else
        {
            alignmentAxis &= ~AlignmentAxis.Horizontal;
        }

        if (alignmentKind.HasFlag(AlignmentKind.Top) || stack == StackKind.Horizontal && (itemsAlignment == ItemsAlignmentKind.Begin || alignmentKind.HasFlag(AlignmentKind.Start)))
        {
            y = -1;
        }
        else if (alignmentKind.HasFlag(AlignmentKind.Bottom) || stack == StackKind.Horizontal && (itemsAlignment == ItemsAlignmentKind.End || alignmentKind.HasFlag(AlignmentKind.End)))
        {
            y = 1;
        }
        else if (alignmentKind.HasFlag(AlignmentKind.Center) || stack == StackKind.Horizontal && itemsAlignment == ItemsAlignmentKind.Center)
        {
            y = 0;
        }
        else
        {
            if (itemsAlignment == ItemsAlignmentKind.Baseline || alignmentKind.HasFlag(AlignmentKind.Baseline))
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
            this.Target.SingleCommand = command = new()
            {
                Flags           = Flags.ColorAsBackground,
                PipelineVariant = PipelineVariant.Color | PipelineVariant.Index,
            };
        }

        return command;
    }
     private void CalculateLayout()
    {
        var stack = this.State.Style.Stack ?? StackKind.Horizontal;

        this.content       = new Size<uint>();
        this.staticContent = new Size<uint>();
        this.BaseLine      = -1;

        foreach (var node in this.Target)
        {
            if (node is not ContainerNode child)
            {
                continue;
            }

            child.Layout.Update();

            if (child.Layout.Hidden)
            {
                continue;
            }

            Size<uint> childSize;

            var dependencies = Dependency.None;

            if (child is Element element)
            {
                childSize = element.Layout.BoundingsWithMargin;
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

        if (this.contentDependent.HasFlag(Dependency.Width) || this.contentDependent.HasFlag(Dependency.Height))
        {
            this.CalculatePendingMargin(ref this.content);
        }

        var size = this.content;

        var resolvedMargin  = this.ResolveMargin();
        var resolvedPadding = this.ResolvePadding();
        var resolvedSize    = this.ResolveSize(ref size);

        var sizeHasChanged = this.Size != size;

        this.Size = size;

        if (resolvedSize && resolvedMargin && resolvedPadding)
        {
            if (sizeHasChanged || this.childsChanged || this.dependenciesHasChanged || this.Target is Canvas)
            {
                this.CalculatePendingLayouts();
            }

            this.UpdateRect();
        }
    }

    private void CalculatePendingMargin(ref Size<uint> size)
    {
        var contentSize = size;

        var stack = this.State.Style.Stack ?? StackKind.Horizontal;

        foreach (var dependent in this.dependents)
        {
            if (dependent.Layout.parentDependent.HasFlag(Dependency.Padding) || dependent.Layout.parentDependent.HasFlag(Dependency.Margin))
            {
                var margin = dependent.Layout.margin;

                if (!this.parentDependent.HasFlag(Dependency.Width))
                {
                    CalculatePendingMarginHorizontal(dependent.Layout, stack, size, ref margin, ref contentSize);
                }

                if (!this.parentDependent.HasFlag(Dependency.Height))
                {
                    CalculatePendingMarginVertical(dependent.Layout, stack, size, ref margin, ref contentSize);
                }

                dependent.Layout.margin = margin;
            }
        }

        size = contentSize;
    }

    private void CalculatePendingLayouts()
    {
        var content        = this.content;
        var avaliableSpace = this.Size.ClampSubtract(this.staticContent);
        var stack          = this.State.Style.Stack ?? StackKind.Horizontal;

        foreach (var dependent in this.dependents)
        {
            var margin  = dependent.Layout.margin;
            var padding = dependent.Layout.padding;
            var size    = dependent.Layout.Size;

            if (!this.contentDependent.HasFlag(Dependency.Width) || stack == StackKind.Vertical)
            {
                if (!this.contentDependent.HasFlag(Dependency.Width))
                {
                    CalculatePendingPaddingHorizontal(dependent.Layout, this.Size, ref padding);
                    CalculatePendingMarginHorizontal(dependent.Layout, stack, this.Size, ref margin, ref content);
                }

                if (dependent.Layout.parentDependent.HasFlag(Dependency.Width))
                {
                    var modified = false;

                    if (dependent.Layout.State.Style.Size?.Width?.TryGetPercentage(out var percentage) == true)
                    {
                        size.Width = (uint)(this.Size.Width * percentage);

                        if (dependent.Layout.State.Style.MinSize?.Width?.TryGetPixel(out var min) == true && this.State.Style.MaxSize?.Width?.TryGetPixel(out var max) == true)
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
                        else if (dependent.Layout.State.Style.MinSize?.Width?.TryGetPixel(out min) == true)
                        {
                            if (size.Width < min)
                            {
                                size.Width = min;
                            }
                        }
                        else if (dependent.Layout.State.Style.MaxSize?.Width?.TryGetPixel(out max) == true)
                        {
                            if (size.Width > max)
                            {
                                size.Width = max;
                            }
                        }

                        modified = true;
                    }
                    else if (dependent.Layout.State.Style.MinSize?.Width?.TryGetPercentage(out var min) == true && dependent.Layout.State.Style.MaxSize?.Width?.TryGetPercentage(out var max) == true)
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
                    else if (dependent.Layout.State.Style.MinSize?.Width?.TryGetPercentage(out min) == true)
                    {
                        var minValue = (uint)(this.Size.Width * min);

                        if (size.Width < minValue)
                        {
                            size.Width = minValue;

                            modified = true;
                        }
                    }
                    else if (dependent.Layout.State.Style.MaxSize?.Width?.TryGetPercentage(out max) == true)
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
                        content.Width -= dependent.Layout.BoundingsWithMargin.Width;

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
                            .ClampSubtract(dependent.Layout.border.Horizontal)
                            .ClampSubtract(dependent.Layout.padding.Horizontal)
                            .ClampSubtract(dependent.Layout.margin.Horizontal);
                    }
                }
            }

            if (!this.contentDependent.HasFlag(Dependency.Height) || stack == StackKind.Horizontal)
            {
                if (!this.contentDependent.HasFlag(Dependency.Height))
                {
                    CalculatePendingPaddingVertical(dependent.Layout, this.Size, ref padding);
                    CalculatePendingMarginVertical(dependent.Layout, stack, this.Size, ref margin, ref content);
                }

                if (dependent.Layout.parentDependent.HasFlag(Dependency.Height))
                {
                    var modified = false;

                    if (dependent.Layout.State.Style.Size?.Height?.TryGetPercentage(out var percentage) == true)
                    {
                        size.Height = (uint)(this.Size.Height * percentage);

                        if (dependent.Layout.State.Style.MinSize?.Height?.TryGetPixel(out var min) == true && this.State.Style.MaxSize?.Height?.TryGetPixel(out var max) == true)
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
                        else if (dependent.Layout.State.Style.MinSize?.Height?.TryGetPixel(out min) == true)
                        {
                            if (size.Height < min)
                            {
                                size.Height = min;
                            }
                        }
                        else if (dependent.Layout.State.Style.MaxSize?.Height?.TryGetPixel(out max) == true)
                        {
                            if (size.Height > max)
                            {
                                size.Height = max;
                            }
                        }

                        modified = true;
                    }
                    else if (dependent.Layout.State.Style.MinSize?.Height?.TryGetPercentage(out var min) == true && dependent.Layout.State.Style.MaxSize?.Height?.TryGetPercentage(out var max) == true)
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
                    else if (dependent.Layout.State.Style.MinSize?.Height?.TryGetPercentage(out min) == true)
                    {
                        var minValue = (uint)(this.Size.Height * min);

                        if (size.Height < minValue)
                        {
                            size.Height = minValue;

                            modified = true;
                        }
                    }
                    else if (dependent.Layout.State.Style.MaxSize?.Height?.TryGetPercentage(out max) == true)
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
                        content.Height -= dependent.Layout.BoundingsWithMargin.Height;

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
                            .ClampSubtract(dependent.Layout.border.Vertical)
                            .ClampSubtract(dependent.Layout.padding.Vertical)
                            .ClampSubtract(dependent.Layout.margin.Vertical);
                    }
                }
            }

            if (dependent.Layout.dependenciesHasChanged || dependent.Layout.childsChanged || size != dependent.Layout.Size || padding != dependent.Layout.padding || margin != dependent.Layout.margin)
            {
                dependent.Layout.childsChanged          = false;
                dependent.Layout.dependenciesHasChanged = false;
                dependent.Layout.Size    = size;
                dependent.Layout.padding = padding;
                dependent.Layout.margin  = margin;

                dependent.Layout.CalculatePendingLayouts();
                dependent.Layout.UpdateDisposition();
            }

            dependent.Layout.UpdateRect();

            dependent.Layout.HasPendingUpdate = false;

            this.CheckHightestInlineChild(stack, dependent);
        }

        this.content = content;
    }

    private void CheckHightestInlineChild(StackKind stack, ContainerNode child)
    {
        if (child.Layout.BaseLine == -1)
        {
            return;
        }

        var baseline     = child.Layout.BaseLine;
        var hasAlignment = false;

        if (child is Element element)
        {
            hasAlignment = element.Layout.State.Style.Alignment.HasValue
                && (
                    element.Layout.State.Style.Alignment.Value == AlignmentKind.Center
                    || element.Layout.State.Style.Alignment.Value.HasFlag(AlignmentKind.Top)
                    || element.Layout.State.Style.Alignment.Value.HasFlag(AlignmentKind.Bottom)
                    || stack == StackKind.Vertical && element.Layout.State.Style.Alignment.Value.HasFlag(AlignmentKind.Start)
                    || stack == StackKind.Vertical && element.Layout.State.Style.Alignment.Value.HasFlag(AlignmentKind.Center)
                    || stack == StackKind.Vertical && element.Layout.State.Style.Alignment.Value.HasFlag(AlignmentKind.End)
                );

            baseline += (int)(element.Layout.padding.Top + element.Layout.border.Top + element.Layout.margin.Top);
        }

        if (!hasAlignment && baseline > this.BaseLine)
        {
            this.BaseLine = baseline;
        }
    }

    private void OnScroll(in MouseEvent mouseEvent)
    {
        if (this.State.Style.Overflow is OverflowKind.Scroll or OverflowKind.ScrollX && mouseEvent.KeyStates.HasFlag(Platforms.Display.MouseKeyStates.Shift))
        {
            this.ScrollOffset = this.ScrollOffset with
            {
                X = this.ScrollOffset.X + (int)(5 * mouseEvent.Delta)
            };
        }
        else if (this.State.Style.Overflow is OverflowKind.Scroll or OverflowKind.ScrollY)
        {
            this.ScrollOffset = this.ScrollOffset with
            {
                Y = this.ScrollOffset.Y + (int)(5 * mouseEvent.Delta)
            };
        }
    }

    private void StyleChanged(StyleProperty property)
    {
        var hidden = this.State.Style.Hidden == true;

        this.FontSize = this.State.Style.FontSize ?? 16;

        if (property is StyleProperty.All or StyleProperty.Border)
        {
            this.border = new()
            {
                Top    = this.State.Style.Border?.Top.Thickness ?? 0,
                Right  = this.State.Style.Border?.Right.Thickness ?? 0,
                Bottom = this.State.Style.Border?.Bottom.Thickness ?? 0,
                Left   = this.State.Style.Border?.Left.Thickness ?? 0,
            };
        }

        if (property is StyleProperty.All or StyleProperty.Cursor && this.target.IsHovered && !this.IsHoveringText && this.target.Tree is RenderTree renderTree)
        {
            renderTree.Window.Cursor = this.State.Style.Cursor ?? default;
        }

        var oldParentDependent = this.parentDependent;
        var relativePropertiesHasChanged = false;

        if (property is StyleProperty.All or StyleProperty.Size or StyleProperty.MinSize or StyleProperty.MaxSize)
        {
            this.contentDependent = Dependency.None;
            this.parentDependent  = Dependency.None;

            if (this.State.Style.Size?.Width == null && this.State.Style.MinSize?.Width == null && this.State.Style.MaxSize?.Width == null)
            {
                this.contentDependent |= Dependency.Width;

                relativePropertiesHasChanged = true;
            }
            else if (this.State.Style.Size?.Width?.Kind == UnitKind.Percentage || this.State.Style.MinSize?.Width?.Kind == UnitKind.Percentage || this.State.Style.MaxSize?.Width?.Kind == UnitKind.Percentage)
            {
                this.parentDependent |= Dependency.Width;

                relativePropertiesHasChanged = true;
            }

            if (this.State.Style.Size?.Height == null && this.State.Style.MinSize?.Height == null && this.State.Style.MaxSize?.Height == null)
            {
                this.contentDependent |= Dependency.Height;
            }
            else if (this.State.Style.Size?.Height?.Kind == UnitKind.Percentage || this.State.Style.MinSize?.Height?.Kind == UnitKind.Percentage || this.State.Style.MaxSize?.Height?.Kind == UnitKind.Percentage)
            {
                this.parentDependent |= Dependency.Height;

                relativePropertiesHasChanged = true;
            }
        }

        if (property is StyleProperty.All or StyleProperty.Margin)
        {
            if (this.State.Style.Margin?.Top?.Kind == UnitKind.Percentage || this.State.Style.Margin?.Right?.Kind == UnitKind.Percentage || this.State.Style.Margin?.Bottom?.Kind == UnitKind.Percentage || this.State.Style.Margin?.Left?.Kind == UnitKind.Percentage)
            {
                this.parentDependent |= Dependency.Margin;

                relativePropertiesHasChanged = true;
            }
        }

        if (property is StyleProperty.All or StyleProperty.Padding)
        {
            if (this.State.Style.Padding?.Top?.Kind == UnitKind.Percentage || this.State.Style.Padding?.Right?.Kind == UnitKind.Percentage || this.State.Style.Padding?.Bottom?.Kind == UnitKind.Percentage || this.State.Style.Padding?.Left?.Kind == UnitKind.Percentage)
            {
                this.parentDependent |= Dependency.Padding;

                relativePropertiesHasChanged = true;
            }
        }

        var justHidden      = hidden && !this.Hidden;
        var justUnhidden    = !hidden && this.Hidden;
        var justUndependent = oldParentDependent != Dependency.None && this.parentDependent == Dependency.None;
        var justDependent   = oldParentDependent == Dependency.None && this.parentDependent != Dependency.None;

        if (this.Parent != null)
        {
            this.Parent.dependenciesHasChanged = relativePropertiesHasChanged;

            if (justUnhidden || justDependent)
            {
                if (justUnhidden)
                {
                    this.Parent.renderableNodesCount++;
                }

                if (this.parentDependent != Dependency.None)
                {
                    this.Parent.dependents.Add(this.Target);
                    this.Parent.dependents.Sort();
                }
            }
            else if (justHidden || justUndependent)
            {
                if (justHidden)
                {
                    this.Parent.renderableNodesCount--;
                }

                this.Parent.dependents.Remove(this.Target);
            }
        }

        if (property is StyleProperty.All or StyleProperty.Overflow)
        {
            var currentIsScrollable = this.State.Style.Overflow is not OverflowKind.None and not OverflowKind.Clipping && this.contentDependent != (Dependency.Width | Dependency.Height);

            if (currentIsScrollable != this.IsScrollable)
            {
                if (currentIsScrollable)
                {
                    this.target.Scrolled += this.OnScroll;
                }
                else
                {
                    this.target.Scrolled -= this.OnScroll;
                    this.ScrollOffset = default;
                }

                this.IsScrollable = currentIsScrollable;
            }

            if (this.State.Style.Overflow is not (null or OverflowKind.None) && this.contentDependent != (Dependency.Width | Dependency.Height))
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

            SetStencilLayer(this, this.ContentStencilLayer);
        }

        if (hidden)
        {
            this.RequestUpdate();

            this.Hidden = hidden;
        }
        else
        {
            this.Hidden = hidden;

            this.RequestUpdate();
        }

        this.Target.Visible = !hidden;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool ResolveHeight(ref uint height)
    {
        var resolved = !this.parentDependent.HasFlag(Dependency.Height);

        if (!this.contentDependent.HasFlag(Dependency.Height))
        {
            if (this.State.Style.Size?.Height?.TryGetPixel(out var pixel) == true)
            {
                height = pixel;

                resolved = true;
            }
            else if (this.State.Style.Size?.Height?.TryGetEm(out var em) == true)
            {
                height = (uint)(em.Value * this.FontSize);

                resolved = true;
            }
            else
            {
                var min = height;
                var max = height;

                if (this.State.Style.MinSize?.Height?.TryGetPixel(out var minPixel) == true)
                {
                    min = minPixel;
                }
                else if (this.State.Style.MinSize?.Height?.TryGetEm(out var minEm) == true)
                {
                    min = (uint)(minEm.Value * this.FontSize);
                }

                if (this.State.Style.MaxSize?.Height?.TryGetPixel(out var maxPixel) == true)
                {
                    max = maxPixel;
                }
                else if (this.State.Style.MaxSize?.Height?.TryGetEm(out var maxEm) == true)
                {
                    max = (uint)(maxEm.Value * this.FontSize);
                }

                if (height < min)
                {
                    height = min;

                    resolved = true;
                }
                else if (height > max)
                {
                    height = max;

                    resolved = true;
                }
            }

            if (this.State.Style.BoxSizing == BoxSizing.Border)
            {
                height = height.ClampSubtract(this.border.Vertical);
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
        if (this.State.Style.Margin?.Top?.TryGetPixel(out var topPixel) == true)
        {
            this.margin.Top = topPixel;
        }
        else if (this.State.Style.Margin?.Top?.TryGetEm(out var topEm) == true)
        {
            this.margin.Top = (uint)(topEm * this.FontSize);
        }

        if (this.State.Style.Margin?.Right?.TryGetPixel(out var rightPixel) == true)
        {
            this.margin.Right = rightPixel;
        }
        else if (this.State.Style.Margin?.Right?.TryGetEm(out var rightEm) == true)
        {
            this.margin.Top = (uint)(rightEm * this.FontSize);
        }

        if (this.State.Style.Margin?.Bottom?.TryGetPixel(out var bottomPixel) == true)
        {
            this.margin.Bottom = bottomPixel;
        }
        else if (this.State.Style.Margin?.Bottom?.TryGetEm(out var bottomEm) == true)
        {
            this.margin.Top = (uint)(bottomEm * this.FontSize);
        }

        if (this.State.Style.Margin?.Left?.TryGetPixel(out var leftPixel) == true)
        {
            this.margin.Left = leftPixel;
        }
        else if (this.State.Style.Margin?.Left?.TryGetEm(out var leftEm) == true)
        {
            this.margin.Top = (uint)(leftEm * this.FontSize);
        }

        return !this.parentDependent.HasFlag(Dependency.Margin);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool ResolvePadding()
    {
        if (this.State.Style.Padding?.Top?.TryGetPixel(out var topPixel) == true)
        {
            this.padding.Top = topPixel;
        }
        else if (this.State.Style.Padding?.Top?.TryGetEm(out var topEm) == true)
        {
            this.padding.Top = (uint)(topEm * this.FontSize);
        }

        if (this.State.Style.Padding?.Right?.TryGetPixel(out var rightPixel) == true)
        {
            this.padding.Right = rightPixel;
        }
        else if (this.State.Style.Padding?.Right?.TryGetEm(out var rightEm) == true)
        {
            this.padding.Top = (uint)(rightEm * this.FontSize);
        }

        if (this.State.Style.Padding?.Bottom?.TryGetPixel(out var bottomPixel) == true)
        {
            this.padding.Bottom = bottomPixel;
        }
        else if (this.State.Style.Padding?.Bottom?.TryGetEm(out var bottomEm) == true)
        {
            this.padding.Top = (uint)(bottomEm * this.FontSize);
        }

        if (this.State.Style.Padding?.Left?.TryGetPixel(out var leftPixel) == true)
        {
            this.padding.Left = leftPixel;
        }
        else if (this.State.Style.Padding?.Left?.TryGetEm(out var leftEm) == true)
        {
            this.padding.Top = (uint)(leftEm * this.FontSize);
        }

        return !this.parentDependent.HasFlag(Dependency.Padding);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool ResolveWidth(ref uint width)
    {
        var resolved = !this.parentDependent.HasFlag(Dependency.Width);

        if (!this.contentDependent.HasFlag(Dependency.Width))
        {
            if (this.State.Style.Size?.Width?.TryGetPixel(out var pixel) == true)
            {
                width = pixel;

                resolved = true;
            }
            else if (this.State.Style.Size?.Width?.TryGetEm(out var em) == true)
            {
                width = (uint)(em.Value * this.FontSize);

                resolved = true;
            }
            else
            {
                var min = width;
                var max = width;

                if (this.State.Style.MinSize?.Width?.TryGetPixel(out var minPixel) == true)
                {
                    min = minPixel;
                }
                else if (this.State.Style.MinSize?.Width?.TryGetEm(out var minEm) == true)
                {
                    min = (uint)(minEm.Value * this.FontSize);
                }

                if (this.State.Style.MaxSize?.Width?.TryGetPixel(out var maxPixel) == true)
                {
                    max = maxPixel;
                }
                else if (this.State.Style.MaxSize?.Width?.TryGetEm(out var maxEm) == true)
                {
                    max = (uint)(maxEm.Value * this.FontSize);
                }

                if (width < min)
                {
                    width = min;

                    resolved = true;
                }
                else if (width > max)
                {
                    width = max;

                    resolved = true;
                }
            }

            if (this.State.Style.BoxSizing == BoxSizing.Border)
            {
                width = width.ClampSubtract(this.border.Horizontal);
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
        var size                 = this.Size;
        var stack                = this.State.Style.Stack ?? StackKind.Horizontal;
        var contentJustification = this.State.Style.ContentJustification ?? ContentJustificationKind.None;

        var avaliableSpace = stack == StackKind.Horizontal
            ? new Size<float>(size.Width.ClampSubtract(this.content.Width), size.Height)
            : new Size<float>(size.Width, size.Height.ClampSubtract(this.content.Height));

        cursor.X += this.padding.Left + this.border.Left;
        cursor.Y -= this.padding.Top  + this.border.Top;

        var index = 0;

        foreach (var node in this.Target)
        {
            if (node is not ContainerNode child || child.Layout.Hidden)
            {
                continue;
            }

            var alignmentType  = AlignmentKind.None;
            var childBoundings = child.Layout.Size;
            var contentOffsetY = 0u;

            RawRectEdges margin  = default;

            if (child is Element element)
            {
                margin         = element.Layout.margin;
                contentOffsetY = element.Layout.padding.Top + element.Layout.border.Top + element.Layout.margin.Top;
                childBoundings = element.Layout.BoundingsWithMargin;
                alignmentType  = element.Layout.State.Style.Alignment ?? AlignmentKind.None;
            }

            var alignment = this.GetAlignment(stack, alignmentType, out var alignmentAxis);

            var position  = new Vector2<float>();
            var usedSpace = new Size<float>();

            if (stack == StackKind.Horizontal)
            {
                if (contentJustification == ContentJustificationKind.None)
                {
                    avaliableSpace.Width += childBoundings.Width;
                }

                if (contentJustification == ContentJustificationKind.None && alignmentAxis.HasFlag(AlignmentAxis.Horizontal))
                {
                    position.X = avaliableSpace.Width.ClampSubtract(childBoundings.Width) * alignment.X;
                }
                else if (contentJustification == ContentJustificationKind.End && index == 0)
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

                if (alignmentAxis.HasFlag(AlignmentAxis.Vertical))
                {
                    position.Y = size.Height.ClampSubtract(childBoundings.Height) * alignment.Y;
                }
                else if (alignmentAxis.HasFlag(AlignmentAxis.Baseline) && child.Layout.BaseLine > -1)
                {
                    position.Y = this.BaseLine - (contentOffsetY + child.Layout.BaseLine);
                }

                usedSpace.Width = alignmentAxis.HasFlag(AlignmentAxis.Horizontal)
                    ? float.Max(childBoundings.Width, avaliableSpace.Width - position.X)
                    : childBoundings.Width;

                if (contentJustification == ContentJustificationKind.None)
                {
                    avaliableSpace.Width = avaliableSpace.Width.ClampSubtract((uint)usedSpace.Width);
                }
            }
            else
            {
                if (contentJustification == ContentJustificationKind.None)
                {
                    avaliableSpace.Height += childBoundings.Height;
                }

                position.X = size.Width.ClampSubtract(childBoundings.Width) * alignment.X;

                if (contentJustification == ContentJustificationKind.None && alignmentAxis.HasFlag(AlignmentAxis.Vertical))
                {
                    position.Y = (uint)(avaliableSpace.Height.ClampSubtract(childBoundings.Height) * alignment.Y);
                }
                else if (contentJustification == ContentJustificationKind.End && index == 0)
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

                usedSpace.Height = alignmentAxis.HasFlag(AlignmentAxis.Vertical)
                    ? float.Max(childBoundings.Height, avaliableSpace.Height - position.Y)
                    : childBoundings.Height;

                if (contentJustification == ContentJustificationKind.None)
                {
                    avaliableSpace.Height = avaliableSpace.Height.ClampSubtract((uint)usedSpace.Height);
                }
            }

            child.Layout.Offset = new(float.Round(this.ScrollOffset.X + cursor.X + position.X + margin.Left), -float.Round(-this.ScrollOffset.Y + -cursor.Y + position.Y + margin.Top));

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

    private void UpdateRect()
    {
        var command = this.GetRectCommand();

        var isDrawable = this.State.Style.Border.HasValue || this.State.Style.BackgroundColor.HasValue;

        if (isDrawable)
        {
            command.Rect             = new(this.Boundings.Cast<float>(), default);
            command.Border           = this.State.Style.Border ?? default;
            command.Color            = this.State.Style.BackgroundColor ?? default;
            command.PipelineVariant |= PipelineVariant.Color;
        }
        else
        {
            command.PipelineVariant &= ~PipelineVariant.Color;
        }

        command.StencilLayer = this.StencilLayer;

        this.ownStencilLayer?.MakeDirty();
    }

    protected override void Disposed() =>
        this.ownStencilLayer?.Dispose();

    public void ContainerNodeRemoved(ContainerNode containerNode)
    {
        if (!containerNode.Layout.Hidden)
        {
            this.childsChanged = true;
            this.renderableNodesCount--;
            this.RequestUpdate();
        }
    }

    public void ContainerNodeAppended(ContainerNode containerNode)
    {
        if (!containerNode.Layout.Hidden)
        {
            this.childsChanged = true;
            this.renderableNodesCount++;
            this.RequestUpdate();
        }
    }

    public void ElementAppended(Element element)
    {
        if (!element.Layout.Hidden && element.Layout.parentDependent != Dependency.None)
        {
            this.dependents.Add(element);
        }
    }

    public void ElementRemoved(Element element)
    {
        if (!element.Layout.Hidden && element.Layout.parentDependent != Dependency.None)
        {
            this.dependents.Remove(element);
        }
    }

    public void TargetMouseOut()
    {
        if (this.target.Tree is RenderTree renderTree)
        {
            renderTree.Window.Cursor = default;
        }
    }

    public void TargetMouseOver()
    {
        if (this.State.Style.Cursor.HasValue && this.target.Tree is RenderTree renderTree)
        {
            renderTree.Window.Cursor = this.State.Style.Cursor.Value;
        }
    }

    public void TargetIndexed()
    {
        var command = this.GetRectCommand();

        command.ObjectId = this.Target.Index == -1
            ? default
            : this.State.Style.Border.HasValue || this.State.Style.BackgroundColor.HasValue ? (uint)(this.Target.Index + 1) : 0;
    }

    public override void TargetConnected()
    {
        base.TargetConnected();

        if (this.ownStencilLayer != null)
        {
            this.StencilLayer?.AppendChild(this.ownStencilLayer);
        }
    }

    public override void Update()
    {
        if (this.HasPendingUpdate)
        {
            if (!this.Hidden)
            {
                this.CalculateLayout();

                if (this.parentDependent == Dependency.None)
                {
                    this.UpdateDisposition();
                    this.childsChanged          = false;
                    this.dependenciesHasChanged = false;
                }

            }

            this.HasPendingUpdate = false;
        }
    }
}
