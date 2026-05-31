namespace Age.Platforms.Display;

public record struct WindowKeyEvent
{
    public Key      Key;
    public Modifier Modifiers;

    public char Char;
}
