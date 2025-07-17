using Age.Core.Extensions;
using Age.Extensions;
using Age.Numerics;
using Age.Styling;

namespace Age.Elements;

public sealed partial class FlexBox : Element
{
    public override string NodeName => nameof(FlexBox);

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

    private protected override void UpdateDisposition()
    {
        var cursor               = new Point<float>();
        var size                 = this.Size;
        var direction            = this.ComputedStyle.StackDirection ?? StackDirection.Horizontal;
        var contentJustification = this.ComputedStyle.ContentJustification ?? ContentJustification.None;

        var avaliableSpace = direction == StackDirection.Horizontal
            ? new Size<uint>(size.Width.ClampSubtract(this.Content.Width), size.Height)
            : new Size<uint>(size.Width, size.Height.ClampSubtract(this.Content.Height));

        cursor.X += this.Padding.Left + this.Border.Left;
        cursor.Y -= this.Padding.Top  + this.Border.Top;

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
                margin         = element.Margin;
                contentOffsetY = (uint)(element.Padding.Top + element.Border.Top + element.Margin.Top);
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
                        position.X = (index == 0 ? 1 : 2) * avaliableSpace.Width / (this.RenderableNodes * 2);
                    }
                    else if (contentJustification == ContentJustification.SpaceBetween && index > 0)
                    {
                        position.X = avaliableSpace.Width / (this.RenderableNodes - 1);
                    }
                    else if (contentJustification == ContentJustification.SpaceEvenly)
                    {
                        position.X = avaliableSpace.Width / (this.RenderableNodes + 1);
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
                        position.Y = (index == 0 ? 1 : 2) * avaliableSpace.Height / (this.RenderableNodes * 2);
                    }
                    else if (contentJustification == ContentJustification.SpaceBetween && index > 0)
                    {
                        position.Y = avaliableSpace.Height / (this.RenderableNodes - 1);
                    }
                    else if (contentJustification == ContentJustification.SpaceEvenly)
                    {
                        position.Y = avaliableSpace.Height / (this.RenderableNodes + 1);
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
}
