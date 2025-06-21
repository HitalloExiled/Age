using Age.Numerics;
using Age.Platforms.Display;

namespace Age.Themes;

public class Theme
{
    public static Theme Dark { get; } = new()
    {
        Button = new()
        {
            Flat = new()
            {
                Base = ButtonStyles.Base with
                {
                    BackgroundColor = Color.Green,
                    Color           = Color.Black,
                },
                Focus = new()
                {
                    BackgroundColor = Color.Green + (Color.White * 0.5f),
                },
                Hovered = new()
                {
                    BackgroundColor = Color.Green + (Color.White * 0.6f),
                },
                Active = new()
                {
                    BackgroundColor = Color.Green + (Color.White * 0.9f),
                },
            },
            Outlined = new()
            {
                Base = ButtonStyles.Base with
                {
                    Border = new(1, 4, Color.Green),
                    Color  = Color.Green,
                },
                Focus = new()
                {
                    BackgroundColor = Color.Green.WithAlpha(0.15f),
                },
                Hovered = new()
                {
                    Color           = Color.Green + (Color.White * 0.6f),
                    BackgroundColor = Color.Green.WithAlpha(0.2f),
                },
                Active = new()
                {
                    Color           = Color.Green + (Color.White * 0.9f),
                    BackgroundColor = Color.Green.WithAlpha(0.5f),
                },
            },
            Text = new()
            {
                Base = ButtonStyles.Base with
                {
                    Color           = Color.Green,
                    BackgroundColor = Color.Green.WithAlpha(0),
                },
                Focus = new()
                {
                    BackgroundColor = Color.Green.WithAlpha(0.15f),
                },
                Hovered = new()
                {
                    Color           = Color.Green + (Color.White * 0.6f),
                    BackgroundColor = Color.Green.WithAlpha(0.2f),
                },
                Active = new()
                {
                    Color           = Color.Green + (Color.White * 0.9f),
                    BackgroundColor = Color.Green.WithAlpha(0.5f),
                },
            }
        },
        TextBox = new()
        {
            Outlined = new()
            {
                Base = new()
                {
                    Border          = new(1, 4, Color.Green),
                    Color           = Color.Green,
                    BackgroundColor = Color.Black,
                    Padding         = new(Unit.Px(4)),
                    MinSize         = new(Unit.Px(100), Unit.Em(1.5f))
                },
                Hovered = new()
                {
                    Cursor = Cursor.Text,
                }
            }
        }
    };

    // public static Theme Light { get; } = new()
    // {
    //     Button = new(),
    // };

    public static Theme Current => Dark;

    public required ButtonStyles  Button  { get; init; }
    public required TextBoxStyles TextBox { get; init; }
}
