using Age.Numerics;
using Age.Elements;
using Age.Styling;
using Age.Platforms.Display;
using Age.Extensions;

namespace Age.Editor.Tests;

public class AlignmentTest
{
    public static void Setup(Canvas canvas)
    {
        var borderSize = 10u;
        var marginSize = 10u;

        var root = new FlexBox()
        {
            Name  = "root",
            Style = new()
            {
                Border         = new(borderSize, 0, Color.Green),
                ItemsAlignment = ItemsAlignmentKind.Baseline,
            }
        };

        var horizontal_a_container = new FlexBox()
        {
            Name  = "horizontal_a_container",
            Text  = "Horizontal A",
            Style = new()
            {
                Border         = new(borderSize, 0, Color.Margenta),
                Color          = Color.White,
                FontSize       = 32,
                ItemsAlignment = ItemsAlignmentKind.Baseline,
            }
        };

        var horizontal_a_left = new FlexBox()
        {
            Name  = "horizontal_a_left",
            Text  = "Left",
            Style = new()
            {
                Alignment = AlignmentKind.Left,
                Border    = new(borderSize, 0, Color.Cyan),
                Color     = Color.White,
            }
        };

        var horizontal_a_right = new FlexBox()
        {
            Name  = "horizontal_a_right",
            Text  = "Right",
            Style = new()
            {
                Alignment = AlignmentKind.Right,
                Border    = new(borderSize, 0, Color.Cyan),
                Color     = Color.White,
                //Margin    = new((Pixel)marginSize),
            }
        };

        var verticalPixelSize      = 30u;
        var verticalPercentageSize = 100u;
        var percentage             = false;

        horizontal_a_right.KeyDown += (in KeyEvent keyEvent) =>
        {
            if (keyEvent.Holding)
            {
                return;
            }

            if (keyEvent.Modifiers.HasFlag(KeyStates.Control))
            {
                switch (keyEvent.Key)
                {
                    case Key.Add:
                        borderSize++;

                        break;

                    case Key.Subtract:
                        borderSize = borderSize.ClampSubtract(1);

                        break;
                    default:
                        break;
                }

                horizontal_a_right.Style.Border = new(borderSize, 0, Color.Cyan);
            }
            else
            {
                var updateSize = false;

                switch (keyEvent.Key)
                {
                    case Key.Space:
                        horizontal_a_right.Style.Hidden = !(horizontal_a_right.Style.Hidden ?? false);
                        break;

                    case Key.Up:
                        if (percentage)
                        {
                            verticalPercentageSize = uint.Min(verticalPercentageSize + 1, 100);
                        }
                        else
                        {
                            verticalPixelSize++;
                        }
                        updateSize = true;
                        break;

                    case Key.Down:
                        if (percentage)
                        {
                            verticalPercentageSize = verticalPercentageSize.ClampSubtract(1);
                        }
                        else
                        {
                            verticalPixelSize = verticalPixelSize.ClampSubtract(1);
                        }
                        updateSize = true;
                        break;

                    case Key.X:
                        percentage = !percentage;

                        updateSize = true;
                        break;
                    default:
                        break;
                }

                if (updateSize)
                {
                    horizontal_a_right.Style.Size = !percentage
                        ? new(null, (Pixel)verticalPixelSize)
                        : new(null, (Percentage)verticalPercentageSize);
                }                
            }            
        };

        horizontal_a_right.Focus();

        var horizontal_b_container = new FlexBox()
        {
            Name  = "horizontal_b_container",
            Text  = "Horizontal B",
            Style = new()
            {
                Border   = new(borderSize, 0, Color.Margenta),
                FontSize = 32,
                Color    = Color.White,
            }
        };

        var horizontal_b_top = new FlexBox()
        {
            Name  = "horizontal_b_top",
            Text  = "Top",
            Style = new()
            {
                Alignment = AlignmentKind.Top,
                Border    = new(borderSize, 0, Color.Cyan),
                Color     = Color.White,
            }
        };

        var horizontal_b_bottom = new FlexBox()
        {
            Name  = "horizontal_b_bottom",
            Text  = "Down",
            Style = new()
            {
                Alignment = AlignmentKind.Bottom,
                Border    = new(borderSize, 0, Color.Cyan),
                Color     = Color.White,
                Margin    = new((Pixel)marginSize),
            }
        };

        var vertical_a_container = new FlexBox()
        {
            Name  = "vertical_a_container",
            Text  = "Vertical A",
            Style = new()
            {
                Border         = new(borderSize, 0, Color.Margenta),
                Color          = Color.White,
                FontSize       = 32,
                ItemsAlignment = ItemsAlignmentKind.Baseline,
                Stack          = StackKind.Vertical,
            }
        };

        var vertical_a_left = new FlexBox()
        {
            Name  = "vertical_a_left",
            Text  = "Left",
            Style = new()
            {
                Alignment = AlignmentKind.Left,
                Border    = new(borderSize, 0, Color.Cyan),
                Color     = Color.White,
            }
        };

        var vertical_a_right = new FlexBox()
        {
            Name  = "vertical_a_right",
            Text  = "Right",
            Style = new()
            {
                Alignment = AlignmentKind.Right,
                Border    = new(borderSize, 0, Color.Cyan),
                Color     = Color.White,
            }
        };

        var vertical_b_container = new FlexBox()
        {
            Name  = "vertical_b_container",
            Text  = "Vertical B",
            Style = new()
            {
                Border   = new(borderSize, 0, Color.Margenta),
                FontSize = 32,
                Color    = Color.White,
                Stack    = StackKind.Vertical,
            }
        };

        var vertical_b_top = new FlexBox()
        {
            Name  = "vertical_b_top",
            Text  = "Top",
            Style = new()
            {
                Alignment = AlignmentKind.Top,
                Border    = new(borderSize, 0, Color.Cyan),
                Color     = Color.White,
            }
        };

        var vertical_b_bottom = new FlexBox()
        {
            Name  = "vertical_b_bottom",
            Text  = "Down",
            Style = new()
            {
                Alignment = AlignmentKind.Bottom,
                Border    = new(borderSize, 0, Color.Cyan),
                Color     = Color.White,
            }
        };

        canvas.AppendChild(root);

            root.AppendChild(horizontal_a_container);
                horizontal_a_container.AppendChild(horizontal_a_left);
                horizontal_a_container.AppendChild(horizontal_a_right);

            root.AppendChild(horizontal_b_container);
                horizontal_b_container.AppendChild(horizontal_b_top);
                horizontal_b_container.AppendChild(horizontal_b_bottom);

            root.AppendChild(vertical_a_container);
                vertical_a_container.AppendChild(vertical_a_left);
                vertical_a_container.AppendChild(vertical_a_right);

            root.AppendChild(vertical_b_container);
                vertical_b_container.AppendChild(vertical_b_top);
                vertical_b_container.AppendChild(vertical_b_bottom);
    }
}
