using Age.Styling;

namespace Age.Themes;

public class ButtonStyles
{
    public static Style Base { get; } = new()
    {
        TextSelection        = false,
        Padding              = new((Pixel)4),
        FontWeight           = FontWeight.Medium,
        ContentJustification = ContentJustification.Center,
        Cursor               = Platforms.Display.Cursor.Hand,
    };

    public required StyleSheet Flat     { get; init; }
    public required StyleSheet Outlined { get; init; }
    public required StyleSheet Text     { get; init; }
}
