using Age.Numerics;
using Age.Elements;
using Age.Styling;
using Age.Platforms.Display;
using Age.Extensions;
using Age.Core.Extensions;

namespace Age.Playground.Tests;

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
                ItemsAlignment = ItemsAlignment.Baseline,
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
                ItemsAlignment = ItemsAlignment.Baseline,
            },
            States = new()
            {
                Active = new()
                {
                    Border = new(borderSize, 0, Color.White),
                },
                Focus = new()
                {
                    Border = new(borderSize, 0, Color.Cyan),
                },
                Hovered = new()
                {
                    Border = new(borderSize, 0, Color.Red),
                    // Stack  = StackKind.Vertical,
                }
            }
        };

        horizontal_a_container.KeyDown += (in KeyEvent keyEvent) =>
        {
            if (keyEvent.Holding)
            {
                return;
            }

            switch (keyEvent.Key)
            {
                case Key.Space:
                    horizontal_a_container.Style.Hidden = !(horizontal_a_container.Style.Hidden ?? false);
                    break;
            }
        };

        horizontal_a_container.Focus();

        var horizontal_a_left = new FlexBox()
        {
            Name  = "horizontal_a_left",
            Text  = "Left",
            Style = new()
            {
                Alignment = Alignment.Left,
                Border    = new(borderSize, 0, Color.Cyan),
                Color     = Color.White,
                Size      = new(null, (Percentage)100),
            }
        };

        var horizontal_a_right = new FlexBox()
        {
            Name  = "horizontal_a_right",
            Text  = "Right",
            Style = new()
            {
                Alignment = Alignment.Right,
                Border    = new(borderSize, 0, Color.Cyan),
                Color     = Color.White,
                //Margin    = new((Pixel)marginSize),
            }
        };

        var verticalPixelSize      = 30u;
        var verticalPercentageSize = 100u;
        var percentage             = false;
        var text                   = "Changed";

        horizontal_a_right.KeyDown += (in KeyEvent keyEvent) =>
        {
            if (keyEvent.Holding)
            {
                return;
            }

            if (keyEvent.Modifiers.HasFlags(KeyStates.Control))
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

                    case Key.C:
                        horizontal_a_right.Text = text += "*";
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
                Alignment = Alignment.Top,
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
                Alignment = Alignment.Bottom,
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
                ItemsAlignment = ItemsAlignment.Baseline,
                StackDirection = StackDirection.Vertical,
            }
        };

        var vertical_a_left = new FlexBox()
        {
            Name  = "vertical_a_left",
            Text  = "Left",
            Style = new()
            {
                Alignment = Alignment.Left,
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
                Alignment = Alignment.Right,
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
                Border         = new(borderSize, 0, Color.Margenta),
                FontSize       = 32,
                Color          = Color.White,
                StackDirection = StackDirection.Vertical,
            }
        };

        var vertical_b_top = new FlexBox()
        {
            Name  = "vertical_b_top",
            Text  = "Top",
            Style = new()
            {
                Alignment = Alignment.Top,
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
                Alignment = Alignment.Bottom,
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
