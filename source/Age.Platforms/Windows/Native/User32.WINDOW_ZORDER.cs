namespace Age.Platforms.Windows.Native;

internal static partial class User32
{
    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowpos"></see>
    /// </summary>
    public enum WINDOW_ZORDER : int
    {
        HWND_BOTTOM    = 1,
        HWND_NOTOPMOST = -2,
        HWND_TOP       = 0,
        HWND_TOPMOST   = -1
    }
}
