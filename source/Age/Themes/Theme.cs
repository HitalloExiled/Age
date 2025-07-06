namespace Age.Themes;

public class Theme
{
    public static Theme Dark { get; } = new()
    {
        Button  = ButtonStyles.GetDarkVariant(),
        Icon    = IconStyles.GetDarkVariant(),
        TextBox = TextBoxStyles.GetDarkVariant(),
    };

    // public static Theme Light { get; } = new()
    // {
    //     Button = new(),
    // };

    public static Theme Current => Dark;

    public required ButtonStyles  Button  { get; init; }
    public required IconStyles    Icon    { get; init; }
    public required TextBoxStyles TextBox { get; init; }
}
