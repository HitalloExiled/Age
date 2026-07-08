namespace Age.Platforms.Display;

public record struct WindowKeyEvent
{
    public required Key Key;
    public required Key PhysicalKey;

    public required char Char;

    public required bool        IsPressed;
    public required KeyLocation Location;
    public required Modifier    Modifiers;

    public required bool Echo;
}
