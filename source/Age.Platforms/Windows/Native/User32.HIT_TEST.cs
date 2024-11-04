namespace Age.Platforms.Windows.Native;

internal static partial class User32
{
    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/inputdev/wm-nchittest"></see>
    /// </summary>
    public enum HIT_TEST
    {
        HTBORDER      = 18,
        HTBOTTOM      = 15,
        HTBOTTOMLEFT  = 16,
        HTBOTTOMRIGHT = 17,
        HTCAPTION     = 2,
        HTCLIENT      = 1,
        HTCLOSE       = 20,
        HTERROR       = -2,
        HTGROWBOX     = 4,
        HTHELP        = 21,
        HTHSCROLL     = 6,
        HTLEFT        = 10,
        HTMENU        = 5,
        HTMAXBUTTON   = 9,
        HTMINBUTTON   = 8,
        HTNOWHERE     = 0,
        HTREDUCE      = 8,
        HTRIGHT       = 11,
        HTSIZE        = 4,
        HTSYSMENU     = 3,
        HTTOP         = 12,
        HTTOPLEFT     = 13,
        HTTOPRIGHT    = 14,
        HTTRANSPARENT = -1,
        HTVSCROLL     = 7,
        HTZOOM        = HTMAXBUTTON,
    }
}
