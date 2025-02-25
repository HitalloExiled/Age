using Age.Styling;

namespace Age.Themes;

public class TextBoxStyles
{
    public static Style Base { get; } = new()
    {
        ContentJustification = ContentJustificationKind.Center,
        FontWeight           = FontWeight.Medium,
        Overflow             = OverflowKind.Scroll,
        Padding              = new((Pixel)4),
        TextSelection        = false,
    };

    public required StyledStates Outlined { get; init; }
}
