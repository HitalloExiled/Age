using Age.Styling;

namespace Age.Themes;

public class TextBoxStyles
{
    public static Style Base { get; } = new()
    {
        ContentJustification = ContentJustification.Center,
        FontWeight           = FontWeight.Medium,
        Overflow             = Overflow.Scroll,
        Padding              = new((Pixel)4),
        TextSelection        = false,
    };

    public required StyleSheet Outlined { get; init; }
}
