using Age.Numerics;

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
                    BackgroundColor = Color.Green + Color.White * 0.5f,
                },
                Hovered = new()
                {
                    BackgroundColor = Color.Green + Color.White * 0.6f,
                },
                Active = new()
                {
                    BackgroundColor = Color.Green + Color.White * 0.7f,
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
                    BackgroundColor = Color.Green.WithAlpha(0.2f),
                },
                Active = new()
                {
                    BackgroundColor = Color.Green.WithAlpha(0.25f),
                },
            },
            Text = new()
            {
                Base = ButtonStyles.Base with
                {
                    Color = Color.Green,
                },
                Focus = new()
                {
                    BackgroundColor = Color.Green.WithAlpha(0.15f),
                },
                Hovered = new()
                {
                    BackgroundColor = Color.Green.WithAlpha(0.2f),
                },
                Active = new()
                {
                    BackgroundColor = Color.Green.WithAlpha(0.25f),
                },
            }
        },
    };

    // public static Theme Light { get; } = new()
    // {
    //     Button = new(),
    // };

    public static Theme Current => Dark;

    public required ButtonStyles Button { get; init; }
}
