using Age.Numerics;
using Age.Styling;

namespace Age.Themes;

public class Theme
{
    public static Theme Dark { get; } = new()
    {
        Button = new()
        {
            Base = new()
            {
                Border        = new(1, 4, Color.White),
                Color         = new(1, 4, Color.White),
                TextSelection = false,
                Padding       = new((Pixel)4),
                FontWeight    = FontWeight.Bold,
            },
            Active = new()
            {
                BackgroundColor = Color.White * 0.75f,
            },
            Focus = new()
            {
                BackgroundColor = Color.Red * 0.5f,
            },
            Hovered = new()
            {
                BackgroundColor = Color.White * 0.4f,
            },
        },
    };

    public static Theme Light { get; } = new()
    {
        Button = new(),
    };

    public static Theme Current => Dark;

    public required StyledStates Button { get; init; }
}
