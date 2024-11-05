namespace Age.Platforms.Windows.Native;

internal static partial class User32
{
    /// <summary>
    /// See <see href="">https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-peekmessagew</see>
    /// </summary>
    public enum QUEUE_STATUS : uint
    {
        QS_ALLEVENTS      = QS_INPUT | QS_POSTMESSAGE | QS_TIMER | QS_PAINT | QS_HOTKEY,
        QS_ALLINPUT       = QS_INPUT | QS_POSTMESSAGE | QS_TIMER | QS_PAINT | QS_HOTKEY | QS_SENDMESSAGE,
        QS_ALLPOSTMESSAGE = 0x0100,
        QS_HOTKEY         = 0x0080,
        QS_INPUT          = QS_MOUSE | QS_KEY | QS_RAWINPUT,
        QS_KEY            = 0x0001,
        QS_MOUSE          = QS_MOUSEMOVE | QS_MOUSEBUTTON,
        QS_MOUSEBUTTON    = 0x0004,
        QS_MOUSEMOVE      = 0x0002,
        QS_PAINT          = 0x0020,
        QS_POSTMESSAGE    = 0x0008,
        QS_RAWINPUT       = 0x0400,
        QS_SENDMESSAGE    = 0x0040,
        QS_TIMER          = 0x0010,
    }
}
