namespace Age.Platforms.Windows.Native;

internal static partial class User32
{
    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getwindowlongptrw"></see>
    /// </summary>
    public enum WINDOW_LONG_INDEX
    {
        GWL_EXSTYLE     = -20,
        GWLP_HINSTANCE  = -6,
        GWLP_HWNDPARENT = -8,
        GWLP_ID         = -12,
        GWL_STYLE       = -16,
        GWLP_USERDATA   = -21,
        GWLP_WNDPROC    = -4
    }
}
