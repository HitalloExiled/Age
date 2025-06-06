namespace Age.Platforms.Windows.Native;

internal static partial class User32
{
    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowpos"></see>
    /// </summary>
    [Flags]
    public enum WINDOW_POS_FLAGS : uint
    {
        SWP_ASYNCWINDOWPOS = 0x4000,
        SWP_DEFERERASE     = 0x2000,
        SWP_DRAWFRAME      = 0x0020,
        SWP_FRAMECHANGED   = SWP_DRAWFRAME,
        SWP_HIDEWINDOW     = 0x0080,
        SWP_NOACTIVATE     = 0x0010,
        SWP_NOCOPYBITS     = 0x0100,
        SWP_NOMOVE         = 0x0002,
        SWP_NOOWNERZORDER  = 0x0200,
        SWP_NOREDRAW       = 0x0008,
        SWP_NOREPOSITION   = SWP_NOOWNERZORDER,
        SWP_NOSENDCHANGING = 0x0400,
        SWP_NOSIZE         = 0x0001,
        SWP_NOZORDER       = 0x0004,
        SWP_SHOWWINDOW     = 0x0040
    }
}
