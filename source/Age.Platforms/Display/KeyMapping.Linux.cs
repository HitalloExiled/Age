#if LINUX
using Age.Platforms.Linux.LibXKBCommon;

namespace Age.Platforms.Display;

public enum KeyLocation : byte
{
	Unspecified,
	Left,
	Right
};

public static partial class KeyMapping
{
    private static readonly Dictionary<xkbcommon_keysyms, Key> keys = new()
    {
        [xkbcommon_keysyms.XKB_KEY_Escape]               = Key.Escape,
        [xkbcommon_keysyms.XKB_KEY_Tab]                  = Key.Tab,
        [xkbcommon_keysyms.XKB_KEY_ISO_Left_Tab]         = Key.Backtab,
        [xkbcommon_keysyms.XKB_KEY_BackSpace]            = Key.Backspace,
        [xkbcommon_keysyms.XKB_KEY_Return]               = Key.Enter,
        [xkbcommon_keysyms.XKB_KEY_Insert]               = Key.Insert,
        [xkbcommon_keysyms.XKB_KEY_Delete]               = Key.KeyDelete,
        [xkbcommon_keysyms.XKB_KEY_Clear]                = Key.KeyDelete,
        [xkbcommon_keysyms.XKB_KEY_Pause]                = Key.Pause,
        [xkbcommon_keysyms.XKB_KEY_Print]                = Key.Print,
        [xkbcommon_keysyms.XKB_KEY_Home]                 = Key.Home,
        [xkbcommon_keysyms.XKB_KEY_End]                  = Key.End,
        [xkbcommon_keysyms.XKB_KEY_Left]                 = Key.Left,
        [xkbcommon_keysyms.XKB_KEY_Up]                   = Key.Up,
        [xkbcommon_keysyms.XKB_KEY_Right]                = Key.Right,
        [xkbcommon_keysyms.XKB_KEY_Down]                 = Key.Down,
        [xkbcommon_keysyms.XKB_KEY_Prior]                = Key.Pageup,
        [xkbcommon_keysyms.XKB_KEY_Next]                 = Key.Pagedown,
        [xkbcommon_keysyms.XKB_KEY_Shift_L]              = Key.Shift,
        [xkbcommon_keysyms.XKB_KEY_Shift_R]              = Key.Shift,
        [xkbcommon_keysyms.XKB_KEY_Shift_Lock]           = Key.Shift,
        [xkbcommon_keysyms.XKB_KEY_Control_L]            = Key.Ctrl,
        [xkbcommon_keysyms.XKB_KEY_Control_R]            = Key.Ctrl,
        [xkbcommon_keysyms.XKB_KEY_Meta_L]               = Key.Meta,
        [xkbcommon_keysyms.XKB_KEY_Meta_R]               = Key.Meta,
        [xkbcommon_keysyms.XKB_KEY_Alt_L]                = Key.Alt,
        [xkbcommon_keysyms.XKB_KEY_Alt_R]                = Key.Alt,
        [xkbcommon_keysyms.XKB_KEY_Caps_Lock]            = Key.Capslock,
        [xkbcommon_keysyms.XKB_KEY_Num_Lock]             = Key.Numlock,
        [xkbcommon_keysyms.XKB_KEY_Scroll_Lock]          = Key.Scrolllock,
        [xkbcommon_keysyms.XKB_KEY_less]                 = Key.QuoteLeft,
        [xkbcommon_keysyms.XKB_KEY_grave]                = Key.Section,
        [xkbcommon_keysyms.XKB_KEY_Super_L]              = Key.Meta,
        [xkbcommon_keysyms.XKB_KEY_Super_R]              = Key.Meta,
        [xkbcommon_keysyms.XKB_KEY_Menu]                 = Key.Menu,
        [xkbcommon_keysyms.XKB_KEY_Hyper_L]              = Key.Hyper,
        [xkbcommon_keysyms.XKB_KEY_Hyper_R]              = Key.Hyper,
        [xkbcommon_keysyms.XKB_KEY_Help]                 = Key.Help,
        [xkbcommon_keysyms.XKB_KEY_KP_Space]             = Key.Space,
        [xkbcommon_keysyms.XKB_KEY_KP_Tab]               = Key.Tab,
        [xkbcommon_keysyms.XKB_KEY_KP_Enter]             = Key.NumPadEnter,
        [xkbcommon_keysyms.XKB_KEY_Home]                 = Key.Home,
        [xkbcommon_keysyms.XKB_KEY_Left]                 = Key.Left,
        [xkbcommon_keysyms.XKB_KEY_Up]                   = Key.Up,
        [xkbcommon_keysyms.XKB_KEY_Right]                = Key.Right,
        [xkbcommon_keysyms.XKB_KEY_Down]                 = Key.Down,
        [xkbcommon_keysyms.XKB_KEY_Prior]                = Key.Pageup,
        [xkbcommon_keysyms.XKB_KEY_Next]                 = Key.Pagedown,
        [xkbcommon_keysyms.XKB_KEY_End]                  = Key.End,
        [xkbcommon_keysyms.XKB_KEY_Begin]                = Key.Clear,
        [xkbcommon_keysyms.XKB_KEY_Insert]               = Key.Insert,
        [xkbcommon_keysyms.XKB_KEY_Delete]               = Key.KeyDelete,
        [xkbcommon_keysyms.XKB_KEY_KP_Equal]             = Key.Equal,
        [xkbcommon_keysyms.XKB_KEY_KP_Separator]         = Key.Comma,
        [xkbcommon_keysyms.XKB_KEY_KP_Decimal]           = Key.NumPadPeriod,
        [xkbcommon_keysyms.XKB_KEY_KP_Multiply]          = Key.NumPadMultiply,
        [xkbcommon_keysyms.XKB_KEY_KP_Divide]            = Key.NumPadDivide,
        [xkbcommon_keysyms.XKB_KEY_KP_Subtract]          = Key.NumPadSubtract,
        [xkbcommon_keysyms.XKB_KEY_KP_Add]               = Key.NumPadAdd,
        [xkbcommon_keysyms.XKB_KEY_KP_0]                 = Key.NumPad0,
        [xkbcommon_keysyms.XKB_KEY_KP_1]                 = Key.NumPad1,
        [xkbcommon_keysyms.XKB_KEY_KP_2]                 = Key.NumPad2,
        [xkbcommon_keysyms.XKB_KEY_KP_3]                 = Key.NumPad3,
        [xkbcommon_keysyms.XKB_KEY_KP_4]                 = Key.NumPad4,
        [xkbcommon_keysyms.XKB_KEY_KP_5]                 = Key.NumPad5,
        [xkbcommon_keysyms.XKB_KEY_KP_6]                 = Key.NumPad6,
        [xkbcommon_keysyms.XKB_KEY_KP_7]                 = Key.NumPad7,
        [xkbcommon_keysyms.XKB_KEY_KP_8]                 = Key.NumPad8,
        [xkbcommon_keysyms.XKB_KEY_KP_9]                 = Key.NumPad9,
        // Same keys but with numlock off.
        [xkbcommon_keysyms.XKB_KEY_KP_Insert]            = Key.Insert,
        [xkbcommon_keysyms.XKB_KEY_KP_Delete]            = Key.KeyDelete,
        [xkbcommon_keysyms.XKB_KEY_KP_End]               = Key.End,
        [xkbcommon_keysyms.XKB_KEY_KP_Down]              = Key.Down,
        [xkbcommon_keysyms.XKB_KEY_KP_Page_Down]         = Key.Pagedown,
        [xkbcommon_keysyms.XKB_KEY_KP_Left]              = Key.Left,
        // X11 documents this (numpad 5) as "begin of line" but no toolkit seems to interpret it this way.
        // On Windows this is emitting Key               :: Clear so for consistency it will be mapped to Key.Clear,
        [xkbcommon_keysyms.XKB_KEY_KP_Begin]             = Key.Clear,
        [xkbcommon_keysyms.XKB_KEY_KP_Right]             = Key.Right,
        [xkbcommon_keysyms.XKB_KEY_KP_Home]              = Key.Home,
        [xkbcommon_keysyms.XKB_KEY_KP_Up]                = Key.Up,
        [xkbcommon_keysyms.XKB_KEY_KP_Page_Up]           = Key.Pageup,
        [xkbcommon_keysyms.XKB_KEY_F1]                   = Key.F1,
        [xkbcommon_keysyms.XKB_KEY_F2]                   = Key.F2,
        [xkbcommon_keysyms.XKB_KEY_F3]                   = Key.F3,
        [xkbcommon_keysyms.XKB_KEY_F4]                   = Key.F4,
        [xkbcommon_keysyms.XKB_KEY_F5]                   = Key.F5,
        [xkbcommon_keysyms.XKB_KEY_F6]                   = Key.F6,
        [xkbcommon_keysyms.XKB_KEY_F7]                   = Key.F7,
        [xkbcommon_keysyms.XKB_KEY_F8]                   = Key.F8,
        [xkbcommon_keysyms.XKB_KEY_F9]                   = Key.F9,
        [xkbcommon_keysyms.XKB_KEY_F10]                  = Key.F10,
        [xkbcommon_keysyms.XKB_KEY_F11]                  = Key.F11,
        [xkbcommon_keysyms.XKB_KEY_F12]                  = Key.F12,
        [xkbcommon_keysyms.XKB_KEY_F13]                  = Key.F13,
        [xkbcommon_keysyms.XKB_KEY_F14]                  = Key.F14,
        [xkbcommon_keysyms.XKB_KEY_F15]                  = Key.F15,
        [xkbcommon_keysyms.XKB_KEY_F16]                  = Key.F16,
        [xkbcommon_keysyms.XKB_KEY_F17]                  = Key.F17,
        [xkbcommon_keysyms.XKB_KEY_F18]                  = Key.F18,
        [xkbcommon_keysyms.XKB_KEY_F19]                  = Key.F19,
        [xkbcommon_keysyms.XKB_KEY_F20]                  = Key.F20,
        [xkbcommon_keysyms.XKB_KEY_F21]                  = Key.F21,
        [xkbcommon_keysyms.XKB_KEY_F22]                  = Key.F22,
        [xkbcommon_keysyms.XKB_KEY_F23]                  = Key.F23,
        [xkbcommon_keysyms.XKB_KEY_F24]                  = Key.F24,
        [xkbcommon_keysyms.XKB_KEY_F25]                  = Key.F25,
        [xkbcommon_keysyms.XKB_KEY_F26]                  = Key.F26,
        [xkbcommon_keysyms.XKB_KEY_F27]                  = Key.F27,
        [xkbcommon_keysyms.XKB_KEY_F28]                  = Key.F28,
        [xkbcommon_keysyms.XKB_KEY_F29]                  = Key.F29,
        [xkbcommon_keysyms.XKB_KEY_F30]                  = Key.F30,
        [xkbcommon_keysyms.XKB_KEY_F31]                  = Key.F31,
        [xkbcommon_keysyms.XKB_KEY_F32]                  = Key.F32,
        [xkbcommon_keysyms.XKB_KEY_F33]                  = Key.F33,
        [xkbcommon_keysyms.XKB_KEY_F34]                  = Key.F34,
        [xkbcommon_keysyms.XKB_KEY_F35]                  = Key.F35,
        [xkbcommon_keysyms.XKB_KEY_yen]                  = Key.Yen,
        [xkbcommon_keysyms.XKB_KEY_section]              = Key.Section,
        // Media keys.
        [xkbcommon_keysyms.XKB_KEY_XF86Back]             = Key.Back,
        [xkbcommon_keysyms.XKB_KEY_XF86Forward]          = Key.Forward,
        [xkbcommon_keysyms.XKB_KEY_XF86Stop]             = Key.Stop,
        [xkbcommon_keysyms.XKB_KEY_XF86Refresh]          = Key.Refresh,
        [xkbcommon_keysyms.XKB_KEY_XF86Favorites]        = Key.Favorites,
        [xkbcommon_keysyms.XKB_KEY_XF86OpenURL]          = Key.OpenUrl,
        [xkbcommon_keysyms.XKB_KEY_XF86HomePage]         = Key.Homepage,
        [xkbcommon_keysyms.XKB_KEY_XF86Search]           = Key.Search,
        [xkbcommon_keysyms.XKB_KEY_XF86AudioLowerVolume] = Key.VolumeDown,
        [xkbcommon_keysyms.XKB_KEY_XF86AudioMute]        = Key.VolumeMute,
        [xkbcommon_keysyms.XKB_KEY_XF86AudioRaiseVolume] = Key.VolumeUp,
        [xkbcommon_keysyms.XKB_KEY_XF86AudioPlay]        = Key.MediaPlay,
        [xkbcommon_keysyms.XKB_KEY_XF86AudioStop]        = Key.MediaStop,
        [xkbcommon_keysyms.XKB_KEY_XF86AudioPrev]        = Key.MediaPrevious,
        [xkbcommon_keysyms.XKB_KEY_XF86AudioNext]        = Key.MediaNext,
        [xkbcommon_keysyms.XKB_KEY_XF86AudioRecord]      = Key.MediaRecord,
        [xkbcommon_keysyms.XKB_KEY_XF86Standby]          = Key.Standby,
        // Launch keys.
        [xkbcommon_keysyms.XKB_KEY_XF86Mail]             = Key.LaunchMail,
        [xkbcommon_keysyms.XKB_KEY_XF86AudioMedia]       = Key.LaunchMedia,
        [xkbcommon_keysyms.XKB_KEY_XF86MyComputer]       = Key.Launch0,
        [xkbcommon_keysyms.XKB_KEY_XF86Calculator]       = Key.Launch1,
        [xkbcommon_keysyms.XKB_KEY_XF86Launch0]          = Key.Launch2,
        [xkbcommon_keysyms.XKB_KEY_XF86Launch1]          = Key.Launch3,
        [xkbcommon_keysyms.XKB_KEY_XF86Launch2]          = Key.Launch4,
        [xkbcommon_keysyms.XKB_KEY_XF86Launch3]          = Key.Launch5,
        [xkbcommon_keysyms.XKB_KEY_XF86Launch4]          = Key.Launch6,
        [xkbcommon_keysyms.XKB_KEY_XF86Launch5]          = Key.Launch7,
        [xkbcommon_keysyms.XKB_KEY_XF86Launch6]          = Key.Launch8,
        [xkbcommon_keysyms.XKB_KEY_XF86Launch7]          = Key.Launch9,
        [xkbcommon_keysyms.XKB_KEY_XF86Launch8]          = Key.LaunchA,
        [xkbcommon_keysyms.XKB_KEY_XF86Launch9]          = Key.LaunchB,
        [xkbcommon_keysyms.XKB_KEY_XF86LaunchA]          = Key.LaunchC,
        [xkbcommon_keysyms.XKB_KEY_XF86LaunchB]          = Key.LaunchD,
        [xkbcommon_keysyms.XKB_KEY_XF86LaunchC]          = Key.LaunchE,
        [xkbcommon_keysyms.XKB_KEY_XF86LaunchD]          = Key.LaunchF,
    };

    private static readonly Dictionary<uint, KeyLocation> locations = new()
    {
        // Ctrl.
        [0x25] = KeyLocation.Left,
        [0x69] = KeyLocation.Right,
        // Shift.
        [0x32] = KeyLocation.Left,
        [0x3E] = KeyLocation.Right,
        // Alt.
        [0x40] = KeyLocation.Left,
        [0x6C] = KeyLocation.Right,
        // Meta.
        [0x85] = KeyLocation.Left,
        [0x86] = KeyLocation.Right,
    };

    private static readonly Dictionary<uint, Key> scancodes = new()
    {
        [0x09] = Key.Escape,
        [0x0A] = Key.Key1,
        [0x0B] = Key.Key2,
        [0x0C] = Key.Key3,
        [0x0D] = Key.Key4,
        [0x0E] = Key.Key5,
        [0x0F] = Key.Key6,
        [0x10] = Key.Key7,
        [0x11] = Key.Key8,
        [0x12] = Key.Key9,
        [0x13] = Key.Key0,
        [0x14] = Key.Minus,
        [0x15] = Key.Equal,
        [0x16] = Key.Backspace,
        [0x17] = Key.Tab,
        [0x18] = Key.Q,
        [0x19] = Key.W,
        [0x1A] = Key.E,
        [0x1B] = Key.R,
        [0x1C] = Key.T,
        [0x1D] = Key.Y,
        [0x1E] = Key.U,
        [0x1F] = Key.I,
        [0x20] = Key.O,
        [0x21] = Key.P,
        [0x22] = Key.BraceLeft,
        [0x23] = Key.BraceRight,
        [0x24] = Key.Enter,
        [0x25] = Key.Ctrl, // Left
        [0x26] = Key.A,
        [0x27] = Key.S,
        [0x28] = Key.D,
        [0x29] = Key.F,
        [0x2A] = Key.G,
        [0x2B] = Key.H,
        [0x2C] = Key.J,
        [0x2D] = Key.K,
        [0x2E] = Key.L,
        [0x2F] = Key.Semicolon,
        [0x30] = Key.Apostrophe,
        [0x31] = Key.Section,
        [0x32] = Key.Shift, // Left
        [0x33] = Key.Backslash,
        [0x34] = Key.Z,
        [0x35] = Key.X,
        [0x36] = Key.C,
        [0x37] = Key.V,
        [0x38] = Key.B,
        [0x39] = Key.N,
        [0x3A] = Key.M,
        [0x3B] = Key.Comma,
        [0x3C] = Key.Period,
        [0x3D] = Key.Slash,
        [0x3E] = Key.Shift, // Right
        [0x3F] = Key.NumPadMultiply,
        [0x40] = Key.Alt, // Left
        [0x41] = Key.Space,
        [0x42] = Key.Capslock,
        [0x43] = Key.F1,
        [0x44] = Key.F2,
        [0x45] = Key.F3,
        [0x46] = Key.F4,
        [0x47] = Key.F5,
        [0x48] = Key.F6,
        [0x49] = Key.F7,
        [0x4A] = Key.F8,
        [0x4B] = Key.F9,
        [0x4C] = Key.F10,
        [0x4D] = Key.Numlock,
        [0x4E] = Key.Scrolllock,
        [0x4F] = Key.NumPad7,
        [0x50] = Key.NumPad8,
        [0x51] = Key.NumPad9,
        [0x52] = Key.NumPadSubtract,
        [0x53] = Key.NumPad4,
        [0x54] = Key.NumPad5,
        [0x55] = Key.NumPad6,
        [0x56] = Key.NumPadAdd,
        [0x57] = Key.NumPad1,
        [0x58] = Key.NumPad2,
        [0x59] = Key.NumPad3,
        [0x5A] = Key.NumPad0,
        [0x5B] = Key.NumPadPeriod,
        //[0x5C]
        //[0x5D] // Zenkaku Hankaku
        [0x5E] = Key.QuoteLeft,
        [0x5F] = Key.F11,
        [0x60] = Key.F12,
        //[0x61] // Romaji
        //[0x62] // Katakana
        //[0x63] // Hiragana
        //[0x64] // Henkan
        //[0x65] // Hiragana Katakana
        //[0x66] // Muhenkan
        [0x67] = Key.Comma, // KP_Separator
        [0x68] = Key.NumPadEnter,
        [0x69] = Key.Ctrl, // Right
        [0x6A] = Key.NumPadDivide,
        [0x6B] = Key.Print,
        [0x6C] = Key.Alt, // Right
        [0x6D] = Key.Enter,
        [0x6E] = Key.Home,
        [0x6F] = Key.Up,
        [0x70] = Key.Pageup,
        [0x71] = Key.Left,
        [0x72] = Key.Right,
        [0x73] = Key.End,
        [0x74] = Key.Down,
        [0x75] = Key.Pagedown,
        [0x76] = Key.Insert,
        [0x77] = Key.KeyDelete,
        //[0x78] // Macro
        [0x79] = Key.VolumeMute,
        [0x7A] = Key.VolumeDown,
        [0x7B] = Key.VolumeUp,
        //[0x7C] // Power
        [0x7D] = Key.Equal, // KP_Equal
        //[0x7E] // KP_PlusMinus
        [0x7F] = Key.Pause,
        [0x80] = Key.Launch0,
        [0x81] = Key.Comma, // KP_Comma
        //[0x82] // Hangul
        //[0x83] // Hangul_Hanja
        [0x84] = Key.Yen,
        [0x85] = Key.Meta, // Left
        [0x86] = Key.Meta, // Right
        [0x87] = Key.Menu,

        [0xA6] = Key.Back, // On Chromebooks
        [0xA7] = Key.Forward, // On Chromebooks

        [0xB5] = Key.Refresh, // On Chromebooks

        [0xBF] = Key.F13,
        [0xC0] = Key.F14,
        [0xC1] = Key.F15,
        [0xC2] = Key.F16,
        [0xC3] = Key.F17,
        [0xC4] = Key.F18,
        [0xC5] = Key.F19,
        [0xC6] = Key.F20,
        [0xC7] = Key.F21,
        [0xC8] = Key.F22,
        [0xC9] = Key.F23,
        [0xCA] = Key.F24,
        [0xCB] = Key.F25,
        [0xCC] = Key.F26,
        [0xCD] = Key.F27,
        [0xCE] = Key.F28,
        [0xCF] = Key.F29,
        [0xD0] = Key.F30,
        [0xD1] = Key.F31,
        [0xD2] = Key.F32,
        [0xD3] = Key.F33,
        [0xD4] = Key.F34,
        [0xD5] = Key.F35,
    };

    private static readonly Dictionary<Key, uint> scancodesInv = new();

    static KeyMapping()
    {
        foreach (var item in scancodes)
        {
            scancodesInv[item.Value] = item.Key;
        }
    }

    public static partial Key GetKeycode(uint code) =>
        code >= 32 && code < 0x7E
            ? (Key)code
            : keys.TryGetValue((xkbcommon_keysyms)code, out var value) ? value : default;

    public static partial KeyLocation GetLocation(uint code) =>
        locations.TryGetValue(code, out var value) ? value : default;

    public static partial Key GetScancode(uint code) =>
        scancodes.TryGetValue(code, out var value) ? value : default;

    public static uint GetXkbKeycode(Key key) =>
        scancodesInv.TryGetValue(key, out var value) ? value : default;
}
#endif
