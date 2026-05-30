#if LINUX
namespace Age.Platforms.Display;

public enum KeyLocation
{
	Unspecified,
	Left,
	Right
};

public static class KeyMapping
{
    private static readonly Dictionary<uint, Key> keys = [];
    private static readonly Dictionary<uint, Key> scancodes = [];
    private static readonly Dictionary<uint, KeyLocation> locations = [];

    public static Key GetKeycode(uint code) =>
        keys.TryGetValue(code, out var value) ? value : default;

    public static Key GetScancode(uint code) =>
        scancodes.TryGetValue(code, out var value) ? value : default;

    public static KeyLocation GetLocation(uint code) =>
        locations.TryGetValue(code, out var value) ? value : default;
}
#endif
