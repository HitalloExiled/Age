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

internal sealed partial class BoxLayout(Element target) : StyledLayout
{
    private bool          childsChanged;
    private LayoutCommand layoutCommands;
    private StencilLayer? ownStencilLayer;
    private Size<uint>    staticContent;

    private Size<uint> BoundingsWithMargin =>
        new(
            this.Size.Width  + this.Padding.Horizontal + this.Border.Horizontal + this.Margin.Horizontal,
            this.Size.Height + this.Padding.Vertical   + this.Border.Vertical   + this.Margin.Vertical
        );

    private Size<uint> SizeWithPadding =>
        new(
            this.Size.Width  + this.Padding.Horizontal,
            this.Size.Height + this.Padding.Vertical
        );

    protected override StencilLayer? ContentStencilLayer => this.ownStencilLayer ?? this.StencilLayer;

    public Point<uint> ContentOffset
    {
        get;
        set
        {
            value.X = Math<uint>.MinMax(0, this.Content.Width.ClampSubtract(this.Size.Width),   value.X);
            value.Y = Math<uint>.MinMax(0, this.Content.Height.ClampSubtract(this.Size.Height), value.Y);

            if (field != value)
            {
                field = value;

                this.ownStencilLayer?.MakeChildrenDirty();
                this.RequestUpdate(false);
            }
        }
    }

    public bool CanScrollX => this.ComputedStyle.Overflow?.HasFlags(Overflow.ScrollX) == true;
    public bool CanScrollY => this.ComputedStyle.Overflow?.HasFlags(Overflow.ScrollY) == true;

    public override Element Target { get; } = target;

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

    public override bool        IsParentDependent => this.ParentDependencies != Dependency.None;
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

    private static void CalculatePendingHeight(Styleable dependent, StackDirection direction, in uint reference, ref uint height, ref uint content, ref uint avaliableSpace)
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
                .ClampSubtract(dependent.Layout.Border.Vertical)
                .ClampSubtract(dependent.Layout.Padding.Vertical)
                .ClampSubtract(dependent.Layout.Margin.Vertical);
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

    private static void CalculatePendingMarginHorizontal(StyledLayout layout, in StyleRectEdges? styleMargin, StackDirection direction, uint reference, ref RectEdges margin, ref Size<uint> contentSize)
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
                contentSize.Width = uint.Max(layout.Size.Width + layout.Padding.Horizontal + layout.Border.Horizontal + horizontal, contentSize.Width);
            }
        }
    }

    private static void CalculatePendingMarginVertical(StyledLayout layout, in StyleRectEdges? styleMargin, StackDirection direction, uint reference, ref RectEdges margin, ref Size<uint> contentSize)
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
                contentSize.Height = uint.Max(layout.Size.Height + layout.Padding.Vertical + layout.Border.Vertical + vertical, contentSize.Height);
            }
        }
    }

    private static void CalculatePendingWidth(Styleable dependent, StackDirection direction, in uint reference, ref uint width, ref uint content, ref uint avaliableSpace)
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
                .ClampSubtract(dependent.Layout.Border.Horizontal)
                .ClampSubtract(dependent.Layout.Padding.Horizontal)
                .ClampSubtract(dependent.Layout.Margin.Horizontal);
        }
    }

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

                    var offset = new Vector2<float>(x + this.Border.Left, -y + -this.Border.Top);

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

                    var offset = new Vector2<float>(x + this.Border.Left, y + this.Border.Top);

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

                        offset.X = this.Border.Left;
                    }
                    else
                    {
                        offset.X = this.Border.Left + x;
                    }

                    if (imageRepeat.HasFlags(ImageRepeat.RepeatY))
                    {
                        repeatY = boundings.Height / height;
                        offsetY = -(y / height);

                        height = boundings.Height;

                        offset.Y = this.Border.Top;
                    }
                    else
                    {
                        offset.Y = this.Border.Top + y;
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

    private void CalculateLayout(Style style)
    {
        var direction = style.StackDirection ?? StackDirection.Horizontal;

        this.Content       = new Size<uint>();
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

            if (child is Styleable element)
            {
                var boudings = element.Layout.AbsoluteBoundings;

                childSize.Width  = boudings.Width;
                childSize.Height = boudings.Height;

                dependencies = element.Layout.ParentDependencies;
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

                this.Content.Width += childSize.Width;
                this.Content.Height = uint.Max(this.Content.Height, childSize.Height);

                this.CheckHightestInlineChild(direction, child);
            }
            else
            {
                if (!dependencies.HasFlags(Dependency.Height))
                {
                    this.staticContent.Width = uint.Max(this.staticContent.Width, childSize.Width);
                    this.staticContent.Height += childSize.Height;
                }

                this.Content.Width = uint.Max(this.Content.Width, childSize.Width);
                this.Content.Height += childSize.Height;

                if (child == this.Target.FirstChild)
                {
                    this.CheckHightestInlineChild(direction, child);
                }
            }
        }

        if (this.ContentDependencies.HasAnyFlag(Dependency.Width | Dependency.Height))
        {
            this.CalculatePendingMargin(style, ref this.Content);
        }

        var size = this.Content;

        var fontSize = GetFontSize(style);

        var resolvedMargin  = this.ResolveMargin(fontSize, style.Margin);
        var resolvedPadding = this.ResolvePadding(fontSize, style.Padding);
        var resolvedSize    = this.ResolveSize(style, fontSize, ref size);

        var sizeHasChanged = this.Size != size;

        this.Size = size;

        if (resolvedSize && resolvedMargin && resolvedPadding)
        {
            if (this.Dependents.Count > 0 && (sizeHasChanged || this.childsChanged || this.DependenciesHasChanged))
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

        foreach (var dependent in this.Dependents)
        {
            if (dependent.Layout.ParentDependencies.HasAnyFlag(Dependency.Padding | Dependency.Margin))
            {
                var margin = dependent.Layout.Margin;

                var dependentStyle = dependent.Layout.ComputedStyle;

                if (!this.ParentDependencies.HasFlags(Dependency.Width))
                {
                    CalculatePendingMarginHorizontal(dependent.Layout, dependentStyle.Margin, direction, size.Width, ref margin, ref contentSize);
                }

                if (!this.ParentDependencies.HasFlags(Dependency.Height))
                {
                    CalculatePendingMarginVertical(dependent.Layout, dependentStyle.Margin, direction, size.Height, ref margin, ref contentSize);
                }

                dependent.Layout.Margin = margin;
            }
        }

        size = contentSize;
    }

    private void CalculatePendingLayouts(Style style)
    {
        var content        = this.Content;
        var avaliableSpace = this.Size.ClampSubtract(this.staticContent);
        var direction      = style.StackDirection ?? StackDirection.Horizontal;

        foreach (var dependent in this.Dependents)
        {
            var margin         = dependent.Layout.Margin;
            var padding        = dependent.Layout.Padding;
            var size           = dependent.Layout.Size;
            var dependentStyle = dependent.Layout.ComputedStyle;

            if (!this.ContentDependencies.HasFlags(Dependency.Width) || direction == StackDirection.Vertical)
            {
                if (!this.ContentDependencies.HasFlags(Dependency.Width))
                {
                    CalculatePendingPaddingHorizontal(dependentStyle.Padding, this.Size.Width, ref padding);
                    CalculatePendingMarginHorizontal(dependent.Layout, dependentStyle.Margin, direction, this.Size.Height, ref margin, ref content);
                }

                if (dependent.Layout.ParentDependencies.HasFlags(Dependency.Width))
                {
                    CalculatePendingWidth(dependent, direction, this.Size.Width, ref size.Width, ref content.Width, ref avaliableSpace.Width);
                }
            }

            if (!this.ContentDependencies.HasFlags(Dependency.Height) || direction == StackDirection.Horizontal)
            {
                if (!this.ContentDependencies.HasFlags(Dependency.Height))
                {
                    CalculatePendingPaddingVertical(dependentStyle.Padding, this.Size.Height, ref padding);
                    CalculatePendingMarginVertical(dependent.Layout, dependentStyle.Margin, direction, this.Size.Height, ref margin, ref content);
                }

                if (dependent.Layout.ParentDependencies.HasFlags(Dependency.Height))
                {
                    CalculatePendingHeight(dependent, direction, this.Size.Height, ref size.Height, ref content.Height, ref avaliableSpace.Height);
                }
            }

            if (dependent is Element element)
            {
                if (element.Layout.DependenciesHasChanged || element.Layout.childsChanged || size != element.Layout.Size || padding != element.Layout.Padding || margin != element.Layout.Margin)
                {
                    element.Layout.childsChanged          = false;
                    element.Layout.DependenciesHasChanged = false;
                    element.Layout.Size                   = size;
                    element.Layout.Padding                = padding;
                    element.Layout.Margin                 = margin;

                    if (element.Layout.Dependents.Count > 0)
                    {
                        element.Layout.CalculatePendingLayouts(dependentStyle);
                    }

                    element.Layout.UpdateDisposition(dependentStyle);
                    element.Layout.UpdateBoundings(dependentStyle);
                }
            }
            else
            {
                dependent.Layout.UpdateBoundings();
            }

            dependent.Layout.MakePristine();

            this.CheckHightestInlineChild(direction, dependent);
        }

        this.Content = content;
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
                    || direction == StackDirection.Vertical && alignment.Value.HasAnyFlag(Alignment.Start | Alignment.Center | Alignment.End)
                );

            baseline += element.Layout.Padding.Top + element.Layout.Border.Top + element.Layout.Margin.Top;
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
        if (this.RenderableNodesCount == 0)
        {
            return;
        }

        var cursor               = new Point<float>();
        var size                 = this.Size;
        var direction            = style.StackDirection ?? StackDirection.Horizontal;
        var contentJustification = style.ContentJustification ?? ContentJustification.None;

        var avaliableSpace = direction == StackDirection.Horizontal
            ? new Size<float>(size.Width.ClampSubtract(this.Content.Width), size.Height)
            : new Size<float>(size.Width, size.Height.ClampSubtract(this.Content.Height));

        cursor.X += this.Padding.Left + this.Border.Left;
        cursor.Y -= this.Padding.Top  + this.Border.Top;

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
                margin         = element.Layout.Margin;
                contentOffsetY = (uint)(element.Layout.Padding.Top + element.Layout.Border.Top + element.Layout.Margin.Top);
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
                        position.X = (index == 0 ? 1 : 2) * avaliableSpace.Width / (this.RenderableNodesCount * 2);
                    }
                    else if (contentJustification == ContentJustification.SpaceBetween && index > 0)
                    {
                        position.X = avaliableSpace.Width / (this.RenderableNodesCount - 1);
                    }
                    else if (contentJustification == ContentJustification.SpaceEvenly)
                    {
                        position.X = avaliableSpace.Width / (this.RenderableNodesCount + 1);
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
                        position.Y = (index == 0 ? 1 : 2) * avaliableSpace.Height / (this.RenderableNodesCount * 2);
                    }
                    else if (contentJustification == ContentJustification.SpaceBetween && index > 0)
                    {
                        position.Y = avaliableSpace.Height / (this.RenderableNodesCount - 1);
                    }
                    else if (contentJustification == ContentJustification.SpaceEvenly)
                    {
                        position.Y = avaliableSpace.Height / (this.RenderableNodesCount + 1);
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
        this.UpdateBoundings();
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
        var style = this.ComputedStyle;

        if (property.HasFlags(StyleProperty.BackgroundImage))
        {
            this.SetBackgroundImage(style.BackgroundImage);
        }

        if (property.HasFlags(StyleProperty.Overflow))
        {
            var currentIsScrollable = (style.Overflow?.HasFlags(Overflow.ScrollX) == true || style.Overflow?.HasFlags(Overflow.ScrollY) == true) && this.ContentDependencies != (Dependency.Width | Dependency.Height);

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

            if (style.Overflow?.HasFlags(Overflow.Clipping) == true && this.ContentDependencies != (Dependency.Width | Dependency.Height))
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

        if (!this.Hidden)
        {
            this.UpdateCommands(style);
        }
    }

    public RectCommand GetLayoutLayer(LayoutLayer layer) =>
        this.GetLayoutCommand((LayoutCommand)layer);

    public void ReleaseLayoutLayer(LayoutLayer layer) =>
        this.ReleaseLayoutCommand((LayoutCommand)layer);

    public void HandleStyleableRemoved(Styleable styleable)
    {
        if (!styleable.Layout.Hidden && styleable.Layout.ParentDependencies != Dependency.None)
        {
            this.Dependents.Remove(styleable);
        }
    }

    public void HandleLayoutableAppended(Layoutable layoutable)
    {
        if (!layoutable.Layout.Hidden)
        {
            this.childsChanged = true;
            this.IncreaseRenderableNodes();
            this.RequestUpdate(true);
        }
    }

    public void HandleLayoutableRemoved(Layoutable layoutable)
    {
        if (layoutable is Styleable styleable)
        {
            this.HandleStyleableRemoved(styleable);
        }

        if (!layoutable.Layout.Hidden)
        {
            this.childsChanged = true;
            this.DecreaseRenderableNodes();
            this.RequestUpdate(true);
        }
    }

    public override void HandleTargetConnected()
    {
        base.HandleTargetConnected();

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

    public override void Update()
    {
        var style = this.ComputedStyle;

        this.CalculateLayout(style);

        if (this.ParentDependencies == Dependency.None)
        {
            this.UpdateDisposition(style);
            this.childsChanged          = false;
            this.DependenciesHasChanged = false;
        }
    }
}
