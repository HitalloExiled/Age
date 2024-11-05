namespace Age.Platforms.Windows.Native;

internal static partial class User32
{
    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-rawmouse"></see>
    /// </summary>
    public enum MOUSE_STATE : ushort
    {
        MOUSE_MOVE_RELATIVE      = 0x00,
        MOUSE_MOVE_ABSOLUTE      = 0x01,
        MOUSE_VIRTUAL_DESKTOP    = 0x02,
        MOUSE_ATTRIBUTES_CHANGED = 0x04,
        MOUSE_MOVE_NOCOALESCE    = 0x08
    }
}
