using Age.Numerics;
using Age.Styling;

namespace Age.Themes;

public class ButtonStyles
{
    private static readonly Style @base = new()
    {
        TextSelection        = false,
        Padding              = new(Unit.Px(4)),
        FontWeight           = FontWeight.Medium,
        ContentJustification = ContentJustification.Center,
        Cursor               = Platforms.Display.Cursor.Hand,
    };

    public static ButtonStyles GetDarkVariant() =>
        new()
        {
            Flat = new()
            {
                Base = @base with
                {
                    BackgroundColor = Color.Green,
                    Color           = Color.Black,
                },
                Focused = new()
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
                Base = @base with
                {
                    Border = new(1, 4, Color.Green),
                    Color  = Color.Green,
                },
                Focused = new()
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
                Base = @base with
                {
                    Color           = Color.Green,
                    BackgroundColor = Color.Green.WithAlpha(0),
                },
                Focused = new()
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
        };

    public required StyleSheet Flat     { get; init; }
    public required StyleSheet Outlined { get; init; }
    public required StyleSheet Text     { get; init; }
}
