#if WINDOWS
namespace Age.Platforms.Display;

public enum KeyLocation
{
	Unspecified,
	Left,
	Right
};

public static partial class KeyMapping
{
    private static readonly Dictionary<uint, Key> keys = new()
    {
        [0x08] = Key.Backspace,
        [0x09] = Key.Tab,
        [0x0C] = Key.Clear,
        [0x0D] = Key.Enter,
        [0x10] = Key.Shift,
        [0x11] = Key.Ctrl,
        [0x12] = Key.Alt,
        [0x13] = Key.Pause,
        [0x14] = Key.Capslock,
        [0x1B] = Key.Escape,
        [0x20] = Key.Space,
        [0x21] = Key.Pageup,
        [0x22] = Key.Pagedown,
        [0x23] = Key.End,
        [0x24] = Key.Home,
        [0x25] = Key.Left,
        [0x26] = Key.Up,
        [0x27] = Key.Right,
        [0x28] = Key.Down,
        [0x29] = Key.Unknown,
        [0x2A] = Key.Print,
        [0x2B] = Key.Unknown,
        [0x2C] = Key.Print,
        [0x2D] = Key.Insert,
        [0x2E] = Key.KeyDelete,
        [0x2F] = Key.Help,

        [0x5B] = Key.Meta,
        [0x5C] = Key.Meta,
        [0x5D] = Key.Menu,
        [0x5F] = Key.Standby,

        // Numpad
        [0x60] = Key.NumPad0,
        [0x61] = Key.NumPad1,
        [0x62] = Key.NumPad2,
        [0x63] = Key.NumPad3,
        [0x64] = Key.NumPad4,
        [0x65] = Key.NumPad5,
        [0x66] = Key.NumPad6,
        [0x67] = Key.NumPad7,
        [0x68] = Key.NumPad8,
        [0x69] = Key.NumPad9,
        [0x6A] = Key.NumPadMultiply,
        [0x6B] = Key.NumPadAdd,
        [0x6C] = Key.NumPadPeriod,
        [0x6D] = Key.NumPadSubtract,
        [0x6E] = Key.NumPadPeriod,
        [0x6F] = Key.NumPadDivide,

        // F keys
        [0x70] = Key.F1,  [0x71] = Key.F2,
        [0x72] = Key.F3,  [0x73] = Key.F4,
        [0x74] = Key.F5,  [0x75] = Key.F6,
        [0x76] = Key.F7,  [0x77] = Key.F8,
        [0x78] = Key.F9,  [0x79] = Key.F10,
        [0x7A] = Key.F11, [0x7B] = Key.F12,
        [0x7C] = Key.F13, [0x7D] = Key.F14,
        [0x7E] = Key.F15, [0x7F] = Key.F16,
        [0x80] = Key.F17, [0x81] = Key.F18,
        [0x82] = Key.F19, [0x83] = Key.F20,
        [0x84] = Key.F21, [0x85] = Key.F22,
        [0x86] = Key.F23, [0x87] = Key.F24,

        [0x90] = Key.Numlock,
        [0x91] = Key.Scrolllock,

        // Left/Right specific
        [0xA0] = Key.Shift,
        [0xA1] = Key.Shift,
        [0xA2] = Key.Ctrl,
        [0xA3] = Key.Ctrl,
        [0xA4] = Key.Alt,
        [0xA5] = Key.Alt,

        // Browser keys
        [0xA6] = Key.Back,
        [0xA7] = Key.Forward,
        [0xA8] = Key.Refresh,
        [0xA9] = Key.Stop,
        [0xAA] = Key.Search,
        [0xAB] = Key.Favorites,
        [0xAC] = Key.Homepage,

        // Volume keys
        [0xAD] = Key.VolumeMute,
        [0xAE] = Key.VolumeDown,
        [0xAF] = Key.VolumeUp,

        // Media keys
        [0xB0] = Key.MediaNext,
        [0xB1] = Key.MediaPrevious,
        [0xB2] = Key.MediaStop,
        [0xB3] = Key.MediaPlay,

        // Launch keys
        [0xB4] = Key.LaunchMail,
        [0xB5] = Key.LaunchMedia,
        [0xB6] = Key.Launch0,
        [0xB7] = Key.Launch1,

        // OEM keys (US layout mapping)
        [0xBA] = (Key)0x003B, // VK_OEM_1 → ;
        [0xBB] = (Key)0x003D, // VK_OEM_PLUS → =
        [0xBC] = (Key)0x002C, // VK_OEM_COMMA → ,
        [0xBD] = (Key)0x002D, // VK_OEM_MINUS → -
        [0xBE] = (Key)0x002E, // VK_OEM_PERIOD → .
        [0xBF] = (Key)0x002F, // VK_OEM_2 → /
        [0xC0] = (Key)0x0060, // VK_OEM_3 → `
        [0xDB] = (Key)0x005B, // VK_OEM_4 → [
        [0xDC] = (Key)0x005C, // VK_OEM_5 → \
        [0xDD] = (Key)0x005D, // VK_OEM_6 → ]
        [0xDE] = (Key)0x0027, // VK_OEM_7 → '
        [0xDF] = Key.Unknown,
        [0xE2] = Key.Bar,
    };

    private static readonly Dictionary<uint, KeyLocation> locations = new()
    {
        [0xA0] = KeyLocation.Left,  // VK_LSHIFT
        [0xA1] = KeyLocation.Right, // VK_RSHIFT
        [0xA2] = KeyLocation.Left,  // VK_LCONTROL
        [0xA3] = KeyLocation.Right, // VK_RCONTROL
        [0xA4] = KeyLocation.Left,  // VK_LMENU
        [0xA5] = KeyLocation.Right, // VK_RMENU
        [0x5B] = KeyLocation.Left,  // VK_LWIN
        [0x5C] = KeyLocation.Right, // VK_RWIN
    };

    private static readonly Dictionary<uint, Key> scancodes = new()
    {
        [0x00] = Key.Pause,
        [0x01] = Key.Escape,
        [0x02] = Key.Key1,  [0x03] = Key.Key2,
        [0x04] = Key.Key3,  [0x05] = Key.Key4,
        [0x06] = Key.Key5,  [0x07] = Key.Key6,
        [0x08] = Key.Key7,  [0x09] = Key.Key8,
        [0x0A] = Key.Key9,  [0x0B] = Key.Key0,
        [0x0C] = Key.Minus, [0x0D] = Key.Equal,
        [0x0E] = Key.Backspace,
        [0x0F] = Key.Tab,
        [0x10] = Key.Q,    [0x11] = Key.W,
        [0x12] = Key.E,    [0x13] = Key.R,
        [0x14] = Key.T,    [0x15] = Key.Y,
        [0x16] = Key.U,    [0x17] = Key.I,
        [0x18] = Key.O,    [0x19] = Key.P,
        [0x1A] = Key.BracketLeft,
        [0x1B] = Key.BracketRight,
        [0x1C] = Key.Enter,
        [0x1D] = Key.Ctrl,
        [0x1E] = Key.A,    [0x1F] = Key.S,
        [0x20] = Key.D,    [0x21] = Key.F,
        [0x22] = Key.G,    [0x23] = Key.H,
        [0x24] = Key.J,    [0x25] = Key.K,
        [0x26] = Key.L,
        [0x27] = Key.Semicolon,
        [0x28] = Key.Apostrophe,
        [0x29] = Key.QuoteLeft,
        [0x2A] = Key.Shift,
        [0x2B] = Key.Backslash,
        [0x2C] = Key.Z,    [0x2D] = Key.X,
        [0x2E] = Key.C,    [0x2F] = Key.V,
        [0x30] = Key.B,    [0x31] = Key.N,
        [0x32] = Key.M,
        [0x33] = Key.Comma,
        [0x34] = Key.Period,
        [0x35] = Key.Slash,
        [0x36] = Key.Shift,
        [0x37] = Key.NumPadMultiply,
        [0x38] = Key.Alt,
        [0x39] = Key.Space,
        [0x3A] = Key.Capslock,
        [0x3B] = Key.F1,   [0x3C] = Key.F2,
        [0x3D] = Key.F3,   [0x3E] = Key.F4,
        [0x3F] = Key.F5,   [0x40] = Key.F6,
        [0x41] = Key.F7,   [0x42] = Key.F8,
        [0x43] = Key.F9,   [0x44] = Key.F10,
        [0x45] = Key.Numlock,
        [0x46] = Key.Scrolllock,
        [0x47] = Key.NumPad7,
        [0x48] = Key.NumPad8,
        [0x49] = Key.NumPad9,
        [0x4A] = Key.NumPadSubtract,
        [0x4B] = Key.NumPad4,
        [0x4C] = Key.NumPad5,
        [0x4D] = Key.NumPad6,
        [0x4E] = Key.NumPadAdd,
        [0x4F] = Key.NumPad1,
        [0x50] = Key.NumPad2,
        [0x51] = Key.NumPad3,
        [0x52] = Key.NumPad0,
        [0x53] = Key.NumPadPeriod,
        [0x56] = Key.Section,
        [0x57] = Key.F11,  [0x58] = Key.F12,
        [0x5B] = Key.Meta,
        [0x5C] = Key.Meta,
        [0x5D] = Key.Menu,
        [0x64] = Key.F13,  [0x65] = Key.F14,
        [0x66] = Key.F15,  [0x67] = Key.F16,
        [0x68] = Key.F17,  [0x69] = Key.F18,
        [0x6A] = Key.F19,  [0x6B] = Key.F20,
        [0x6C] = Key.F21,  [0x6D] = Key.F22,
        [0x6E] = Key.F23,
        [0x76] = Key.F24,
    };

    private static readonly Dictionary<uint, Key> scancodesExt = new()
    {
        [0x09] = Key.Menu,
        [0x10] = Key.MediaPrevious,
        [0x19] = Key.MediaNext,
        [0x1C] = Key.NumPadEnter,
        [0x20] = Key.VolumeMute,
        [0x21] = Key.Launch1,
        [0x22] = Key.MediaPlay,
        [0x24] = Key.MediaStop,
        [0x2E] = Key.VolumeDown,
        [0x30] = Key.VolumeUp,
        [0x32] = Key.Homepage,
        [0x35] = Key.NumPadDivide,
        [0x37] = Key.Print,
        [0x3A] = Key.NumPadAdd,
        [0x45] = Key.Numlock,
        [0x47] = Key.Home,
        [0x48] = Key.Up,
        [0x49] = Key.Pageup,
        [0x4A] = Key.NumPadSubtract,
        [0x4B] = Key.Left,
        [0x4C] = Key.NumPad5,
        [0x4D] = Key.Right,
        [0x4E] = Key.NumPadAdd,
        [0x4F] = Key.End,
        [0x50] = Key.Down,
        [0x51] = Key.Pagedown,
        [0x52] = Key.Insert,
        [0x53] = Key.KeyDelete,
        [0x5D] = Key.Menu,
        [0x5F] = Key.Standby,
        [0x65] = Key.Search,
        [0x66] = Key.Favorites,
        [0x67] = Key.Refresh,
        [0x68] = Key.Stop,
        [0x69] = Key.Forward,
        [0x6A] = Key.Back,
        [0x6B] = Key.Launch0,
        [0x6C] = Key.LaunchMail,
        [0x6D] = Key.LaunchMedia,
        [0x78] = Key.MediaRecord,
    };

    static KeyMapping()
    {
        // Letters (VK_A-VK_Z = 0x41-0x5A) map directly to Key.A-Key.Z
        // Digits (VK_0-VK_9 = 0x30-0x39) map directly to Key.Key0-Key.Key9
        for (uint vk = 0x30; vk <= 0x39; vk++)
        {
            keys[vk] = (Key)vk;
        }

        for (uint vk = 0x41; vk <= 0x5A; vk++)
        {
            keys[vk] = (Key)vk;
        }
    }

    public static partial Key GetKeycode(uint code) =>
        keys.TryGetValue(code, out var key) ? key : Key.None;

    public static partial Key GetScancode(uint code) =>
        scancodes.TryGetValue(code, out var key) ? key : Key.None;

    public static Key GetScancode(uint code, bool extended) =>
        extended && scancodesExt.TryGetValue(code, out var key) ? key : GetScancode(code);

    public static partial KeyLocation GetLocation(uint code) =>
        locations.TryGetValue(code, out var location) ? location : KeyLocation.Unspecified;

    public static KeyLocation GetLocation(uint vk, uint scancode, bool extended)
    {
        var location = GetLocation(vk);

        if (location == KeyLocation.Unspecified)
        {
            if (vk == 0x10)
            {
                location = scancode == 0x36 ? KeyLocation.Right : KeyLocation.Left;
            }
            else if (vk == 0x11 || vk == 0x12)
            {
                location = extended ? KeyLocation.Right : KeyLocation.Left;
            }
        }

        return location;
    }
}
#endif
