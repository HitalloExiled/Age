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
        Cursor               = Platforms.Display.CursorKind.Hand,
    };

    public required StyledStates Flat     { get; init; }
    public required StyledStates Outlined { get; init; }
    public required StyledStates Text     { get; init; }
}
