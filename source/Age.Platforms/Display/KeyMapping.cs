namespace Age.Platforms.Display;

public static partial class KeyMapping
{
    public static partial Key GetKeycode(uint code);

    public static partial Key GetScancode(uint code);

    public static partial KeyLocation GetLocation(uint code);
}
