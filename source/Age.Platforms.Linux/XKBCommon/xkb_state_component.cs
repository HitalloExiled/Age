namespace Age.Platforms.Linux.XKBCommon;

internal unsafe static partial class XKBCommon
{
    internal enum xkb_state_component
    {
        XKB_STATE_MODS_DEPRESSED   = 1 << 0,
        XKB_STATE_MODS_LATCHED     = 1 << 1,
        XKB_STATE_MODS_LOCKED      = 1 << 2,
        XKB_STATE_MODS_EFFECTIVE   = 1 << 3,
        XKB_STATE_LAYOUT_DEPRESSED = 1 << 4,
        XKB_STATE_LAYOUT_LATCHED   = 1 << 5,
        XKB_STATE_LAYOUT_LOCKED    = 1 << 6,
        XKB_STATE_LAYOUT_EFFECTIVE = 1 << 7,
        XKB_STATE_LEDS             = 1 << 8
    }
}
