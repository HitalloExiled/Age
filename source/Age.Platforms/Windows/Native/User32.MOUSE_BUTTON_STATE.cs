namespace Age.Platforms.Windows.Native;

internal static partial class User32
{
    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-rawmouse"></see>
    /// </summary>
    [Flags]
    public enum MOUSE_BUTTON_STATE : ushort
    {
        RI_MOUSE_BUTTON_1_DOWN      = 0x0001,
        RI_MOUSE_LEFT_BUTTON_DOWN   = RI_MOUSE_BUTTON_1_DOWN,
        RI_MOUSE_BUTTON_1_UP        = 0x0002,
        RI_MOUSE_LEFT_BUTTON_UP     = RI_MOUSE_BUTTON_1_UP,
        RI_MOUSE_BUTTON_2_DOWN      = 0x0004,
        RI_MOUSE_RIGHT_BUTTON_DOWN  = RI_MOUSE_BUTTON_2_DOWN,
        RI_MOUSE_BUTTON_2_UP        = 0x0008,
        RI_MOUSE_RIGHT_BUTTON_UP    = RI_MOUSE_BUTTON_2_UP,
        RI_MOUSE_BUTTON_3_DOWN      = 0x0010,
        RI_MOUSE_MIDDLE_BUTTON_DOWN = RI_MOUSE_BUTTON_3_DOWN,
        RI_MOUSE_BUTTON_3_UP        = 0x0020,
        RI_MOUSE_BUTTON_4_DOWN      = 0x0040,
        RI_MOUSE_BUTTON_4_UP        = 0x0080,
        RI_MOUSE_BUTTON_5_DOWN      = 0x0100,
        RI_MOUSE_BUTTON_5_UP        = 0x0200,
        RI_MOUSE_WHEEL              = 0x0400,
        RI_MOUSE_HWHEEL             = 0x0800
    }
}
