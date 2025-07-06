using Age.Numerics;
using Age.Platforms.Display;
using Age.Styling;

namespace Age.Themes;

public class TextBoxStyles
{
    internal static TextBoxStyles GetDarkVariant() => new()
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
    };

    public required StyleSheet Outlined { get; init; }
}
